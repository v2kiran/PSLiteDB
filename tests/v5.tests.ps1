

BeforeAll {
  if (-not (Get-Command -module pslitedb -ErrorAction SilentlyContinue))
  {
    Import-Module $PSScriptRoot\..\module\pslitedb.psd1
  }

  # data for testing
  $dataset = @"
MovieID,Title,MPAA,Budget,Gross,Release Date,Genre,Runtime,Rating,RatingCount
1,Look Who's Talking,PG-13,7500000,296000000,1989-10-12,Romance,93,5.9,73638
2,Driving Miss Daisy,PG,7500000,145793296,1989-12-13,Comedy,99,7.4,91075
3,Turner & Hooch,PG,13000000,71079915,1989-07-28,Crime,100,7.2,91415
4,Born on the Fourth of July,R,14000000,161001698,1989-12-20,War,145,7.2,91415
5,Field of Dreams,PG,15000000,84431625,1989-04-21,Drama,107,7.5,101702
6,Uncle Buck,PG,15000000,79258538,1989-08-16,Family,100,7,77659
7,When Harry Met Sally...,R,16000000,92800000,1989-07-21,Romance,96,7.6,180871
8,Dead Poets Society,PG,16400000,235860116,1989-06-02,Drama,129,8.1,382002
9,Parenthood,PG-13,20000000,126297830,1989-07-31,Comedy,124,7,41866
10,Lethal Weapon 2,R,25000000,227853986,1989-07-07,Comedy,114,7.2,151737
"@

  $movies = $dataset |
    ConvertFrom-Csv |
    Select-Object @{Name = "_id"; E = { [int]$_.MovieID } },
    @{Name = "Budget"; E = { [int64]$_.Budget } },
    @{Name = "Gross"; E = { [int64]$_.Gross } },
    @{Name = "ReleaseDate"; E = { [datetime]$_.'Release Date' } },
    @{Name = "RunTime"; E = { [int]$_.Runtime } },
    @{Name = "Rating"; E = { [Double]$_.Rating } },
    @{Name = "RatingCount"; E = { [int64]$_.RatingCount } },
    Title, MPAA -First 15

  # Create an in-memory database that we will re-use for all our describe blocks
  $null = Open-litedbconnection

  # will be used to populate the database
  $MovieCount = $movies.count

}

# Describe Open-LiteDBConnection
Describe "Open-LitedbConnection" -Tag oldb -Fixture {
  It "creates a in-memory database" {
    $LiteDBPSConnection | Should -Not -BeNullOrEmpty
    $LiteDBPSConnection.LimitSize | Should -Be 9223372036854775807
  }
}

# Describe New-LiteDBCollection
Describe "New-LiteDBCollection" -Tag Collection -Fixture {
  It "Creates a new Collection" {
    { New-LiteDBCollection -Collection movies } | Should -Not -Throw
    (Get-LiteDBCollectionName).Collection | Should -Contain "movies"
  }
}

# Describe Rename-LiteDBCollection
Describe "Rename-LiteDBCollection" -Tag Collection -Fixture {
  It "Renames a Collection" {
    { New-LiteDBCollection -Collection moviecol } | Should -Not -Throw
    {Rename-LiteDBCollection -Collection moviecol -NewCollection moviecol2} | Should -Not -Throw
    (Get-LiteDBCollectionName).Collection | Should -Contain "moviecol2"
  }
}

# Describe Remove-LiteDBCollection
Describe "Remove-LiteDBCollection" -Tag Collection -Fixture {
  It "Remove a Collection" {
    { Remove-LiteDBCollection -Collection moviecol2 -Confirm:$false } | Should -Not -Throw
    (Get-LiteDBCollectionName).Collection | Should -Not -Contain "moviecol2"
  }
}

# Describe New-LiteDBIndex
Describe "New-LiteDBIndex" -Tag Index -Fixture {
  It "Creates a new Index" {
    { New-LiteDBIndex -Collection movies -Field Genre -Expression "LOWER($.Genre)" } | Should -Not -Throw
    (Get-LitedbIndex -Collection movies).Expression | Should -Contain "LOWER($.Genre)"
  }
}

# Describe Remove-LiteDBIndex
Describe "Remove-LiteDBIndex" -Tag Remove,Index -Fixture {
  It "Removes an Index" {
    { Remove-LiteDBIndex -Collection movies -Field Genre } | Should -Not -Throw
    (Get-LitedbIndex -Collection movies).Expression | Should -Not -Contain "LOWER($.Genre)"
  }
}

Describe "ConvertTo-LiteDbBSON" -Tag Insert -Fixture {
  It "Converts psobjects to BSON Documents" {
    $bsondocs = $movies | ConvertTo-LiteDbBSON
    $bsondocs[0].GetType().Name | Should -Be "BsonDocument" -Because "PSObject to bson conversion should work"
  }
}

# Describe Add-LiteDBDocument
Describe "Add-LiteDBDocument" -Tag Insert, Add -Fixture {
  It "Adds\inserts documents" {
    { $movies | ConvertTo-LiteDbBSON | Add-LiteDBDocument -Collection movies } | Should -Not -Throw
    (Get-LiteDBCollectionName | Select-Object -expand Docs) | Should -Be 10
  }
}

# Describe Find-LiteDBDocument
Describe "Find-LiteDBDocument" -Tag Find -Fixture {
  Context "Find All" -Fixture {
    It "Finds all documents in the database" {
      $result = Find-LiteDBDocument -Collection movies
      $result.count | Should -Be $MovieCount


      # second test - property names
      $proplist = ($result[0] | Get-Member -MemberType Properties).name
      foreach ($p in @("Collection", "Budget", "Gross", "MPAA", "_id", 'Rating', 'RatingCount', 'ReleaseDate', 'RunTime', 'Title'))
      {
        $proplist | Should -contain $p
      }

      # 3rd test - property value
      $result[0].Title | Should -Not -BeNullOrEmpty
    }

    It "Limits the results returned with the Limit parameter" {
      $result = Find-LiteDBDocument -Collection movies -Limit 5
      $result | Should -Not -BeNullOrEmpty
      $result.count | Should -Be 5
    }
  }# context find all

  Context "Find by ID" -Fixture {
    It "finds documents by ID" {
      $result = Find-LiteDBDocument -Collection movies -ID 6
      $result | Should -Not -BeNullOrEmpty
      $result.Title | Should -Be "Uncle Buck"
    }
  }

  Context "Find by SQL" -Fixture {
    It "finds the first 5 documents" {
      $result = Find-LiteDBDocument movies -Sql "Select $ from movies limit 5"
      $result | Should -Not -BeNullOrEmpty
      $result.count | Should -Be 5
      $result[0].collection | Should -Be 'movies'
      $result[0].Title | Should -Not -BeNullOrEmpty
    }#it

    It "finds the first 2 documents with select properties" {
      $result = Find-LiteDBDocument movies -Sql "Select _ID,Title,Rating,Runtime,MPAA from movies limit 2"
      $result | Should -Not -BeNullOrEmpty
      $result.count | Should -Be 2
      $result[0].collection | Should -Be 'movies'
      $result[0].Title | Should -Not -BeNullOrEmpty
      $proplist = ($result[0] | Get-Member -MemberType Properties).name
      $proplist.count | Should -Be 6
      $proplist | Should -not -contain "Budget"
      $proplist | Should -not -contain "Gross"
    }#it

    It "Where clause" {
      $result = Find-LiteDBDocument movies -Sql "Select _id,Title,MPAA from movies where MPAA='PG-13'"
      $result | Should -Not -BeNullOrEmpty
      $result.count | Should -Be 2
      $result.Title | Should -contain "Look Who's Talking"
      $result.Title | Should -contain 'Parenthood'
      [string[]]$r = $result.mpaa | Select-Object -Unique
      $r.Count | Should -Be 1
      $r[0] | Should -Be 'PG-13'
    }#it

    It "Functions" {
      $result = Find-LiteDBDocument movies -Sql "Select concat(string(_id),MPAA) as concat,_id,MPAA from movies where MPAA='PG-13'"
      $result | Should -Not -BeNullOrEmpty
      ($result[0].concat)[0] | Should -Be "1"
      ($result[1].concat)[0] | Should -Be "9"
      ($result[0].concat)[1] | Should -Be "PG-13"
      $result[0].Concat.count | Should -Be 2
    }#it
  }#context 'Find by SQL'

  Context "Named Queries" -Fixture {
    It "Where filter" {
      $result = Find-LiteDBDocument movies -Where "MPAA='PG-13'" -Select "{_id,Title,MPAA}"
      $result | Should -Not -BeNullOrEmpty
      $result.count | Should -Be 2
      $result.Title | Should -contain "Look Who's Talking"
      $result.Title | Should -contain 'Parenthood'
    }#where filter

    It "Custom Columns" {
      $result = Find-LiteDBDocument movies -Where "MPAA='PG-13'" -Select "{_id,MovieName:Title}"
      $result | Should -Not -BeNullOrEmpty
      $result.count | Should -Be 2
      $result.MovieName | Should -contain "Look Who's Talking"
      $result.MovieName | Should -contain 'Parenthood'
    }

    It "Limit results" {
      $result = Find-LiteDBDocument movies -Where "MPAA='PG-13'" -Select "{_id,MovieName:Title}" -Limit 1
      $result | Should -Not -BeNullOrEmpty
      @($result).count | Should -Be 1
      $result.MovieName | Should -contain "Look Who's Talking"
    }

    It "Functions" {
      $result = Find-LiteDBDocument movies -Where "MPAA='PG-13'" -Select "{_id,concat:concat(string(_id),Title)}"
      $result | Should -Not -BeNullOrEmpty
      $result.count | Should -Be 2
      $result[0].Concat.count | Should -Be 2
      $result[0].Concat | Should -contain "Look Who's Talking"
      $result[1].Concat | Should -contain 'Parenthood'
    }

  }#context 'Named Queries'

}# find-litedbdocument

# Describe Update-LiteDBDocument
Describe "Update-LiteDBDocument" -Tag Update -Fixture {
  Context "Update By BSON Expression" -Fixture {
    It "Changes a property value" {
      $result = Find-LiteDBDocument movies -id 3
      $result.Title | Should -be "Turner & Hooch"
      Update-LiteDBDocument movies -Set "{Title:'Turner and Hooch'}"  -Where "_id = 3"
      $result = Find-LiteDBDocument movies -id 3
      $result.Title | Should -be "Turner and Hooch"
    }
  }
  Context "Update By SQL" -Fixture {
    It "Changes multiple property values" {
      $result = Find-LiteDBDocument movies -id 3
      $result.Title | Should -be "Turner and Hooch"
      $result.Runtime | Should -Be 100
      Update-LiteDBDocument 'movies' -sql "UPDATE movies SET Title='Turner & Hooch',RunTime = 101 where _id = 3"
      $result = Find-LiteDBDocument movies -id 3
      $result.Title | Should -be "Turner & Hooch"
      $result.RunTime | Should -Be 101
    }
  }
}

# Describe Remove-LiteDBDocument
Describe "Remove-LiteDBDocument" -Tag Delete, Remove -Fixture {
  Context "Remove By ID" -Fixture {
    It "Removes a document by ID" {
      $result = Find-LiteDBDocument movies -id 3
      $result.Title | Should -be "Turner & Hooch"
      Remove-LiteDBDocument -Collection movies -ID 3
      $result = Find-LiteDBDocument movies -id 3
      $result | Should -be $null
    }
  }
  Context "Remove By BSON Expression" -Fixture {
    It "removes document(s)" {
      $result = Find-LiteDBDocument movies -Sql "Select _id,Title,MPAA from movies where MPAA='PG-13'"
      $result.count | Should -be 2
      Remove-LiteDBDocument movies -Query "MPAA='PG-13'"
      $result = Find-LiteDBDocument movies -Sql "Select _id,Title,MPAA from movies where MPAA='PG-13'"
      $result | Should -be $null
    }
  }
  Context "Remove By SQL" -Fixture {
    It "Removes multiple documents" {
      $result = Find-LiteDBDocument movies "select _id,Title,ReleaseDate from movies where ReleaseDate > datetime('12/10/1989')"
      $result.count | Should -be 2
      Remove-LiteDBDocument 'movies' -Sql "delete movies where ReleaseDate > datetime('12/10/1989')"
      $result = Find-LiteDBDocument movies "select _id,Title,ReleaseDate from movies where ReleaseDate > datetime('12/10/1989')"
      $result | Should -be $null
    }
  }
}

# Describe Upsert-LiteDBDocument
Describe "Upsert-LiteDBDocument" -Tag Upsert -Fixture {
  Context "Upsert by ID" -Fixture {
    It "Adds missing documents" {
      $result = Find-LiteDBDocument movies
      $result.Count | Should -be 5
      $movies.count | Should -be 10
      $movies | ConvertTo-LiteDbBSON | Upsert-LiteDBDocument movies
      $result = Find-LiteDBDocument movies
      $result.Count | Should -be 10
    }
  }
}

# Describe Upsert-LiteDBDocument
Describe "Close-LiteDBConnection" -Tag Close -Fixture {
  It "Closes the connection" {
    Close-LiteDBConnection
    $LiteDBPSConnection | Should -BeNullOrEmpty
  }
}
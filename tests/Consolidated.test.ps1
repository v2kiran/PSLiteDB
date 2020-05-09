$modulename = "PSlitedb"
$psd1 = "{0}{1}" -f ($PSScriptRoot -replace "tests"), ("module\$modulename`.psd1")
ipmo $psd1 -Force

try
{
  # Create an in-memory database that we will re-use for all our describe blocks
  $null = Open-litedbconnection

  # will be used to populate the database
  $services = Get-Service b* | select @{Name = "_id"; E = { $_.Name } }, DisplayName, Status, StartType
  $svcCount = $services.count

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
      { New-LiteDBCollection -Collection SvcCollection } | Should -Not -Throw
      (Get-LiteDBCollectionName).Collection | Should -Contain "SvcCollection"
    }
  }

  # Describe New-LiteDBIndex
  Describe "New-LiteDBCollection" -Tag Index -Fixture {
    It "Creates a new Index" {
      { New-LiteDBIndex -Collection SvcCollection -Field DisplayName -Expression "LOWER($.DisplayName)" } | Should -Not -Throw
      (Get-LitedbIndex -Collection SvcCollection).Expression | Should -Contain "LOWER($.DisplayName)"
    }
  }

  # Describe Insert-LiteDBDocument
  Describe "Insert-LiteDBDocument" -Tag Insert -Fixture {
    It "inserts documents" {
      $bsondocs = ConvertTo-LiteDbBSON -i $services
      $bsondocs | Should -Not -BeNullOrEmpty -Because "PSObject to bson conversion should work"
      { $bsondocs | Add-LiteDBDocument -Collection SvcCollection } | Should -Not -Throw
    }
  }

  # Describe Find-LiteDBDocument
  Describe "Find-LiteDBDocument" -Tag Find -Fixture {
    Context "Find All" -Fixture {
      It "Finds all documents in the database" {
        $result = Find-LiteDBDocument -Collection SvcCollection
        $result.count | Should -Be $svcCount

        # second test - property names
        $proplist = ($result[0] | Get-Member -MemberType Properties).name
        foreach ($p in @("Collection", "DisplayName", "Starttype", "Status", "_id"))
        {
          $proplist | Should -contain $p
        }

        # third test - property value
        $result[0].collection | Should -Be 'SvcCollection'

        # 4th test - property value
        $result[0].displayname | Should -Not -BeNullOrEmpty
      }

      It "Limits the results returned with the Limit parameter" {
        $result = Find-LiteDBDocument -Collection SvcCollection -Limit 5
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 5
      }
    }# context find all

    Context "Find by ID" -Fixture {
      It "finds documents by ID" {
        $result = Find-LiteDBDocument -Collection SvcCollection -ID bits
        $result | Should -Not -BeNullOrEmpty
        $result.DisplayName | Should -Be "Background Intelligent Transfer Service"
      }
    }

    Context "Find by SQL" -Fixture {
      It "finds the first 5 documents" {
        $result = Find-LiteDBDocument SvcCollection -Sql "Select $ from SvcCollection limit 5"
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 5
        $result[0].collection | Should -Be 'SvcCollection'
        $result[0].Status | Should -Not -BeNullOrEmpty
      }

      It "finds the first 2 documents with select properties" {
        $result = Find-LiteDBDocument SvcCollection -Sql "Select _ID,Status from SvcCollection limit 2"
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 2
        $result[0].collection | Should -Be 'SvcCollection'
        $result[0].Status | Should -Not -BeNullOrEmpty
        $proplist = ($result[0] | Get-Member -MemberType Properties).name
        $proplist.count | Should -Be 3
        $proplist | Should -not -contain "StartType"
        $proplist | Should -not -contain "DisplayName"
      }

      It "Where clause" {
        $result = Find-LiteDBDocument SvcCollection -Sql "Select _id,DisplayName from SvcCollection Where _id like 'bth%'"
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 2
        $result.DisplayName | Should -contain 'Bluetooth Support Service'
        $result.DisplayName | Should -contain 'AVCTP service'
      }

      It "Functions" {
        $result = Find-LiteDBDocument SvcCollection -Sql "Select concat(_id,displayname) as concat,_id,DisplayName from SvcCollection where _id='bits'"
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 1
        $result.DisplayName | Should -contain 'Background Intelligent Transfer Service'
        $result.Concat.count | Should -Be 2
        $result.Concat | Should -Contain 'Background Intelligent Transfer Service'
        $result.Concat | Should -Contain 'Bits'
      }

    }#context sql

    Context "Named Queries" -Fixture {
      It "Where filter" {
        $result = Find-LiteDBDocument SvcCollection -Where "_id like 'Bth%'" -Select "{_id,DisplayName}"
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 2
        $result.DisplayName | Should -contain 'Bluetooth Support Service'
        $result.DisplayName | Should -contain 'AVCTP service'
      }

      It "Custom Columns" {
        $result = Find-LiteDBDocument SvcCollection -Where "_id like 'Bth%'" -Select "{_id,DN:DisplayName}"
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 2
        $result.DN | Should -contain 'Bluetooth Support Service'
        $result.DN | Should -contain 'AVCTP service'
      }

      It "Limit results" {
        $result = Find-LiteDBDocument SvcCollection -Where "_id like 'Bth%'" -Select "{_id,DN:DisplayName}" -Limit 1
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 1
        $result.DN | Should -Be 'AVCTP service'
      }

      It "Functions" {
        $result = Find-LiteDBDocument SvcCollection -Where "_id = 'Bits'" -Select "{_id,concat:concat(_id,displayname)}"
        $result | Should -Not -BeNullOrEmpty
        $result.count | Should -Be 1
        $result.Concat.count | Should -Be 2
        $result.Concat | Should -Contain 'Background Intelligent Transfer Service'
        $result.Concat | Should -Contain 'Bits'
      }
    }#context named queries

  }# find-litedbdocument

  # Describe Update-LiteDBDocument
  Describe "Update-LiteDBDocument" -Tag Update -Fixture {
    Context "Update By BSON Expression" -Fixture {
      It "Changes a property value" {
        $result = Find-LiteDBDocument SvcCollection -id bfe
        $result.status | Should -be "running"
        Update-LiteDBDocument SvcCollection -Set "{status:'stopped'}"  -Where "_id = 'bfe'"
        $result = Find-LiteDBDocument SvcCollection -id bfe
        $result.status | Should -be "stopped"
      }
    }
    Context "Update By SQL" -Fixture {
      It "Changes multiple property values" {
        $result = Find-LiteDBDocument SvcCollection -id bfe
        $result.status | Should -be "stopped"
        $result.starttype | Should -Be 'Automatic'
        Update-LiteDBDocument 'SvcCollection' -sql "UPDATE SvcCollection SET Status='Running',StartType = 'Manual' where _id = 'bfe'"
        $result = Find-LiteDBDocument SvcCollection -id bfe
        $result.status | Should -be "Running"
        $result.starttype | Should -Be 'Manual'
      }
    }
  }
  Remove-LiteDBDocument -Collection SvcCollection -ID BITS
  # Describe Remove-LiteDBDocument
  Describe "Remove-LiteDBDocument" -Tag Delete, Remove -Fixture {
    Context "Remove By ID" -Fixture {
      It "Removes a document by ID" {
        $result = Find-LiteDBDocument SvcCollection -id BthAvctpSvc
        $result.DisplayName | Should -be "AVCTP service"
        Remove-LiteDBDocument -Collection SvcCollection -ID BthAvctpSvc
        $result = Find-LiteDBDocument SvcCollection -id BthAvctpSvc
        $result | Should -be $null
      }
    }
    Context "Remove By BSON Expression" -Fixture {
      It "Changes a property value" {
        $result = Find-LiteDBDocument SvcCollection -id bfe
        $result._id | Should -be "bfe"
        Remove-LiteDBDocument SvcCollection -Query "DisplayName = 'Base Filtering Engine'"
        $result = Find-LiteDBDocument SvcCollection -id bfe
        $result | Should -be $null
      }
    }
    Context "Remove By SQL" -Fixture {
      It "Removes multiple documents" {
        $result = Find-LiteDBDocument SvcCollection "select _id,Displayname from svccollection where displayname like 'bluetooth%'"
        $result.count | Should -be 4
        Remove-LiteDBDocument 'SvcCollection' -Sql "delete SvcCollection where displayname like 'bluetooth%'"
        $result = Find-LiteDBDocument SvcCollection "select _id,Displayname from svccollection where displayname like 'bluetooth%'"
        $result | Should -be $null
      }
    }
  }

}#try
finally
{
  close-litedbconnection
}

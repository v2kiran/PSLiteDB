Import-Module 'C:\gh\PSLiteDB\module\PSLiteDB.psd1' -Force -Verbose

$dbPath = "C:\temp\temp\LiteDB\Movie2.db"
Remove-Item $dbPath
New-LiteDBDatabase -Path $dbPath -Verbose

Open-LiteDBConnection -Database $dbPath

New-LiteDBCollection -Collection movies

# verify that the collection was created
Get-LiteDBCollectionName

# Creates an index in the collection `movies` with all `DisplayName` property values in `lowercase`
New-LiteDBIndex -Collection movies -Field Genre -Expression "LOWER($.Genre)"

# verify that the index was created
Get-LiteDBIndex -Collection movies

# Sample dataset taken from [data-world](https://data.world/)
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

# Convert the dataset from CSV to PSObjects
# Since a csv converts all object properties to string we need to convert those string properties back to their original type
$movies = $dataset |
    ConvertFrom-Csv |
    Select-Object @{Name = "_id"; E = { [int]$_.MovieID } },
    @{Name = "Budget"; E = { [int64]$_.Budget } },
    @{Name = "Gross"; E = { [int64]$_.Gross } },
    @{Name = "ReleaseDate"; E = { [datetime]$_.'Release Date' } },
    @{Name = "RunTime"; E = { [int]$_.Runtime } },
    @{Name = "Rating"; E = { [Double]$_.Rating } },
    @{Name = "RatingCount"; E = { [int64]$_.RatingCount } },
    Title, MPAA

$movies |
    ConvertTo-LiteDbBSON |
    Add-LiteDBDocument -Collection movies

    $movie_array = $movies | ConvertTo-LiteDbBSON -as array

Add-LiteDBDocument 'movies' -BsonDocumentArray $movie_array -BatchSize 1000 -BulkInsert


Find-LiteDBDocument -Collection movies -ID 10
Find-LiteDBDocument -Collection movies -Limit 5 -Skip 2

Find-LiteDBDocument movies -Sql "Select $ from movies limit 5"

# get the first 5 documents with selected properties or fields from the movies collection
Find-LiteDBDocument movies -Sql "Select _id,Title from movies limit 5"

# using where to filter the results - string filter
Find-LiteDBDocument movies -Sql "Select _id,Title,MPAA from movies Where MPAA = 'PG-13'"

# using where to filter the results - greaterthan
Find-LiteDBDocument movies -Sql "Select _id,Title,Rating from movies Where Rating > 7.5"

# using1 multiple where filters. ( movies that contain 'talking' in their title)
Find-LiteDBDocument movies -Sql "Select _id,Title from movies where MPAA = 'PG-13' and Title like '%talking'"

# Sorting by name descending
Find-LiteDBDocument movies -Sql "Select _id,Title from movies where MPAA = 'PG-13' order by Title desc"

# date filter
Find-LiteDBDocument movies -Sql "select _id,Title,ReleaseDate from test where ReleaseDate > datetime('8/16/1989') order by Releasedate desc"

# using Functions
# get the first 5 documents with selected properties or fields from the movies collection
Find-LiteDBDocument movies -Sql "Select upper(Title),_id,MPAA from movies limit 5"

Update-ModuleManifest -Path 'C:\gh\PSLiteDB\module\PSLiteDB.psd1' -FunctionsToExport (gcm -module pslitedb | select -ExpandProperty name)
# :rocket: PSLiteDB

Pester Tests
---
[![GitHub Workflow - CI](https://github.com/v2kiran/PSLiteDB/workflows/CI/badge.svg)](https://github.com/v2kiran/PSLiteDB/actions?workflow=CI)

## :boom: OverView

[LiteDB](http://www.litedb.org/) is a noSQL singlefile datastore just like SQLite.
PSLiteDB is a PowerShell wrapper for LiteDB

>Note: in V5 everything is case in-sensitive

- Collection names are case-insensitive
- FieldNames or property names are case-insensitive
- FieldValues or property values are case-insensitive.

## :cyclone: Clone Module

```powershell
#Clone the repo in c:\temp
cd c:\temp
git clone https://github.com/v2kiran/PSLiteDB.git
```

***

## :droplet: Import Module

```powershell
Import-Module c:\temp\PSLiteDB\module\PSLiteDB.psd1 -verbose
```

***

## :palm_tree: Create Database

```powershell
$dbPath = "C:\temp\LiteDB\Movie.db"
New-LiteDBDatabase -Path $dbPath -Verbose
```

***

## :saxophone: Connect Database

- The connection to the first db is stored in a session variable called `$LiteDBPSConnection`.
- This connection variable is re-used in various cmdlets from this module making it efficient by having to type less.
- if you want to work with multiple databases then you will need to store each connection from `Open-LiteDBConnection` in a different variable and pass
that to each cmdlet's `Connection` parameter.
Check the [Wiki](https://github.com/v2kiran/PSLiteDB/wiki/Working-with-Multiple-Databases) for an example on how to work with multiple databases simultaneously.

```powershell
Open-LiteDBConnection -Database $dbPath
```

***

## :trumpet: Create a Collection

```powershell
New-LiteDBCollection -Collection movies

# verify that the collection was created
Get-LiteDBCollectionName
```

***

## :guitar: Create an Index

```powershell
# Creates an index in the collection `movies` with all `DisplayName` property values in `lowercase`
New-LiteDBIndex -Collection movies -Field Genre -Expression "LOWER($.Genre)"

# verify that the index was created
Get-LiteDBIndex -Collection movies
```

***

## :tada: Insert Records

Create a movie database from csv records/file.
Force the `MovieID` property to become the `_id` property in the LiteDB collection
Serialize the selected records and finally insert them into the `movies`

##### :one::arrow_forward: Insert by `ID`

```powershell

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
```

##### :two::arrow_forward: Bulk Insert

```powershell
$movie_array = $movies | ConvertTo-LiteDbBSON -as array

Add-LiteDBDocument 'movies' -BsonDocumentArray $movie_array -BatchSize 1000 -BulkInsert

```

> :point_right: **Note**:  The `ConvertTo-LiteDbBSON` Function returns a Bsondocument array which will be unrolled by the `Add-LitedbDocument` cmdlet by default so if you want to avoid that and add the entire array. Usefull when inserting a huge number of documents in batches.

***

## :snowflake: Find Records

Because we used the `MovieID` property of the `dataset` as our `_id` in the LiteDb collection, we can search for records using the `MovieID` value

##### :one::arrow_forward: Find by `ID`

```powershell
#Note that the value of parameter ID: 'BITS' is case-Insensitive
Find-LiteDBDocument -Collection movies -ID 10

Output:

Collection  : movies
_id         : 10
Title       : Lethal Weapon 2
ReleaseDate : 7/7/1989 12:00:00 AM
Budget      : 25000000
Rating      : 7.2
RatingCount : 151737
Gross       : 227853986
MPAA        : R
RunTime     : 114


<#
List all documents in a collection, limiting the total docs displayed to 5 and skipping the first 2.
'Limit' and 'skip' are optional parameters
By default if you omit the limit parameter only the first 1000 docs are displayed
#>
Find-LiteDBDocument -Collection movies -Limit 5 -Skip 2
```




##### :two::arrow_forward:  Find by `SQL Query`

```powershell
# get the first 5 documents from the movies collection
Find-LiteDBDocument movies -Sql "Select $ from movies limit 5"

# get the first 5 documents with selected properties or fields from the movies collection
Find-LiteDBDocument movies -Sql "Select _id,Title from movies limit 5"

# using where to filter the results - string filter
Find-LiteDBDocument movies -Sql "Select _id,Title from movies Where MPAA = 'PG-13'"

# using where to filter the results - greaterthan
Find-LiteDBDocument movies -Sql "Select _id,Title from movies Where Rating > 7.5"

# using multiple where filters. ( movies that contain 'talking' in their title)
Find-LiteDBDocument movies -Sql "Select _id,Title from movies where MPAA = 'PG-13' and Title like '%talking'"

# Sorting by name descending
Find-LiteDBDocument movies -Sql "Select _id,Title from movies where MPAA = 'PG-13' order by Title desc"

# date filter
Find-LiteDBDocument movies -Sql "select _id,Title,ReleaseDate from test where ReleaseDate > datetime('8/16/1989') order by Releasedate desc"

# using Functions
# get the first 5 documents with selected properties or fields from the movies collection
Find-LiteDBDocument movies -Sql "Select upper(Title),_id,MPAA from movies limit 5"

```


##### :three::arrow_forward: Find by `Named Queries`

```powershell
# Wildcard filter B*. Select 2 properties _id & status to display in the output
# Select is a mandatory parameter when used with -Where
Find-LiteDBDocument movies -Where "Title like '%talking'" -Select "{_id,Title,MPAA}"

# Calculated properties: show Title as MovieName
Find-LiteDBDocument movies -Where "Title like '%talking'" -Select "{_id,MovieName: Title}"

# limit: output to 2 documents
Find-LiteDBDocument movies -Where "Title like '%s%'" -Select "{_id,MovieName: Title}" -Limit 2

# Skip: skip first 2 documents
Find-LiteDBDocument movies -Where "Title like '%s%'" -Select "{_id,MovieName: Title}" -Limit 2 -Skip 2

# using Functions
Find-LiteDBDocument movies -Where "Title like '%talking'" -Select "{Name: _id,MovieName : UPPER(Title)}" -Limit 2

# for a list of other functions refer to : http://www.litedb.org/docs/expressions/

```
##### :four::arrow_forward: Find `All Documents`

By default when used with no other parameters the cmdlet lists all documents in the collection.

```powershell
Find-LiteDBDocument movies
```

***

## :beetle: Update records

##### :one::arrow_forward: Update by `SQL Query`

```powershell
#update multiple fields with a wildcard where query
Update-LiteDBDocument 'movies' -sql "UPDATE movies SET Title='Turner and Hooch',Runtime = 101 where _id=3"
```

lets stop the BITS service and then update the collection with the new `status`

##### :two::arrow_forward: Update by `Id`

```powershell
$mymovie = [PSCustomObject]@{
    Title = 'Turner and Hooch'
    _id = 3
    Budget = [int64]13000001
    Gross = [int64]71079915
    MPAA = 'PG'
    Rating = 7.2
    RatingCount = 91415
    ReleaseDate = [Datetime]'7/28/1989'
    RunTime = 101
    }

    $mymovie |  ConvertTo-LiteDbBSON | Update-LiteDBDocument -Collection movies

# retrieve the movie record in the litedb collection to see the updated status
Find-LiteDBDocument -Collection movies -ID 3

Output:


Collection  : movies
_id         : 3
Title       : Turner and Hooch
ReleaseDate : 7/28/1989 12:00:00 AM
Budget      : 13000001
Rating      : 7.2
RatingCount : 91415
Gross       : 71079915
MPAA        : PG
RunTime     : 101
```

##### :three::arrow_forward: Update by `BsonExpression`

The where statement is a predicate or condition which will determine the documents to be updated.

```powershell
# set the status property of the service named bfe to 4
Update-LiteDBDocument movies -Set "{Runtime:101}"  -Where "_id = 3"

# Retrieve all documents whose displayname begins with blue and Transform the name property to uppercase
Update-LiteDBDocument movies -set "{Title:UPPER(Title)}" -Where "Title like '%talking'"
```




***

## :no_entry: Delete Records

##### :one::arrow_forward: Delete by `Id`


```powershell
# Delete record by ID
Remove-LiteDBDocument -Collection movies -ID 3

# If we try to retrieve the record we should see a warning
 Find-LiteDBDocument -Collection movies -ID 3
WARNING: Document with ID ['3'] does not exist in the collection ['movies']
```

##### :two::arrow_forward: Delete by `BsonExpression`

```powershell
# delete all records from the movies collection where the property Budget is null
Remove-LiteDBDocument movies -Query "Budget = null"
```

##### :three::arrow_forward: Delete by `SQL Query`

```powershell
#Deleteall records from the service collectionwhose displayname matches'xbox live'
Remove-LiteDBDocument 'movies' -Sql "delete movies where Title like '%talking'"

```

***

## :sunrise: Upsert Records

Upsert stands for - Add if missing else update existing.

```powershell
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

$movies |
    ConvertTo-LiteDbBSON |
    Upsert-LiteDBDocument -Collection movies
```

***

## Query Filters

Using Query filters is not recomended anymore, it may be deprecated in future.

***


## :taxi: Close LiteDB Connection

```powershell
Close-LiteDBConnection
```

## WIKI

- [Create a password protected LiteDB database](https://github.com/v2kiran/PSLiteDB/wiki/Database-with-Password)
- [LiteDB Connection Options](https://github.com/v2kiran/PSLiteDB/wiki/Open-LiteDBConnection)
- [Speed-Test](https://github.com/v2kiran/PSLiteDB/wiki/Speed-test)
- [Work on Multiple Databases in parallel](https://github.com/v2kiran/PSLiteDB/wiki/Working-with-Multiple-Databases)

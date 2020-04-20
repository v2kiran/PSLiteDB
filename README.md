# :rocket: PSLiteDB
<!-- TOC -->

- [:rocket: PSLiteDB](#rocket-pslitedb)
  - [:boom: OverView](#boom-overview)
  - [:cyclone: Clone Module](#cyclone-clone-module)
  - [:droplet: Import Module](#droplet-import-module)
  - [:palm_tree: Create Database](#palmtree-create-database)
  - [:saxophone: Connect Database](#saxophone-connect-database)
  - [:trumpet: Create a Collection.](#trumpet-create-a-collection)
  - [:guitar: Create an Index.](#guitar-create-an-index)
  - [:tada: Insert Records](#tada-insert-records)
        - [:one::arrow_forward: Insert by `ID`](#onearrowforward-insert-by-id)
        - [:two::arrow_forward: Bulk Insert](#twoarrowforward-bulk-insert)
  - [:snowflake: Find Records](#snowflake-find-records)
        - [:one::arrow_forward: Find by `ID`](#onearrowforward-find-by-id)
        - [:two::arrow_forward:  Find by `SQL Query`](#twoarrowforward-find-by-sql-query)
        - [:three::arrow_forward: Find by `Named Queries`](#threearrowforward-find-by-named-queries)
        - [:four::arrow_forward: Listing all documents](#fourarrowforward-listing-all-documents)
  - [:beetle: Update records](#beetle-update-records)
        - [:one::arrow_forward: Update by `Id`](#onearrowforward-update-by-id)
        - [:two::arrow_forward: Update by `BsonExpression`](#twoarrowforward-update-by-bsonexpression)
  - [:no_entry: Delete Records](#noentry-delete-records)
        - [:one::arrow_forward: Delete by `Id`](#onearrowforward-delete-by-id)
        - [:two::arrow_forward: Delete by `BsonExpression`](#twoarrowforward-delete-by-bsonexpression)
  - [:sunrise: Upsert Records](#sunrise-upsert-records)
  - [Query Filters](#query-filters)
  - [:taxi: Close LiteDB Connection](#taxi-close-litedb-connection)
  - [WIKI](#wiki)

<!-- /TOC -->
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
$dbPath = "C:\temp\LiteDB\Service.db"
New-LiteDBDatabase -Path $dbPath -Verbose
```

***

## :saxophone: Connect Database
- :arrow_forward:  The connection to the first db is stored in a session variable called `$LiteDBPSConnection`.
- :arrow_forward:  This connection variable is re-used in various cmdlets from this module making it efficient by having to type less.
- :arrow_forward:  if you want to work with multiple databases then you will need to store each connection from `Open-LiteDBConnection` in a different variable and pass
that to each cmdlet's `Connection` parameter.
Check the [Wiki](https://github.com/v2kiran/PSLiteDB/wiki/Working-with-Multiple-Databases) for an example on how to work with multiple databases simultaneously.
```powershell
Open-LiteDBConnection -Database $dbPath
```

***

## :trumpet: Create a Collection.
```powershell
New-LiteDBCollection -Collection SvcCollection

# verify that the collection was created
Get-LiteDBCollectionName
```

***

## :guitar: Create an Index.
```powershell
# Creates an index in the collection `SvcCollection` with all `DisplayName` property values in `lowercase`
New-LiteDBIndex -Collection SvcCollection -Field DisplayName -Expression "LOWER($.DisplayName)"

# verify that the index was created
Get-LiteDBIndex -Collection SvcCollection
```

***

## :tada: Insert Records
Get all the services whose name starts with bfollowed by any sequence of characters.
Force the `Name` property to become the `_id` property in the LiteDB collection
Serialize the selected records and finally insert them into the `SvcCollection`

##### :one::arrow_forward: Insert by `ID`
```powershell
Get-Service b* |
  select @{Name="_id";E={$_.Name}},DisplayName,Status,StartType |
      ConvertTo-LiteDbBSON |
         Add-LiteDBDocument -Collection SvcCollection
```

##### :two::arrow_forward: Bulk Insert
```powershell
Get-Service b* |
  select @{Name="_id";E={$_.Name}},DisplayName,Status,StartType |
      ConvertTo-LiteDbBSON |
         Add-LiteDBDocument -Collection SvcCollection -Bulk -Batch 5000

```

> :dart: **Note**:  The `ConvertTo-LiteDbBSON` Function returns a Bsondocument array which will be unrolled by the `Add-LitedbDocument` cmdlet by default so if you want to avoid that and add the array as a whole you need to use the `-bulk` switch. Usefull when inserting a huge number of documents in batches.

***

## :snowflake: Find Records
Because we used the `Name` property of the `servicecontroller` object as our `_id` in the LiteDb collection, we can search for records using the `ServiceName`

##### :one::arrow_forward: Find by `ID`
```powershell
#Note that the value of parameter ID: 'BITS' is case-sensitive
Find-LiteDBDocument -Collection SvcCollection -ID BITS

Output:

Collection  : SvcCollection
_id         : "BITS"
DisplayName : "Background Intelligent Transfer Service"
Status      : 4
StartType   : 2

# just to illustrate that lowercase bits also now works( this didnt work in LiteDB v4)
Find-LiteDBDocument -Collection SvcCollection -ID bits


<#
List all documents in a collection, limiting the total docs displayed to 5 and skipping the first 2.
'Limit' and 'skip' are optional parameters
By default if you omit the limit parameter only the first 1000 docs are displayed
#>
Find-LiteDBDocument -Collection SvcCollection -Limit 5 -Skip 2
```




##### :two::arrow_forward:  Find by `SQL Query`
```powershell
# get the first 5 documents from the service collection
Find-LiteDBDocument SvcCollection -Sql "Select $ from SvcCollection limit 5"

# get the first 5 documents with selected properties or fields from the service collection
Find-LiteDBDocument SvcCollection -Sql "Select Name,Status from SvcCollection limit 5"

# using where to filter the results ( listing the first 5 stopped services)
Find-LiteDBDocument SvcCollection -Sql "Select Name,Status from SvcCollection Where Status = 1 limit 5"

# using where to filter the results - greaterthan
Find-LiteDBDocument SvcCollection -Sql "Select Name,Status from SvcCollection Where Status > 1 limit 5"

# using multiple where filters. ( stopped services along with a wildcard name filter)
Find-LiteDBDocument SvcCollection -Sql "Select Name,Status from SvcCollection Where Status = 1 and Name like 'app%' limit 5"

# Sorting by name descending
Find-LiteDBDocument SvcCollection -Sql "Select Name,Status from SvcCollection Where Status = 1 order by name desc limit 5"

# using Functions
# get the first 5 documents with selected properties or fields from the service collection
Find-LiteDBDocument SvcCollection -Sql "Select upper(Name),Status from SvcCollection limit 5"

```


##### :three::arrow_forward: Find by `Named Queries`
```powershell
# Wildcard filter B*. Select 2 properties _id & status to display in the output
# Select is a mandatory parameter when used with -Where
Find-LiteDBDocument SvcCollection -Where "DisplayName like 'B%'" -Select "{_id,Status}"

# Calculated properties: show status as svcstatus
Find-LiteDBDocument SvcCollection -Where "DisplayName like 'B%'" -Select "{_id,SvcStatus: Status}"

# limit: output to 2 documents
Find-LiteDBDocument SvcCollection -Where "DisplayName like 'B%'" -Select "{_id,SvcStatus: Status}" -Limit 2

# Skip: skip first 2 documents
Find-LiteDBDocument SvcCollection -Where "DisplayName like 'B%'" -Select "{_id,SvcStatus: Status}" -Limit 2 -Skip 2

# using Functions
Find-LiteDBDocument SvcCollection -Where "DisplayName like 'B%'" -Select "{Name: _id,DN : UPPER(DisplayName)}" -Limit 2

# for a list of other functions refer to : http://www.litedb.org/docs/expressions/

```
##### :four::arrow_forward: Listing all documents
By default when used with no other parameters the cmdlet lists all documents in the collection.
```powershell
Find-LiteDBDocument SvcCollection
```
***

## :beetle: Update records
lets stop the BITS service and then update the collection with the new `status`

##### :one::arrow_forward: Update by `Id`
```powershell
Get-Service BITS |
  Select @{Name="_id";E={$_.Name}},DisplayName,Status,StartType |
     ConvertTo-LiteDbBSON |
        Update-LiteDBDocument -Collection SvcCollection

# retrieve the bits service record in the litedb collection to see the updated status
Find-LiteDBDocument -Collection SvcCollection -ID BITS

Output:

Collection  : SvcCollection
_id         : "BITS"
DisplayName : "Background Intelligent Transfer Service"
Status      : 1
StartType   : 2
```
##### :two::arrow_forward: Update by `BsonExpression`
You can also use a sql statement now to update one or more records.
The where statement is a predicate or condition which will determine the documents to be updated.
```powershell
# set the status property of the service named bfe to 4
Update-LiteDBDocument SvcCollection -Set "{status:4}"  -Where "_id = 'bfe'"

# Retrieve all documents whose displayname begins with blue and Transform the name property to uppercase
Update-LiteDBDocument SvcCollection -set "{Name:UPPER(Name)}" -Where "DisplayName like 'blue%'"
```
***

## :no_entry: Delete Records

##### :one::arrow_forward: Delete by `Id`

```powershell
# Delete record by ID
Remove-LiteDBDocument -Collection SvcCollection -ID BITS

# If we now try to retrieve the `BITS` record we should see a warning
Find-LiteDBDocument -Collection SvcCollection -ID BITS
WARNING: Document with ID ['"BITS"'] does not exist in the collection ['SvcCollection']
```
##### :two::arrow_forward: Delete by `BsonExpression`
```powershell
# delete all records from the test2 collection where the property osname is null
Remove-LiteDBDocument test2 -Query "osname = null"
```

***

## :sunrise: Upsert Records
Upsert stands for - Add if not record exists or update if it does exist.
```powershell
Get-Service b* |
  select @{Name="_id";E={$_.Name}},DisplayName,Status,StartType |
      ConvertTo-LiteDbBSON |
         Upsertldb -Collection SvcCollection
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

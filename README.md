# PSLiteDB

## OverView
[LiteDB](http://www.litedb.org/) is a noSQL singlefile datastore just like SQLite.
PSLiteDB is a PowerShell wrapper for LiteDB
PSLiteDB has been compiled against the .NET Standard 2 which means you can use this module with both Windows PowerShell and [PowerShell Core](https://blogs.msdn.microsoft.com/powershell/2018/01/10/powershell-core-6-0-generally-available-ga-and-supported/).

Note: In LiteDB
- CollectionNames are case-insensitive
- FieldNames or property Names are case-sensitive
- FieldValues or PropertyValues are case-sensitive unless queried with an Index that has been explicity created with lowercase values.

## Clone Module

```powershell
#Clone the repo in c:\temp
cd c:\temp
git clone https://github.com/v2kiran/PSLiteDB.git
```

***

## New to PowerShell?
If you never used Powershell before you may get this error when you try to use the module:
"execution of scripts is disabled on this system"
Check this [link](https://stackoverflow.com/questions/4037939/powershell-says-execution-of-scripts-is-disabled-on-this-system)

Other Resources:
[Official microsoft Documentation](https://docs.microsoft.com/en-us/powershell/index?view=powershell-5.1)

***


## Import Module
```powershell
Import-Module c:\temp\PSLiteDB -verbose
```

***

## Create Database
```powershell
$dbPath = "C:\temp\LiteDB\Service.db"
New-LiteDBDatabase -Path $dbPath -Verbose
```

***

## Connect Database
The connection to the first db is stored in a session variable called `$LiteDBPSConnection`.
This connection variable is re-used in various cmdlets from this module making it efficient by having to type less.
if you want to work with multiple databases then you will need to store each connection from `Open-LiteDBConnection` in a different variable and pass
that to each cmdlet's `Connection` parameter.
Check the [Wiki](https://github.com/v2kiran/PSLiteDB/wiki/Working-with-Multiple-Databases) for an example on how to work with multiple databases simultaneously.
```powershell
Open-LiteDBConnection -Database $dbPath
```

***

## Create a Collection.
```powershell
New-LiteDBCollection -Collection SvcCollection

# verify that the collection was created
Get-LiteDBCollectionName
```

***

## Create an Index.
```powershell
# Creates an index in the collection `SvcCollection` with all `DisplayName` property values in `lowercase`
New-LiteDBIndex -Collection SvcCollection -Field DisplayName -Expression "LOWER($.DisplayName)"

# verify that the index was created
Get-LiteDBIndex -Collection SvcCollection
```

***

## Insert Records
Get all the services whose name starts with bfollowed by any sequence of characters.
Force the `Name` property to become the `_id` property in the LiteDB collection
Serialize the selected records and finally insert them into the `SvcCollection`
```powershell
Get-Service b* | 
  select @{Name="_id";E={$_.Name}},DisplayName,Status,StartType | 
      ConvertTo-LiteDbBSON | 
         Add-LiteDBDocument -Collection SvcCollection
```

***

## Find Records
Because we used the `Name` property of the `servicecontroller` object as our `_id` in the LiteDb collection, we can search for records using the `ServiceName`
```powershell
#Note that the value of parameter ID: 'BITS' is case-sensitive
Find-LiteDBDocument -Collection SvcCollection -ID BITS

Output:

Collection  : SvcCollection
_id         : "BITS"
DisplayName : "Background Intelligent Transfer Service"
Status      : 4
StartType   : 2

# just to illustrate that lowercase bits wont show up in the results
Find-LiteDBDocument -Collection P6 -ID bits
WARNING: Document with ID ['"bits"'] does not exist in the collection ['SvcCollection']

<#
List all documents in a collection, limiting the total docs displayed to 5 and skipping the first 2. 
'Limit' and 'skip' are optional parameters
By default if you omit the limit parameter only the first 1000 docs are displayed
#>
Find-LiteDBDocument -Collection SvcCollection -Limit 5 -Skip 2
```

***

## Update records
lets stop the BITS service and then update the collection with the new `status`
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

***

## Delete Records
```powershell
# Delete record by ID
Remove-LiteDBDocument -Collection SvcCollection -ID BITS

# If we now try to retrieve the `BITS` record we should see a warning
Find-LiteDBDocument -Collection SvcCollection -ID BITS
WARNING: Document with ID ['"BITS"'] does not exist in the collection ['SvcCollection']
```

***

## Upsert Records
Upsert stands for - Add if not record exists or update if it does exist.
```powershell
Get-Service b* | 
  select @{Name="_id";E={$_.Name}},DisplayName,Status,StartType | 
      ConvertTo-LiteDbBSON | 
         Upsertldb -Collection SvcCollection
```

***

## Custom Queries
By default the parameter values are case-sensitive 
```powershell
# Find all documents that contain the word 'Bluetooth' in the `SvcCollection` property `DisplayName`
New-LiteDBQuery -Field DisplayName -Value 'Bluetooth' -Operator Contains | Find-LiteDBDocument -Collection SvcCollection

# Find all records whose `Status` property greaterthan 3 meaning `started`
New-LiteDBQuery -Field Status -Value 3 -Operator GT | Find-LiteDBDocument -Collection SvcCollection

# Find all records whose `Status` property lessthan 3 meaning `stopped`
New-LiteDBQuery -Field Status -Value 3 -Operator LT | Find-LiteDBDocument -Collection SvcCollection

# Combining queries with AND ($QueryLDB is an alias for the litedb query class)
$And_Query = $QueryLDB::And($QueryLDB::StartsWith("DisplayName","Blue"),$QueryLDB::GT("Status",3))
Find-LiteDBDocument -Collection SvcCollection -Query $And_Query

# Combining queries with OR
$OR_Query = $QueryLDB::Or($QueryLDB::StartsWith("DisplayName","Blue"),$QueryLDB::Contains("DisplayName","Encryption"))
Find-LiteDBDocument -Collection SvcCollection -Query $OR_Query
```

***

## Close LiteDB Connection
```powershell
Close-LiteDBConnection
```

## WIKI
- [Create A LiteDB database that is passwordprotected](https://github.com/v2kiran/PSLiteDB/wiki/Database-with-Password)
- [LiteDB Connection Options](https://github.com/v2kiran/PSLiteDB/wiki/Open-LiteDBConnection)
- [Demo: PersonCollection](https://github.com/v2kiran/PSLiteDB/wiki/PersonCollection:-Demo-1)
- [Speed-Test](https://github.com/v2kiran/PSLiteDB/wiki/Speed-test)
- [Work on Multiple Databases in parallel](https://github.com/v2kiran/PSLiteDB/wiki/Working-with-Multiple-Databases)
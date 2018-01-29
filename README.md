# PSLiteDB

## OverView
[LiteDB](http://www.litedb.org/) is a noSQL singlefile datastore just like SQLite.
PSLiteDB is a PowerShell wrapper for LiteDB

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

## Import Module
```powershell
Import-Module c:\temp\PSLiteDB -verbose
```


## Create Database
```powershell
$dbPath = "C:\temp\LiteDB\Service.db"
New-LiteDBDatabase -Path $dbPath -Verbose
```

## Connect Database
```powershell
Open-LiteDBConnection -Database $dbPath
```

## Create a Collection.
```powershell
New-LiteDBCollection -Collection SvcCollection

# verify that the collection was created
Get-LiteDBCollectionName
```

## Create an Index.
```powershell
# Creates an index in the collection `SvcCollection` with all `DisplayName` property values in `lowercase`
New-LiteDBIndex -Collection SvcCollection -Field DisplayName -Expression "LOWER($.DisplayName)"

# verify that the index was created
Get-LiteDBIndex -Collection SvcCollection
```

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

## Find Records
Because we used the `Name` property of the `servicecontroller` object as our `_id` in the LiteDb collection, we can search for records using the service `Name`
```powershell
Find-LiteDBDocument -Collection SvcCollection -ID BITS

Output:

Collection  : SvcCollection
_id         : "BITS"
DisplayName : "Background Intelligent Transfer Service"
Status      : 4
StartType   : 2
```

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

## Delete Records
```powershell
# Delete record by ID
Remove-LiteDBDocument -Collection SvcCollection -ID BITS

# If we now try to retrieve the `BITS` record we should see a warning
Find-LiteDBDocument -Collection SvcCollection -ID BITS
WARNING: Document with ID ['"BITS"'] does not exist in the collection ['SvcCollection']
```

## Upsert Records
Upsert stands for - Add if not record exists or update if it does exist.
```powershell
Get-Service b* | 
  select @{Name="_id";E={$_.Name}},DisplayName,Status,StartType | 
      ConvertTo-LiteDbBSON | 
         Upsert-LiteDBDocument -Collection SvcCollection
```

## Custom Queries
by default the document values are case-sensitive 
```powershell
# Find all documents that contain the word 'Bluetooth' in the `SvcCollection` property `DisplayName`
New-LiteDBQuery -Field DisplayName -Value 'Bluetooth' -Operator Contains | Find-LiteDBDocument -Collection SvcCollection

# Find all records whose `Status` property greaterthan 3 meaning `started`
New-LiteDBQuery -Field Status -Value 3 -Operator GT | Find-LiteDBDocument -Collection SvcCollection

# Find all records whose `Status` property lessthan 3 meaning `stopped`
New-LiteDBQuery -Field Status -Value 3 -Operator LT | Find-LiteDBDocument -Collection SvcCollection

# Combining queries with AND ($QueryLDB is an alias for the litedb query class)
Find-LiteDBDocument -Collection SvcCollection -Query ($QueryLDB::And($QueryLDB::StartsWith("DisplayName","Blue"),$QueryLDB::GT("Status",3)))

# Combining queries with OR
Find-LiteDBDocument -Collection SvcCollection -Query ($QueryLDB::Or($QueryLDB::StartsWith("DisplayName","Blue"),$QueryLDB::Contains("DisplayName","Encryption")))
```
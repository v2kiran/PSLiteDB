# PSLiteDB



[LiteDB](http://www.litedb.org/) is a noSQL singlefile datastore just like SQLite.

PSLiteDB is a PowerShell wrapper for LiteDB

## How to use this Module

Clone the repo in c:\temp
```powershell
cd c:\temp
git clone https://github.com/v2kiran/PSLiteDB.git
```

Import the module
```powershell
Import-Module c:\temp\PSLiteDB -verbose
```

Create a new LiteDB database
```powershell
$dbPath = "C:\temp\LiteDB\Person.db"
New-LiteDBDatabase -Path $dbPath -Verbose
```

Connect to the database
```powershell
Open-LiteDBConnection -Database $dbPath
```

Create a Collection. Collections are like Tables in SQL
```powershell
New-LiteDBCollection -Collection PersonCollection_1
```

Create a `Person` Custom Object
```powershell
$PersonOBJ = [PSCustomObject]@{
    FirstName = "John"
    LastName  = "Doe"
    Age       = 22
    Occupation = "Deploper"
}
```

Person PSObject needs to be converted to BSON before it can be inserted into the LiteDB Collection
```powershell
$PersonBSON = $PersonOBJ | ConvertTo-LiteDbBson
```

Add the BSON document to the PersonCollection_1
```powershell
Add-LiteDBDocument -Collection Person -Document $PersonBSON
```

Check that the ocument was added
```powershell
Find-LiteDBDocument -Collection PersonCollection_1
```

Lets add a couple more documents
```powershell
@(
    [PSCustomObject]@{
        FirstName  = "Richard"
        LastName   = "Alpert"
        Age        = 90
        Occupation = "Gangster"
    } ,
    [PSCustomObject]@{
        FirstName  = "Elvis"
        LastName   = "Presley"
        Age        = 45
        Occupation = "Singer"
    } 
) | ConvertTo-LiteDbBson | Add-LiteDBDocument PersonCollection_1
```

Find records using powershell client-side filtering
by default the document values are case-sensitive 
```powershell
Find-LiteDBDocument PersonCollection_1 -as PSObject | Where Occupation -eq 'Gangster'
```

Remove a record from the collection
```powershell
Find-LiteDBDocument PersonCollection_1 | 
    Where-Object Occupation -eq 'Gangster' |
        Remove-LiteDBDocument
```


in the first document i.e Person "John Doe" has a typo in the Occupation 
string so lets correct that.

Find the document using LiteDB filtering and Output as BSONDocument
```powershell
$FirstDoc = Find-LiteDBDocument PersonCollection_1 -as BSON -Query ($QueryLDB::EQ("Occupation", "Deploper"))
```

Set the occupation property to the correct value
```powershell
$FirstDoc.Occupation = "Developer"
```

Update the collection
```powershell
Update-LiteDBDocument -Collection PersonCollection_1 -Document $FirstDoc
```

verify that the collection is updated
```powershell
Find-LiteDBDocument PersonCollection_1
```

lets see if we can add a duplicate record in the collection
```powershell
[PSCustomObject]@{
    FirstName  = "Elvis"
    LastName   = "Presley"
    Age        = 45
    Occupation = "Singer"
} | ConvertTo-LiteDbBson | Add-LiteDBDocument PersonCollection_1
```

the above succeeds because the `_id` autocrement property is unique for each document
so lets create an index and make sure we dont have duplicates
before we create the index we need to first remove all duplicates from the collection
```powershell
Remove-LiteDBDocument -Collection PersonCollection_1 -Query ($QueryLDB::EQ("FirstName", "Elvis"))
```


lets create an index on the `FirstName` property, enforce the Unique constraint
and also make the index lowercase
```powershell
New-LiteDBIndex -Collection PersonCollection_1 -Field "FirstName" -Expression "LOWER($.FirstName)" -Unique
```

lets re-add the documents
```powershell
@(
    [PSCustomObject]@{
        FirstName  = "Richard"
        LastName   = "Alpert"
        Age        = 90
        Occupation = "Gangster"
    } ,
    [PSCustomObject]@{
        FirstName  = "Elvis"
        LastName   = "Presley"
        Age        = 45
        Occupation = "Singer"
    } 
) | ConvertTo-LiteDbBson | Add-LiteDBDocument PersonCollection_1
```


if we try to run the same command again we will get error
```powershell
"cannot insert duplicate key in unique index 'FirstName'.
```


since we made the index lowercase we can now search using lowercase values
```powershell
Find-LiteDBDocument PersonCollection_1 -Query ($QueryLDB::EQ("FirstName", "elvis"))
```
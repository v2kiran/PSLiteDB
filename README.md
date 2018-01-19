# PSLiteDB


### **What is LiteDB**
[LiteDB](http://www.litedb.org/) is a noSQL singlefile datastore just like SQLite.

PSLiteDB is a PowerShell wrapper for LiteDB

## How to use this Module
### Clone the repo in c:\temp
`cd c:\temp`

`git clone https://github.com/v2kiran/PSLiteDB.git`

### Import the module
`Import-Module c:\temp\PSLiteDB -verbose`

### Create a new LiteDB database
`$dbPath = "C:\temp\LiteDB\Person.db"`
`New-LiteDBDatabase -Path $dbPath -Verbose`

### Connect to the database
`Open-LiteDBConnection -Database $dbPath`

### Create a Collection. Collections are like Tables in SQL
`New-LiteDBCollection -Collection PersonCollection_1`

### Create a `Person` Custom Object
`$PersonOBJ = [PSCustomObject]@{
    FirstName = "John"
    LastName  = "Doe"
    Age       = 22
    Occupation = "Deploper"
} `

### Person PSObject needs to be converted to BSON before it can be inserted into ### the LiteDB Collection
`$PersonBSON = $PersonOBJ | ConvertTo-LiteDbBson`

### Add the BSON document to the PersonCollection_1
Add-LiteDBDocument -Collection Person -Document $PersonBSON 

### Check that the ocument was added
### By default documents are output in BSON format but we can change that by 
### specifying the `-As` parameter with value `PSObject`
Find-LiteDBDocument -Collection PersonCollection_1  -As PSObject

### Lets add a couple more documents
`@(
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
) | ConvertTo-LiteDbBson | Add-LiteDBDocument PersonCollection_1`


### Find records using powershell client-side filtering
# by default the document values are case-sensitive 
Find-LiteDBDocument PersonCollection_1 -as PSObject | Where Occupation -eq 'Gangster'

### Remove a record from the collection
`Find-LiteDBDocument PersonCollection_1 -as PSObject | 
    Where-Object Occupation -eq 'Gangster' |
        Remove-LiteDBDocument `


### in the first document i.e Person "John Doe" has a typo in the Occupation 
### string so lets correct that.

### Find the document using LiteDB filtering and Output as BSONDocument
`$FirstDoc = Find-LiteDBDocument PersonCollection_1 -as BSON -Query ($QueryLDB::EQ("Occupation", "Deploper"))`

### set the occupation property to the correct value
`$FirstDoc.Occupation = "Developer"`

### Update the collection
Update-LiteDBDocument -Collection PersonCollection_1 -Document $FirstDoc

### verify that the collection is updated
`Find-LiteDBDocument PersonCollection_1`

### lets see if we can add a duplicate record in the collection
[PSCustomObject]@{
    FirstName  = "Elvis"
    LastName   = "Presley"
    Age        = 45
    Occupation = "Singer"
} | ConvertTo-LiteDbBson | Add-LiteDBDocument PersonCollection_1

### the above succeeds because the `_id` autocrement property is unique for each document
### so lets create an index and make sure we dont have duplicates

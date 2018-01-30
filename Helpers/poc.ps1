# Load the LiteDB dll 
$LiteDB_dll = Join-Path -Path $PSScriptRoot -ChildPath 'lib\LiteDB.dll'
Add-Type -Path $LiteDB_dll


# Create a  DB
$PersonDB = [LiteDB.LiteDatabase]::new("c:\temp\LiteDB\person.db")

# collection is like a table. Collections are created if they dont exist
$PersonCollection = $PersonDB.GetCollection("Person")

#BSON is extended JSON with type data
$bsondoc = [LiteDB.BsonDocument]::new()
$bsondoc["_id"] = [LiteDB.ObjectId]::NewObjectId()
$bsondoc["FirstName"] = "John"
$bsondoc["LastName"] = "Doe"
$bsondoc["Age"] = 22
$bsondoc["Occupation"] = "Developer"

$bsondoc1 = [LiteDB.BsonDocument]::new()
$bsondoc1["_id"] = [LiteDB.ObjectId]::NewObjectId()
$bsondoc1["FirstName"] = "Kiran"
$bsondoc1["LastName"] = "Reddy"
$bsondoc1["Age"] = 10
$bsondoc1["Occupation"] = "Devops"

#Insert BSON doc into the PersonDB
$PersonCollection.Insert($bsondoc)
$PersonCollection.Insert($bsondoc1)

#List all docs in the collection
$PersonCollection.FindAll()

#find a particular doc by a property name
# property values are CASE-SENISTIVE
$PersonCollection.Find([LiteDB.Query]::EQ("FirstName", "Kiran"))

# Create a 2nd collection in the same DB
$PersonCollection_1 = $PersonDB.GetCollection("Person1")

#Create an Index for faster lookups.
$PersonCollection_1.EnsureIndex('FirstName',$true)

$bsondoc0 = [LiteDB.BsonDocument]::new()
$bsondoc0["FirstName"] = "test"
$bsondoc0["LastName"] = "user"
$bsondoc0["Age"] = 25
$bsondoc0["Occupation"] = "Teacher"

#BSON doc with a custom INT ID
$bsondoc2 = [LiteDB.BsonDocument]::new()
$bsondoc2["_id"] = 2
$bsondoc2["FirstName"] = "TEST-1"
$bsondoc2["LastName"] = "User"
$bsondoc2["Age"] = 30
$bsondoc2["Occupation"] = "Scientist"


#BSON doc with a custom String ID
$bsondoc3 = [LiteDB.BsonDocument]::new()
$bsondoc3["_id"] = 'str'
$bsondoc3["FirstName"] = "Test-2"
$bsondoc3["LastName"] = "User"
$bsondoc3["Age"] = 32
$bsondoc3["Occupation"] = "Manager"

#Insert 3 docs into 2nd collection - Person1
$PersonCollection_1.Insert($bsondoc0)
$PersonCollection_1.Insert($bsondoc2)
$PersonCollection_1.Insert($bsondoc3)

# FInd a doc using its ID
$PersonCollection_1.FindById(2)
$PersonCollection_1.FindById('str')

#insert doc into 2nd collection
$PersonCollection_1.Insert($bsondoc)

ipmo "C:\Github\Public\dev\PSLiteDB" -Force
Get-Module pslitedb | select -ExpandProperty ExportedVariables
$dbPath = "C:\temp\LiteDB\Person.db"
New-LiteDBDatabase -Path $dbPath -Verbose 

### Connect to the database
Open-LiteDBConnection -Database $dbPath

New-LiteDBCollection -Collection PersonCollection_1

$PersonOBJ = [PSCustomObject]@{
    FirstName = "John"
    LastName  = "Doe"
    Age       = 22
    Occupation = "Deploper"
} 

#Person PSObject needs to be converted to BSON before it can be inserted into the 
#LiteDB Collection
$PersonBSON = $PersonOBJ | ConvertTo-LiteDbBson 

Add-LiteDBDocument -Collection PersonCollection_1 -Document $PersonBSON 

Find-LiteDBDocument -Collection PersonCollection_1 -as PSObject

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


Find-LiteDBDocument PersonCollection_1 | 
    Where-Object Occupation -eq 'Deploper' |
        Remove-LiteDBDocument 




Find-LiteDBDocument PersonCollection_1
$FirstDoc = Find-LiteDBDocument PersonCollection_1 -Query ($QueryLDB::EQ("Occupation", "Deploper")) -as BSON 
$FirstDoc["Occupation"] = "Developer"

$FirstDoc | % { "hey $_"}

Update-LiteDBDocument -Collection PersonCollection_1 -Document $FirstDoc

$FirstDoc | ConvertTo-LiteDbBson | Update-LiteDBDocument -Collection PersonCollection_1

Get-LiteDBIndex -Collection PersonCollection_1

New-LiteDBIndex -Collection PersonCollection_1 -Field "FirstName" -Unique

Find-LiteDBDocument PersonCollection_1 -Query ($QueryLDB::EQ("FirstName", "Elvis")) -as BSON |
select -First 1 | Remove-LiteDBDocument

Get-LiteDBIndex PersonCollection_1
Remove-LiteDBIndex PersonCollection_1 -Field FirstName

New-LiteDBIndex -Collection PersonCollection_1 -Field "FirstName" -Expression "LOWER($.FirstName)" -Unique

Find-LiteDBDocument PersonCollection_1 -Query ($QueryLDB::EQ("FirstName", "elvis"))


$bsonarray = @(
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
) | ConvertTo-LiteDbBson

Add-LiteDBDocument -Collection p2 -BsonDocumentArray $bsonarray -BulkInsert

# time - 12 min
$bsonarray = [System.Collections.ArrayList]::new()
foreach ($i in 1..30000) 
{
    $bsonarray.Add(  
        ([PSCustomObject]@{
                FirstName  = "user-$i"
                LastName   = "lastname-$i"
                Age        = $i
                Occupation = "Singer$i"
            } | ConvertTo-LiteDbBson))
}

Add-LiteDBDocument -Collection p2 -BsonDocumentArray $bsonarray -BulkInsert


# time - 42 sec
$bsonarray = [System.Collections.ArrayList]::new()
foreach ($i in 1..30000) 
{
    $bsonarray.Add(  
        (
            [LiteDB.JsonSerializer]::Deserialize(
                (
                    [PSCustomObject]@{
                        FirstName  = "user-$i"
                        LastName   = "lastname-$i"
                        Age        = $i
                        Occupation = "Singer$i"
                    } | ConvertTo-Json
                )
            )
        )
    )
}

Add-LiteDBDocument -Collection p2 -BsonDocumentArray $bsonarray -BulkInsert


$d = gsv b* | ConvertTo-LiteDBBSON3 
$d.GetType()
$d | gm

$d1 = gsv b* | ConvertTo-LiteDBBSON3 -as Document
$d1.GetType()
$d1 | gm

Remove-LiteDBCollection -Collection p2 

1..30000 | % {
    [PSCustomObject]@{
        FirstName  = "user-{0}" -f $_
        LastName   = "lastname-{0}" -f $_
        Age        = $_
        Occupation = "Singer{0}" -f $_
    }
} | ConvertTo-LiteDBBSON3 |Add-LiteDBDocument -Collection p2 


gsv |  ConvertTo-LiteDBBSON3 |Add-LiteDBDocument -Collection p3


$dbPath = "C:\temp\LiteDB\speedtest.db"
New-LiteDBDatabase -Path $dbPath -Verbose

#Connect Database
Open-LiteDBConnection -Database $dbPath

#Create a Collection.
New-LiteDBCollection -Collection P1

#Insert 30000 records
1..30000 | % {
    [PSCustomObject]@{
        FirstName  = "user-{0}" -f $_
        LastName   = "lastname-{0}" -f $_
        Age        = $_
        Occupation = "Singer{0}" -f $_
    }
} | ConvertTo-LiteDBBSON | Add-LiteDBDocument -Collection P1
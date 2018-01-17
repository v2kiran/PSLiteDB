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




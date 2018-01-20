ipmo "C:\Github\Public\dev\PSLiteDB" -Force
$dbPath = "C:\temp\LiteDB\Person.db"
Open-LiteDBConnection -Database $dbPath
$per = $LiteDBPSConnection.GetCollection("Personcollection_1")

$source = 'C:\Github\Public\dev\PSLiteDB\src\PSLiteDB\bin\Debug\netstandard2.0\PSLiteDB.dll'
$dest = 'C:\Github\Public\dev\PSLiteDB\lib\PSLiteDB.dll'
Copy-Item $source $dest -Force
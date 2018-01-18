# PSLiteDB


### **What is LiteDB**
[LiteDB](http://www.litedb.org/) is a noSQL singlefile datastore just like SQLite.

PSLiteDB is a PowerShell wrapper for LiteDB

## How to use this Module
### Clone the repo
`cd c:\temp`

`git clone https://github.com/v2kiran/PSLiteDB.git`

### Import the module
`Import-Module c:\temp\PSLiteDB -verbose`

### Create a new LiteDB database
`New-LiteDBDatabase -Path C:\temp\LiteDB\test2.db -Verbose`

### Connect to the database
`Open-LiteDBConnection -Database C:\temp\LiteDB\test2.db`





---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Find-LiteDBDocument

## SYNOPSIS
Find document(s) within a litedb database.

## SYNTAX

### All (Default)
```
Find-LiteDBDocument [-Collection] <String> [-Skip <Int32>] [-Limit <Int32>] [-As <String>]
 [-Connection <LiteDatabase>] [<CommonParameters>]
```

### ID
```
Find-LiteDBDocument [-Collection] <String> -ID <BsonValue> [-Skip <Int32>] [-Limit <Int32>] [-As <String>]
 [-Connection <LiteDatabase>] [<CommonParameters>]
```

### Query
```
Find-LiteDBDocument [-Collection] <String> [-Skip <Int32>] [-Limit <Int32>] [-As <String>] [-Where <String>]
 -Select <String> [-Connection <LiteDatabase>] [<CommonParameters>]
```

### Sql
```
Find-LiteDBDocument [-Collection] <String> [-Skip <Int32>] [-Limit <Int32>] [-As <String>] [-Sql] <String>
 [-Connection <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
Find document(s) within a litedb database.

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-LiteDBDocument -Collection movies
```

List all documents inside the movies collection.

### Example 2
```powershell
PS C:\> Find-LiteDBDocument -Collection movies -Limit 5
```

List the first 5 documents inside the movies collection.


### Example 3
```powershell
PS C:\> Find-LiteDBDocument -Collection movies -ID 6
```

List document with ID 6 inside the movies collection.


### Example 4
```powershell
PS C:\> Find-LiteDBDocument movies -Sql "Select $ from movies limit 5"
```

List the first 5 documents inside the movies collection using a SQL statement.


### Example 5
```powershell
PS C:\> Find-LiteDBDocument movies -Sql "Select _id,Title,MPAA from movies where MPAA='PG-13'"
```

List all documents inside the movies collection whose MPAA property value matches 'PG-13'.
Note: We only retrieve a select few properties(_id,Title,MPAA) of the collection.


### Example 6
```powershell
PS C:\> Find-LiteDBDocument movies -Sql "Select concat(string(_id),MPAA) as concat,_id,MPAA from movies where MPAA='PG-13'"
```

Concatenate 2 fields _id & MPAA and show that as a new calculated field 'concat'
Note: this does not actually add the field concat to the collection.

### Example 7
```powershell
PS C:\> Find-LiteDBDocument movies -Where "MPAA='PG-13'" -Select "{_id,Title,MPAA}"
```

List 3 properties of all documents that match the where criteria.

### Example 6
```powershell
PS C:\> Find-LiteDBDocument movies -Where "MPAA='PG-13'" -Select "{_id,MovieName:Title}"
```

Show the field Title as MovieName.
Note: this does not actually rename the field Title to moviename.



## PARAMETERS

### -As
List objects in either of 2 formats:
PSObject or BSON document.

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: PSObject, BSON

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Collection
The collection or table name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Connection
The litedb connection object.

```yaml
Type: LiteDatabase
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ID
The ID of the document to be found.

```yaml
Type: BsonValue
Parameter Sets: ID
Aliases: _id

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Limit
The number of documents to be listed.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Select
BSON expression to select properties\fields to show.

```yaml
Type: String
Parameter Sets: Query
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Skip
Ignores the specified number of objects and then gets the remaining objects.
Enter the number of objects to skip.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Sql
Find documents by a SQL statement.

```yaml
Type: String
Parameter Sets: Sql
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Where
Where filter criteria by BSOn expression.

```yaml
Type: String
Parameter Sets: Query
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

### LiteDB.BsonValue

### System.Int32

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

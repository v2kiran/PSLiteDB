---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Update-LiteDBDocument

## SYNOPSIS
Update a litedb document.

## SYNTAX

### sql (Default)
```
Update-LiteDBDocument [-Collection] <String> [-Connection <LiteDatabase>] [<CommonParameters>]
```

### ID
```
Update-LiteDBDocument [-Collection] <String> [-ID] <BsonValue> -Document <BsonDocument>
 [-Connection <LiteDatabase>] [<CommonParameters>]
```

### Array
```
Update-LiteDBDocument [-Collection] <String> [-BsonDocumentArray] <BsonDocument[]> [-Connection <LiteDatabase>]
 [<CommonParameters>]
```

### Document
```
Update-LiteDBDocument [-Collection] <String> -Document <BsonDocument> [-Connection <LiteDatabase>]
 [<CommonParameters>]
```

### Query
```
Update-LiteDBDocument [-Collection] <String> [-Set] <BsonExpression> [-Where] <BsonExpression>
 [-Connection <LiteDatabase>] [<CommonParameters>]
```

### Sql
```
Update-LiteDBDocument [-Collection] <String> [-Sql] <String> [-Connection <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
Update a litedb document.

## EXAMPLES

### Example 1
```powershell
PS C:\> $mymovie = [PSCustomObject]@{
    Title = 'Turner and Hooch'
    _id = 3
    Budget = [int64]13000001
    Gross = [int64]71079915
    MPAA = 'PG'
    Rating = 7.2
    RatingCount = 91415
    ReleaseDate = [Datetime]'7/28/1989'
    RunTime = 101
    }

    $mymovie |  ConvertTo-LiteDbBSON | Update-LiteDBDocument -Collection movies
```

Updates document with id 3(by ID)

### Example 2
```powershell
PS C:\> Update-LiteDBDocument movies -Set "{Title:'Turner and Hooch'}"  -Where "_id = 3"
```

Updates document with id 3(by BSON expression). The title property value is changed.

### Example 3
```powershell
PS C:\> Update-LiteDBDocument 'movies' -sql "UPDATE movies SET Title='Turner & Hooch',RunTime = 101 where _id = 3"
```

Update multiple fields of a document with ID 3(by SQL)



## PARAMETERS

### -BsonDocumentArray
Update multiple documents by passing a bson document array.

```yaml
Type: BsonDocument[]
Parameter Sets: Array
Aliases:

Required: True
Position: 1
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

### -Document
The bson document to be updated.

```yaml
Type: BsonDocument
Parameter Sets: ID, Document
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -ID
The ID of the bson document to be updated.

```yaml
Type: BsonValue
Parameter Sets: ID
Aliases: _id

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Set
The BSON expression for updating a document(s)

```yaml
Type: BsonExpression
Parameter Sets: Query
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Sql
The SQL expression for updating a document(s)

```yaml
Type: String
Parameter Sets: Sql
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Where
The BSON expression for updating a document(s)

```yaml
Type: BsonExpression
Parameter Sets: Query
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

### LiteDB.BsonValue

### LiteDB.BsonDocument[]

### LiteDB.BsonDocument

### LiteDB.BsonExpression

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

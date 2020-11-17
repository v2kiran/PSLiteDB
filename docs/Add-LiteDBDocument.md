---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Add-LiteDBDocument

## SYNOPSIS
Add\Insert one or more BSON documents into a litedb database.

## SYNTAX

### Document (Default)
```
Add-LiteDBDocument [-Collection] <String> [-Document] <BsonDocument> [-Connection <LiteDatabase>]
 [<CommonParameters>]
```

### ID
```
Add-LiteDBDocument [-Collection] <String> [-ID] <BsonValue> [-Document] <BsonDocument>
 [-Connection <LiteDatabase>] [<CommonParameters>]
```

### Array
```
Add-LiteDBDocument [-Collection] <String> [-BsonDocumentArray] <BsonDocument[]> [-BulkInsert]
 [-BatchSize <Int32>] [-Connection <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
Add\Insert one or more BSON documents into a litedb database.

## EXAMPLES

### Example 1
```powershell
PS C:\> $mymovie = [PSCustomObject]@{
    Title = 'Turner & Hooch'
    _id = 3
    Budget = [int64]13000001
    Gross = [int64]71079915
    MPAA = 'PG'
    Rating = 7.2
    RatingCount = 91415
    ReleaseDate = [Datetime]'7/28/1989'
    RunTime = 101
    }

    $mymovie |  ConvertTo-LiteDbBSON | Add-LiteDBDocument -Collection movies
```

add a powershell customobject containing movie data into a collection named 'movies'.
Note: this assumes that the litedb database already contains a collection named 'movies'.
Before we can add a document to the litedb the powershell object needs to be converted into a format(BSONDocument) that Litedb can understand.

## PARAMETERS

### -BatchSize
The number of documents to insert in one single batch

```yaml
Type: Int32
Parameter Sets: Array
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -BsonDocumentArray
Specify an array of BSON documents to insert. Useful in bulk insert scenarios.

```yaml
Type: BsonDocument[]
Parameter Sets: Array
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -BulkInsert
use with bsonarray for bulk inserting documents.

```yaml
Type: SwitchParameter
Parameter Sets: Array
Aliases:

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

### -Document
The BSON document to add to the litedb database.

```yaml
Type: BsonDocument
Parameter Sets: Document, ID
Aliases:

Required: True
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -ID
specify the id of the bson document to be added.

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

### LiteDB.BsonValue

### LiteDB.BsonDocument[]

### System.Management.Automation.SwitchParameter

### System.Int32

### LiteDB.BsonDocument

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

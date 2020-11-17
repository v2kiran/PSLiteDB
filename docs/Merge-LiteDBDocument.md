---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Merge-LiteDBDocument

## SYNOPSIS
Update an existing document or if it is missing add to the collection.

## SYNTAX

### Document (Default)
```
Merge-LiteDBDocument [-Collection] <String> [-Document] <BsonDocument> [-Connection <LiteDatabase>]
 [<CommonParameters>]
```

### ID
```
Merge-LiteDBDocument [-Collection] <String> [-ID] <BsonValue> [-Document] <BsonDocument>
 [-Connection <LiteDatabase>] [<CommonParameters>]
```

### Array
```
Merge-LiteDBDocument [-Collection] <String> [-BsonDocumentArray] <BsonDocument[]> [-Connection <LiteDatabase>]
 [<CommonParameters>]
```

## DESCRIPTION
Update an existing document or if it is missing add to the collection.

## EXAMPLES

### Example 1
```powershell
PS C:\> $movies | ConvertTo-LiteDbBSON | Upsert-LiteDBDocument movies
```

Update existing and add missing documents into the movies collection.

## PARAMETERS

### -BsonDocumentArray
An array of BSON documents to update.

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
The bson document to upsert.

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
The ID of the BSON document.

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

### LiteDB.BsonDocument

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

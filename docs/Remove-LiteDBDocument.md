---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Remove-LiteDBDocument

## SYNOPSIS
Remove\delete a litedb document.

## SYNTAX

### ID (Default)
```
Remove-LiteDBDocument [-Collection] <String> [-ID] <BsonValue> [-Connection <LiteDatabase>]
 [<CommonParameters>]
```

### Query
```
Remove-LiteDBDocument [-Collection] <String> [-Query] <BsonExpression> [-Connection <LiteDatabase>]
 [<CommonParameters>]
```

### sql
```
Remove-LiteDBDocument [-Collection] <String> [-Sql] <String> [-Connection <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
Remove\delete a litedb document.

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-LiteDBDocument -Collection movies -ID 3
```

Remove a document by ID

### Example 2
```powershell
PS C:\> Remove-LiteDBDocument movies -Query "MPAA='PG-13'"
```

Remove a document(s) by BSON expression.
All documents whose MPAA field matches the value 'PG-13' will be removed.

### Example 3
```powershell
PS C:\> Remove-LiteDBDocument 'movies' -Sql "delete movies where ReleaseDate > datetime('12/10/1989')"
```

Remove a document(s) by SQL statement.
Documents greater than a certain datetime are removed with this sql query.


## PARAMETERS

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
The document ID.

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

### -Query
BSON expression that forms the delete query.

```yaml
Type: BsonExpression
Parameter Sets: Query
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Sql
SQL expression that forms the delete query.

```yaml
Type: String
Parameter Sets: sql
Aliases:

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

### LiteDB.BsonExpression

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

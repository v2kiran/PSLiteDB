---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Remove-LiteDBIndex

## SYNOPSIS
Remove\Delete a litedb index.

## SYNTAX

```
Remove-LiteDBIndex [-Collection] <String> [-Field] <String> [[-Connection] <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
Remove\Delete a litedb index.

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-LiteDBIndex -Collection movies -Field Genre
```

Removes index associated with field Genre.

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
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Field
The indexed field name.

```yaml
Type: String
Parameter Sets: (All)
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

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

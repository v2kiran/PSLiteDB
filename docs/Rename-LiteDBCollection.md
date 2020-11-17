---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Rename-LiteDBCollection

## SYNOPSIS
Rename a collection.

## SYNTAX

```
Rename-LiteDBCollection [-Collection] <String> [-NewCollection] <String> [[-Connection] <LiteDatabase>]
 [<CommonParameters>]
```

## DESCRIPTION
Rename a collection.

## EXAMPLES

### Example 1
```powershell
PS C:\> Rename-LiteDBCollection -Collection moviecol -NewCollection moviecol2
```

Rename a collection named 'moviecol' to 'moviecol2'

## PARAMETERS

### -Collection
The old collection or table name.

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

### -NewCollection
The new collection name.

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

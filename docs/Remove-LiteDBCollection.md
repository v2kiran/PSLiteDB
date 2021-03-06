---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Remove-LiteDBCollection

## SYNOPSIS
Remove or delete a collection.

## SYNTAX

```
Remove-LiteDBCollection [-Collection] <String[]> [[-Connection] <LiteDatabase>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Remove or delete a collection.

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-LiteDBCollection -Collection movies
```

Caution: Removes the collection named 'movies' along with all data stored inside the collection.

## PARAMETERS

### -Collection
The collection to be removed

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Connection
The litedb connection object.

```yaml
Type: LiteDatabase
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

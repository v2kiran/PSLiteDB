---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Close-LiteDBConnection

## SYNOPSIS
Close\Dispose an open litedb connection.

## SYNTAX

```
Close-LiteDBConnection [[-Connection] <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
Close\Dispose an open litedb connection.

## EXAMPLES

### Example 1
```powershell
PS C:\> Close-LiteDBConnection
```

This will close the default litedb connection stored in the hidden variable - $LiteDBPSConnection

### Example 2
```powershell
PS C:\> Close-LiteDBConnection -Connection $myconnection
```

This will close the litedb connection stored in the variable - $myconnection

## PARAMETERS

### -Connection
The litedbconnection object.

```yaml
Type: LiteDatabase
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Get-LiteDBIndex

## SYNOPSIS
List all indices for a litedb database.

## SYNTAX

```
Get-LiteDBIndex [-Collection] <String> [[-Connection] <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
List all indices for a litedb database.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-LiteDBIndex -Collection 'one'

collection : one
Connection : LiteDB.LiteDatabase
name       : _id
expression : $._id
unique     : True
maxLevel   : 7
```

 _id field which is mandatory is indexed by default.

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

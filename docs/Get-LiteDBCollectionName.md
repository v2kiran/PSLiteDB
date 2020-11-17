---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Get-LiteDBCollectionName

## SYNOPSIS
List all collections in a litedb database.

## SYNTAX

```
Get-LiteDBCollectionName [[-Connection] <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
List all collections in a litedb database.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-LiteDBCollectionName

Connection          collection   AutoId Docs
----------          ----------   ------ ----
LiteDB.LiteDatabase one        ObjectId    3
LiteDB.LiteDatabase test       ObjectId    0
LiteDB.LiteDatabase movies     ObjectId    0
```

shows 3 collections along with the document count in each collection.

## PARAMETERS

### -Connection
The litedb connection object.

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

### Collection

## NOTES

## RELATED LINKS

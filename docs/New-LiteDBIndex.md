---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# New-LiteDBIndex

## SYNOPSIS
Create a new LiteDb index.

## SYNTAX

```
New-LiteDBIndex [-Collection] <String> [-Field] <String> [-Unique] [-Expression <String>]
 [[-Connection] <LiteDatabase>] [<CommonParameters>]
```

## DESCRIPTION
Create a new LiteDb index.
Even if multiple indexed expressions are used on a query, only one of the indexes is used, with the remaining expressions being filtered using a full scan.
Index keys must have at most 1023 bytes
Up to 255 indexes per collections, including the _id primary key, but limited to 8096 bytes for index definition. Each index uses: 41 bytes + LEN(name) + LEN(expression)

## EXAMPLES

### Example 1
```powershell
PS C:\> New-LiteDBIndex -Collection movies -Field Genre -Expression "LOWER($.Genre)"
```

Creates an index on the 'genre' field with the values in lowercase.

### Example 2
```powershell
PS C:\> New-LiteDBIndex -Collection movies -Field Total -Expression "SUM($.Items[*].Price)"
```

Creates an index on the 'Total' field which is a calculated field.
Items is a field that contains an array of bson documents with one of the properties in the embedded documents being price.

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

### -Expression
The expression based index that is to be created.
see http://www.litedb.org/docs/indexes/ &
http://www.litedb.org/docs/expressions/

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Field
The field\property name in the collection that is to be indexed.

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

### -Unique
Speicy if the indexed field should allow duplicates.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

### System.Management.Automation.SwitchParameter

### LiteDB.LiteDatabase

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
[expressions](http://www.litedb.org/docs/expressions/)
[indexes](http://www.litedb.org/docs/indexes/)

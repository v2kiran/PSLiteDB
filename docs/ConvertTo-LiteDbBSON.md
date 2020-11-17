---
external help file: PSLiteDB.dll-help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# ConvertTo-LiteDbBSON

## SYNOPSIS
Convert psobjects into LiteDB compatible BSON documents key-value pair.

## SYNTAX

```
ConvertTo-LiteDbBSON [-InputObject] <Object[]> [[-Depth] <UInt16>] [[-As] <String>] [<CommonParameters>]
```

## DESCRIPTION
Convert psobjects into LiteDB compatible BSON documents key-value pair.

## EXAMPLES

### Example 1
```powershell
PS C:\> PS C:\> $mymovie = [PSCustomObject]@{
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

    $mymovie |  ConvertTo-LiteDbBSON
```

converts the psobject stored in the variable $mymovie into a bsondocument(key,value)pair.

## PARAMETERS

### -As
Output as a single document or as an array(useful when you want to do add\insert documents in batches)

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: Document, Array

Required: False
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Depth
the depth of serialization of psobjetcs.

```yaml
Type: UInt16
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -InputObject
psobject to be converted to bson document.

```yaml
Type: Object[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object[]

### System.UInt16

### System.String

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

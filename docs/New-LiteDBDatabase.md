---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# New-LiteDBDatabase

## SYNOPSIS
Creates a new litedb database on the file system.

## SYNTAX

```
New-LiteDBDatabase [-Path] <String> [-Credential <PSCredential>] [<CommonParameters>]
```

## DESCRIPTION
Creates a new litedb database on the file system.

## EXAMPLES

### Example 1
```powershell
PS C:\> New-LiteDBDatabase -Path c:\temp\movie.db
```

creates a new litedb database at c:\temp\movie.db

### Example 2
```powershell
PS C:\> New-LiteDBDatabase -Path c:\temp\movie.db -Credential (Get-Credential)
```

creates a new litedb database at c:\temp\movie.db with password protection.
In order to open the database you would have to provide the password used when the db was created.

## PARAMETERS

### -Credential
Credential to secure the database

```yaml
Type: PSCredential
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
Path to LiteDB database

```yaml
Type: String
Parameter Sets: (All)
Aliases: FullName

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

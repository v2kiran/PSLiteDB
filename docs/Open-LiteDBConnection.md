---
external help file: PSLiteDB.dll-Help.xml
Module Name: pslitedb
online version:
schema: 2.0.0
---

# Open-LiteDBConnection

## SYNOPSIS
Open a connection to a litedb database on the filesystem or in-memory.

## SYNTAX

### Simple (Default)
```
Open-LiteDBConnection [[-Database] <String>] [-Credential <PSCredential>] [-Mode <ConnectionType>] [-Upgrade]
 [-InitialSize <Int64>] [-collation <Collation>] [-ReadOnly] [-DontSerializeNullValues] [-DontTrimWhitespace]
 [-DontConvertEmptyStringToNull] [-IncludeFields] [<CommonParameters>]
```

### Manual
```
Open-LiteDBConnection [-ConnectionString] <String> [-ReadOnly] [-DontSerializeNullValues] [-DontTrimWhitespace]
 [-DontConvertEmptyStringToNull] [-IncludeFields] [<CommonParameters>]
```

## DESCRIPTION
Open a connection to a litedb database on the filesystem or in-memory.

## EXAMPLES

### Example 1
```powershell
PS C:\> Open-LiteDBConnection

Database       :
ConnectionInfo :
Mapper         : LiteDB.BsonMapper
FileStorage    : LiteDB.LiteStorage`1[System.String]
UserVersion    : 0
Timeout        : 00:01:00
UtcDate        : False
LimitSize      : 9223372036854775807
CheckpointSize : 1000
Collation      : en-US/IgnoreCase
```

The database property is empty suggesting that the we are working with an in-memory database.

### Example 2
```powershell
PS C:\> Open-LiteDBConnection -Database c:\temp\movie.db

Database       : C:\temp\movie.db
ConnectionInfo : LiteDB.ConnectionString
Mapper         : LiteDB.BsonMapper
FileStorage    : LiteDB.LiteStorage`1[System.String]
UserVersion    : 0
Timeout        : 00:01:00
UtcDate        : False
LimitSize      : 9223372036854775807
CheckpointSize : 1000
Collation      : en-US/IgnoreCase
```

Opens a connection to the litedb located at c:\temp\movie.db

### Example 3
```powershell
PS C:\> Open-LiteDBConnection -Database c:\temp\movie.db -Credential (Get-Credential)

Database       : C:\temp\movie.db
ConnectionInfo : LiteDB.ConnectionString
Mapper         : LiteDB.BsonMapper
FileStorage    : LiteDB.LiteStorage`1[System.String]
UserVersion    : 0
Timeout        : 00:01:00
UtcDate        : False
LimitSize      : 9223372036854775807
CheckpointSize : 1000
Collation      : en-US/IgnoreCase
```

If you created the database with password protection then the same password will need to be passed every time you want to open the litedb database.

## PARAMETERS

### -ConnectionString
Pass your own connectionstring to LiteDB

```yaml
Type: String
Parameter Sets: Manual
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
Credential to secure the database

```yaml
Type: PSCredential
Parameter Sets: Simple
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Database
Path to LiteDB database

```yaml
Type: String
Parameter Sets: Simple
Aliases: Fullname, Path, Datasource

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```


### -IncludeFields
Not implemented.

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

### -InitialSize
Create a LiteDB databse with a specific starting size. By default the starting size is 8 KB.

```yaml
Type: Int64
Parameter Sets: Simple
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Mode
LiteDB offers 2 types of connections: Direct and Shared. This affect how engine will open data file.

Direct: Engine will open the datafile in exclusive mode and will keep it open until Dispose(). The datafile cannot be opened by another process. This is the recommended mode because it’s faster and cachable.
Shared: Engine will be close the datafile after each operation. Locks are made using Mutex. This is more expensive but you can open same file from multiple processes.

```yaml
Type: ConnectionType
Parameter Sets: Simple
Aliases:
Accepted values: Direct, Shared

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ReadOnly
Open datafile in read-only mode

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

### -Upgrade
Check if datafile is of an older version and upgrade it before opening

```yaml
Type: SwitchParameter
Parameter Sets: Simple
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -collation
A collation is a special pragma (for more info, see Pragmas) that allows users to specify a culture and string compare options for a datafile.

Collation is a read-only pragma and can only be changed with a rebuild.

A collation is specified with the format CultureName/CompareOption1[,CompareOptionN]. For more info about compare options, check the .NET documentation.

Datafiles are always created with CultureInfo.CurrentCulture as their culture and with IgnoreCase as the compare option. The collation can be change by rebuilding the datafile.

Examples:
rebuild {"collation": "en-US/None"}; rebuilds the datafile with the en-US culture and regular string comparison

rebuild {"collation": "en-GB/IgnoreCase"}; rebuilds the datafile with the en-GB culture and case-insensitive string comparison

rebuild {"collation": "pt-BR/IgnoreCase,IgnoreSymbols"}; rebuilds the datafile with the pt-BR culture and case-insensitive string comparison that also ignores symbols (white spaces, punctuation, math symbols etc.)

```yaml
Type: Collation
Parameter Sets: Simple
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Management.Automation.SwitchParameter

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
[collation](http://www.litedb.org/docs/collation/)
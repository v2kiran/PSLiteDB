function ConvertTo-LiteDbBSON
{
    [CmdletBinding()]
    param
    (
        # Input object can be any powershell object
        [Parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Object[]]
        $InputObject,

        # Serialization Depth
        [Parameter(ValueFromPipelineByPropertyName)]
        [uint16]
        $Depth = 3,

        # Return Array or an Object
        [ValidateSet("Document", "Array")]
        [Parameter(ValueFromPipelineByPropertyName)]
        [string]
        $As = "Array"
    )

    begin
    {
        $bsonarray = New-Object System.Collections.Generic.List[LiteDB.BsonDocument]
    }

    process
    {
        # determine all props that are of type int64 or long
        #$All_props = $InputObject[0] | Get-Member -MemberType Properties | Select-Object -ExpandProperty Name
        $int64_props = $InputObject[0] | Get-Member -MemberType Properties | Where-Object definition -match "int64|long" | Select-Object -ExpandProperty Name
        $Timespan_props = $InputObject[0] | Get-Member -MemberType Properties | Where-Object definition -match "timespan" | Select-Object -ExpandProperty Name

        foreach ($i in $InputObject)
        {
            # create a new hashtable
            #$customobj_hash = @{ }
            #$All_props | ForEach-Object { $customobj_hash[$_] = $i.$_ }

            if ($int64_props)
            {
                # force all int64 prop values to string because litedb does not allow us to store int64
                $int64_props | ForEach-Object { $i.$_ = $i.$_.tostring() }
                #$int64_props | ForEach-Object { $customobj_hash[$_] = $i.$_.tostring() }

            }
            if ($Timespan_props)
            {
                # force all int64 prop values to string because litedb does not allow us to store int64
                $Timespan_props | ForEach-Object { $i.$_ = $i.$_.ticks.tostring() }
                #$Timespan_props | ForEach-Object { $customobj_hash[$_] = $i.$_.ticks.tostring() }

            }

            #$obj = [PSCustomObject]$customobj_hash
            if ($As -eq 'Array')
            {
                try
                {
                    $bsonarray.Add(
                        (
                            [LiteDB.JsonSerializer]::Deserialize(
                                (
                                    ConvertTo-Json  -InputObject $i -Depth $Depth
                                )
                            )
                        )
                    )
                }
                catch
                {
                    Write-Error "an error occoured...$($_.exception.Message)" -ErrorAction Stop
                }

            }
            else
            {
                try
                {
                    [LiteDB.JsonSerializer]::Deserialize(
                        (
                            ConvertTo-Json  -InputObject $i -Depth $Depth
                        )
                    )
                }
                catch
                {
                    Write-Error "an error occoured...$($_.exception.Message)" -ErrorAction Stop
                }

            }
        }

    }

    end
    {
        if ($bsonarray.Count -gt 0)
        {
            Write-Output $bsonarray
        }
    }
}


function ConvertTo-LiteDbBSON1
{
    [CmdletBinding()]
    param
    (
        # Input object can be any powershell object
        [Parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Object[]]
        $InputObject,

        # Serialization Depth
        [Parameter(ValueFromPipelineByPropertyName)]
        [uint16]
        $Depth = 3,

        # Return Array or an Object
        [ValidateSet("Document", "Array")]
        [Parameter(ValueFromPipelineByPropertyName)]
        [string]
        $As = "Array"
    )

    begin
    {
        $bsonarray = New-Object System.Collections.Generic.List[LiteDB.BsonDocument]
    }

    process
    {
        # determine all props that are of type int64 or long
        $All_props = $InputObject[0] | Get-Member -MemberType Properties | Select-Object -ExpandProperty Name
        $int64_props = $InputObject[0] | Get-Member -MemberType Properties | Where-Object definition -match "int64|long" | Select-Object -ExpandProperty Name
        $Timespan_props = $InputObject[0] | Get-Member -MemberType Properties | Where-Object definition -match "timespan" | Select-Object -ExpandProperty Name

        foreach ($i in $InputObject)
        {
            # create a new hashtable
            $customobj_hash = @{ }
            $All_props | ForEach-Object { $customobj_hash[$_] = $i.$_ }

            if ($int64_props)
            {
                # force all int64 prop values to string because litedb does not allow us to store int64
                #$int64_props | ForEach-Object { $i.$_ = $i.$_.tostring() }
                $int64_props | ForEach-Object { $customobj_hash[$_] = $i.$_.tostring() }

            }
            if ($Timespan_props)
            {
                # force all int64 prop values to string because litedb does not allow us to store int64
                #$Timespan_props | ForEach-Object { $i.$_ = $i.$_.ticks.tostring() }
                $Timespan_props | ForEach-Object { $customobj_hash[$_] = $i.$_.ticks.tostring() }

            }

            $obj = [PSCustomObject]$customobj_hash
            if ($As -eq 'Array')
            {
                try
                {
                    $bsonarray.Add(
                        (
                            [LiteDB.JsonSerializer]::Deserialize(
                                (
                                    ConvertTo-Json  -InputObject $obj -Depth $Depth
                                )
                            )
                        )
                    )
                }
                catch
                {
                    Write-Error "an error occoured...$($_.exception.Message)" -ErrorAction Stop
                }

            }
            else
            {
                try
                {
                    [LiteDB.JsonSerializer]::Deserialize(
                        (
                            ConvertTo-Json  -InputObject $obj -Depth $Depth
                        )
                    )
                }
                catch
                {
                    Write-Error "an error occoured...$($_.exception.Message)" -ErrorAction Stop
                }

            }
        }

    }

    end
    {
        if ($bsonarray.Count -gt 0)
        {
            Write-Output $bsonarray
        }
    }
}
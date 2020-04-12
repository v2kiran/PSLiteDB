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
        foreach ($i in $InputObject)
        {
            if ($As -eq 'Array')
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
            else
            {
                [LiteDB.JsonSerializer]::Deserialize(
                    (
                        ConvertTo-Json  -InputObject $i -Depth $Depth
                    )
                )
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



Export-ModuleMember -Variable QueryLDB -Function ConvertTo-LiteDBBSON -alias fldb, oldb
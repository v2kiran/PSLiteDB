$script:QueryLDB = [LiteDB.Query]


function ConvertTo-LiteDbBSON2
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
                $obj = [LiteDB.JsonSerializer]::Deserialize(
                    (
                        ConvertTo-Json  -InputObject $i -Depth $Depth
                    )
                )

                
                $hash = @{ }

                $obj | Where-Object key -match "date|time" |
                    ForEach-Object {
                        $kvp = $_
                        if ($kvp.value -match "Date|(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})")
                        {
                            $hash[$kvp.key] = [PSLiteDB.Helpers.MSJsonDateConverter1]::Convert($kvp)
                        }
                    }


            $hash.GetEnumerator() | ForEach-Object {
                $obj[$_.key] = $_.Value
            }      


                              
        $bsonarray.Add(  
            (
                $obj
            )
        )  
                
    }
    else
    {
        $obj = [LiteDB.JsonSerializer]::Deserialize(
            (
                ConvertTo-Json  -InputObject $i -Depth $Depth
            )
        )
        $hash = @{ }

        $obj | Where-Object key -match "date|time" |
            ForEach-Object {
                $kvp = $_
                if ($kvp.value -match "Date|(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})")
                {
                    $hash[$kvp.key] = [PSLiteDB.Helpers.MSJsonDateConverter1]::Convert($kvp)
                }
            }


    $hash.GetEnumerator() | ForEach-Object {
        $obj[$_.key] = $_.Value
    }  

$obj 
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


Export-ModuleMember -Variable QueryLDB -Function ConvertTo-LiteDBBSON
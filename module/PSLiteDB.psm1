using namespace PSLiteDB.Helpers
using namespace System.Collections.Generic
using namespace LiteDB
using namespace System.Management.Automation


function ConvertTo-LiteDbBSON
{
    [CmdletBinding()]
    [Alias('CTLB')]
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
        $bsonarray = [List[BsonDocument]]::new()
    }

    process
    {
        # determine all props that are of type int64 or long
        [List[string]]$All_props = $InputObject[0] | Get-Member -MemberType Properties | Select-Object -ExpandProperty Name
        [List[string]]$int64_props = $InputObject[0] | Get-Member -MemberType Properties | Where-Object membertype -match 'Noteproperty|^property' | Where-Object definition -match "int64|long" | Select-Object -ExpandProperty Name
        [List[string]]$Timespan_props = $InputObject[0] | Get-Member -MemberType Properties | Where-Object definition -match "timespan" | Select-Object -ExpandProperty Name

        foreach ($i in $InputObject)
        {
            # create a new customobject to clone all properties from the incoming inputobject
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

            try
            {
                if ($PSVersionTable.PSEdition -eq 'Desktop')
                {
                    $bsonobj = [LiteDB.JsonSerializer]::Deserialize(
                        (
                            ConvertTo-Json  -InputObject $obj -Depth $Depth
                        )
                    )
                }
                else
                {
                    $bsonobj = [LiteDB.JsonSerializer]::Deserialize(
                        (
                            ConvertTo-Json  -InputObject $obj -Depth $Depth -EnumsAsStrings
                        )
                    )
                }

            }
            catch
            {
                $message = "Error occurred in converting json to BSON...$($_.exception.Message)"
                [ErrorRecord]$errorRecord = [ErrorRecord]::new([Exception]::new($message), 'CannotDeserializeException', ([ErrorCategory]::ParserError), $obj)
                $PSCmdlet.ThrowTerminatingError($errorRecord)
            }


            # this hashtable will only hold the datetime key-value pair(s)
            $hash = @{ }

            <#
                        # Check for keys with the word date or time in them and convert value to datetime if possible
            $bsonobj.GetEnumerator() |
                Where-Object key -match "date|time" |
                ForEach-Object {
                    $kvp = $_
                    if ($kvp.value -match "Date|(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})")
                    {
                        $hash[$kvp.key] = [MSJsonDateConverter]::Convert($kvp)
                    }
                }
            #>


            # Convert JSON datetime string value to bson datetime values
            $bsonobj.GetEnumerator() |
                Where-Object value -match "date|(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})" |
                ForEach-Object {
                    $kvp = $_
                    $hash[$kvp.key] = [MSJsonDateConverter]::Convert($kvp)
                }


            # Convert int64 values cast to string to BSON int64 values
            if ($int64_props.count -gt 0)
            {
                $int64_props |
                    ForEach-Object {
                        $bsonobj[$_] = [int64]($bsonobj[$_].AsString)
                    }

            }

            # Convert timespan ticks values cast to string to BSON int64 values
            if ($Timespan_props.count -gt 0)
            {
                $Timespan_props |
                    ForEach-Object {
                        $bsonobj[$_] = [int64]($bsonobj[$_].AsString)
                    }
            }

            # assign the datetime values stored in our temp hashtable back to the bsonobj
            $hash.GetEnumerator() |
                ForEach-Object {
                    $bsonobj[$_.key] = $_.Value
                }


            # do we output as an array or as as individual documents
            if ($As -eq 'Array')
            {
                # finally add our converted bson document to the bson array
                $null = $bsonarray.Add(
                    (
                        $bsonobj
                    )
                )
            }# as array
            else
            {
                $bsonobj
            }
        }

    }

    end
    {
        if ($bsonarray.Count -gt 0)
        {
            Write-Output $bsonarray -NoEnumerate
        }
    }
}





# Arguement completer for the Name parameter
$ScriptBlock = [scriptblock]::Create( {
        param ( $CommandName,
            $ParameterName,
            $WordToComplete,
            $CommandAst,
            $FakeBoundParameters )

        $wildcard = ("*" + $wordToComplete + "*")

        (Get-LiteDBCollectionName).where( { $_.Collection -like $wildcard }) |
            ForEach-Object {
                [System.Management.Automation.CompletionResult]::new("'" + $_.collection + "'")
            }
    })



[string[]]$funcs = "Find-LiteDBDocument", "Add-LiteDBDocument", "Get-LiteDBIndex", "New-LiteDBIndex", "Remove-LiteDBCollection", "Remove-LiteDBIndex", "Rename-LitedbCollection", "Update-LiteDbDocument", "Upsert-LiteDbDocument", "Remove-LiteDbDocument"
Register-ArgumentCompleter -CommandName $funcs -ParameterName Collection -ScriptBlock $ScriptBlock
Export-ModuleMember -Function ConvertTo-LiteDBBSON -alias fldb, oldb, ctlb, cldb, aldb

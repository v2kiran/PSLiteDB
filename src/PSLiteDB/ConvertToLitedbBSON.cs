using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsData.ConvertTo, "LiteDbBson")]
    public class ConvertToLiteDbBson : PSCmdlet
    {

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public object InputObject { get; set; }


        protected override void ProcessRecord()
        {
            /*
            using (var runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                using (var powerShell = PowerShell.Create())
                {
                    powerShell.Runspace = runspace;
                    powerShell.Commands.AddCommand("ConvertTo-Json").AddParameter("inputobject", InputObject).AddParameter("depth", 5);
                    var PSOutput = powerShell.Invoke();

                    foreach (PSObject outputItem in PSOutput)
                    {
                        if (outputItem != null)
                        {
                            WriteObject(LiteDB.JsonSerializer.Deserialize(outputItem.BaseObject.ToString()));
                        }
                    }
                }
            }
            */

            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.Commands.AddCommand("ConvertTo-Json").AddParameter("inputobject", InputObject).AddParameter("depth", 5);
                var PSOutput = PowerShellInstance.Invoke();

                foreach (PSObject outputItem in PSOutput)
                {
                    if (outputItem != null)
                    {
                        WriteObject(LiteDB.JsonSerializer.Deserialize(outputItem.BaseObject.ToString()));
                    }
                }
            }

        }
    }
}
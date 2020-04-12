using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Remove, "LiteDBCollection",ConfirmImpact = ConfirmImpact.High,SupportsShouldProcess = true)]
    public class RemoveLiteDBCollection : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public string[] Collection { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1
            )]
        public LiteDatabase Connection { get; set; }


        protected override void BeginProcessing()
        {
            if (Connection == null)
            {
                try
                {
                    Connection = (LiteDatabase)SessionState.PSVariable.Get("LiteDBPSConnection").Value;
                }
                catch (Exception)
                {

                    throw (new Exception("You must use 'Open-LiteDBConnection' to initiate a connection to a database"));
                }
            }

        }

        protected override void ProcessRecord()
        {
            foreach (var c in Collection)
            {
                if (!Connection.CollectionExists(c))
                {
                    WriteWarning($"Collection\t['{c}'] does not exist");
                }
                else
                {

                    if (ShouldProcess(c, "Delete Collection with all data & indexes"))
                    {
                        try
                        {
                            Connection.DropCollection(c);
                            Connection.Shrink();
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    }

                }
            }

        }
    }
}
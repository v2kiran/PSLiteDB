using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.New, "LiteDBCollection")]
    //[CmdletBinding(DefaultParameterSetName = "All")]
    public class NewLiteDBCollection : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public string Collection { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1
            )]
        public LiteDatabase Connection { get; set; }


        protected override void ProcessRecord()
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

            var collections = Connection.GetCollectionNames();
            if (!string.IsNullOrEmpty(collections.ToString()))
            {
                PSObject psObject = new PSObject();
                foreach (string col in collections)
                {
                    psObject.Properties.Add(new PSNoteProperty("Collection", col));
                    WriteObject(psObject);

                }
            }
            else
            {
                WriteWarning($"Could not find any collections in {Connection}");
            }



        }
    }
}
using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Get, "LiteDBCollectionName")]
    [Alias("ldbc")]
    public class GetLiteDBCollectionName : PSCmdlet
    {

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0
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
                    psObject.Properties.Add(new PSNoteProperty("Docs", Connection.GetCollection(col).Count()));
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
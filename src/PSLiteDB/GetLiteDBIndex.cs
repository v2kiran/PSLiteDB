using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Get, "LiteDBIndex")]
    public class GetLiteDBIndex : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
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
            if (!Connection.CollectionExists(Collection))
            {
                WriteWarning($"Collection\t['{Collection}'] does not exist");
            }
            else
            {
                try
                {
                    var indexcol = Connection.GetCollection(Collection).GetIndexes();
                    foreach (var i in indexcol)
                    {
                        PSObject psObject = new PSObject();
                        psObject.Properties.Add(new PSNoteProperty("Slot", i.Slot));
                        psObject.Properties.Add(new PSNoteProperty("Field", i.Field));
                        psObject.Properties.Add(new PSNoteProperty("Expression", i.Expression));
                        psObject.Properties.Add(new PSNoteProperty("Unique", i.Unique));
                        psObject.Properties.Add(new PSNoteProperty("MaxLevel", i.MaxLevel));
                        psObject.Properties.Add(new PSNoteProperty("Collection", Collection));
                        WriteObject(psObject);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
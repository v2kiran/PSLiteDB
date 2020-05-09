using LiteDB;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSLiteDB.Helpers;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Get, "LiteDBIndex")]
    [Alias("ildb")]
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
                    var col = Connection.GetCollection("$indexes");
                    string exp = $"$.collection = '{Collection}'";
                    var indexcol = col.Find(exp);
                    foreach (BsonDocument b in indexcol)
                    {
                        PSObject psObject = new PSObject();
                        foreach (KeyValuePair<string, BsonValue> kvp  in b)
                        {                           
                            psObject.Properties.Add(new PSNoteProperty(kvp.Key, MSJsonDateConverter.ReadObject(kvp.Value)));
                            psObject.Properties.Add(new PSNoteProperty("Connection", Connection));
                        }
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
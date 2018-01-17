using LiteDB;
using System;
using System.Management.Automation;
using System.Collections.Generic;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Get, "LiteDBDocument",DefaultParameterSetName = "All")]
    public class GetLiteDBDocument : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public string Collection { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "ID"
            )]
        public BsonValue ID { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
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

            if (Connection.CollectionExists(Collection))
            {
                var Table = Connection.GetCollection(Collection);


                if (ParameterSetName == "ID")
                {
                    var results = Table.FindById(ID);
                    if (results != null)
                    {

                        PSObject Obj = new PSObject();
                        foreach (KeyValuePair<string, BsonValue> kvp in results)
                        {
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, kvp.Value));
                        }
                        WriteObject(Obj);                      
                    }
                    else
                    {
                        WriteWarning($"Document with ID ['{ID}'] does not exist in the collection ['{Collection}']");
                    }
                    //results not null
                }
                else
                {
                    var results = Table.FindAll();
                    if (results != null)
                    {
                        foreach (var r in results)
                        {
                            PSObject Obj = new PSObject();
                            foreach (KeyValuePair<string, BsonValue> kvp in r)
                            {
                                Obj.Properties.Add(new PSNoteProperty(kvp.Key, kvp.Value));
                            }
                            WriteObject(Obj);

                        }
                    }
                    //results not null
                }



            }
            else
            {
                WriteWarning($"Collection ['{Collection}'] does not exist in the database");
            }
                     
        }
    }
}
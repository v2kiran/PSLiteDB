using LiteDB;
using System;
using System.Management.Automation;
using System.Collections.Generic;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Add, "LiteDBDocument", DefaultParameterSetName = "Document")]
    [Alias("Aldb")]
    public class AddLiteDBDocument : PSCmdlet
    {
        [ValidateNotNullOrEmpty()]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public string Collection { get; set; }


        [ValidateNotNullOrEmpty()]
        [Alias("_id")]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            ParameterSetName = "ID"
            )]
        public BsonValue ID { get; set; }

        [ValidateNotNullOrEmpty()]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            ParameterSetName = "Array"
            )]
        public BsonDocument[] BsonDocumentArray { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false, ParameterSetName = "Array")]
        public SwitchParameter BulkInsert
        {
            get { return _bulk; }
            set { _bulk = value; }
        }
        private bool _bulk;

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "Array"
            )]
        public int BatchSize { get; set; } = 5000;


        [ValidateNotNullOrEmpty()]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 2,
            ParameterSetName = "ID"
            )]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 2,
            ParameterSetName = "Document"
            )]
        public BsonDocument Document { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public LiteDatabase Connection { get; set; }

        private LiteCollection<BsonDocument> Table;

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

            //if collection does not exist it will be created and then the document will be inserted into the collection

        }
        protected override void ProcessRecord()
        {
            if (!Connection.CollectionExists(Collection))
            {
                WriteWarning($"Collection\t['{Collection}'] does not exist");
            }
            else
            {
                Table = Connection.GetCollection(Collection);
                try
                {
                    if (ParameterSetName == "ID")
                    {
                        Table.Insert(ID, Document);
                    }
                    else if (ParameterSetName == "Array")
                    {
                        if (BulkInsert)
                        {
                            Table.InsertBulk(BsonDocumentArray, BatchSize);
                        }
                        else
                        {
                            Table.Insert(BsonDocumentArray);
                        }

                    }
                    else
                    {
                        Table.Insert(Document);
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
using LiteDB;
using System;
using System.Management.Automation;
using System.Collections.Generic;

namespace PSLiteDB
{
    [Cmdlet(VerbsData.Merge, "LiteDBDocument",DefaultParameterSetName ="Document")]
    [Alias("Upsertldb")]
    public class MergeLiteDBDocument : PSCmdlet
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
                        Table.Upsert(ID, Document);
                    }
                    else if (ParameterSetName == "Array")
                    {
                        Table.Upsert(BsonDocumentArray);
                    }
                    else
                    {
                        Table.Upsert(Document);
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
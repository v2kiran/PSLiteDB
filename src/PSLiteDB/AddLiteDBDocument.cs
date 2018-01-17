using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Add, "LiteDBDocument", DefaultParameterSetName = "All")]
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
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1
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
            Table = Connection.GetCollection(Collection);
        }
        protected override void ProcessRecord()
        {
            try
            {
                Table.Insert(Document);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
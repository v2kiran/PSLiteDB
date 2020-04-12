using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Remove, "LiteDBDocument",DefaultParameterSetName ="ID")]
    public class RemoveLiteDBDocument : PSCmdlet
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
            ParameterSetName ="ID"
            )]
        public BsonValue ID { get; set; }

        [ValidateNotNullOrEmpty()]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            ParameterSetName = "Query"
            )]
        public Query Query { get; set; }

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
                        Table.Delete(ID);
                    }
                    else
                    {
                        Table.Delete(Query);
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
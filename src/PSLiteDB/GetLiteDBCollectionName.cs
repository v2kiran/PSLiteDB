using LiteDB;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Get, "LiteDBCollectionName")]
    [Alias("gcldb")]
    [OutputType("Collection")]
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


            var names = Collection.GetCollectionNames(Connection);
            if (names.Count > 0)
            {
                //PSObject psObject = new PSObject();
                foreach (string col in names)
                {
                    Collection colname = new Collection(Connection, col);
                    WriteObject(colname);
                }
            }
            else
            {
                WriteWarning($"Could not find any collections in {Connection}");
            }

        }
    }



    public class Collection
    {
        public LiteDatabase Connection { get; set; }
        public  string collection { get;private set; }

        public BsonAutoId AutoId { get;private set; }
        public int Docs { get;private set; }

        public static List<string> GetCollectionNames(LiteDatabase conn)
        {
            // cast ienumerable string to list string
            return new List<string>(conn.GetCollectionNames());
        }

        public Collection(LiteDatabase conn, string colname)
        {
            this.Connection = conn;
            this.collection = conn.GetCollection(colname).Name;
            this.AutoId = conn.GetCollection(colname).AutoId;
            this.Docs = conn.GetCollection(colname).Count();
        }

    }

}
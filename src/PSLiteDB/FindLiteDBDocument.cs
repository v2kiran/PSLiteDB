using LiteDB;
using System;
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Find, "LiteDBDocument", DefaultParameterSetName = "All")]
    [Alias("fldb")]
    public class FindLiteDBDocument : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public string Collection { get; set; }

        [Alias("_id")]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "ID"
            )]
        public BsonValue ID { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public int Skip { get; set; } = 0;

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public int Limit { get; set; } = 1000;

        [ValidateSet("PSObject", "BSON")]
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1
            )]
        public string As { get; set; } = "PSObject";

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "Query"
            )]
        public Query Query { get; set; }

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
                        if (As.ToLower() == "psobject")
                        {
                            var Obj = Helpers.MSJsonDateConverter.BSONtoPSObjectConverter(results, Collection);

                            WriteObject(Obj);
                        }
                        else
                        {
                            WriteObject(Connection.Mapper.ToDocument(results));                          
                        }

                    }
                    else
                    {
                        WriteWarning($"Document with ID ['{ID}'] does not exist in the collection ['{Collection}']");
                    }
                    //results not null
                }
                else if (ParameterSetName == "Query")
                {
                    var results = Table.Find(Query, Skip, Limit).ToList<BsonDocument>();
                    if (results != null)
                    {
                        if (As.ToLower() == "psobject")
                        {
                            foreach (var r in results)
                            {
                                var Obj = Helpers.MSJsonDateConverter.BSONtoPSObjectConverter(r, Collection);
                                WriteObject(Obj);
                            }
                        }
                        else
                        {
                            
                            foreach (var r in results)
                            {
                                WriteObject(Connection.Mapper.ToDocument(r));
                            }

                            
                        }
                    }

                }
                else
                {
                    var results = Table.Find(Query.All(Query.Descending), Skip, Limit).ToList<BsonDocument>();
                    if (results != null)
                    {
                        if (As.ToLower() == "psobject")
                        {
                            foreach (var r in results)
                            {
                                var Obj = Helpers.MSJsonDateConverter.BSONtoPSObjectConverter(r, Collection);
                                WriteObject(Obj);
                            }
                        }
                        else
                        {
                            foreach (var r in results)
                            {
                                WriteObject(Connection.Mapper.ToDocument(r));
                            }
                              
                        }
                    }

                }

            }
            else
            {
                WriteWarning($"Collection ['{Collection}'] does not exist in the database");
            }

        }
    }
}
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
    Mandatory = false,
    ValueFromPipeline = true,
    ValueFromPipelineByPropertyName = true,
    ParameterSetName = "Query"
    )]
        public string Where { get; set; }

        [Parameter(
Mandatory = true,
ValueFromPipeline = true,
ValueFromPipelineByPropertyName = true,
ParameterSetName = "Query"
)]
        public string Select { get; set; }

        [Parameter(
Mandatory = true,
ValueFromPipeline = true,
ValueFromPipelineByPropertyName = true,
ParameterSetName = "Sql"
)]
        public string Sql { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public LiteDatabase Connection { get; set; }


        private List<BsonDocument> results;


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

            if (ParameterSetName == "ID")
            {
                if (Connection.CollectionExists(Collection))
                {
                    var Table = Connection.GetCollection(Collection);
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
                else
                {
                    WriteWarning($"Collection ['{Collection}'] does not exist in the database");
                }

            }
            else if (ParameterSetName == "Query")
            {
                if (Connection.CollectionExists(Collection))
                {
                    var Table = Connection.GetCollection(Collection);
                    //var results = Table.Find(Query, Skip, Limit).ToList<BsonDocument>();
                    if (Where != null)
                    {
                        results = Table.Query().Where(Where).Select(Select).Limit(Limit).Skip(Skip).ToList();
                    }
                    else
                    {
                        results = Table.Query().Select(Select).Limit(Limit).Skip(Skip).ToList();
                    }

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
                    WriteWarning($"Collection ['{Collection}'] does not exist in the database");
                }

            }
            else if (ParameterSetName == "Sql")
            {
                var results = Connection.Execute(Sql).ToList().Select(x => x.AsDocument);
                //WriteVerbose($"results is {results} and type is {results.GetType()}");
                if (results != null)
                {
                   
                    if (As.ToLower() == "psobject")
                    {
                        foreach (var r in results)
                        {
                            if (r != null)
                            {
                                var Obj = Helpers.MSJsonDateConverter.BSONtoPSObjectConverter(r, Collection);
                                WriteObject(Obj);
                            }
  
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
                if (Connection.CollectionExists(Collection))
                {
                    var Table = Connection.GetCollection(Collection);

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
                else
                {
                    WriteWarning($"Collection ['{Collection}'] does not exist in the database");
                }
            }



        }
    }
}
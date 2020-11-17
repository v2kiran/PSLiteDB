﻿using LiteDB;
using System;
using System.Management.Automation;
using System.Collections.Generic;

namespace PSLiteDB
{
    [Cmdlet(VerbsData.Update, "LiteDBDocument", DefaultParameterSetName = "sql")]
    [Alias("uldb")]
    public class UpdateLiteDBDocument : PSCmdlet
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
            ParameterSetName = "ID"
            )]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "Document"
            )]
        public BsonDocument Document { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            ParameterSetName = "Query"
            )]
        public BsonExpression Set { get; set; }

        [Parameter(
            Mandatory = true,       
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            ParameterSetName = "Query"
            )]
        public BsonExpression Where { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "Sql",
            Position = 1
            )]
        [ValidateNotNullOrEmpty()]
        public string Sql { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public LiteDatabase Connection { get; set; }

        private ILiteCollection<BsonDocument> Table;

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
                        Table.Update(ID, Document);
                    }
                    else if (ParameterSetName == "Array")
                    {
                        Table.Update(BsonDocumentArray);
                    }
                    else if(ParameterSetName == "Query")
                    {
                        Table.UpdateMany(Set, Where);
                    }
                    else if (ParameterSetName == "Sql")
                    {
                        WriteVerbose($"sql: {Sql}");
                        Connection.Execute(Sql);
                    }
                    else
                    {
                        Table.Update(Document);
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
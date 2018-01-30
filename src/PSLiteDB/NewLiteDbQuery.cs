using LiteDB;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.New, "LiteDBQuery")]
    [Alias("qldb")]
    public class NewLiteDBQuery : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public string Field { get; set; }


        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1
            )]
        public string Value { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public Func<BsonValue,bool> Predicate { get; set; }

        [ValidateSet("EQ", "LT", "LTE", "GT", "GTE", "Not", "StartsWith", "Contains", "Where")]
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 2
            )]
        public string Operator { get; set; } = "Contains";

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

            try
            {
                switch (Operator)
                {
                    case "EQ":
                        WriteObject(Query.EQ(Field, Value));
                        break;
                    case "LT":
                        WriteObject(Query.LT(Field, Convert.ToInt32(Value)));
                        break;
                    case "LTE":
                        WriteObject(Query.LTE(Field, Convert.ToInt32(Value)));
                        break;
                    case "GT":
                        WriteObject(Query.GT(Field, Convert.ToInt32(Value)));
                        break;
                    case "GTE":
                        WriteObject(Query.GTE(Field, Convert.ToInt32(Value)));
                        break;
                    case "Not":
                        WriteObject(Query.Not(Field, Value));
                        break;
                    case "StartsWith":
                        WriteObject(Query.StartsWith(Field, Value));
                        break;
                    case "Contains":
                        WriteObject(Query.Contains(Field, Value));
                        break;
                    case "Where":
                        WriteObject(Query.Where(Field, Predicate));
                        break;
                    default:
                        WriteObject(Query.StartsWith(Field, Value));
                        break;
                }
                    
                    
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
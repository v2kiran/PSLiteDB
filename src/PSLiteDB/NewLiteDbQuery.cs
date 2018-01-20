using LiteDB;
using System;
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
        public BsonArray ValueArray { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public Func<BsonValue,bool> Predicate { get; set; }

        [ValidateSet("EQ", "LT", "LTE", "GT", "GTE", "In", "Not", "StartsWith", "Contains", "Where", "And", "Or")]
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 2
            )]
        public string Operator { get; set; } = "StartsWith";

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
                        WriteObject(Query.LT(Field, Value));
                        break;
                    case "LTE":
                        WriteObject(Query.LTE(Field, Value));
                        break;
                    case "GT":
                        WriteObject(Query.GT(Field, Value));
                        break;
                    case "GTE":
                        WriteObject(Query.GTE(Field, Value));
                        break;
                    case "In":
                        WriteObject(Query.In(Field, ValueArray));
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
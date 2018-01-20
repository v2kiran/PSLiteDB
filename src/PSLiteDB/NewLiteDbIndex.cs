using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.New, "LiteDBIndex")]
    public class NewLiteDBIndex : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public string Collection { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 1
            )]
        public string Field { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
        public SwitchParameter Unique
        {
            get { return _unique; }
            set { _unique = value; }
        }
        private bool _unique;

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true
            )]
        public string Expression { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            Position = 2
            )]
        public LiteDatabase Connection { get; set; }

        private bool uniquestring = false;

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
            if (Unique)
            {
                uniquestring = true;
            }


            if (!Connection.CollectionExists(Collection))
            {
                WriteWarning($"Collection\t['{Collection}'] does not exist");
            }
            else
            {
                try
                {
                    var col = Connection.GetCollection(Collection);
                    if (Expression!=null)
                    {
                        col.EnsureIndex(Field, Expression, uniquestring);
                    }
                    else
                    {
                        col.EnsureIndex(Field, uniquestring);
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
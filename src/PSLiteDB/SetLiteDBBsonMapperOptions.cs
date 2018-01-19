using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Set, "LiteDBBsonMapperOptions")]
    [Alias("closeldb")]
    public class SetLiteDBBsonMapperOptions : PSCmdlet
    {
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public LiteDatabase Connection { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
        public SwitchParameter SerializeNullValues
        {
            get { return _serializeNullValues; }
            set { _serializeNullValues = value; }
        }
        private bool _serializeNullValues;

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
        public SwitchParameter DontTrimWhitespace
        {
            get { return _disableTrimWhitespace; }
            set { _disableTrimWhitespace = value; }
        }
        private bool _disableTrimWhitespace;

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
        public SwitchParameter DontConvertEmptyStringToNull
        {
            get { return _emptyStringToNull; }
            set { _emptyStringToNull = value; }
        }
        private bool _emptyStringToNull;

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
        public SwitchParameter IncludeFields
        {
            get { return _includeFields; }
            set { _includeFields = value; }
        }
        private bool _includeFields;


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

            if (SerializeNullValues)
            {
                Connection.Mapper.SerializeNullValues = true;
            }

            if (DontTrimWhitespace)
            {
                Connection.Mapper.TrimWhitespace = false;
            }

            if (DontConvertEmptyStringToNull)
            {
                Connection.Mapper.EmptyStringToNull = false;
            }

            if (IncludeFields)
            {
                Connection.Mapper.IncludeFields = true;
            }

            WriteObject(Connection);


        }
    }
}
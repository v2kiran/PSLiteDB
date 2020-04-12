using System;
using System.Data;
using LiteDB;
using System.IO;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Open, "LiteDBConnection", DefaultParameterSetName = "Simple")]
    [Alias("Oldb")]
    public class OpenLiteDBConnection : PSCmdlet
    {
        [Alias("Fullname", "Path", "Datasource")]
        [Parameter(
            Position = 0,
            HelpMessage = "Path to LiteDB database",
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public string Database { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "Pass your own connectionstring to LiteDB",
            ParameterSetName = "Manual",
            Mandatory = true
            )]
        public string ConnectionString { get; set; }

        [Parameter(
            HelpMessage = "Credential to secure the database",
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public PSCredential Credential { get; set; }


        [Parameter(
     HelpMessage = "Open in Read-Only Mode",
     ParameterSetName = "Simple"
     )]
        public LiteDB.ConnectionType Mode { get; set; } = LiteDB.ConnectionType.Direct;

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false, ParameterSetName = "Simple")]
        public SwitchParameter Upgrade
        {
            get { return _upgrade; }
            set { _upgrade = value; }
        }
        private bool _upgrade;


        [AllowNull()]
        [Parameter(
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public long InitialSize { get; set; } = 8192;


        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
        public SwitchParameter DontSerializeNullValues
        {
            get { return _dontSerializeNullValues; }
            set { _dontSerializeNullValues = value; }
        }
        private bool _dontSerializeNullValues;

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


        private string resolvedPath;
        private ConnectionString connbuilder;
        private LiteDatabase _connection;
        private ConnectionString _connectioninfo;


        protected override void EndProcessing()
        {


            if (ParameterSetName == "Simple")
            {

                connbuilder = new ConnectionString();
                connbuilder.Connection = Mode;
                connbuilder.InitialSize = InitialSize;
                

                if (Credential != null)
                {
                    connbuilder.Password = Credential.GetNetworkCredential().Password;
                }


                if (Upgrade)
                {
                    connbuilder.Upgrade = LiteDB.UpgradeOption.True;
                }

                

                if (Database != null && !string.IsNullOrEmpty(Database) && !string.IsNullOrWhiteSpace(Database))
                {
                    resolvedPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(Database);

                    if (File.Exists(resolvedPath))
                    {
                        connbuilder.Filename = resolvedPath;
                        _connectioninfo = connbuilder;
                        try
                        {
                            _connection = new LiteDatabase(connbuilder);
                            _connectioninfo.Password = null;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Path not found:\t['{0}']", resolvedPath));
                    }
                }
                else
                {
                    _connection = new LiteDatabase(new MemoryStream());
                    WriteVerbose($"Open-LiteDBConnection: Opened connection to In-Memory database");
                }
            }
            else
            {
                //custom connection string passed by the user
                try
                {
                    _connection = new LiteDatabase(ConnectionString);
                   _connectioninfo = null;
                }
                catch (Exception)
                {

                    throw;
                }
            }

            BsonMapper.Global.SerializeNullValues = true;

            if (DontSerializeNullValues)
            {
                BsonMapper.Global.SerializeNullValues = false;
            }

            if (DontTrimWhitespace)
            {
                BsonMapper.Global.TrimWhitespace = false;
            }

            if (DontConvertEmptyStringToNull)
            {
                BsonMapper.Global.EmptyStringToNull = false;
            }

            if (IncludeFields)
            {
                BsonMapper.Global.IncludeFields = true;
            }

            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                //add the database path as a custom property to the connection object
                PowerShellInstance.Commands.AddCommand("Add-Member").AddParameter("inputobject", _connection).AddParameter("MemberType", "NoteProperty").
                    AddParameter("Name", "Database").AddParameter("Value", resolvedPath);
                PowerShellInstance.AddStatement();
                PowerShellInstance.Commands.AddCommand("Add-Member").AddParameter("inputobject", _connection).AddParameter("MemberType", "NoteProperty").
                AddParameter("Name", "ConnectionInfo").AddParameter("Value", _connectioninfo).AddParameter("PassThru", true);

                    
                var PSOutput = PowerShellInstance.Invoke();
            }

            //check if there is an existing connection variable if yes then dont overwrite it!
            try
            {
                var conns = (LiteDatabase)SessionState.PSVariable.Get("LiteDBPSConnection").Value;
            }
            catch (Exception)
            {
                SessionState.PSVariable.Set("LiteDBPSConnection", _connection);
            }



            //write the connection to the pipeline
            WriteObject(_connection);
        }
    }

}
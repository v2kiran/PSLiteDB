using System;
using System.Data;
using LiteDB;
using System.IO;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Open, "LiteDBConnection", DefaultParameterSetName = "Simple")]
    public class OpenLiteDBConnection : PSCmdlet
    {
        [Alias("Fullname", "Path", "Datasource")]
        [Parameter(
            Position = 0,
            HelpMessage = "Path to LiteDB database",
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public string Database { get; set; } = @"C:\temp\litedb\person.db";

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
        public LiteDB.FileMode Mode { get; set; } = LiteDB.FileMode.Shared;

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false,ParameterSetName = "Simple")]
        public SwitchParameter DisableJournal
        {
            get { return _disablejournal; }
            set { _disablejournal = value; }
        }
        private bool _disablejournal;

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false, ParameterSetName = "Simple")]
        public SwitchParameter Upgrade
        {
            get { return _upgrade; }
            set { _upgrade = value; }
        }
        private bool _upgrade;

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false, ParameterSetName = "Simple")]
        public SwitchParameter UtcDate
        {
            get { return _UtcDate; }
            set { _UtcDate = value; }
        }
        private bool _UtcDate;

        [Parameter(
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public int CacheSize { get; set; } = 5000;

        [AllowNull()]
        [Parameter(
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public long InitialSize { get; set; } = 8192;

        [AllowNull()]
        [Parameter(
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public long LimitSize { get; set; } = 16106127360000;

        [AllowNull()]
        [Parameter(
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public TimeSpan TimeOut { get; set; } = new TimeSpan(0, 1, 0);

        [Parameter(
            ParameterSetName = "Simple",
            Mandatory = false
            )]
        public Byte Log { get; set; } = Logger.NONE;


        private string resolvedPath;
        private ConnectionString connbuilder;
        private LiteDatabase _connection;


        protected override void EndProcessing()
        {

 
            if (ParameterSetName == "Simple")
            {

                connbuilder = new ConnectionString();
                connbuilder.Mode = Mode;
                connbuilder.Journal = true;

                if (Credential != null)
                {
                    connbuilder.Password = Credential.GetNetworkCredential().Password;
                }

                if(DisableJournal)
                {
                    connbuilder.Journal = false;
                }

                if (Upgrade)
                {
                    connbuilder.Upgrade = true;
                }

                if (UtcDate)
                {
                    connbuilder.UtcDate = true;
                }



                connbuilder.CacheSize = CacheSize;
                connbuilder.InitialSize = InitialSize;
                connbuilder.LimitSize = LimitSize;
                connbuilder.Timeout = TimeOut;
                connbuilder.Log = Log;


                if (Database != null && !string.IsNullOrEmpty(Database) && !string.IsNullOrWhiteSpace(Database))
                {
                    resolvedPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(Database);

                    if (File.Exists(resolvedPath))
                    {
                        connbuilder.Filename = resolvedPath;
                        try
                        {
                            //WriteObject(connbuilder);
                            _connection = new LiteDatabase(connbuilder);
                            var eng = _connection.Engine;
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
                    var eng = _connection.Engine;
                }
                catch (Exception)
                {

                    throw;
                }
            }




            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                //add the database path as a custom property to the connection object
                PowerShellInstance.Commands.AddCommand("Add-Member").AddParameter("inputobject", _connection).AddParameter("MemberType", "NoteProperty").
                    AddParameter("Name", "Database").AddParameter("Value", resolvedPath).AddParameter("PassThru", true);
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
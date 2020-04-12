using System;
using System.Data;
using LiteDB;
using System.IO;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.New, "LiteDBDatabase")]
    [Alias("NewLdb")]
    public class NewLiteDBDatabase : PSCmdlet
    {
        [Alias("FullName")]
        [ValidateNotNullOrEmpty()]
        [Parameter(
            Position = 0,
            HelpMessage = "Path to LiteDB database",
            Mandatory = true
            )]
        public string Path { get; set; }

        [Parameter(
            HelpMessage = "Credential to secure the database",
            Mandatory = false
            )]
        public PSCredential Credential { get; set; }

        private string resolvedPath;

        protected override void EndProcessing()
        {
            resolvedPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(Path);

            if (File.Exists(resolvedPath))
            {
                WriteWarning(string.Format("A LiteDB Database already exists at Path:\t['{0}']", resolvedPath));
            }
            else
            {
                ConnectionString connbuilder = new ConnectionString();
                connbuilder.Filename = resolvedPath;
                if (Credential != null)
                {
                    connbuilder.Password = Credential.GetNetworkCredential().Password;
                }

                LiteDatabase db = new LiteDatabase(connbuilder);
                db.Dispose();
                //verify that the database was created
                if (File.Exists(resolvedPath))
                {
                    WriteVerbose(string.Format("Sucessfully created a LiteDB Database at Path:\t['{0}']", resolvedPath));
                }
                else
                {
                    throw new Exception(string.Format("Could not create LiteDb database at Path:\t['{0}']", resolvedPath));
                }

            }

        }
    }

}
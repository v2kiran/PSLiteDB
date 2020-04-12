using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Close, "LiteDBConnection")]
    [Alias("closeldb")]
    public class CloseLiteDBConnection : PSCmdlet
    {

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0
            )]
        public LiteDatabase Connection { get; set; }


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
                    throw;
                }

                try
                {
                    Connection.Dispose();
                    SessionState.PSVariable.Remove("LiteDBPSConnection");
                    WriteVerbose($"Closed connection to databse in {Connection}");
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                try
                {
                    Connection.Dispose();
                    WriteVerbose($"Closed connection to databse in {Connection}");
                }
                catch (Exception)
                {

                    throw;
                }
            }



        }
    }
}
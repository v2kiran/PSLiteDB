using LiteDB;
using System;
using System.Management.Automation;

namespace PSLiteDB
{
    [Cmdlet(VerbsCommon.Close, "LiteDBConnection")]
    [Alias("cldb")]
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
                    ThrowTerminatingError(new ErrorRecord(new Exception("A LiteDB Connection was not found"), "ConnectionNotFound", ErrorCategory.ObjectNotFound, null));
                }

                
                try
                {
                    Connection.Dispose();
                    SessionState.PSVariable.Remove("LiteDBPSConnection");
                    WriteVerbose($"Closed connection to databse in {Connection}");
                }
                catch (Exception e)
                {
                    ThrowTerminatingError(new ErrorRecord(new Exception($"An error occurred in closing the litedb connection {e}"), "CloseConnectionError", ErrorCategory.CloseError, Connection));
                }

            }
            else
            {
                try
                {
                    Connection.Dispose();
                    WriteVerbose($"Closed connection to databse in {Connection}");
                }
                catch (Exception e)
                {

                    ThrowTerminatingError(new ErrorRecord(new Exception($"An error occurred in closing the litedb connection {e}"), "CloseConnectionError", ErrorCategory.CloseError, Connection));

                }
            }



        }
    }
}
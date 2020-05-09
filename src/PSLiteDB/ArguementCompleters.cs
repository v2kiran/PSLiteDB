using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using LiteDB;

namespace PSLiteDB
{
    public class LiteDBCollectionCompleter 
        //: IArgumentCompleter
    {
        /**
        private LiteDatabase Connection;
        IEnumerable<CompletionResult> IArgumentCompleter.CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters
        )
        {
            WildcardPattern wildcard = new WildcardPattern("*" + wordToComplete + "*", WildcardOptions.IgnoreCase);
            return GetLiteDBCollectionName.Where(wildcard.IsMatch).Select(s => new CompletionResult("'" + s. + "'"));
        }
       **/
    }

}

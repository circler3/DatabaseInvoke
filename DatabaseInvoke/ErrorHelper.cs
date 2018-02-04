using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseInvoke
{
    /// <summary>
    /// Handle all the errors. Usually a log service will be presented here to keep the process fluent.
    /// </summary>
    static class ErrorHelper
    {
        /// <summary>
        /// Implement your own error handle logic here. log service, echo can be included.
        /// </summary>
        /// <param name="e"></param>
        public static void Error(Exception e)
        {
            //use your own logic for handle errors.
        }
    }
}

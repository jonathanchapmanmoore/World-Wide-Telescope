namespace Renci.Wwt.DataManager.Common
{
    using Microsoft.Practices.Prism.Logging;
    using System;

    public class Logger : ILoggerFacade
    {
        #region ILoggerFacade Members

        public void Log(string message, Category category, Priority priority)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        #endregion
    }
}

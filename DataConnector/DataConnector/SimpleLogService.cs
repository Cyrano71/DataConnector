using Serilog;
using System;

namespace DataConnector
{
    public class SimpleLogService
    {
        private ILogger _log;
        public SimpleLogService(String logFolder)
        {
            _log = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Console()
           .WriteTo.File($"{logFolder}\\Log.log", rollingInterval: RollingInterval.Day)
           .CreateLogger();
        }

        public void Error(String message)
        {
            _log.Error(message);
        }
    }
}

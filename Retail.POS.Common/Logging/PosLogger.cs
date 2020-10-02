using Microsoft.Extensions.Logging;
using System;

namespace Retail.POS.Common.Logging
{
    public class PosLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            // What is this? Doesn't matter since I'm only building the POS
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // Always true
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Do nothing
        }
    }
}

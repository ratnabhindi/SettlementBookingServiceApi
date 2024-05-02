using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Services.Implementations
{
    public class ApiLogger<T>(ILogger<T> logger) : IApiLogger<T>
    {
        private readonly ILogger<T> _logger = logger;

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }
    }

}

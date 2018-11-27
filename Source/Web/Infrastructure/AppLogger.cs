using System;
using DrakeLambert.Peerra.WebApi.SharedKernel.Interfaces.Infrastructure;
using Microsoft.Extensions.Logging;

namespace DrakeLambert.Peerra.WebApi.Web.Infrastructure
{
    public class AppLogger<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;

        public AppLogger(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogError(string message, Exception exception, params object[] args)
        {
            _logger.LogError(message, exception, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}

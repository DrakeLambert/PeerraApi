using System;

namespace DrakeLambert.Peerra.WebApi.SharedKernel.Interfaces.Infrastructure
{
    public interface IAppLogger<T>
    {
        void LogDebug(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, Exception exception, params object[] args);
    }
}

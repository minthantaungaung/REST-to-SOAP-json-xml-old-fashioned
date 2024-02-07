namespace app.api.Infrastructure.Logger
{
    public interface IloggerManager
    {
        void LogInfo(string message);

        void LogWarn(string message);

        void LogDebug(string message);

        void LogError(string message);
    }
}

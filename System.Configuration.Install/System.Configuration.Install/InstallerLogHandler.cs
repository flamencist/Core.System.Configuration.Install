namespace System.Configuration.Install
{
    public class InstallerLogHandler
    {
        public static InstallerLogHandler Instance { get; } = new InstallerLogHandler();
        public event EventHandler<string> OnLog;

        internal void Log(string message)
        {
            OnLog?.Invoke(this, message);
        }
    }
}
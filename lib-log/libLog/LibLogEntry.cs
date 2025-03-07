namespace LogUtility
{
    // log entry
    public class LibLogEntry
    {
        public string Message;

        public DateTime LoggedAt;

        public LogLevel Level;

        public string Project;
        public string Namespace;
        public string ClassName;
        public string MethodName;
        public string FilePath;
        public int LineNumber;
    }
}
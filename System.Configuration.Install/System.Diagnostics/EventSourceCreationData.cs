namespace System.Diagnostics
{
    /// <summary>Represents the configuration settings used to create an event log source on the local computer or a remote computer.</summary>
    /// <filterpriority>2</filterpriority>
    public class EventSourceCreationData
    {
        private int _categoryCount;

        /// <summary>Gets or sets the name of the event log to which the source writes entries.</summary>
        /// <returns>The name of the event log. This can be Application, System, or a custom log name. The default value is "Application."</returns>
        /// <filterpriority>2</filterpriority>
        public string LogName { get; set; } = "Application";

        /// <summary>Gets or sets the name of the computer on which to register the event source.</summary>
        /// <returns>The name of the system on which to register the event source. The default is the local computer (".").</returns>
        /// <exception cref="T:System.ArgumentException">The computer name is invalid. </exception>
        /// <filterpriority>2</filterpriority>
        public string MachineName { get; set; } = ".";

        /// <summary>Gets or sets the name to register with the event log as an event source.</summary>
        /// <returns>The name to register with the event log as a source of entries. The default is an empty string ("").</returns>
        /// <filterpriority>2</filterpriority>
        public string Source { get; set; }

        /// <summary>Gets or sets the path of the message resource file that contains message formatting strings for the source.</summary>
        /// <returns>The path of the message resource file. The default is an empty string ("").</returns>
        /// <filterpriority>2</filterpriority>
        public string MessageResourceFile { get; set; }

        /// <summary>Gets or sets the path of the resource file that contains message parameter strings for the source.</summary>
        /// <returns>The path of the parameter resource file. The default is an empty string ("").</returns>
        /// <filterpriority>2</filterpriority>
        public string ParameterResourceFile { get; set; }

        /// <summary>Gets or sets the path of the resource file that contains category strings for the source.</summary>
        /// <returns>The path of the category resource file. The default is an empty string ("").</returns>
        /// <filterpriority>2</filterpriority>
        public string CategoryResourceFile { get; set; }

        /// <summary>Gets or sets the number of categories in the category resource file.</summary>
        /// <returns>The number of categories in the category resource file. The default value is zero.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a negative value or to a value larger than <see cref="F:System.UInt16.MaxValue" />. </exception>
        /// <filterpriority>2</filterpriority>
        public int CategoryCount
        {
            get => _categoryCount;
            set
            {
                if (value > 65535 || value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _categoryCount = value;
            }
        }

        private EventSourceCreationData()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.EventSourceCreationData" /> class with a specified event source and event log name.</summary>
        /// <param name="source">The name to register with the event log as a source of entries. </param>
        /// <param name="logName">The name of the log to which entries from the source are written. </param>
        public EventSourceCreationData(string source, string logName)
        {
            Source = source;
            LogName = logName;
        }

        internal EventSourceCreationData(string source, string logName, string machineName)
        {
            Source = source;
            LogName = logName;
            MachineName = machineName;
        }

        private EventSourceCreationData(string source, string logName, string machineName, string messageResourceFile, string parameterResourceFile, string categoryResourceFile, short categoryCount)
        {
            Source = source;
            LogName = logName;
            MachineName = machineName;
            MessageResourceFile = messageResourceFile;
            ParameterResourceFile = parameterResourceFile;
            CategoryResourceFile = categoryResourceFile;
            CategoryCount = categoryCount;
        }
    }
}
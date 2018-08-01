namespace System.Diagnostics
{
    public enum PerformanceCounterCategoryType
    {
        /// <summary>The instance functionality for the performance counter category is unknown. </summary>
        Unknown = -1,
        /// <summary>The performance counter category can have only a single instance.</summary>
        SingleInstance,
        /// <summary>The performance counter category can have multiple instances.</summary>
        MultiInstance
    }
}
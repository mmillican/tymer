using System;

namespace tymer
{
    public class TimeEntry
    {
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }

        public double Duration => (EndTime - StartTime).TotalHours;

        public string Comments { get; set; }        
    }
}

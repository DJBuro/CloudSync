using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CloudSyncModel
{
    public class TimeSpanBlock
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool OpenAllDay { get; set; }
    }
}

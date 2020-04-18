using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Room133.Models
{
    public class LogHistory
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string PointInTime { get; set; }
        public decimal Temperature { get; set; }
    }
}

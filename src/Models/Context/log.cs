using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Room133.Context
{
    public class Log
    {
        public int LogId { get; set; }

        public DateTime Date { get; set; }
        public string Room { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
    }
}

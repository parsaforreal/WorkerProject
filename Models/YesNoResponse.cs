using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntervalWorkerService.Models
{
    public class YesNoResponse { 
        public string? Answer { get; set; } 
        public bool forced { get; set; } 
        public string? image { get; set; } 
    }
}

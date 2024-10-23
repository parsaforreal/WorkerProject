using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntervalWorkerService.Models
{
    public class CatFactObject
    {
        public string? fact { get; set; }
        public int length { get; set; }
        public override string ToString()
        {
            return $"Fact: {fact}, Length: {length}";
        }
    }
}


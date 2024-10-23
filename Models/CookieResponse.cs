using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntervalWorkerService.Models
{
    public class CookieResponse
    {
        public Input Input { get; set; } = new Input();
        public string _RequestVerificationToken { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntervalWorkerService.Models
{
    public class Response
    {
        public string tokenType { get; set; } = string.Empty;
        public string accessToken { get; set; } = string.Empty;
        public int expiresIn { get; set; }
        public string refreshToken { get; set; } = string.Empty;
    }
}

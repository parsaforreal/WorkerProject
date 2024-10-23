using System.Net;

namespace IntervalWorkerService.Models
{
    public class MessageCode
    {
        public DateTime time { get; set; }
        public int state { get; set; }
        public string? description { get; set; }
        public override string ToString()
        {
            return $"Time: {time}, State: {state}, Description: {description}";
        }
    }
}

using System;

namespace Gamm.Agent.Models
{
    public class AgentPartModel
    {
        public Guid Id { get; set; }
        public Guid ScanId { get; set; }
        public string Part { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Comment { get; set; }
    }
}

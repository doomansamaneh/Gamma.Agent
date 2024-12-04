using System;

namespace Gamm.Agent.Models
{
    public class AgentSoftwareModel
    {
        public Guid Id { get; set; }
        public Guid ScanId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime DateInstalled { get; set; }
        public string Comment { get; set; }
    }
}

using System;

namespace Gamm.Agent.Models
{
    public class AgentMainModel
    {
        public Guid Id { get; set; }
        public string Domain { get; set; }
        public string ComputerName { get; set; }
        public string MacAddress { get; set; }
        public string UserName { get; set; }
        public string Ip { get; set; }
        public string Comment { get; set; }
    }
}

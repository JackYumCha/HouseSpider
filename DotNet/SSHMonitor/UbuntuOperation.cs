using System;
using System.Collections.Generic;
using System.Text;

namespace SSHMonitor
{
    public class UbuntuOperation
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<BashCommand> Commands { get; set; }
    }
}

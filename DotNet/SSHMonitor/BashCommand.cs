using System;
using System.Collections.Generic;
using System.Text;

namespace SSHMonitor
{
    public class BashCommand
    {
        public string Command { get; set; }
        public string Title { get; set; }
        public List<BashResult> Results { get; set; }
    }
}

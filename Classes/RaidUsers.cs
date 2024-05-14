using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBox.Classes
{
    public class RaidUsers
    {
        public string MentionName { get; set; }
        public string Name { get; set; }

        public List<RaidRecord> RaidRecords = new List<RaidRecord>();
    }
}

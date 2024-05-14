using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBox.Classes
{
    internal class UserRecord
    {
        internal string MentionName { get; set; }
        internal string PlayerName { get; set; }
        internal int CombatPower { get; set; }
        internal int Level { get; set; }
        internal int AttendanceCount { get; set; }
        internal int LastSesssion { get; set; }
        internal bool LegendaryPlayer { get; set; } = false;
        internal bool EpicPlayer { get; set; } = false;
        internal int CommonBoxes { get; set; }
        internal int EpicBoxes { get; set; }
        internal int LegendaryBoxes { get; set; }
        internal int Ranking { get; set; }

        internal int Total
        {
            get { return EpicBoxes + LegendaryBoxes + CommonBoxes; }
        }
    }
}


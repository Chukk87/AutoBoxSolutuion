using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBox.Classes
{
    public class Settings
    {
        public string _discordToken;
        public ulong _summaryChannelId;
        public ulong _peak7SummaryPostId;
        public ulong _peak8SummaryPostId;
        public ulong _peak9SummaryPostId;
        public ulong _peak10SummaryPostId;
        public ulong _peak11SummaryPostId;
        public ulong _peak12SummaryPostId;
        public ulong _peak13SummaryPostId;
        public ulong _peak14SummaryPostId;
        public List<string> _peakManagerRole = new List<string>();
        public string _specialRole;
        public bool _specialRoleEnabled;
        public int _maxNormalTickets;
        public int _maxSpecialTickets;
        public int MaxBossSessionsNormal;
        public int MaxBossSessionsSpecial;
        public bool DisableBooking;
        public string DisableBookingTime;
        public string DisableBookingReason;
        public bool AllowExtend;
        public int MaxSessionExtend;
        public int MaxTicketExtend;
        public int MinMinutesBeforeExtend;
        public List<string> EnabledPeaks = new List<string>();

        public const string SettingsFile = @"C:\PeakBot\Settings.txt";

        public void InitialiseSettings()
        {
            List<string> settingLines = File.ReadAllLines(SettingsFile).ToList();

            _discordToken = settingLines[0].Split(':')[1];

            _summaryChannelId = Convert.ToUInt64(settingLines[1].Split(':')[1]);
            _peak7SummaryPostId = Convert.ToUInt64(settingLines[2].Split(':')[1]);
            _peak8SummaryPostId = Convert.ToUInt64(settingLines[3].Split(':')[1]);
            _peak9SummaryPostId = Convert.ToUInt64(settingLines[4].Split(':')[1]);
            _peak10SummaryPostId = Convert.ToUInt64(settingLines[5].Split(':')[1]);
            _peak11SummaryPostId = Convert.ToUInt64(settingLines[6].Split(':')[1]);
            _peak12SummaryPostId = Convert.ToUInt64(settingLines[7].Split(':')[1]);
            _peak13SummaryPostId = Convert.ToUInt64(settingLines[8].Split(':')[1]);
            _peak14SummaryPostId = Convert.ToUInt64(settingLines[9].Split(':')[1]);

            _peakManagerRole = settingLines[10].Split(':')[1].Split(',').ToList();
            _specialRole = settingLines[11].Split(':')[1];
            _specialRoleEnabled = Convert.ToBoolean(settingLines[12].Split(':')[1]);
            _maxNormalTickets = Convert.ToInt32(settingLines[13].Split(':')[1]);
            _maxSpecialTickets = Convert.ToInt32(settingLines[14].Split(':')[1]);
            MaxBossSessionsNormal = Convert.ToInt32(settingLines[15].Split(':')[1]);
            MaxBossSessionsSpecial = Convert.ToInt32(settingLines[16].Split(':')[1]);
            DisableBooking = Convert.ToBoolean(settingLines[17].Split(':')[1]);
            DisableBookingTime = settingLines[18].Split(':')[1];
            DisableBookingReason = settingLines[19].Split(':')[1];
            AllowExtend = Convert.ToBoolean(settingLines[20].Split(':')[1]);
            MaxSessionExtend = Convert.ToInt32(settingLines[21].Split(':')[1]);
            MaxTicketExtend = Convert.ToInt32(settingLines[22].Split(':')[1]);
            MinMinutesBeforeExtend = Convert.ToInt32(settingLines[23].Split(':')[1]);
            EnabledPeaks = settingLines[24].Split(':')[1].Split(',').ToList();
        }
    }
}

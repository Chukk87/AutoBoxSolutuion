using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using AutoBox.Classes;
using Discord.Addons.Interactive;
using System.IO;

namespace AutoBox.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private IUserMessage CurrentMessage;
        private ulong MessageId;
        private bool running;
        private List<AttendanceData> AllUserData = new List<AttendanceData>();
        private int count = 0;

        [Command("help", RunMode = RunMode.Async)]
        public async Task Help()
        {
            string message = @"Updating your level and combat score. Type *updatecp#999999#99 (999999 being your own combat score and 99 being your level. Updated numbers are refreshed at the first boss)

!newday	- Attendance cleared ready to record a new day (red's continually rotate) this is ran at the very first boss of the day
!start	- Bot ready to record attendance
!+	- Users attendance recorded
!r1	- Number of reds chests available
!y1	- Number of yellows chests available
!b15	- Number of blue chests available
!end	- Ends the attendance session and distributes boxes
!resync	- resyncs memory after bot crash";

            CurrentMessage = await ReplyAsync(message);
        }

        [Command("resync", RunMode = RunMode.Async)]
        public async Task Resync()
        {
            CurrentMessage = await ReplyAsync("Not available");
        }


        [Command("newday", RunMode = RunMode.Async)]
        public async Task NewDay()
        {
            System.IO.File.WriteAllText(@"C:\Users\craig\Desktop\running.txt", "true");
            CurrentMessage = await ReplyAsync("Ok i'm ready, post a message if you are here");
        }

        [Command("start", RunMode = RunMode.Async)]
        public async Task Start()
        {
            System.IO.File.WriteAllText(@"C:\Users\craig\Desktop\running.txt", "true");
            //int SessionId = Convert.ToInt32(System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\sessionId.txt"));
            System.IO.File.WriteAllText(@"C:\Users\craig\Desktop\sessionId.txt", "0");

            CurrentMessage = await ReplyAsync("Ok i'm ready, post a message if you are here");
        }

        [Command("+", RunMode = RunMode.Async)]
        public async Task RecordAttendance()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Mention.ToString());

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("end", RunMode = RunMode.Async)]
        public async Task End()
        {
            //System.IO.File.WriteAllText(@"C:\Users\craig\Desktop\running.txt", "false");
            //string users = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\Names.txt");

            //await ReplyAsync(users + " take 2 yellow boxes");
        }

        [Command("levels", RunMode = RunMode.Async)]
        public async Task level()
        {
            string levels = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\levels.txt");

            await ReplyAsync(levels);
        }

        [Command("legend", RunMode = RunMode.Async)]
        public async Task Legends()
        {
            string legends = "Legendary boxes go to: \r\n" + System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\legendary.txt");

            await ReplyAsync(legends);
        }

        [Command("epic", RunMode = RunMode.Async)]
        public async Task Epic()
        {
            string epics = "Epic boxes go to: \r\n" + System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\epic.txt");

            await ReplyAsync(epics);
        }

        [Command("roll", RunMode = RunMode.Async)]
        public async Task Roll()
        {
            Random rnd = new Random();

            await ReplyAsync("You rolled " + rnd.Next(100));


            //running = true;
            //CurrentMessage = await ReplyAsync("Ok i'm ready, drop a message and let me know you are here");

            //running = true;
            //MessageId = CurrentMessage.Id;
            //await ReplyAsync(MessageId.ToString());

            //Emoji traceEmoji = new Emoji(":ok_hand:");

            //await ReplyAsync("Attendance recorded, the following gets a box: post list from forms here");
            //running = false;
        }

        [Command("y1", RunMode = RunMode.Async)]
        public async Task Yellow1()
        {
            await ReplyAsync("One yellow box registered");
        }

        [Command("y2", RunMode = RunMode.Async)]
        public async Task Yellow2()
        {
            await ReplyAsync("Two yellow boxes registered");
        }

        [Command("y3", RunMode = RunMode.Async)]
        public async Task Yellow3()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("y4", RunMode = RunMode.Async)]
        public async Task Yellow4()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("y5", RunMode = RunMode.Async)]
        public async Task Yellow5()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("r1", RunMode = RunMode.Async)]
        public async Task Red1()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("r2", RunMode = RunMode.Async)]
        public async Task Red2()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\red.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("r3", RunMode = RunMode.Async)]
        public async Task Red3()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("r4", RunMode = RunMode.Async)]
        public async Task Red4()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }

        [Command("r5", RunMode = RunMode.Async)]
        public async Task Red5()
        {
            bool ready = System.IO.File.ReadAllText(@"C:\Users\craig\Desktop\running.txt").Contains("true");

            if (ready)
            {
                System.IO.File.AppendAllText(@"C:\Users\craig\Desktop\Names.txt", Context.Message.Author.Username);

                Emoji traceEmoji = new Emoji(":ok_hand:");
                await Context.Message.AddReactionAsync(traceEmoji);
            }
        }


        private async Task MessageReceived(SocketMessage message)
        {
            if (running)
            {
                var attendance = new AttendanceData();
                attendance.Name = Context.Message.Author.Username;

            }

            if (running)
            {
                var YourEmoji = new Emoji("😀");
                await Context.Message.AddReactionAsync(YourEmoji);
            }
        }

        [Command("id")]
        public async Task Id()
        {
            await ReplyAsync(CurrentMessage.Id.ToString());
        }


        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage ="You don't have the permission ``ban_member``!")]
        public async Task BanMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("Please specify a user!"); 
                return;
            }
            if (reason == null) reason = "Not specified";

            await Context.Guild.AddBanAsync(user, 1, reason);

            var EmbedBuilder = new EmbedBuilder()
                .WithDescription($":white_check_mark: {user.Mention} was banned\n**Reason** {reason}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);

            ITextChannel logChannel = Context.Client.GetChannel(642698444431032330) as ITextChannel;
            var EmbedBuilderLog = new EmbedBuilder()
                .WithDescription($"{user.Mention} was banned\n**Reason** {reason}\n**Moderator** {Context.User.Mention}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embedLog = EmbedBuilderLog.Build();
            await logChannel.SendMessageAsync(embed: embedLog);

        }

        private class AttendanceData
        {
            internal string Name;
        }
    }
}

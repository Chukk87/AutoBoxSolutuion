using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AutoBox.Classes;
using System.Data;
using AutoBox.Modules;
using AutoBox.Classes.enums;
using System.IO;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using AutoBox.Classes.SlashCommands;

namespace AutoBox
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private List<PeakData> _peakData = new List<PeakData>();
        private Settings _settings = new Settings();
        private bool _initialRun = true;
        private DiscordClient _slashCommandClient { get; set; }

        private DateTime _euServerTIme = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Europe Standard Time");

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            InitialRunCheck();


            //DiscordConfiguration slashDiscordConfig = new DiscordConfiguration
            //{
            //    Intents = DiscordIntents.All,
            //    Token = _settings._discordToken,
            //    TokenType = DSharpPlus.TokenType.Bot,
            //    AutoReconnect = true
            //};

            //_slashCommandClient = new DiscordClient(slashDiscordConfig);
            //var slashCommandsConfig = _slashCommandClient.UseSlashCommands();
            //slashCommandsConfig.RegisterCommands<PeakCmd>(951433060916817960);

            //await _slashCommandClient.ConnectAsync();'s 


            _client.Log += _client_Log;

            await RegisterCommandsAsync();
            await _client.LoginAsync(Discord.TokenType.Bot, _settings._discordToken);
            await _client.StartAsync();
            await Task.Delay(-1);

        }

        private void InitialRunCheck()
        {
            if (_initialRun)
            {
                var peaktextData = File.ReadLines(@"C:\PeakBot\PeakData.txt");

                foreach (var line in peaktextData)
                {
                    string[] textDataLine = line.Split('|');
                    var peakData = new PeakData
                    {
                        whom = textDataLine[1],
                        area = textDataLine[2],
                        peakNumber = Convert.ToInt32(textDataLine[3]),
                        tickets = Convert.ToInt32(textDataLine[4]),
                        entryTime = DateTime.Parse(textDataLine[5]),
                        bosses = Convert.ToBoolean(textDataLine[6])
                    };

                    _peakData.Add(peakData);
                }

                _settings.InitialiseSettings();
            }

            _initialRun = false;
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.ReactionAdded += HandleReactionAsync;
        }

        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            if (_client.GetUser(reaction.UserId).IsBot) return;

            if (reaction.Emote.Name == "✅")
            {
                var emotes = await message.GetOrDownloadAsync().Result.GetReactionUsersAsync(new Emoji("👍"), 1000).FlattenAsync();
                int reactionCount = emotes.Count();
                Console.WriteLine(reactionCount.ToString());
            }
            else if (reaction.Emote.Name == "❌")
            {
                ulong botMessageId = 1204803123751948408;
                ulong chanlId = 953285309091676240;

                var chnl = _client.GetChannel(chanlId) as IMessageChannel;
                var reactions = await reaction.Channel.GetMessageAsync(botMessageId) as IUserMessage;



                IEmote oneEmote = new Emoji("\u0031\u20e3");
                IEmote twoEmote = new Emoji("2️⃣");
                IEmote threeEmote = new Emoji("3️⃣");
                IEmote leftEmote = new Emoji("⬅️");  
                IEmote rightEmote = new Emoji("➡️");
                IEmote upEmote = new Emoji("⬆️");
                IEmote downEmote = new Emoji("⬇️");
                IEmote bossEmote = new Emoji("🐗");
                IEmote summonEmote = new Emoji("🎟️");
                IEmote tickEmote = new Emoji("✅");
                IEmote crossEmote = new Emoji("❌");

                try { await reaction.Message.Value.RemoveReactionAsync(oneEmote, reaction.User.Value); } catch(Exception ex) { }

                await reaction.Message.Value.RemoveReactionAsync(oneEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(twoEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(threeEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(leftEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(rightEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(upEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(downEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(bossEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(summonEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(tickEmote, reaction.User.Value);
                await reaction.Message.Value.RemoveReactionAsync(crossEmote, reaction.User.Value);

                //await message.GetOrDownloadAsync().Result.RemoveReactionAsync("👍", reaction.UserId);
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            _euServerTIme = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Europe Standard Time");

            var message = arg as SocketUserMessage;
            if (message.Author.IsBot) return;

            bool discordManager = false;
            bool discordSpecialUser = false;

            foreach(string dRole in _settings._peakManagerRole)
            {
                var roleCheck = (message.Author as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name.Equals(dRole));

                if (roleCheck != null)
                {
                    if (roleCheck.Name == dRole)
                    {
                        discordManager = true;
                        continue;
                    }
                }
            }

            var specialRoleCheck = (message.Author as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name.Equals(_settings._specialRole));
            if (specialRoleCheck != null && _settings._specialRoleEnabled)
            {
                discordSpecialUser = true;
            }
            else
            {
                discordSpecialUser = false;
            }

            if (discordManager)
            {
                await ReserveEmojiBotSetup(message);
                await BotSetup(message);
                await PeakClear(message);
            }


            //await BotSendMessage(message);

            //await PeakReserve(message, discordSpecialUser);

            //await PeakExtend(message, discordSpecialUser);
            //await PeakCancel(message, discordManager);
            //await PeakBan(message, discordManager);
            //await DisableReserve(message, discordManager);
        }

        private async Task ReserveEmojiBotSetup(SocketUserMessage message)
        {
            var chnl = _client.GetChannel(message.Channel.Id) as IMessageChannel;

            if (message.Content.ToUpper().Contains("!PEAKMSG"))
            {
                try
                {
                    string discordMessage = "TEST MESSAGE: To reserve your peak activity, using the emojis select your number of tickets (1️⃣, 2️⃣, 3️⃣)" + Environment.NewLine +
                                            "Then select one activity(⬅️ Left Aggresive, ➡️ Right Aggresive, ⬆️ North Aggresive, ⬇️ South Aggresive, 🐗 Boss Rotation, 🎟️ Boss Summoning)" + Environment.NewLine +
                                            "Then to confirm your choice Select the White check emoji ✅, if you make a mistake click the cross emoji ❌";

                    var messageSetPost = await chnl.SendMessageAsync(discordMessage);
                    await messageSetPost.AddReactionAsync(new Emoji("1️⃣"));
                    await messageSetPost.AddReactionAsync(new Emoji("2️⃣"));
                    await messageSetPost.AddReactionAsync(new Emoji("3️⃣"));
                    await messageSetPost.AddReactionAsync(new Emoji("⬅️"));
                    await messageSetPost.AddReactionAsync(new Emoji("➡️"));
                    await messageSetPost.AddReactionAsync(new Emoji("⬆️"));
                    await messageSetPost.AddReactionAsync(new Emoji("⬇️"));
                    await messageSetPost.AddReactionAsync(new Emoji("🐗"));
                    await messageSetPost.AddReactionAsync(new Emoji("🎟️"));
                    await messageSetPost.AddReactionAsync(new Emoji("✅"));
                    await messageSetPost.AddReactionAsync(new Emoji("❌"));

                    await chnl.SendMessageAsync("Message Id is: " + messageSetPost.Id.ToString());

                    //byte[] image = File.ReadAllBytes(@"C:\Users\craig\Downloads\PeakImage.png");

                    //var messageSetPost = await chnl.SendFileAsync(new MemoryStream(image), "PeakImage.png", "Test");
                }
                catch(Exception ex)
                {
                    await message.ReplyAsync(ex.ToString());
                }


            }
            else
            {
                await chnl.SendMessageAsync("Failed");
            }
        }

        private async Task HandlePeakReactionAsync(SocketReaction reaction)
        {
            if (_client.GetUser(reaction.UserId).IsBot) return;

            if (reaction.Emote.Name == "👍")
            {
                //var emotes = await reaction.GetOrDownloadAsync().Result.GetReactionUsersAsync(new Emoji("👍"), 1000).FlattenAsync();
                //int reactionCount = emotes.Count();
                //Console.WriteLine(reactionCount.ToString());
            }
        }




        private async Task DisableReserve(SocketUserMessage message, bool discordManager)
        {
            //PEAKDISABLE 21/09 18:00 8 (REASON)

            if (message.Content.ToUpper().Contains("PEAKDISABLE"))
            {
                try
                {
                    if (!discordManager)
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("You don't have permisions to do this");

                        return;
                    }

                    string[] messageData = message.Content.ToUpper().Trim().Split(' ');
                    string textDate = messageData[1] + "/2023 " + messageData[2];
                    DateTime suppliedTime = DateTime.Parse(textDate);
                    int ticketCount = Convert.ToInt32(messageData[3]);

                    string reason = message.Content.ToUpper().Trim().Split('(')[1];
                    reason = reason.Replace("(", string.Empty);
                    reason = reason.Replace(")", string.Empty);


                    string settingsFile = @"C:\PeakBot\Settings.txt";
                    _settings.DisableBooking = true;

                    LineChanger("DisableBooking:true", settingsFile, 17);
                    LineChanger("DisableBookingTime:" + suppliedTime, settingsFile, 18);
                    LineChanger("DisableBookingReason:" + reason, settingsFile, 19);


                    await message.ReplyAsync("Peak has been disabled from " 
                        + suppliedTime.ToString("dd/MM hh:mm") 
                        + " until " 
                        + suppliedTime.AddMinutes(ticketCount * 30).ToString("dd/MM hh:mm"));
                }
                catch
                {
                    var thumbsDownEmoji = new Emoji("👎");
                    await message.AddReactionAsync(thumbsDownEmoji);
                    await message.ReplyAsync("There was a problem with your request");

                    return;
                }
            }
        }


        //private async Task PeakBan(SocketUserMessage message, bool discordManager)
        //{
        //    if (message.Content.ToUpper().Contains("PEAKBAN"))
        //    {
        //        if (!discordManager)
        //        {
        //            var thumbsDownEmoji = new Emoji("👎");
        //            await message.AddReactionAsync(thumbsDownEmoji);
        //            await message.ReplyAsync("You don't have permisions to do this");

        //            return;
        //        }

        //        string[] messageData = message.Content.ToUpper().Trim().Split(' ');
        //        string who = messageData[1];
        //        string reason = message.Content.ToUpper().Trim().Split('(')[0];
        //        reason.Replace("(", string.Empty);
        //        reason.Replace(")", string.Empty);

        //        //create an incident
        //        var peakIncident = new Incident
        //        {
        //            When = _euServerTIme,
        //            DaysBanned = Convert.ToInt32(messageData[2]),
        //            Reason = reason
        //        };

        //        await message.ReplyAsync("Incident created, banned " + messageData[1] + " for " + peakIncident.DaysBanned + " Reason: " + peakIncident.Reason);
        //    }
        //}

        private async Task PeakExtend(SocketUserMessage message, bool specialUser)
        {
            _euServerTIme = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Europe Standard Time");

            if (message.Content.ToUpper().Contains("PEAKE"))
            {
                string[] messageData = message.Content.ToUpper().Trim().Split(' ');

                try
                {
                    int ticketNumber = Convert.ToInt32(messageData[1]);
                    bool bosses = false;

                    if (messageData.Length > 2)
                    {
                        if (messageData[2] == "B")
                        {
                            bosses = true;
                        }
                    }

                    var usersCurrentSession = _peakData.Find(x => x.whom == message.Author.Mention &&
                                                              x.entryTime.AddMinutes(x.tickets * 30) > _euServerTIme);

                    var dateTimeCanEnter = usersCurrentSession.entryTime.AddMinutes((usersCurrentSession.tickets * 30) - 15);

                    if (dateTimeCanEnter >= _euServerTIme)
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("You can extend when you have 15 minutes left of your current reservation");
                    }
                    else
                    {
                        var extendedSession = new PeakData();

                        extendedSession.whom = usersCurrentSession.whom;
                        extendedSession.area = usersCurrentSession.area;
                        extendedSession.peakNumber = usersCurrentSession.peakNumber;
                        extendedSession.tickets = ticketNumber;
                        extendedSession.entryTime = usersCurrentSession.entryTime.AddMinutes(usersCurrentSession.tickets * 30);
                        extendedSession.bosses = bosses;

                        extendedSession.textLine = string.Concat(_euServerTIme, "|", extendedSession.whom, "|", extendedSession.area, "|", extendedSession.peakNumber, "|", extendedSession.tickets, "|", extendedSession.entryTime, "|", extendedSession.bosses.ToString());

                        //Check if time overlapses with someone else
                        List<PeakData> available = _peakData.FindAll(
                                x => x.entryTime.AddMinutes((30 * x.tickets) - 1) >= extendedSession.entryTime &&
                                x.entryTime <= extendedSession.entryTime.AddMinutes(30 * extendedSession.tickets) &&
                                x.peakNumber == extendedSession.peakNumber &&
                                x.bosses == extendedSession.bosses &&
                                x.area == extendedSession.area);
                        List<PeakData> bossesAvailable = _peakData.FindAll(
                                x => x.entryTime.AddMinutes((30 * x.tickets) - 1) >= extendedSession.entryTime &&
                                x.entryTime <= extendedSession.entryTime.AddMinutes((30 * extendedSession.tickets) - 1) &&
                                x.peakNumber == extendedSession.peakNumber &&
                                x.bosses == true);
                        int usersActivereservations = _peakData.FindAll(
                                x => x.entryTime.AddMinutes((30 * x.tickets) - 1) >= extendedSession.entryTime &&
                                x.entryTime <= extendedSession.entryTime.AddMinutes(30 * extendedSession.tickets) &&
                                x.whom == extendedSession.whom).Count;
                        int peakReservationNearSessions = _peakData.FindAll(
                                x => x.entryTime.AddMinutes((30 * (x.tickets) - 1)) >= extendedSession.entryTime &&
                                x.whom == extendedSession.whom).Count;

                        //Need to do a global boss check
                        if (peakReservationNearSessions > 0)
                        {
                            var thumbsDownEmoji = new Emoji("👎");
                            await message.AddReactionAsync(thumbsDownEmoji);
                            await message.ReplyAsync("You can not reserve more than one peak spot consistently, there must be a 30 minute gap, you can extend in the last 15 minutes using the PEAKE 'Number of tickets' 'B' add B for bosses aswell (optional)");

                            return;
                        }

                        if (usersActivereservations > 0)
                        {
                            var thumbsDownEmoji = new Emoji("👎");
                            await message.AddReactionAsync(thumbsDownEmoji);
                            await message.ReplyAsync("You have another reservation that clashes with these times");

                            return;
                        }

                        if (extendedSession.bosses == true && bossesAvailable.Count > 0)
                        {
                            var thumbsDownEmoji = new Emoji("👎");
                            await message.AddReactionAsync(thumbsDownEmoji);
                            await message.ReplyAsync("Bosses have already been taken by someone else");

                            return;
                        }

                        if (!Enum.IsDefined(typeof(PeakTypes), extendedSession.area))
                        {
                            var thumbsDownEmoji = new Emoji("👎");
                            await message.AddReactionAsync(thumbsDownEmoji);
                            await message.ReplyAsync("Unknown peak type supplied");

                            return;
                        }

                        //CONDITION CHECK TO SEE WHAT APPLIES TO THIS USER
                        int maxTicketNumber = _settings.MaxTicketExtend;

                        if (extendedSession.tickets <= maxTicketNumber)
                        {
                            if (available.Count == 0)
                            {
                                _peakData.Add(extendedSession);

                                var thumbsUpEmoji = new Emoji("👍");
                                await message.AddReactionAsync(thumbsUpEmoji);

                                //ADD TO TEXT FILE
                                File.AppendAllText(@"C:\PeakBot\PeakData.txt", extendedSession.textLine + Environment.NewLine);

                                await PeakQueue(message, extendedSession.peakNumber);
                            }
                            else
                            {
                                if (usersActivereservations == 0)
                                {
                                    var thumbsDownEmoji = new Emoji("👎");
                                    await message.AddReactionAsync(thumbsDownEmoji);
                                    await message.ReplyAsync("This slot is already taken");
                                }
                            }
                        }
                        else
                        {
                            var thumbsDownEmoji = new Emoji("👎");
                            await message.AddReactionAsync(thumbsDownEmoji);
                            await message.ReplyAsync("Maximum number of tickets you can use in 1 session is 8");
                        }
                    }
                }
                catch
                {
                    var thumbsDownEmoji = new Emoji("👎");
                    await message.AddReactionAsync(thumbsDownEmoji);
                    await message.ReplyAsync("There was an issue with your request");
                }
            }
        }

        private async Task PeakCancel(SocketUserMessage message, bool discordManager)
        {
            if (message.Content.ToUpper().Contains("PEAKC") && !message.Content.ToUpper().StartsWith("!"))
            {
                try
                {
                    string[] messageData = message.Content.ToUpper().Split(' ');
                    string textDate = messageData[1] + "/2023 " + messageData[2];

                    DateTime suppliedEntryTime = DateTime.Parse(textDate);
                    string forWho = string.Empty;

                    if (messageData.Count() > 3 && discordManager)
                    {
                        forWho = messageData[3].ToString();
                        forWho = forWho.Insert(2, "!");
                    }
                    else
                    {
                        forWho = message.Author.Mention;
                    }

                    PeakData findSession = _peakData.Find(x => x.whom == forWho &&
                        x.entryTime == suppliedEntryTime);

                    if (findSession == null)
                    {
                        await message.ReplyAsync("I couldn't find your reservation");
                    }
                    else
                    {
                        //FIND DATA IN TEXT AND REMOVE
                        string searchFor = string.Join("|", findSession.whom,
                                                            findSession.area,
                                                            findSession.peakNumber,
                                                            findSession.tickets,
                                                            findSession.entryTime);

                        List<string> lines = File.ReadAllLines(@"C:\PeakBot\PeakData.txt").ToList();
                        int lineNumber = 0;
                        bool removed = false;

                        try
                        {
                            foreach (string line in lines)
                            {
                                if (line.Contains(searchFor))
                                {
                                    lines.RemoveAt(lineNumber);

                                    removed = true;
                                    var thumbsUpEmoji = new Emoji("👍");
                                    await message.AddReactionAsync(thumbsUpEmoji);

                                    File.WriteAllLines(@"C:\PeakBot\PeakData.txt", lines);
                                    continue;
                                }

                                lineNumber++;
                            }
                        }
                        catch { }

                        if(removed)
                        {
                            _peakData.Remove(findSession);
                            await PeakQueue(message, findSession.peakNumber);

                            await message.ReplyAsync("This reservation has been cancelled");
                        }

                    }
                }
                catch
                { }
            }
        }

        private async Task PeakClear(SocketUserMessage message)
        {
            if (message.Content.ToUpper().Contains("!PEAKCLEAR"))
            {
                File.WriteAllText(@"C:\PeakBot\PeakData.txt", string.Empty);

                _peakData = new List<PeakData>();
                InitialRunCheck();

                await message.ReplyAsync("All reservations have been removed");
            }
        }

        private async Task BotSetup(SocketUserMessage message)
        {
            string settingsFile = @"C:\PeakBot\Settings.txt";

            if (message.Content.ToUpper().Contains("!PEAKSETUP"))
            {
                await message.ReplyAsync("You need to supply a channel which can be use to display a summary of peak reservations. To configure this, type !PEAKSUMMARY followed by the channel Id");
            }
            else if (message.Content.ToUpper().Contains("!PEAKSUMMARY"))
            {
                try
                {
                    ulong suppliedId = Convert.ToUInt64(message.Content.Split(' ')[1]);

                    LineChanger("SummaryChannelId:" + suppliedId.ToString(), settingsFile, 1);

                    var thumbsUpEmoji = new Emoji("👍");
                    await message.AddReactionAsync(thumbsUpEmoji);
                    await message.ReplyAsync("Great, please tell me what Peak floors to enable by typing !PEAKFENABLE followed by your floor numbers. Use a ',' to add more than one. E.g. !PEAKFENABLE 7,8,9");

                    _settings.InitialiseSettings();
                }
                catch (Exception ex)
                {
                    var thumbsDownEmoji = new Emoji("👎");
                    await message.AddReactionAsync(thumbsDownEmoji);
                    await message.ReplyAsync("There was a problem with the ChannelId you supplied" + Environment.NewLine + ex.ToString());
                }

                //await message.ReplyAsync(settings);
            }
            else if (message.Content.ToUpper().Contains("!PEAKFENABLE"))
            {
                try
                {
                    List<string> enablePeaks = message.Content.Split(' ')[1].Split(',').ToList();

                    LineChanger("EnabledPeaks:" + message.Content.Split(' ')[1], settingsFile, 24);

                    var chnl = _client.GetChannel(_settings._summaryChannelId) as IMessageChannel;

                    if(enablePeaks.Contains("7"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 7 RESERVATION");

                        LineChanger("Peak7SummaryPostId:" + sevenMessaged.Id, settingsFile, 2);
                    }

                    if (enablePeaks.Contains("8"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 8 RESERVATION");

                        LineChanger("Peak8SummaryPostId:" + sevenMessaged.Id, settingsFile, 3);
                    }

                    if (enablePeaks.Contains("9"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 9 RESERVATION");

                        LineChanger("Peak9SummaryPostId:" + sevenMessaged.Id, settingsFile, 4);
                    }

                    if (enablePeaks.Contains("10"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 10 RESERVATION");

                        LineChanger("Peak10SummaryPostId:" + sevenMessaged.Id, settingsFile, 5);
                    }

                    if (enablePeaks.Contains("11"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 11 RESERVATION");

                        LineChanger("Peak11SummaryPostId:" + sevenMessaged.Id, settingsFile, 6);
                    }

                    if (enablePeaks.Contains("12"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 12 RESERVATION");

                        LineChanger("Peak12SummaryPostId:" + sevenMessaged.Id, settingsFile, 7);
                    }

                    if (enablePeaks.Contains("13"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 13 RESERVATION");

                        LineChanger("Peak13SummaryPostId:" + sevenMessaged.Id, settingsFile, 8);
                    }

                    if (enablePeaks.Contains("14"))
                    {
                        var sevenMessaged = await chnl.SendMessageAsync("PEAK 14 RESERVATION");

                        LineChanger("Peak14SummaryPostId:" + sevenMessaged.Id, settingsFile, 9);
                    }

                    var thumbsUpEmoji = new Emoji("👍");
                    await message.AddReactionAsync(thumbsUpEmoji);
                    await message.ReplyAsync("The bot is set up and ready");

                    _settings.InitialiseSettings();
                }
                catch
                {
                    var thumbsDownEmoji = new Emoji("👎");
                    await message.AddReactionAsync(thumbsDownEmoji);
                    await message.ReplyAsync("There was a problem");
                }
            }
            else if (message.Content.ToUpper().Contains("!PEAKINITIALISE"))
            {
                var thumbsUpEmoji = new Emoji("👍");
                await message.AddReactionAsync(thumbsUpEmoji);
                await message.ReplyAsync("Setting have been initialised");

                _settings.InitialiseSettings();
            }
        }

        static void LineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        private async Task BotSendMessage(SocketUserMessage message)
        {
            ulong id = 1148968931856502846;
            var chnl = _client.GetChannel(id) as IMessageChannel;

            if (message.Content.ToUpper().Contains("!BOTMESSAGE"))
            {
                await chnl.SendMessageAsync(message.Content.Remove(0, 12));
            }
            else if(message.Content.ToUpper().Contains("!PEAKSETTINGS"))
            {
                string settings = File.ReadAllText(@"C://PeakBot/Settings.txt");

                await message.ReplyAsync(settings);
            }
            else if (message.Content.ToUpper().Contains("!EUTIME"))
            {
                await message.ReplyAsync("The EU server time now is: " + _euServerTIme.ToString());
            }
        }

        //GOOD
        private async Task PeakQueue(SocketUserMessage message, int peakNumber)
        {
            string reservedList = string.Empty;

            var peakdata = _peakData.FindAll(
                                x => x.entryTime.AddMinutes(30 * x.tickets) >= _euServerTIme &&
                                x.peakNumber == peakNumber);

            var peakOrdered = peakdata
                .Where(src => src.entryTime.AddMinutes(30 * src.tickets) >= _euServerTIme)
                .OrderBy(src => src.area)
                .ThenBy(src => src.entryTime)
                .ToList();

            string postText = CreatePostText(peakNumber, peakOrdered);

            await Announce(peakNumber, postText);
        }

        //GOOD
        private static string CreatePostText(int peakNumber, List<PeakData> peakOrdered)
        {
            string textLine = "__**PEAK " + peakNumber + " RESERVATIONS**__" + Environment.NewLine + Environment.NewLine;
            foreach (var data in peakOrdered)
            {
                string area = string.Empty;

                switch (data.area)
                {
                    case "PEAKTA":
                        area = "Top Agro";
                        break;
                    case "PEAKBA":
                        area = "Bottom Agro";
                        break;
                    case "PEAKLA":
                        area = "Left Agro";
                        break;
                    case "PEAKRA":
                        area = "Right Agro";
                        break;
                    case "PEAKB":
                        area = "Bosses";
                        break;
                    case "PEAKO":
                        area = "Ores";
                        break;
                }

                if (!data.bosses)
                {
                    textLine += data.whom +
                                data.entryTime.ToString(" dd/MM HH:mm - ") +
                                data.entryTime.AddMinutes(30 * data.tickets).ToString("dd/MM HH:mm") +
                                " In " + area +
                                Environment.NewLine;
                }
                else
                {
                    textLine += data.whom +
                                data.entryTime.ToString(" dd/MM HH:mm - ") +
                                data.entryTime.AddMinutes(30 * data.tickets).ToString("dd/MM HH:mm") +
                                " In " + area +
                                " with __bosses__ " +
                                Environment.NewLine;
                }

                
            }

            textLine += Environment.NewLine + Environment.NewLine;

            return textLine;
        }

        private async Task PeakReserve(SocketUserMessage message, bool specialUser)
        {
            if (message.Content.ToUpper().Contains("PEAK") && !message.Content.ToUpper().StartsWith("!") 
                && !message.Content.ToUpper().StartsWith("PEAKE") 
                && !message.Content.ToUpper().StartsWith("PEAKC")
                && !message.Content.ToUpper().StartsWith("PEAKBAN")
                && !message.Content.ToUpper().StartsWith("PEAKDISABLE"))
            {
                string[] messageData = message.Content.ToUpper().Trim().Split(' ');

                string textDate = messageData[3] + "/" + _euServerTIme.ToString("yyyy") + " " + messageData[4];
                DateTime suppliedEntryTime = DateTime.Parse(textDate);
                DateTime ServerTimePlus = _euServerTIme.AddMinutes(240);

                if (!specialUser)
                {
                    if (suppliedEntryTime > ServerTimePlus)
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("You can only make a reservation up to 4 hours before");

                        return;
                    }
                }

                try
                {
                    var data = new PeakData
                    {
                        whom = message.Author.Mention.ToString(),
                        area = messageData[0],
                        peakNumber = Convert.ToInt32(messageData[1]),
                        tickets = Convert.ToInt32(messageData[2]),
                        entryTime = suppliedEntryTime
                    };

                    if (messageData.Length >= 6)
                    {
                        if (messageData[5].ToUpper() == "B")
                        {
                            data.bosses = true;
                        }
                    }

                    data.textLine = string.Concat(_euServerTIme, "|", data.whom, "|", data.area, "|", data.peakNumber, "|", data.tickets, "|", data.entryTime, "|", data.bosses.ToString());

                    List<PeakData> available, bossesAvailable;
                    int usersActivereservations, peakReservationNearSessions;
                    GetDataStats(data, out available, out bossesAvailable, out usersActivereservations, out peakReservationNearSessions);

                    bool peakFloorEnabled = _settings.EnabledPeaks.Contains(data.peakNumber.ToString());

                    //Need to do a global boss check
                    if (peakReservationNearSessions > 0)
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("You can not reserve more than one peak spot consistently, there must be a 30 minute gap, you can extend in the last 15 minutes using the PEAKE 'Number of tickets' 'B' add B for bosses aswell (optional)");

                        return;
                    }

                    if (usersActivereservations > 0)
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("You have another reservation that clashes with these times");

                        return;
                    }

                    if (data.bosses == true && bossesAvailable.Count > 0)
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("Bosses have already been taken by someone else");

                        return;
                    }

                    if (!Enum.IsDefined(typeof(PeakTypes), data.area))
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("Unknown peak type supplied");

                        return;
                    }

                    //CONDITION CHECK TO SEE WHAT APPLIES TO THIS USER
                    int maxTicketNumber;

                    if (specialUser)
                    {
                        if (_settings._maxSpecialTickets > 0)
                        {
                            maxTicketNumber = _settings._maxSpecialTickets;
                        }
                        else
                        {
                            maxTicketNumber = 9999;
                        }
                    }
                    else
                    {
                        maxTicketNumber = _settings._maxNormalTickets;
                    }

                    if (data.tickets <= maxTicketNumber)
                    {
                        if (available.Count == 0)
                        {
                            _peakData.Add(data);

                            var thumbsUpEmoji = new Emoji("👍");
                            await message.AddReactionAsync(thumbsUpEmoji);

                            //ADD TO TEXT FILE
                            File.AppendAllText(@"C:\PeakBot\PeakData.txt", data.textLine + Environment.NewLine);

                            await PeakQueue(message, data.peakNumber);
                        }
                        else
                        {
                            if (usersActivereservations == 0)
                            {
                                var thumbsDownEmoji = new Emoji("👎");
                                await message.AddReactionAsync(thumbsDownEmoji);
                                await message.ReplyAsync("This slot is already taken");
                            }
                        }
                    }
                    else
                    {
                        var thumbsDownEmoji = new Emoji("👎");
                        await message.AddReactionAsync(thumbsDownEmoji);
                        await message.ReplyAsync("Maximum number of tickets you can use in 1 session is 8, 2 to extend a session");
                    }

                }
                catch
                {
                    var thumbsDownEmoji = new Emoji("👎");
                    await message.AddReactionAsync(thumbsDownEmoji);
                    await message.ReplyAsync("There was an issue with your request");
                }
            }
        }

        private void GetDataStats(PeakData data, out List<PeakData> available, out List<PeakData> bossesAvailable, out int usersActivereservations, out int peakReservationNearSessions)
        {
            //Check if time overlapses with someone else
            available = _peakData.FindAll(
                    x => x.entryTime.AddMinutes((30 * x.tickets) - 1) >= data.entryTime &&
                    x.entryTime <= data.entryTime.AddMinutes(30 * data.tickets) &&
                    x.peakNumber == data.peakNumber &&
                    x.bosses == data.bosses &&
                    x.area == data.area);
            bossesAvailable = _peakData.FindAll(
                    x => x.entryTime.AddMinutes((30 * x.tickets) - 1) >= data.entryTime &&
                    x.entryTime <= data.entryTime.AddMinutes((30 * data.tickets) - 1) &&
                    x.peakNumber == data.peakNumber &&
                    x.bosses == true);
            usersActivereservations = _peakData.FindAll(
                    x => x.entryTime.AddMinutes((30 * x.tickets) - 1) >= data.entryTime &&
                    x.entryTime <= data.entryTime.AddMinutes(30 * data.tickets) &&
                    x.whom == data.whom).Count;
            peakReservationNearSessions = _peakData.FindAll(
                    x => x.entryTime.AddMinutes((30 * (x.tickets + 1) - 1)) >= data.entryTime &&
                    x.whom == data.whom).Count;
        }

        public async Task Announce(int peakNumber, string peakReservations)
        {
            ulong botMessageId = 999;

            switch (peakNumber)
            {
                case 7:
                    botMessageId = _settings._peak7SummaryPostId;
                    break;
                case 8:
                    botMessageId = _settings._peak8SummaryPostId;
                    break;
                case 9:
                    botMessageId = _settings._peak9SummaryPostId;
                    break;
                case 10:
                    botMessageId = _settings._peak10SummaryPostId;
                    break;
                case 11:
                    botMessageId = _settings._peak11SummaryPostId;
                    break;
                case 12:
                    botMessageId = _settings._peak12SummaryPostId;
                    break;
                case 13:
                    botMessageId = _settings._peak13SummaryPostId;
                    break;
                case 14:
                    botMessageId = _settings._peak14SummaryPostId;
                    break;
            }

            var chnl = _client.GetChannel(_settings._summaryChannelId) as IMessageChannel;

            await chnl.ModifyMessageAsync(botMessageId, m => m.Content = peakReservations);
        }
    }
}

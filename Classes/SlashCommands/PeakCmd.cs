using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBox.Classes.SlashCommands
{
    public class PeakCmd : ApplicationCommandModule
    {
        [SlashCommand("PEAKTA", "Peak top agro spot booking")]
        public async Task PeakTaCommand(InteractionContext ctx)
        {
            var embedmessage = new DiscordEmbedBuilder()
            {
                Title = "TEST"
            };

            await ctx.Channel.SendMessageAsync(embed: embedmessage);
        }

    }
}

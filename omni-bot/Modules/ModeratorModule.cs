using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Modules;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using System.IO;
using Newtonsoft.Json;

namespace omni_bot
{
    class ModeratorModule : IModule
    {
        private ModuleManager manager;
        private DiscordClient client;

        private string settingspath = "modules/moderator/moderatorsettings.json";

        private ModeratorSettings settings;

        void IModule.Install(ModuleManager Manager)
        {
            manager = Manager;
            client = Manager.Client;

            settings = Json.InitSettings<ModeratorSettings>(settingspath);

            client.GetService<CommandService>().CreateGroup("", g =>
            {
                g.CreateCommand("kick")
                .Description("Kicks user from the server.")
                .Parameter("user")
                .Parameter("reason", ParameterType.Optional)
                .MinPermissions((int)PermissionLevel.ServerModerator)
                .Do(async e =>
                {
                    var user = e.Message.MentionedUsers.FirstOrDefault();
                    if (user == null) return;
                    await user.SendMessage("You have been kicked." + (e.GetArg("reason") == null ? "" : $"Reason: {e.GetArg("reason")}."));
                    await user.Kick();
                    await e.Channel.SendMessage($"Kicked {user.Name}.");
                });

                g.CreateCommand("ban")
                .Description("Bans a user from the server.")
                .Parameter("user")
                .Parameter("reason")
                .MinPermissions((int)PermissionLevel.ServerModerator)
                .Do(async e =>
                {
                    var user = e.Message.MentionedUsers.FirstOrDefault();
                    if (user == null) return;
                    await user.SendMessage($"You have been banned. Reason: {e.GetArg("reason")}.");
                    await user.Server.Ban(user);
                    await e.Channel.SendMessage($"B A N H A M M E R\r\nvitim: {user.Name}");
                });
                g.CreateCommand("mute")
                .Parameter("user")
                .MinPermissions((int)PermissionLevel.ServerModerator)
                .Do(async e =>
                {
                    var user = e.Message.MentionedUsers.FirstOrDefault();
                    if (user == null) return;

                    await user.Edit(isMuted: true);
                    await e.Channel.SendMessage($"Muted {user.Name}.");
                });
                g.CreateCommand("unmute")
                    .Parameter("user")
                    .MinPermissions((int)PermissionLevel.ServerModerator)
                    .Do(async e =>
                    {
                        var user = e.Message.MentionedUsers.FirstOrDefault();
                        if (user == null) return;

                        await user.Edit(isMuted: false);
                        await e.Channel.SendMessage($"Unmuted {user.Name}.");
                    });
                g.CreateCommand("deafen")
                    .Parameter("user")
                    .Parameter("discriminator", ParameterType.Optional)
                    .MinPermissions((int)PermissionLevel.ServerModerator)
                    .Do(async e =>
                    {
                        var user = e.Message.MentionedUsers.FirstOrDefault();
                        if (user == null) return;

                        await user.Edit(isDeafened: true);
                        await e.Channel.SendMessage($"Deafened {user.Name}.");
                    });
                g.CreateCommand("undeafen")
                    .Parameter("user")
                    .Parameter("discriminator", ParameterType.Optional)
                    .MinPermissions((int)PermissionLevel.ServerModerator)
                    .Do(async e =>
                    {
                        var user = e.Message.MentionedUsers.FirstOrDefault();
                        if (user == null) return;

                        await user.Edit(isDeafened: false);
                        await e.Channel.SendMessage($"Undeafened {user.Name}.");
                    });
            });

            client.MessageReceived += async (s, e) =>
            {
                Console.WriteLine($"[{e.User.Name}] {e.Message.RawText}");
                if (e.Message.IsAuthor) return;
                if (settings.BannedWords.Any(w => e.Message.Text.ToLower().Contains(w)))
                    await e.Message.Delete();
            };
        }
    }
}

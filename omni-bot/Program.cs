using System;
using Discord;
using Discord.Commands;
using Discord.Modules;
using Discord.Audio;
using Discord.Commands.Permissions.Levels;
using System.Linq;
using Newtonsoft.Json;

namespace omni_bot
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();

        private DiscordClient _client;

        private string settingspath = "settings.json";

        private Settings settings;

        public void Start()
        {
            Console.Title = "0MN1-2000 - your friendly neighborhood bot";
            Json.InitDirs();
            settings = Json.InitSettings<Settings>(settingspath);

            _client = new DiscordClient(x =>
            {
                x.AppName = "0MN1-2000";
                x.AppUrl = settings.RepoPath;
                x.AppVersion = settings.Version;
                x.LogLevel = LogSeverity.Debug;
                x.UsePermissionsCache = true;
            })
            .UsingCommands(x =>
            {
                x.PrefixChar = settings.CommandChar;
                x.HelpMode = HelpMode.Public;
                x.ErrorHandler = OnCommandError;
            })
            .UsingModules()
            .UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
                x.EnableEncryption = true;
                x.Bitrate = AudioServiceConfig.MaxBitrate;
                x.BufferLength = 10000;
            })
            .UsingPermissionLevels(PermissionResolver);

            _client.AddModule<ModeratorModule>();

            _client.UserJoined += async (s, e) =>
            {
                Console.WriteLine($"{e.User.Name} joined");
                await e.Server.DefaultChannel.SendMessage($"{e.User.Name} joined.");
            };

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(settings.BotToken);
                Console.Title += $" [online users: {_client.Servers.SelectMany(s => s.Users.Select(u => u.Id)).Distinct().Count()}";
            });
        }
        private void OnCommandError(object sender, CommandErrorEventArgs e)
        {
            string msg = e.Exception?.Message;
            if (msg == null) //No exception - show a generic message
            {
                switch (e.ErrorType)
                {
                    case CommandErrorType.Exception:
                        msg = "Unknown error.";
                        break;
                    case CommandErrorType.BadPermissions:
                        msg = "You do not have permission to run this command.";
                        break;
                    case CommandErrorType.BadArgCount:
                        msg = "You provided the incorrect number of arguments for this command.";
                        break;
                    case CommandErrorType.InvalidInput:
                        msg = "Unable to parse your command, please check your input.";
                        break;
                    case CommandErrorType.UnknownCommand:
                        msg = "Unknown command.";
                        break;
                }
            }
            if (msg != null)
            {
                //_client.ReplyError(e, msg);
                _client.Log.Error("Command", msg);
            }
        }
        private int PermissionResolver(User user, Channel channel)
        {
            if (user.Id == settings.OwnerID)
                return (int)PermissionLevel.BotOwner;
            if (user.Server != null)
            {
                if (user == channel.Server.Owner)
                    return (int)PermissionLevel.ServerOwner;

                var serverPerms = user.ServerPermissions;
                if (serverPerms.ManageRoles)
                    return (int)PermissionLevel.ServerAdmin;
                if (serverPerms.ManageMessages && serverPerms.KickMembers && serverPerms.BanMembers)
                    return (int)PermissionLevel.ServerModerator;

                var channelPerms = user.GetPermissions(channel);
                if (channelPerms.ManagePermissions)
                    return (int)PermissionLevel.ChannelAdmin;
                if (channelPerms.ManageMessages)
                    return (int)PermissionLevel.ChannelModerator;
            }
            return (int)PermissionLevel.User;
        }
    }
}

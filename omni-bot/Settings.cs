using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace omni_bot
{
    public interface ISettings { }

    public class Settings : ISettings
    {
        public string BotToken { get; set; } = "aabbccdd";
        public string RepoPath { get; set; }
        public string Version { get; set; } = "-1.0";
        public char CommandChar { get; set; } = '/';
        public ulong OwnerID { get; set; }
        //public List<string> ActiveModules;
    }
}

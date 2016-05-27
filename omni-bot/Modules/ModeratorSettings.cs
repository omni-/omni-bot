using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace omni_bot
{
    public class ModeratorSettings : ISettings
    {
        public List<string> BannedWords { get; set; }
        public ActionOnViolation action { get; set; } = ActionOnViolation.DeleteMessage;
    }
    public enum ActionOnViolation
    {
        Nothing,
        DeleteMessage,
        Kick,
        Ban
    }
}

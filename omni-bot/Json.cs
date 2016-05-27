using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace omni_bot
{
    public class Json
    {
        public static void InitDirs()
        {
            Directory.CreateDirectory("modules/moderator");
        }
        public static T InitSettings<T>(string path) where T : ISettings
        {
            T o;
            if (!File.Exists(path))
            {
                o = Activator.CreateInstance<T>();
                string json = JsonConvert.SerializeObject(o);
                File.Create(path).Close();
                File.WriteAllText(path, json);
                return o;
            }
            else
            {
                try
                {
                    o = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                }
                catch (JsonSerializationException e)
                {
                    Console.WriteLine(e.InnerException);
                    o = Activator.CreateInstance<T>();
                }
                return o;
            }
        }
    }
}

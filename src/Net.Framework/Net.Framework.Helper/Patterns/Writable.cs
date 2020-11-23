using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Helper.Patterns
{
    public interface IWritable
    {
        void Save(string configPath);
    }

    public abstract class Writable<T> : IWritable where T : IWritable, new()
    {
        public void Save(string configPath) => File.WriteAllText(Path.Combine(configPath, $"{typeof(T).Name}.cfg"), JsonConvert.SerializeObject(this));

        public static T Load(string configPath)
        {
            var path = Path.Combine(configPath, $"{typeof(T).Name}.cfg");

            if (File.Exists(path) == false)
            {
                var config = new T();

                config.Save(configPath);
                return config;
            }

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}

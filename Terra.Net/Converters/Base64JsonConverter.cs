using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Net.Converters
{   
    internal class Base64JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return System.Text.Encoding.UTF8.GetString((Convert.FromBase64String((string)reader.Value)));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var serialized = JsonConvert.SerializeObject(value);
            writer.WriteValue(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(serialized)));
        }
    }
}

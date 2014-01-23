using System;
using System.Collections.Generic;
using System.Linq;
using FollwItPortable.Extensions;
using FollwItPortable.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FollwItPortable.Converters
{
    public class ListTypeConverter : JsonConverter
    {
        private static Dictionary<ListType, string> _listTypeDictionary; 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumType = (ListType) value;
            writer.WriteValue(enumType.GetDescription());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            if (_listTypeDictionary == null)
            {
                var types = Enum.GetValues(typeof (ListType)).Cast<ListType>();
                _listTypeDictionary = types.Select(x => new KeyValuePair<ListType, string>(x, x.GetDescription().ToLower())).ToDictionary(x => x.Key, x => x.Value);
            }

            var result = _listTypeDictionary.FirstOrDefault(x => x.Value == ((string)reader.Value).ToLower());

            return result.Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (string);
        }
    }

    public class DateConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = (DateTime) value;
            writer.WriteValue(date.ToString(FollwItClient.DateFormat));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            var dateString = (string) reader.Value;

            DateTime date;
            if (DateTime.TryParse(dateString, out date))
            {
                return date;
            }

            return null;
        }
    }
}

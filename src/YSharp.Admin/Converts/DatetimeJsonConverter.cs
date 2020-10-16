using NPOI.SS.Formula.Functions;
using YSharp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YSharp.Admin.Converts
{
    public class DatetimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                DateTime? date = null;
                var str = reader.GetString();
                if (str.Length == 10)//format: dd/MM/yyyy
                {
                    date = str.Contains("/") ? str.ToDate("dd/MM/yyyy") : str.ToDate("yyyy-MM-dd");
                }
                else if (str.Length == 16)//format: dd/MM/yyyy HH:mm
                {
                    date = str.Contains("/") ? str.ToDateTime("dd/MM/yyyy HH:mm") : str.ToDate("yyyy-MM-dd HH:mm");
                }
                else if (str.Length == 19)//format: dd/MM/yyyy HH:mm:ss
                {
                    date = str.Contains("/") ? str.ToDateTime("dd/MM/yyyy HH:mm:ss") : str.ToDate("yyyy-MM-dd HH:mm:ss");
                }
                if (date.HasValue == false)
                {
                    throw new Exception("Can't convert the datetime");
                }
                return date.Value;
            }
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd/MM/yyyy HH:mm"));
        }
    }
}

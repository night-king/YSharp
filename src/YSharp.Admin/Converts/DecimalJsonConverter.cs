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
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var ret = 0m;
                var str = reader.GetString();
                if (decimal.TryParse(str, out ret))
                {
                    return ret;
                }
                return 0m;
            }
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("f4"));
        }
    }
}

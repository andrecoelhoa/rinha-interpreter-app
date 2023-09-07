using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TermConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var term = Convert(jsonObject);
        serializer.Populate(jsonObject.CreateReader(), term);
        return term;
    }

    private static Term Convert(JObject jsonObject)
    {
        var kind = jsonObject["kind"]?.Value<string>() ?? "Var";

        return kind switch
        {
            "Bool" => new Bool { Kind = kind, Value = jsonObject["value"].Value<bool>(), Location = jsonObject["location"].ToObject<Loc>() },
            "Int" => new Int { Kind = kind, Value = jsonObject["value"].Value<int>(), Location = jsonObject["location"].ToObject<Loc>() },
            "Str" => new Str { Kind = kind, Value = jsonObject["value"].Value<string>(), Location = jsonObject["location"].ToObject<Loc>() },
            "Var" => new Var { Kind = kind, Text = jsonObject["text"].Value<string>(), Location = jsonObject["location"].ToObject<Loc>() },
            "Binary" => new Binary
            {
                Kind = kind,
                Lhs = Convert(jsonObject["lhs"].ToObject<JObject>()),
                Op = Enum.TryParse<BinaryOp>(jsonObject["op"].Value<string>(), out var binaryOp) ? binaryOp : default,
                Rhs = Convert(jsonObject["rhs"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "Call" => new Call
            {
                Kind = kind,
                Callee = Convert(jsonObject["callee"].ToObject<JObject>()),
                Arguments = jsonObject["arguments"].Select(x => Convert(x.ToObject<JObject>())).ToList(),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "Function" => new Function
            {
                Kind = kind,
                Parameters = jsonObject["parameters"].Select(x => Convert(x.ToObject<JObject>())).ToList(),
                Value = Convert(jsonObject["value"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "Let" => new Let
            {
                Kind = kind,
                Name = Convert(jsonObject["name"].ToObject<JObject>()),
                Value = Convert(jsonObject["value"].ToObject<JObject>()),
                Next = Convert(jsonObject["next"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "If" => new If
            {
                Kind = kind,
                Condition = Convert(jsonObject["condition"].ToObject<JObject>()),
                Then = Convert(jsonObject["then"].ToObject<JObject>()),
                Otherwise = Convert(jsonObject["otherwise"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "Print" => new Print
            {
                Kind = kind,
                Value = Convert(jsonObject["value"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "First" => new First
            {
                Kind = kind,
                Value = Convert(jsonObject["value"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "Second" => new Second
            {
                Kind = kind,
                Value = Convert(jsonObject["value"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            "Tuple" => new Tuple
            {
                Kind = kind,
                First = Convert(jsonObject["first"].ToObject<JObject>()),
                Second = Convert(jsonObject["second"].ToObject<JObject>()),
                Location = jsonObject["location"].ToObject<Loc>()
            },
            _ => new Var { Kind = kind, Text = jsonObject["text"].Value<string>(), Location = jsonObject["location"].ToObject<Loc>() }
        };
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

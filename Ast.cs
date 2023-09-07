

public record File
{
    public string Name { get; set; }

    [JsonConverter(typeof(TermConverter))]
    public Term Expression { get; set; }

    public Loc Location { get; set; }
}

public record Loc
{
    public int Start { get; set; }
    public int End { get; set; }
    public string Filename { get; set; }
}

public record If : Term
{
    [JsonConverter(typeof(TermConverter))]
    public Term Condition { get; set; }

    [JsonConverter(typeof(TermConverter))]
    public Term Then { get; set; }

    [JsonConverter(typeof(TermConverter))]
    public Term Otherwise { get; set; }
}

public record Let : Term
{
    public Term Name { get; set; }

    [JsonConverter(typeof(TermConverter))]
    public Term Value { get; set; }

    [JsonConverter(typeof(TermConverter))]
    public Term Next { get; set; }
}

public record Str : Term
{
    public string Value { get; set; }
}

public record Bool : Term
{
    public bool Value { get; set; }
}

public record Int : Term
{
    public int Value { get; set; }
}

public enum BinaryOp
{
    Add,
    Sub,
    Mul,
    Div,
    Rem,
    Eq,
    Neq,
    Lt,
    Gt,
    Lte,
    Gte,
    And,
    Or,
    Not
}

public record Binary : Term
{
    [JsonConverter(typeof(TermConverter))]
    public Term Lhs { get; set; }

    public BinaryOp Op { get; set; }

    [JsonConverter(typeof(TermConverter))]
    public Term Rhs { get; set; }
}

public record Call : Term
{
    [JsonConverter(typeof(TermConverter))]
    public Term Callee { get; set; }

    public List<Term> Arguments { get; set; }
}

public record Function : Term
{
    public List<Term> Parameters { get; set; }
    [JsonConverter(typeof(TermConverter))]

    public Term Value { get; set; }
}

public record Print : Term
{
    [JsonConverter(typeof(TermConverter))]
    public Term Value { get; set; }
}

public record First : Term
{
    [JsonConverter(typeof(TermConverter))]
    public Term Value { get; set; }
}

public record Second : Term
{
    [JsonConverter(typeof(TermConverter))]
    public Term Value { get; set; }
}

public record Tuple : Term
{
    [JsonConverter(typeof(TermConverter))]
    public Term First { get; set; }

    [JsonConverter(typeof(TermConverter))]
    public Term Second { get; set; }
}

public record Var : Term
{
    public string Text { get; set; }
}

public record Term
{
    public virtual string Kind { get; set; } = "Var";
    public Loc Location { get; set; }

}

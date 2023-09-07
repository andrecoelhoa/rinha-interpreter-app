public record Interpreter
{
    private Dictionary<string, object> variableEnvironment = new();
    private readonly Dictionary<string, Function> functionEnvironment = new();

    public object Run(File ast) =>  Interpret(ast.Expression);

    private object Interpret(Term term) =>
        term.Kind switch
        {
            "Bool" => ((Bool)term).Value,
            "Int" => ((Int)term).Value,
            "Str" => ((Str)term).Value,
            "Var" => InterpretVariable((Var)term),
            "Binary" => InterpretBinary((Binary)term),
            "Call" => InterpretCall((Call)term),
            "Function" => InterpretFunction((Function)term),
            "Let" => InterpretLet((Let)term),
            "If" => InterpretIf((If)term),
            "Print" => InterpretPrint((Print)term),
            "First" => InterpretFirst((First)term),
            "Second" => InterpretSecond((Second)term),
            "Tuple" => InterpretTuple((Tuple)term),
            _ => throw new Exception($"Unknown term kind: {term.Kind}")
        };

    private object InterpretVariable(Var variable)
    {
        return variableEnvironment.TryGetValue(variable.Text, out object value)
            ? value
            : throw new Exception($"Variable '{variable.Text}' is not defined.");
    }

    private object InterpretBinary(Binary binary)
    {
        object lhs = Interpret(binary.Lhs);
        object rhs = Interpret(binary.Rhs);

        return binary.Op switch
        {
            BinaryOp.Add => (int)lhs + (int)rhs,
            BinaryOp.Sub => (int)lhs - (int)rhs,
            BinaryOp.Mul => (int)lhs * (int)rhs,
            BinaryOp.Div => (int)lhs / (int)rhs,
            BinaryOp.Rem => (int)lhs % (int)rhs,
            BinaryOp.Eq => Equals(lhs, rhs),
            BinaryOp.Neq => !Equals(lhs, rhs),
            BinaryOp.Lt => (int)lhs < (int)rhs,
            BinaryOp.Gt => (int)lhs > (int)rhs,
            BinaryOp.Lte => (int)lhs <= (int)rhs,
            BinaryOp.Gte => (int)lhs >= (int)rhs,
            BinaryOp.And => (bool)lhs && (bool)rhs,
            BinaryOp.Or => (bool)lhs || (bool)rhs,
            BinaryOp.Not => !(bool)rhs,
            _ => throw new Exception($"Unsupported binary operator: {binary.Op}")
        };
    }

    private object InterpretCall(Call call)
    {
        if (!(call.Callee is Var varCallee))
            throw new Exception("Function call must have a variable callee.");

        if (!functionEnvironment.TryGetValue(varCallee.Text, out Function function))
            throw new Exception($"Function '{varCallee.Text}' is not defined.");

        if (function.Parameters.Count != call.Arguments.Count)
            throw new Exception($"Function '{varCallee.Text}' expects {function.Parameters.Count} arguments but got {call.Arguments.Count}.");
    
        var newVariableEnvironment = new Dictionary<string, object>(variableEnvironment);
        for (int i = 0; i < function.Parameters.Count; i++)
        {
            var parameter = function.Parameters[i];
            var argument = call.Arguments[i];
            if (parameter is Var)
                newVariableEnvironment[((Var)parameter).Text] = Interpret(argument);
        }

        var oldVariableEnvironment = variableEnvironment;
        variableEnvironment = newVariableEnvironment;
        var result = Interpret(function.Value);
        variableEnvironment = oldVariableEnvironment;

        return result;
    }

    private object InterpretFunction(Function function) => function;

    private object InterpretLet(Let let)
    {
        var value = Interpret(let.Value);
        variableEnvironment[((Var)let.Name).Text] = value;
        if (value is Function function)
            functionEnvironment[((Var)let.Name).Text] = function;
        return Interpret(let.Next);
    }

    private object InterpretIf(If ifTerm) =>
        (bool)Interpret(ifTerm.Condition) ? Interpret(ifTerm.Then) : Interpret(ifTerm.Otherwise);

    private object InterpretPrint(Print print)
    {
        var value = Interpret(print.Value);
        Console.WriteLine(value);
        return value;
    }

    private object InterpretFirst(First first) => Interpret(((Tuple)Interpret(first.Value)).First);

    private object InterpretSecond(Second second) => Interpret(((Tuple)Interpret(second.Value)).Second);

    private object InterpretTuple(Tuple tuple)
    {
        var (first, second) = (Interpret(tuple.First), Interpret(tuple.Second));
        return new TupleValue(first, second);
    }
}

public record TupleValue
{
    public object First { get; }
    public object Second { get; }

    public TupleValue(object first, object second)
    {
        First = first;
        Second = second;
    }
}

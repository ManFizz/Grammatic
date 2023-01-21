namespace RemoveUselessSymbols;

public class Symbol
{
    public string Value = "";

    public Symbol()
    {}
    
    public Symbol(string value)
    {
        Value = value;
    }
    
    public Symbol(char value)
    {
        Value = value.ToString();
    }

    public static bool operator == (Symbol left, Symbol right)
    {
        return left.Value == right.Value;
    }

    public static bool operator != (Symbol left, Symbol right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value;
    }
}
namespace RemoveUselessSymbols;

public class Rule
{
    public readonly Symbol Head;
    public readonly List<Symbol> Tail = new();

    public Rule(Symbol head)
    {
        Head = head;
    }

    public Rule(Symbol head, List<Symbol> tail)
    {
        Head = head;
        Tail = tail;
    }
    
    public override string ToString()
    {
        return Tail.Aggregate(Head + " -> ", (current, t) => current + t);
    }
};
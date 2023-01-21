namespace RemoveUselessSymbols;

internal class Grammatic {
    public static readonly Symbol WhiteSpaceCharacter = new("&");
    
    public readonly List <Symbol> NoTerminals = new();
    public readonly List <Symbol> Terminals = new ();
    public readonly List <Rule> Rules = new();
    public Symbol Axioma;

    public Grammatic()
    {
        Terminals.Add(WhiteSpaceCharacter);
    }

    private readonly List <Symbol> _goodSimvols = new ();
    
    public Symbol AddNoTerminal(Symbol symbol)
    {
        if (NoTerminals.Any(t => t.Value == symbol.Value))
            return NoTerminals.First(t => t.Value == symbol.Value);
        
        NoTerminals.Add(symbol);
        return symbol;
    }
    
    public Symbol AddTerminal(Symbol symbol)
    {
        if (Terminals.Any(t => t.Value == symbol.Value))
            return Terminals.First(t => t.Value == symbol.Value);
        
        Terminals.Add(symbol);
        return symbol;
    }

    public Symbol GetSymbol(string str)
    {
        if (Terminals.Any(t => t.Value == str))
            return Terminals.First(t => t.Value == str);
        
        if (NoTerminals.Any(t => t.Value == str))
            return NoTerminals.First(t => t.Value == str);

        throw new Exception($"Символ '{str}' не обьявлен в грамматике");
    }

    #region EmptyLanguage
    private void UpdateGoodSymbols()
    {
        foreach(var rule in Rules)
        {
            if (_goodSimvols.Contains(rule.Head))
                continue;
                
            if (!IsEndRule(rule))
                continue;
                
            _goodSimvols.Add(rule.Head);
            UpdateGoodSymbols();
            return;
        }
    }

    private bool IsEndRule(Rule rule)
    {
        return rule.Tail.Any(IsEndSymbol);
    }
        
    private bool IsEndSymbol(Symbol symbol)
    {
        return Terminals.Contains(symbol) || _goodSimvols.Contains(symbol);
    }
        
    public bool IsLangEmpty() {
        UpdateGoodSymbols();
        return _goodSimvols.All(t => t != Axioma);
    }
        
    #endregion

    public void RemoveUseless()
    {
        if (IsLangEmpty())
            return;

        UpdateGoodSymbols();
        RemoveUselessSymbols();

        RemoveUnattainableSymbols();
    }

    private void RemoveNoTerminal(Symbol t)
    {
        var removeRules = new List<Rule>();
        foreach (var rule in Rules)
        {
            rule.Tail.RemoveAll(part => part == t);
            if(rule.Tail.Count == 0)
                removeRules.Add(rule);
        }

        foreach (var rule in removeRules)
            Rules.Remove(rule);

        NoTerminals.Remove(t);
    }

    private void RemoveUselessSymbols()
    {
        foreach (var t in NoTerminals.Where(t => !_goodSimvols.Contains(t)).ToList())
            RemoveNoTerminal(t);
    }

    private void RemoveUnattainableSymbols() {
        var attainable = new List <Symbol> { Axioma };

        CheckRulesWithSymbol(Axioma, ref attainable);

        var removeNoTerms = NoTerminals.Where(noTerm => !attainable.Contains(noTerm)).ToList();

        foreach (var noTerm in removeNoTerms)
            RemoveNoTerminal(noTerm);
    }

    private void CheckRulesWithSymbol(Symbol symbol, ref List<Symbol> attainable)
    {
        var arr = new List<Symbol>();
        var rules = Rules.FindAll(r => r.Head == symbol);
        foreach (var rule in rules)
            foreach (var c in rule.Tail)
                if (NoTerminals.Contains(c) && !attainable.Contains(c) && !arr.Contains(c))
                    arr.Add(c);
            
        attainable.AddRange(arr);
        foreach (var sym in arr)
            CheckRulesWithSymbol(sym, ref attainable);
    }

    public void RemoveLeftRecursion()
    {
        var noTerm = NoTerminals.ToList();
        var i = 0;
        while(true)
        {
            var symbol = noTerm[i];
            var badRules = Rules.Where(r => symbol == r.Head && r.Head == r.Tail[0]).ToList();
            if (badRules.Count > 0)
            {
                var tails = badRules.Select(r => r.Tail).ToList();

                //step 2
                Rules.RemoveAll(r => badRules.Contains(r));

                var newTerminal = AddNoTerminal(new Symbol(symbol + "\'"));

                foreach (var r in Rules.Where(r => r.Head == symbol).ToList())
                {
                    var newRule = new Rule(symbol);
                    newRule.Tail.AddRange(r.Tail);
                    newRule.Tail.Add(newTerminal);
                    Rules.Add(newRule);
                }

                foreach (var tail in tails)
                {
                    tail.Remove(tail[0]);
                    
                    var newRule = new Rule(newTerminal);
                    newRule.Tail.AddRange(tail);
                    Rules.Add(newRule);

                    newRule = new Rule(newTerminal);
                    newRule.Tail.AddRange(tail);
                    newRule.Tail.Add(newTerminal);
                    Rules.Add(newRule);
                }
            }

            //step 3
            if (i == noTerm.Count - 1)
                return;
            
            i++;
            //step 4
            symbol = noTerm[i];
            for (var j = 0; j < i; j++)
            {
                var secTerminal = noTerm[j];
                var tails = Rules.Where(r => r.Head == secTerminal).Select(r => r.Tail).ToList();
                foreach (var r in Rules.Where(r => r.Head == symbol && r.Tail[0] == secTerminal).ToList())
                {
                    r.Tail.Remove(r.Tail[0]);
                    if(r.Tail.Count == 0)
                        r.Tail.Add(WhiteSpaceCharacter);
                    foreach (var tail in tails)
                    {
                        var newRule = new Rule(symbol);
                        newRule.Tail.AddRange(tail);
                        newRule.Tail.AddRange(r.Tail);
                        Rules.Add(newRule);
                    }

                    Rules.Remove(r);
                }
            }
        }
    }

    public override string ToString()
    {
        var str = "Axioma: " + Axioma + "\nTerminals: ";
        var emptySymbol = new Symbol("&");
        foreach (var t in Terminals.Where(t => t != emptySymbol))
            str += t + ", ";
        str = str.Remove(str.Length - 2, 2) + "\nNoTerminals: ";
        
        foreach (var t in NoTerminals)
            str += t + ", ";
        str = str.Remove(str.Length - 2, 2) + "\n~ ~ ~ Rules ~ ~ ~\n";

        foreach (var terminal in NoTerminals)
        {
            str += terminal + " -> ";
            foreach (var rule in Rules.Where(r => r.Head == terminal))
            {
                foreach (var symbol in rule.Tail)
                    str += symbol.ToString();
                str += "|";
            }

            str = str.Remove(str.Length - 1, 1) + "\n";
        }
            
        return str;
    }
}
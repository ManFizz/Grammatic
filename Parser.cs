namespace RemoveUselessSymbols;

internal static class Parser
{
    public static void ParseRule(Grammatic grammatic, string sRule)
    {
        if(grammatic == null)
            throw new Exception("Не установлена грамматика");
            
        var arr = sRule.Split("->");
        if (arr.Length != 2)
            throw new Exception("Ожидается комбинация символов '->' в единственном экземпляре");
        
        var head = grammatic.AddNoTerminal(new Symbol(arr[0]));

        arr = arr[1].Split('|');
        foreach (var part in arr)
        {
            var rule = new Rule(head);
            var symbols = part.Split(' ');
            foreach (var symbol in symbols)
                rule.Tail.Add(grammatic.GetSymbol(symbol));
            if(rule.Tail.Count == 0)
                rule.Tail.Add(Grammatic.WhiteSpaceCharacter);
            grammatic.Rules.Add(rule);
        }
    }

    public static List<Symbol> ParseSymbols(string str)
    {
        var symbols = str.Trim().Split(' ');
        return symbols.Select(s => new Symbol(s)).ToList();
    }
}
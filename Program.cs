namespace RemoveUselessSymbols {
    internal static class Program {

        public static void Main(string[] args)
        {
            
            #region definitions
            var g = new Grammatic();
            g.NoTerminals.AddRange(Parser.ParseSymbols("A B C D M E S"));
            g.Terminals.AddRange(Parser.ParseSymbols("a b c d m"));
            
            g.Axioma = new Symbol('A');
            /*Parser.ParseRule(g, "A->B C|a");
            Parser.ParseRule(g, "B->C A|A b");
            Parser.ParseRule(g, "C->A B|C C|a");*/
            
            Parser.ParseRule(g, "A->B|C|M|S");
            Parser.ParseRule(g, "C->D");
            Parser.ParseRule(g, "D->A d|d c c a|A c a");
            Parser.ParseRule(g, "M->M A");
            Parser.ParseRule(g, "M->d S a");
            Parser.ParseRule(g, "S->&");
            
            
            /*g.Axioma = 'S';
            g.Rules.Add(new Rule { "S", "a", "A" });
            g.Rules.Add(new Rule { "A", "AB" });
            g.Rules.Add(new Rule { "B", "b"});*/
            
            /*g.Axioma = 'S';
            g.Rules.Add(new Rule { "S", "A" });
            g.Rules.Add(new Rule { "A", "BS", "a"});
            g.Rules.Add(new Rule { "B", "SBA"});*/
            
            #endregion
            
            Console.WriteLine(g + "\n");
            
            Console.WriteLine("Language is " + (g.IsLangEmpty() ? "empty" : "ok") + '\n');
            if(g.IsLangEmpty())
                return;
            
            g.RemoveUseless();
            Console.WriteLine(g + "\n");

            g.RemoveLeftRecursion();
            Console.WriteLine(g + "\n");
        }
    }
}
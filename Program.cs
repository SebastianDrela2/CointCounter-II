namespace CombinationsSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {            
            Console.WriteLine($"Input total");
            var total = int.Parse(Console.ReadLine()!);

            Console.WriteLine($"With comma separated, input values.");

            var values = GetValues();

            var counter = new CoinCounter(total, values);
            var results = counter.CombinationManager.GetNodes();

            Console.WriteLine($"All combinations for: {total}.");

            foreach(var result in results)
            {
                Console.WriteLine(result.Path);
            }

            Console.WriteLine();
            Console.WriteLine($"There is {results.Count} combinations.");
        }
        
        static List<int> GetValues()
        {
            var input = Console.ReadLine();
            var values = input!.Split(',').Select(int.Parse).ToList();

            return values;
        }
    }

    public class CoinCounter
    {
        private readonly int _goal;
        private readonly List<int> _orderedPossibilites;        
        private readonly int _count = 0;

        public readonly CombinationManager CombinationManager;
        public CoinCounter(int goal,  List<int> possibilites)
        {
            _goal = goal;
            _orderedPossibilites = possibilites.Distinct().OrderByDescending(x => x).ToList();
            _count = GetCount();
            CombinationManager = GetManager();
        }

        private int GetCount()
        {
            var smallestPossibility = _orderedPossibilites.Last();

            return _goal / smallestPossibility;
        }
        
        private CombinationManager GetManager() => new CombinationManager(_orderedPossibilites, _count, _goal);       
    }

    public class CombinationManager
    {       
        private readonly List<Combination> _combinations;

        public CombinationManager(List<int> possibilites, int amount, int goal)
        {
            var amountStruct = new Amount { CurrentAmount = amount, GlobalAmount = amount };
            _combinations = possibilites.Select(x => CreateCombination(x, possibilites, amountStruct, goal, string.Empty)).ToList();
        }

        public List<Combination> GetNodes()
        {
            var resultList = new List<Combination>();
            var combinations = _combinations;

            foreach (var node in combinations)
            {
                Visit(node, resultList);
            }

            return resultList;
        }

        private static Combination CreateCombination(int value, List<int> possibilities, Amount amount, int remainder, string path, Combination? parent = null)
        {
            var decrementedRemainder = remainder - value;
            path += value;

            if (amount.CurrentAmount > 0 && decrementedRemainder > 0)
            {
                amount.CurrentAmount--;

                var limitedPossitilbiites = possibilities.Where(x => x <= decrementedRemainder && x <= value).ToList();

                var children = limitedPossitilbiites
                    .Select(x =>
                    {
                        return CreateCombination(x, possibilities, amount, decrementedRemainder, path, parent);

                    }).ToList();

                return new Combination(value, amount, decrementedRemainder, path, children, parent);

            }

            return new Combination(value, amount, decrementedRemainder, path);
        }
        
        private static void Visit(Combination node, List<Combination> resultList)
        {
            if (node.Remainder == 0)
            {
                resultList.Add(node);
            }

            if (node.Children is not null)
            {
                foreach(var child in node.Children)
                {
                    Visit(child, resultList);
                }
            }
        }
    }

    public class Combination
    {
        public int Depth;    
        public int Remainder;
        public int Value;
        public List<Combination>? Children;
        public Combination? Parent;
        public string Path;
        
        public Combination(int 
            value, Amount 
            amount, int 
            remainder, 
            string path, 
            List<Combination>? children = null, 
            Combination? parent = null)
        {
            Value = value;
            Remainder = remainder;
            Path = path;
            Depth = amount.GlobalAmount - amount.CurrentAmount;
            Children = children;
            Parent = parent;
        }
    }

    public struct Amount
    {
        public int GlobalAmount;
        public int CurrentAmount;
    }
}

using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day14 : AdventBase
    {
        public Day14(int day) : base(day)
        {
            Title = "Space Stoichiometry";
            //            TestInput = @"10 ORE => 10 A
            //1 ORE => 1 B
            //7 A, 1 B => 1 C
            //7 A, 1 C => 1 D
            //7 A, 1 D => 1 E
            //7 A, 1 E => 1 FUEL";
            //            Title = "Space Stoichiometry";
            TestInput = @"157 ORE => 5 NZVS
165 ORE => 6 DCFZ
44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
179 ORE => 7 PSHF
177 ORE => 5 HKGWZ
7 DCFZ, 7 PSHF => 2 XJWVT
165 ORE => 2 GPVTF
3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day14;
            SolutionPart1 = "";
            SolutionPart2 = "";
        }

        public class Reaction

        {
            public Reaction(string[] v)
            {
                var inputstrings = v[0].Split(",");
                foreach (var inputstring in inputstrings)
                {
                    Material material = CreateMaterialFromString(inputstring);
                    AddInput(material);
                }
                var outputstring = v[1];
                Material outputMaterial = CreateMaterialFromString(outputstring);
                AddOutput(outputMaterial);
            }

            private static Material CreateMaterialFromString(string inputstring)
            {
                var materialstring = inputstring.Trim().Split(" ");
                return new Material(Convert.ToInt32(materialstring[0]), materialstring[1]);
            }

            public void AddInput(Material material)
            {
                InputMaterials.Add(material);
            }
            public void AddOutput(Material material)
            {
                OutputMaterials.Add(material);
            }


            public Material FuelMaterial()
            {

                return OutputMaterials.Where(x => x.Type == "FUEL").FirstOrDefault();
            }


            public List<Reaction> InputReactions { get; set; } = new List<Reaction>();
            public List<Material> InputMaterials { get; set; } = new List<Material>();
            public List<Material> OutputMaterials { get; set; } = new List<Material>();

            private int UseCount { get; set; }
            public void Process(List<Material> materialsToUse)
            {
                UseCount++;
                if (InputMaterials.Any(x => x.IsOre()))
                {

                    var material = new Material(OutputMaterials.FirstOrDefault().Quantity, OutputMaterials.FirstOrDefault().Type);
                    var existingmaterial = materialsToUse.Where(x => x.Type == OutputMaterials.FirstOrDefault().Type).FirstOrDefault();

                    if (existingmaterial != null)
                    {

                        existingmaterial.Quantity += OutputMaterials.FirstOrDefault().Quantity;
                    }
                    else
                    {
                        materialsToUse.Add(material);
                    }
                }
                else
                {

                    var sufficientMaterials = InputMaterials.TrueForAll(x => x.IsSufficientAvailable(materialsToUse));

                    if (sufficientMaterials)
                    {
                        var factor = UseMaterialsAndDetermineOutputFactor(materialsToUse);
                        var materials = new List<Material>();
                        var material = new Material(OutputMaterials.FirstOrDefault().Quantity * factor, OutputMaterials.FirstOrDefault().Type);

                        var existingmaterial = materialsToUse.Where(x => x.Type == OutputMaterials.FirstOrDefault().Type).FirstOrDefault();

                        if (existingmaterial != null)
                        {

                            existingmaterial.Quantity += material.Quantity;
                        }
                        else
                        {
                            materialsToUse.Add(material);
                        }
                    }
                }
            }

            private int UseMaterialsAndDetermineOutputFactor(List<Material> materialsToUse)
            {
                var factors = new List<int>();
                foreach (var input in InputMaterials)
                {
                    var sameMaterialTtypes = materialsToUse.Where(x => x.Type == input.Type).ToList();
                    var usematerials = sameMaterialTtypes;//.Select(x => x.GetTotalUseFactor(materialsToUse));
                    var currentFactor = input.GetTotalUseFactor(sameMaterialTtypes);
                    factors.Add(currentFactor);
                }

                var factor = factors.Min();
                foreach (var input in InputMaterials)
                {
                    var sameMaterialTtypes = materialsToUse.Where(x => x.Type == input.Type).ToList();
                    sameMaterialTtypes.ForEach(x => x.Quantity -= input.Quantity * factor);
                }
                return factor;
            }

            internal void AddReaction(Reaction p)
            {
                InputReactions.Add(p);
            }
        }

        public class Material
        {
            public Material(int _amount, string _type)
            {
                Quantity = _amount;
                Type = _type;
            }

            public bool IsOre()
            {
                return Type == "ORE";
            }

            public bool IsSufficientAvailable(List<Material> materialsToUse)
            {
                return materialsToUse.Any(y => y.Type == Type && y.Quantity >= Quantity);
            }

            public int GetTotalUseFactor(List<Material> materialsToUse)
            {
                return materialsToUse.Where(y => y.Type == Type).Select(x => x.Quantity / Quantity).FirstOrDefault();
            }

            public int Quantity { get; set; }
            public string Type { get; private set; }
        }

        public List<Reaction> Reactions { get; set; }
        public override async Task Part1()
        {
            var puzzleinput1 = PuzzleInput.Split("\r\n");
            Reactions = puzzleinput1.Select(x => new Reaction(x.Split("=>"))).ToList();


            var fuelReaction = Reactions.Where(x => x.FuelMaterial() != null).FirstOrDefault();

            var foundOre = false;


            var tree = RecursiveFunction(fuelReaction);
            var reactions = new List<Reaction>();
            var flatlist = Traverse(tree);

            var inputDic = new Dictionary<string, int>();
            var flatlistWithoutORE = flatlist.Where(x => x.InputMaterials.FirstOrDefault().Type != "ORE").ToList();
            var flatlistORE = flatlist.Where(x => x.InputMaterials.FirstOrDefault().Type == "ORE").Distinct().ToList(); ;


            foreach (var item in flatlistWithoutORE)
            {
                var output = item.OutputMaterials.FirstOrDefault();
                var inputMaterial = item.InputMaterials;
                int amount;
                var amountNeededAlready = inputDic.TryGetValue(output.Type, out amount);
                if (amount == 0) amount = 1;
                amount = output.Quantity * amount;
                foreach (var input in inputMaterial)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        if (!inputDic.TryAdd(input.Type, input.Quantity))
                        {
                            inputDic[input.Type] += input.Quantity;
                        }
                    }

                }

            }

            var totalOREAmount = 0d;
            foreach (var oreReaction in flatlistORE)
            {
                var output = oreReaction.OutputMaterials.FirstOrDefault();
                var type = output.Type;
                var howmanyIsNeeded = inputDic[type];
                var reactionMultiply = Math.Ceiling((double)howmanyIsNeeded / (double)output.Quantity);

                var inputOre = oreReaction.InputMaterials.FirstOrDefault();
                totalOREAmount += inputOre.Quantity * reactionMultiply;
            }
            var outputs = flatlistWithoutORE.Select(x => x.OutputMaterials);

            ResultPart1 = totalOREAmount.ToString();
        }

        public IEnumerable<Reaction> Traverse(Reaction root)
        {
            var stack = new Stack<Reaction>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                foreach (var child in current.InputReactions)
                    stack.Push(child);
            }
        }

        public List<Reaction> InputReactions = new List<Reaction>();
        private Reaction RecursiveFunction(Reaction reaction)
        {
            var oreAmount = 0;
            var listMaterialsToBeProcessed = reaction.InputMaterials;
            var outputReactions = Reactions.Where(x => listMaterialsToBeProcessed.Select(y => y.Type).Contains(x.OutputMaterials.FirstOrDefault().Type)).ToList();
            foreach (var outputreaction in outputReactions)
            {
                var amount = outputreaction.OutputMaterials.FirstOrDefault().Quantity;

                //if (amount > )
                // for (int i = 0; i < amount; i++)
                //{
                reaction.AddReaction(RecursiveFunction(outputreaction));

                //}
                InputReactions.AddRange(reaction.InputReactions);

            }
            return reaction;
        }

        public override async Task Part2() => ResultPart2 = "";

    }
}


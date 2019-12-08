using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day6 : AdventBase
    {
        public Day6(int day) : base(day)
        {
            Title = "Universal Orbit Map";
//            TestInput = @"COM)B
//B)C
//C)D
//D)E
//E)F
//B)G
//G)H
//D)I
//E)J
//J)K
//K)L
//K)YOU
//I)SAN";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day6;
            Day6OrbitList = PuzzleInput.Split("\r\n").Select(x=> new Orbit() { PlanetFrom = x.Substring(0,x.IndexOf(')')), PlanetTo = x.Substring(x.IndexOf(')') + 1) }).ToList();
            SolutionPart1 = "227612";
            SolutionPart2 = "";
        }

        public struct Orbit { 
        public string PlanetFrom { get; set; }
        public string PlanetTo { get; set; }
        }

        public class Planet
        {
            public string PlanetName { get; set; }
            public int DeepCount { get; set; }
            public List<Planet> ChildPlanets { get; set; }
            public List<Planet> ParentPlanets { get; set; }
            public List<Planet> TreeHistory { get; set; } = new List<Planet>();
            public List<Orbit> OrbitsTo { get; set; }

            public Planet(int count)
            {
                DeepCount = count;
                ChildPlanets = new List<Planet>();
            }
        }

        public List<Orbit> Day6OrbitList;
        public int TotalDeepCountPart1;
        public int TotalDeepCountPart2;
        public override async Task Part1()
        {
            var orbitList = Day6OrbitList.Where(x=> !Day6OrbitList.Select(y=> y.PlanetTo).ToList().Contains(x.PlanetFrom)).ToList();
            var firstPlanet = orbitList.First();
            var startplanet = new Planet(0) { PlanetName = firstPlanet.PlanetFrom };
            startplanet.ChildPlanets = Day6OrbitList.Where(x => x.PlanetFrom == firstPlanet.PlanetFrom).Select(x => new Planet(startplanet.DeepCount + 1) { PlanetName = x.PlanetTo}).ToList();
            var planetTree = CreateNewPlanetTree(Day6OrbitList, startplanet);
            ResultPart1 = TotalDeepCountPart1.ToString();
        }

        private Planet CreateNewPlanetTree(List<Orbit> orbitList, Planet planet)
        {
            for (int i = 0; i < planet.ChildPlanets.Count(); i++)
            {
                var childPlanet = planet.ChildPlanets[i];
                TotalDeepCountPart1 += childPlanet.DeepCount;
                childPlanet.ChildPlanets = Day6OrbitList.Where(x => x.PlanetFrom == childPlanet.PlanetName).Select(x => new Planet(childPlanet.DeepCount + 1) { PlanetName = x.PlanetTo }).ToList();
                CreateNewPlanetTree(orbitList, childPlanet);
            }
            return planet;
        }

        public override async Task Part2()
        {
            var youtree = FindParentTreePlanet("YOU");
            var flatYOUtree = new List<Planet>();
            BuildFlatTree(youtree.ParentPlanets, flatYOUtree);
            
            var santree = FindParentTreePlanet("SAN");
            var flatSANtree = new List<Planet>();
            BuildFlatTree(santree.ParentPlanets, flatSANtree);
            
            var transferAmountYOU = flatYOUtree.Where(x=> flatSANtree.Select(x=> x.PlanetName).Contains(x.PlanetName)).Min(z=> z.DeepCount);
            var transferAmountSAN = flatSANtree.Where(x => flatYOUtree.Select(x => x.PlanetName).Contains(x.PlanetName)).Min(z => z.DeepCount);

            ResultPart2 = $"{transferAmountYOU + transferAmountSAN}".ToString();
        }

        private List<Planet> BuildFlatTree(List<Planet> parentPlanets, List<Planet> planets)
        {
            var firstParent = parentPlanets.FirstOrDefault();
            if (firstParent != null)
            {
                planets.Add(firstParent);
                BuildFlatTree(firstParent.ParentPlanets, planets);
            }
            return planets;
        }

        private Planet FindParentTreePlanet(string planetName)
        {
            var orbitList = Day6OrbitList.Where(x => x.PlanetTo == planetName).ToList();
            var firstPlanet = orbitList.First();
            var startplanet = new Planet(0) { PlanetName = firstPlanet.PlanetTo };
            startplanet.ParentPlanets = Day6OrbitList.Where(x => x.PlanetTo == firstPlanet.PlanetFrom).Select(x => new Planet(startplanet.DeepCount + 1) { PlanetName = x.PlanetFrom }).ToList();
            var planetTree = CreateNewPlanetTreePart2(Day6OrbitList, startplanet);
            return planetTree;
        }

        private Planet CreateNewPlanetTreePart2(List<Orbit> orbitList, Planet planet)
        {
            for (int i = 0; i < planet.ParentPlanets.Count(); i++)
            {
                var parentPlanet = planet.ParentPlanets[i];
                TotalDeepCountPart1 += parentPlanet.DeepCount;
                parentPlanet.ParentPlanets = Day6OrbitList.Where(x => x.PlanetTo == parentPlanet.PlanetName).Select(x => new Planet(parentPlanet.DeepCount + 1) { PlanetName = x.PlanetFrom }).ToList();
                CreateNewPlanetTreePart2(orbitList, parentPlanet);
            }
            return planet;
        }
    }
}


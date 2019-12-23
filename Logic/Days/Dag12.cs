using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day12 : AdventBase
    {
        public Day12(int day) : base(day)
        {
            Title = "The N-Body Problem";
            //TestInput = @"<x=-1, y=0, z=2>
            //<x=2, y=-10, z=-7>
            //<x=4, y=-8, z=8>
            //<x=3, y=5, z=-1>";
            //TestInput = @"<x=-8, y=-10, z=0>
            //<x=5, y=5, z=10>
            //<x=2, y=-7, z=3>
            //<x=9, y=-8, z=-3>";



            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day12;
            SolutionPart1 = "13399";
            SolutionPart2 = "312992287193064";
        }

        public class Coordinate
        {
            public long CoordinateIndex { get; set; }
            public Coordinate(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public bool HasAll => X > 0 && Y > 0 && Z > 0;
            public Coordinate(int x, int y, int z, long index)
            {
                X = x;
                Y = y;
                Z = z;
                CoordinateIndex = index;
            }
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
        }

        public class Moon
        {
            public int CoordinateIndex { get; set; }

            public Moon(int x, int y, int z)
            {
                Coordinate = new Coordinate(x, y, z);
                InitialCoordinates.Add(new Coordinate(Coordinate.X, Coordinate.Y, Coordinate.Z));
                Velocity = new Coordinate(0, 0, 0);
                InitialVelocities.Add(new Coordinate(Velocity.X, Velocity.Y, Velocity.Z));
            }

            public long RecurrentFromStep { get; set; }

            private struct BelongingSteps
            {
                public long first { get; set; }
                public long second { get; set; }

            }
            public long IsInPreviousState()
            {
                var belongSteps = new List<BelongingSteps>();
                var groupedCoordinates = InitialCoordinates.GroupBy(x => new { x.X, x.Y, x.Z });
                var coordinateExists = groupedCoordinates.Where(g => g.Count() > 1).ToHashSet();
                var isValid = false;
                var recurrentIndexes = new List<long>();
                var stepcount = (long)0;
                var dicStepCount = new Dictionary<long, int>();
                if (coordinateExists.Any())
                {
                    foreach (var coordinateGroup in coordinateExists)
                    {
                        var alleCoorinatesToCheck = InitialCoordinates.Where(x => coordinateGroup.Key.X == x.X && coordinateGroup.Key.Y == x.Y && coordinateGroup.Key.Z == x.Z).ToHashSet();
                        foreach (var coord in alleCoorinatesToCheck)
                        {
                            var getAllVelocitiesByindex = alleCoorinatesToCheck.Select(x => x.CoordinateIndex).ToHashSet();
                            var velocity = InitialVelocities.Where(x => getAllVelocitiesByindex.Contains(x.CoordinateIndex));
                            var belongstoVelocitites = velocity.GroupBy(x => new { x.X, x.Y, x.Z });
                            if (!isValid && belongstoVelocitites.Any(g => g.Count() > 1))
                            {
                                var allvelos = belongstoVelocitites.Where(g => g.Count() > 1).SelectMany(y => y.Select(z => z.CoordinateIndex)).ToList();
                                stepcount = allvelos.Skip(1).Take(1).FirstOrDefault() - allvelos.Take(1).FirstOrDefault();
                                belongSteps.Add(new BelongingSteps() { first = allvelos.Take(1).FirstOrDefault(), second = allvelos.Skip(1).Take(1).FirstOrDefault() });
                                if (!dicStepCount.TryAdd(stepcount, 1))
                                {
                                    dicStepCount[stepcount] += 1;
                                };


                                if (dicStepCount.Count == 3) break;
                                //break;
                                recurrentIndexes.AddRange(allvelos);
                            }
                        }
                        if (dicStepCount.Count == 3) break;
                        //if (stepcount != 0) break;
                    }
                }


                foreach (var belongstep in belongSteps)
                {
                    Debug.WriteLine($"{belongstep.first}\t{belongstep.second}");
                }


                foreach (var step in dicStepCount)
                {
                    Debug.WriteLine($"{step}");
                }

                return stepcount; // recurrentIndexes.Distinct().ToList();
            }


            public long IsInZeroState()
            {
                var belongSteps = new List<BelongingSteps>();
                var coords = new List<Coordinate>();
                var zeroStateCoordinate = InitialCoordinates[0];
                var zeroStateVelocity = InitialVelocities[0];
                var zeroStateCoordinates = InitialCoordinates.Where(x => x.X == zeroStateCoordinate.X && x.Y == zeroStateCoordinate.Y && x.Z == zeroStateCoordinate.Z);
                if (zeroStateCoordinates != null && zeroStateCoordinates.Any())
                {
                    var getAllVelocitiesByindex = zeroStateCoordinates.Select(x => x.CoordinateIndex).ToHashSet();
                    var velocity = InitialVelocities.Where(x => getAllVelocitiesByindex.Contains(x.CoordinateIndex));
                    var belongstoVelocitites = velocity.GroupBy(x => new { x.X, x.Y, x.Z });
                    if (belongstoVelocitites.Any(g => g.Count() > 1))
                    {
                        var allvelos = belongstoVelocitites.Where(g => g.Key.X == zeroStateVelocity.X && g.Key.Y == zeroStateVelocity.Y && zeroStateVelocity.Z == g.Key.Z).SelectMany(y => y.Select(z => z)).ToList();
                        coords.AddRange(allvelos);
                    }
                }

                return 2; // recurrentIndexes.Distinct().ToList();
            }


            public long IsInExample1Steps()
            {

                var checkStepsExample1 = new List<long>() { 924, 1848, 2772 };
                var belongSteps = new List<BelongingSteps>();
                var coords = new List<Coordinate>();
                var zeroStateCoordinate = InitialCoordinates[0];
                var zeroStateVelocity = InitialVelocities[0];
                var zeroStateCoordinates = InitialCoordinates.Where(x => checkStepsExample1.Contains(x.CoordinateIndex));
                if (zeroStateCoordinates != null && zeroStateCoordinates.Any())
                {
                    //var getAllVelocitiesByindex = zeroStateCoordinates.Select(x => x.CoordinateIndex).ToHashSet();
                    var velocity = InitialVelocities.Where(x => checkStepsExample1.Contains(x.CoordinateIndex)).ToList();
                    var coordSteps = InitialCoordinates.Where(x => checkStepsExample1.Contains(x.CoordinateIndex)).ToList();
                    var i = 0;
                    foreach (var step in checkStepsExample1)
                    {
                        Debug.WriteLine($"{step}\t{coordSteps[i].X}-{coordSteps[i].Y}-{coordSteps[i].Z}\t{velocity[i].X}-{velocity[i].Y}-{velocity[i].Z}");
                        i++;
                    }
                }

                return 2; // recurrentIndexes.Distinct().ToList();
            }



            public Coordinate Coordinate { get; set; }
            public List<Coordinate> InitialCoordinates { get; set; } = new List<Coordinate>();
            public List<Coordinate> InitialVelocities { get; set; } = new List<Coordinate>();

            private Coordinate Velocity { get; set; }

            public void DetermineAndSaveVelocity(Coordinate velocity)
            {
                Velocity.X += velocity.X;
                Velocity.Y += velocity.Y;
                Velocity.Z += velocity.Z;
                InitialVelocities.Add(new Coordinate(Velocity.X, Velocity.Y, Velocity.Z, CoordinateIndex));

            }

            public int GetEnergy()
            {
                return GetPotentialEnergy() * GetKineticEnergy();
            }

            private int GetPotentialEnergy()
            {
                return Math.Abs(Coordinate.X) + Math.Abs(Coordinate.Y) + Math.Abs(Coordinate.Z);
            }
            private int GetKineticEnergy()
            {
                return Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);
            }

            internal void ApplyVelocity()
            {

                Coordinate.X += Velocity.X;
                Coordinate.Y += Velocity.Y;
                Coordinate.Z += Velocity.Z;
                InitialCoordinates.Add(new Coordinate(Coordinate.X, Coordinate.Y, Coordinate.Z, CoordinateIndex));
            }

            internal void UpIndex()
            {
                CoordinateIndex++;
            }
        }

        public override async Task Part1()
        {
            var removegtstSigns = PuzzleInput.Replace("<", "").Replace(">", "");
            var lines = removegtstSigns.Split("\r\n").Select(x => x.Split(',').Select(y => new string(y.Trim().SkipWhile(z => z == '=').ToArray()))).Select(x => x.Select(y => y.Substring(y.IndexOf('=') + 1).Trim())); ;
            var moonsLijst = lines.Select(x => new Moon(Convert.ToInt32(x.First()), Convert.ToInt32(x.Skip(1).First()), Convert.ToInt32(x.Last())));
            var moons = moonsLijst.ToList();


            for (int count = 0; count < 1000; count++)
            {

                for (int i = 0; i < moons.Count(); i++)
                {
                    var moon = moons[i];
                    var allMoons = moons.Where(x => x != moon);
                    moon.DetermineAndSaveVelocity(new Coordinate(allMoons.Count(x => x.Coordinate.X > moon.Coordinate.X) - allMoons.Count(x => x.Coordinate.X < moon.Coordinate.X), allMoons.Count(x => x.Coordinate.Y > moon.Coordinate.Y) - allMoons.Count(x => x.Coordinate.Y < moon.Coordinate.Y), allMoons.Count(x => x.Coordinate.Z > moon.Coordinate.Z) - allMoons.Count(x => x.Coordinate.Z < moon.Coordinate.Z)));
                }

                foreach (var moon in moons)
                {
                    moon.ApplyVelocity();
                }
            }

            var totalEnergy = moons.Select(x => x.GetEnergy()).Sum();
            ResultPart1 = string.Empty;
        }

        public override async Task Part2()
        {
            var removegtstSigns = PuzzleInput.Replace("<", "").Replace(">", "");
            var lines = removegtstSigns.Split("\r\n").Select(x => x.Split(',').Select(y => new string(y.Trim().SkipWhile(z => z == '=').ToArray()))).Select(x => x.Select(y => y.Substring(y.IndexOf('=') + 1).Trim())); ;
            var moonsLijst = lines.Select(x => new Moon(Convert.ToInt32(x.First()), Convert.ToInt32(x.Skip(1).First()), Convert.ToInt32(x.Last())));
            var moons = moonsLijst.ToList();
            var numberofMoons = moonsLijst.Count();
            var width = 25;
            var height = 20;
            for (long step = 0; step < 300000; step++)
            {
                var bm = new int[width, height];
                //initialstate = true;
                Parallel.ForEach(moons, (moon) =>
                {
                    // var moon = moons[i];
                    moon.UpIndex();
                    var allMoons = moons.Where(x => x != moon).ToHashSet();
                    moon.DetermineAndSaveVelocity(new Coordinate(allMoons.Count(x => x.Coordinate.X > moon.Coordinate.X) - allMoons.Count(x => x.Coordinate.X < moon.Coordinate.X), allMoons.Count(x => x.Coordinate.Y > moon.Coordinate.Y) - allMoons.Count(x => x.Coordinate.Y < moon.Coordinate.Y), allMoons.Count(x => x.Coordinate.Z > moon.Coordinate.Z) - allMoons.Count(x => x.Coordinate.Z < moon.Coordinate.Z)));
                });

                Parallel.ForEach(moons, (moon) =>
                {
                    moon.ApplyVelocity();
                });
            }

            var indexList = new List<long>();
            var niceCoord = new Coordinate(0, 0, 0);
            for (int step = 0; step < 300000; step++)
            {
                if (niceCoord.X == 0) checkX(moons, niceCoord, step);
                if (niceCoord.Y == 0) checkY(moons, niceCoord, step);
                if (niceCoord.Z == 0) checkZ(moons, niceCoord, step);

                if (niceCoord.HasAll) break;
            }

            var listLong = new List<long>() { niceCoord.X, niceCoord.Y, niceCoord.Z };
            var output = listLong.Aggregate((x, y) => determineLCM(x, y));

           
            var test = indexList.GroupBy(x => x).Where(g => g.Count() == numberofMoons);

            var totalEnergy = moons.Select(x => x.GetEnergy()).Sum();

            ResultPart2 = "";
        }

        public static long determineLCM(long a, long b)
        {
            long num1, num2;
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (int i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }

        private dynamic MoonFilter(Moon x, int step) => new { coord = x.InitialCoordinates[step], velos = x.InitialVelocities[step], initial = x.InitialCoordinates[0] };
        private void checkX(List<Moon> moons, Coordinate niceCoord, int step)
        {
            var allmoons = moons.Select(x => MoonFilter(x,step)).All(y=> y.initial.X == y.coord.X && y.velos.X == 0); //.Where(x=> x.CoordinateIndex == step).Count(x => x.Coordinate.X == x.InitialCoordinates[0].X && x.InitialVelocities[0].X == 0);
            if (allmoons) { niceCoord.X = step; }

        }

        private void checkY(List<Moon> moons, Coordinate niceCoord, int step)
        {
            var allmoons = moons.Select(x => MoonFilter(x, step)).All(y => y.initial.Y == y.coord.Y && y.velos.Y == 0);
            if (allmoons) { niceCoord.Y = step; }

        }

        private void checkZ(List<Moon> moons, Coordinate niceCoord, int step)
        {
            var allmoons = moons.Select(x => MoonFilter(x, step)).All(y => y.initial.Z == y.coord.Z && y.velos.Z == 0);
            if (allmoons) { niceCoord.Z = step; }

        }

        public static void FillGrid(List<Moon> moons, int shiftX, int shiftY, int[,] bm, int width, int height)
        {
            for (int i = 0; i < moons.Count; i++)
            {
                bm[moons[i].Coordinate.X + shiftX, moons[i].Coordinate.Y + shiftY] = i + 1;
            }
            var sb = new StringBuilder();

            var x = 0;
            var y = 0;
            for (int square = 0; square < height; square++)
            {
                for (int w = 0; w < width; w++)
                {
                    sb.Append(bm[w, square]);
                }
                sb.AppendLine();
            }
            Debug.WriteLine(sb.ToString());
        }
    }
}







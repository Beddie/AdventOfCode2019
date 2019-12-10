using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day10 : AdventBase
    {
        public Day10(int day) : base(day)
        {
            Title = "Monitoring Station";
            //            TestInput = @".#..##.###...#######
            //##.############..##.
            //.#.######.########.#
            //.###.#######.####.#.
            //#####.##.#.##.###.##
            //..#####..#.#########
            //####################
            //#.####....###.#.#.##
            //##.#################
            //#####.##.###..####..
            //..######..##.#######
            //####.##.####...##..#
            //.#####..#.######.###
            //##...#.##########...
            //#.##########.#######
            //.####.#.###.###.#.##
            //....##.##.###..#####
            //.#.#.###########.###
            //#.#.#.#####.####.###
            //###.##.####.##.#..##";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day10;
            SolutionPart1 = "263";
            SolutionPart2 = "1110";
        }

        public struct Coordinate
        {
            public Coordinate(double x, double y)
            {
                X = x;
                Y = y;
            }
            public double X { get; set; }
            public double Y { get; set; }
        }

        public class Astroid
        {
            public Astroid(double x, double y)
            {
                Coordinate = new Coordinate(x, y);
            }
            public Coordinate Coordinate { get; set; }
            public double Angle { get; set; }
            public List<Astroid> VisibleAstroids { get; set; } = new List<Astroid>();
            public List<Astroid> InVisibleAstroids { get; set; } = new List<Astroid>();


            internal void TryLookAtAstroids(List<Astroid> astroids)
            {
                foreach (var astroid in astroids)
                {
                    var coordinate = astroid.Coordinate;
                    var lineMap = GetSightLineMap(coordinate);
                    var somethingInBetween = astroids.Where(x => x != astroid && lineMap.Contains(x.Coordinate)).Any();
                    if (!somethingInBetween)
                    {
                        VisibleAstroids.Add(astroid);
                    }
                    else
                    {
                        InVisibleAstroids.Add(astroid);
                    }
                }
            }

            static double GetSmallestRatio(Coordinate coordinate)
            {
                double Remainder;

                while (coordinate.Y != 0)
                {
                    Remainder = coordinate.X % coordinate.Y;
                    coordinate.X = coordinate.Y;
                    coordinate.Y = Remainder;
                }
                return Math.Abs(coordinate.X);
            }

            private List<Coordinate> GetSightLineMap(Coordinate coordinate)
            {
                var sightLine = new List<Coordinate>();
                var varx = Coordinate.X;
                var vary = Coordinate.Y;
                var offsetCoordinate = new Coordinate(coordinate.X - Coordinate.X, coordinate.Y - Coordinate.Y);

                var ratio = GetSmallestRatio(offsetCoordinate);
                var ratioCoordinate = new Coordinate(offsetCoordinate.X / ratio, offsetCoordinate.Y / ratio);
                while (true)
                {
                    varx += ratioCoordinate.X;
                    vary += ratioCoordinate.Y;
                    if (
                            (varx > coordinate.X && offsetCoordinate.X >= 0
                            ||
                            vary > coordinate.Y && offsetCoordinate.Y >= 0)
                            ||
                            varx < coordinate.X && offsetCoordinate.X < 0
                            ||
                            vary < coordinate.Y && offsetCoordinate.Y < 0
                        )
                    {
                        break;
                    }
                    var nextSight = new Coordinate(varx, vary);
                    sightLine.Add(nextSight);
                }
                return sightLine;
            }

            public string Vaporize200Astroids()
            {
                foreach (var astroid in VisibleAstroids)
                {
                    astroid.CalculateAngle(Coordinate);
                }
                var allvisibleastroids = VisibleAstroids.OrderBy(x => x.Angle).ToList();
                var i = 0;
                var puzzle2Answer = string.Empty;
                while (i < 201)
                {
                    i++;
                    var astroid = allvisibleastroids[0];

                    if (i == 200 || allvisibleastroids.Count == 0)
                    {
                        return $"{astroid.Coordinate.X * 100 + astroid.Coordinate.Y}";
                    }
                    allvisibleastroids.Remove(astroid);
                }
                return "nothing found";
            }

            private void CalculateAngle(Coordinate coordinate)
            {
                var xDiff = (float)Coordinate.X - (float)coordinate.X;
                var yDiff = (float)Coordinate.Y - (float)coordinate.Y;
                Angle = (ToDegrees(MathF.Atan2(yDiff, xDiff)) + 450f) % 360f;
            }
            public static float ToDegrees(float radians) => radians * 180f / MathF.PI;
        }


        public List<Astroid> Astroids { get; set; } = new List<Astroid>();

        public override async Task Part1()
        {
            var allPuzzleLines = PuzzleInput.Split("\r\n");
            var length = allPuzzleLines.First().Length;
            var height = allPuzzleLines.Count();
            var maxCoordinate = new Coordinate() { X = length, Y = height };

            for (int verticalLine = 0; verticalLine < maxCoordinate.Y; verticalLine++)
            {
                for (int horizontalLine = 0; horizontalLine < maxCoordinate.X; horizontalLine++)
                {
                    var drawing = allPuzzleLines[verticalLine][horizontalLine];
                    if (drawing == '#')
                    {
                        Astroids.Add(new Astroid(horizontalLine, verticalLine));
                    }
                }
            }

            foreach (var astroid in Astroids)
            {
                astroid.TryLookAtAstroids(Astroids.Where(x => x != astroid).ToList());
            }
            var bestAstroid = Astroids.OrderByDescending(x => x.VisibleAstroids.Count).First();
            ResultPart1 = bestAstroid.VisibleAstroids.Count().ToString();
        }

        public override async Task Part2()
        {
            var allPuzzleLines = PuzzleInput.Split("\r\n");
            var length = allPuzzleLines.First().Length;
            var height = allPuzzleLines.Count();
            var maxCoordinate = new Coordinate() { X = length, Y = height };

            for (int verticalLine = 0; verticalLine < maxCoordinate.Y; verticalLine++)
            {
                for (int horizontalLine = 0; horizontalLine < maxCoordinate.X; horizontalLine++)
                {
                    var drawing = allPuzzleLines[verticalLine][horizontalLine];
                    if (drawing == '#')
                    {
                        Astroids.Add(new Astroid(horizontalLine, verticalLine));
                    }
                }
            }

            foreach (var astroid in Astroids)
            {
                astroid.TryLookAtAstroids(Astroids.Where(x => x != astroid).ToList());
            }
            var bestAstroid = Astroids.OrderByDescending(x => x.VisibleAstroids.Count).First();
            ResultPart2 = bestAstroid.Vaporize200Astroids();
        }
    }
}


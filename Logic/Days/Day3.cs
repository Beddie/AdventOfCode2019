using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day3 : AdventBase
    {
      

        public Day3(int day) : base(day)
        {
            Title = "Crossed Wires";
           // TestInput = "R8,U5,L5,D3\n" + "U7,R6,D4,L4";

            // TestInput = "R75,D30,R83,U83,L12,D49,R71,U7,L72\nU62,R66,U55,R34,D71,R55,D58,R83";
           // TestInput = @"R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51
//U98,R91,D20,R16,D67,R40,U7,R15,U6,R7";
            //TestInput = "R8,U5,L5,D3\n" + "U1,R1,D1,L1";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day3;
            Day3Wires = PuzzleInput.Split('\n');
            SolutionPart1 = "896";
            SolutionPart2 = "16524";
        }

        private string[] Day3Wires;
        private Color red = Color.Red;


        public class Wire
        {
            public int X;
            public int Y;
            public int TotalDistance;
            public List<Wire> Trail = new List<Wire>();
            public void ExecuteInstruction(char insturction)
            {
                var distance = ++TotalDistance;
                switch (insturction)
                {
                    case 'R':
                        Right();
                        break;
                    case 'U':
                        Up();
                        break;
                    case 'L':
                        Left();
                        break;
                    case 'D':
                        Down();
                        break;
                    default:
                        break;
                }
                Trail.Add(new Wire() { X = X, Y = Y, TotalDistance = distance });
            }

            public void Up() { Y++; }
            public void Down() { Y--; }
            public void Left() { X--; }
            public void Right() { X++; }
        }

        public static int ManhattanDistance(int x1, int x2, int y1, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        public override async Task Part1()
        {
            //Set 1 for wire 1 and 2 for wire 2, 3 = crossed
            var wires = new List<Wire>();
            AddWires(wires, 0);
            AddWires(wires, 1);
            var shiftX = 0;
            var shiftY = 0;
            shiftX = wires.SelectMany(x => x.Trail).Select(x => x.X).Min();
            shiftY = wires.SelectMany(x => x.Trail).Select(x => x.Y).Min();

            shiftX = shiftX < 0 ? shiftX * -1 : shiftX;
            shiftY = shiftY < 0 ? shiftY * -1 : shiftY;
            var width = 40000;
            var bm = new int[width, width];
            var colorWire = 1;
            var crosses = new List<int>();
            for (int i = 0; i < wires.Count; i++)
            {
                var traillist = wires[i].Trail;
                foreach (var trail in traillist)
                {
                    var color = bm[trail.X + shiftX, trail.Y + shiftY];

                    if (color == 1 && colorWire > 1)
                    {
                        bm[trail.X + shiftX, trail.Y + shiftY] = colorWire + color;
                        crosses.Add(ManhattanDistance(shiftX, trail.X + shiftX, shiftY, trail.Y + shiftY));
                    }
                    else
                    {
                        bm[trail.X + shiftX, trail.Y + shiftY] = colorWire;
                    }
                }
                colorWire = 2;
            }

            //WriteGrid(shiftX, shiftY, width, bm);
            ResultPart1 = $"{crosses.Min()}";
        }

     

        private void AddWires(List<Wire> wires, int number)
        {
            var wireInstructions = Day3Wires[number].Split(',');
            var wire = new Wire();
            foreach (var wire1Instruction in wireInstructions)
            {
                var w = wire1Instruction.Substring(1);
                var amount = Convert.ToInt32(w);
                for (int i = 0; i < amount; i++)
                {
                    wire.ExecuteInstruction(wire1Instruction[0]);
                }
            }
            wires.Add(wire);
        }

        public override async Task Part2()
        {
            var wires = new List<Wire>();
            AddWires(wires, 0);
            AddWires(wires, 1);
            var shiftX = 0;
            var shiftY = 0;
            shiftX = wires.SelectMany(x => x.Trail).Select(x => x.X).Min();
            shiftY = wires.SelectMany(x => x.Trail).Select(x => x.Y).Min();

            shiftX = shiftX < 0 ? shiftX * -1 : shiftX;
            shiftY = shiftY < 0 ? shiftY * -1 : shiftY;
            var width = 40000;
            var bm = new int[width, width];
            var crosses = new List<Wire>();
            FillGrid(wires, shiftX, shiftY, bm, crosses);

            var minDistance = 999999;
            foreach (var cross in crosses)
            {
                var distanceA = wires[0].Trail.Where(coord => coord.X == cross.X && coord.Y == cross.Y).FirstOrDefault().TotalDistance;
                var distanceB = wires[1].Trail.Where(coord => coord.X == cross.X && coord.Y == cross.Y).FirstOrDefault().TotalDistance;
                minDistance = distanceA + distanceB < minDistance ? distanceA + distanceB : minDistance;
            }
           
            ResultPart2 = $"{minDistance}";
        }

             
        public static void FillGrid(List<Wire> wires, int shiftX, int shiftY, int[,] bm, List<Wire> crosses )
        {
            var colorWire = 1;
           
            for (int i = 0; i < wires.Count; i++)
            {
                var traillist = wires[i].Trail;
                foreach (var trail in traillist)
                {
                    var color = bm[trail.X + shiftX, trail.Y + shiftY];

                    if (color == 1 && colorWire > 1)
                    {
                        bm[trail.X + shiftX, trail.Y + shiftY] = colorWire + color;
                        crosses.Add(trail);
                    }
                    else
                    {
                        bm[trail.X + shiftX, trail.Y + shiftY] = colorWire;
                    }
                }
                colorWire = 2;
            }
        }

        private static void WriteGrid(int shiftX, int shiftY, int width, int[,] bm)
        {
            var sb = new StringBuilder();
            var maxSquare = width;
            var x = 0;
            var y = 0;

            for (int square = 0; square < maxSquare; square++)
            {
                var color = 0;




                for (int w = 0; w <= square; w++)
                {
                    color = bm[w, square];
                    if (color == 3)
                    {
                        x = w;
                        y = square;
                        break;

                    }
                }

                if (x != 0) break;

                for (int h = 0; h <= square; h++)
                {
                    color = bm[square, h];
                    if (color == 3)
                    {
                        x = square;
                        y = h;
                        break;

                    }
                }
            }

            x += shiftX;
            y += shiftY;


            //bm.RotateFlip( RotateFlipType.Rotate180FlipXY);
            bm[shiftX, shiftY] = 99;
            for (int h = width - 1; h >= 0; h--)
            {
                for (int w = 0; w < width; w++)
                {

                    var color = bm[w, h];
                    switch (color)
                    {
                        case 1:
                            sb.Append('R');
                            break;
                        case 2:
                            sb.Append('G');
                            break;
                        case 3:
                            sb.Append('X');
                            break;
                        case 99:
                            sb.Append('O');
                            break;
                        default:
                            sb.Append('.');
                            break;
                    }


                }
                sb.AppendLine();
            }
            Debug.WriteLine(sb.ToString());
        }
    }
}


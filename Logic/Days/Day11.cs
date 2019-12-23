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
    public class Day11 : AdventBase
    {
        public Day11(int day) : base(day)
        {
            Title = "Space Police";
            //TestInput = "109,19,204,-34";
            // TestInput = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day11;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            SolutionPart1 = "2478";
            SolutionPart2 = "hczrugaz";
        }

        private List<long> ComputerMemoryInput;

        public enum Direction
        {
            Left = 0,
            Right = 1
        }

        public enum Vector
        {
            Left = 6,
            Right = 7,
            Up = 8,
            Down = 9
        }

        public enum Color
        {
            Black = 0,
            White = 1
        }

        public struct CurrentCoordinate
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public class Coordinate
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        public class Robot
        {

            private Coordinate coordinate { get; set; } = new Coordinate { X = 0, Y = 0 };

            public Coordinate GetCurrentCoordinate()
            {
                return coordinate;
            }
            private Vector currentVector { get; set; } = Vector.Up;
            private Color currentColor { get; set; } = Color.Black;
            public void Move()
            {
                switch (currentVector)
                {
                    case Vector.Left:
                        coordinate.X--;
                        break;
                    case Vector.Right:
                        coordinate.X++;
                        break;
                    case Vector.Up:
                        coordinate.Y++;
                        break;
                    case Vector.Down:
                        coordinate.Y--;
                        break;
                    default:
                        break;
                }
            }

            public void DetermineDirection(Direction direction)
            {
                switch (direction)
                {
                    case Direction.Left:
                        switch (currentVector)
                        {
                            case Vector.Left:
                                currentVector = Vector.Down;
                                break;
                            case Vector.Right:
                                currentVector = Vector.Up;
                                break;
                            case Vector.Up:
                                currentVector = Vector.Left;
                                break;
                            case Vector.Down:
                                currentVector = Vector.Right;
                                break;
                        }
                        break;
                    case Direction.Right:
                        switch (currentVector)
                        {
                            case Vector.Left:
                                currentVector = Vector.Up;
                                break;
                            case Vector.Right:
                                currentVector = Vector.Down;
                                break;
                            case Vector.Up:
                                currentVector = Vector.Right;
                                break;
                            case Vector.Down:
                                currentVector = Vector.Left;
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            public void Paint(long color)
            {
                currentColor = (Color)color;
            }
        }


        public override async Task Part1()
        {
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 0);
            var inputValue = 0;
            var robot = new Robot();
            var stride = 300;
            var grid = new int[stride, stride];
            var currentPosition = new Coordinate() { X = 0, Y = 0 };
            var shiftX = 150;
            var shiftY = 150;
            var listPaintedCoordinates = new Dictionary<CurrentCoordinate, int>();
            while (true)
            {
                try
                {


                    var currentColor = (Color)grid[currentPosition.X + shiftX, currentPosition.Y + shiftY];
                    var values = intComputer.StartAndReturn2Outputs((int)currentColor);

                    if (values[2] == 1)
                    {
                        break;
                    }

                    grid[currentPosition.X + shiftX, currentPosition.Y + shiftY] = (int)values[0];
                    var coord = new CurrentCoordinate() { X = currentPosition.X, Y = currentPosition.Y };
                    if (listPaintedCoordinates.TryAdd(new CurrentCoordinate() { X = currentPosition.X, Y = currentPosition.Y }, 1))
                    {
                        listPaintedCoordinates[coord] += 1;
                    };
                    //robot.Paint(values[0]);
                    robot.DetermineDirection((Direction)values[1]);
                    robot.Move();
                    Debug.WriteLine($"{values[0]} - {values[1]}");
                    currentPosition = robot.GetCurrentCoordinate();

                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            var paintedSquares = listPaintedCoordinates.Count();

            var puzzleAnswerBuilder = new StringBuilder();
            for (int y = 0; y < stride; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    puzzleAnswerBuilder.Append(grid[x, y] == 0 ? ' ' : '*');
                }
                puzzleAnswerBuilder.AppendLine();
            }
            Debug.WriteLine(puzzleAnswerBuilder.ToString());

            ResultPart1 = intComputer.LocalComputerMemory.ProgramValue.ToString();
        }


        public override async Task Part2()
        {
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 0);
            var inputValue = 0;
            var robot = new Robot();
            var stride = 300;
            var grid = new int[stride, stride];
            var currentPosition = new Coordinate() { X = 0, Y = 0 };
            var shiftX = 150;
            var shiftY = 150;
            var listPaintedCoordinates = new Dictionary<CurrentCoordinate, int>();
            var firstwhite = true;
            while (true)
            {
                try
                {
                    var currentColor = (Color)grid[currentPosition.X + shiftX, currentPosition.Y + shiftY];
                    if (firstwhite) {
                        currentColor =Color.White;
                        firstwhite = false;
                    }
                   
                    var values = intComputer.StartAndReturn2Outputs((int)currentColor);

                    if (values[2] == 1)
                    {
                        break;
                    }

                    grid[currentPosition.X + shiftX, currentPosition.Y + shiftY] = (int)values[0];
                    var coord = new CurrentCoordinate() { X = currentPosition.X, Y = currentPosition.Y };
                    if (listPaintedCoordinates.TryAdd(new CurrentCoordinate() { X = currentPosition.X, Y = currentPosition.Y }, 1))
                    {
                        listPaintedCoordinates[coord] += 1;
                    };
                    //robot.Paint(values[0]);
                    robot.DetermineDirection((Direction)values[1]);
                    robot.Move();
                    Debug.WriteLine($"{values[0]} - {values[1]}");
                    currentPosition = robot.GetCurrentCoordinate();

                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            var paintedSquares = listPaintedCoordinates.Count();

            var puzzleAnswerBuilder = new StringBuilder();
            for (int y = stride - 1; y >= 0; y--)
            {
                for (int x = 0; x < stride; x++)
                {
                    puzzleAnswerBuilder.Append(grid[x, y] == 0 ? ' ' : '*');
                }
                puzzleAnswerBuilder.AppendLine();
            }
            Debug.WriteLine(puzzleAnswerBuilder.ToString());

            ResultPart2 = "hczrugaz";

        }
    }
}


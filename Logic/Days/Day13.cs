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
    public class Day13 : AdventBase
    {
        public Day13(int day) : base(day)
        {
            Title = "Care Package";
            TestInput = "";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day13;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            SolutionPart1 = "412";
            SolutionPart2 = "20940";
        }

        private List<long> ComputerMemoryInput;

        public override async Task Part1()
        {
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 0);
            var stride = 300;
            var grid = new int[stride, stride];
            var shiftX = 150;
            var shiftY = 150;
            var listTiles = new Dictionary<TileType, int>();
            while (true)
            {
                var values = intComputer.StartAndReturn3Outputs();
                var tile = new Tile() { Coordinate = new Coordinate() { X = values[0], Y = values[1] }, TileType = (TileType)values[2] };

                if (!listTiles.TryAdd(tile.TileType, 1))
                {
                    listTiles[tile.TileType] += 1;
                };

                if (values[3] == 1)
                {
                    break;
                }
                grid[tile.Coordinate.X + shiftX, tile.Coordinate.Y + shiftY] = (int)tile.TileType;
            }

            var blockTiles = listTiles[TileType.Block];
            ResultPart1 = blockTiles.ToString();
        }

        public struct Coordinate
        {
            public long X { get; set; }
            public long Y { get; set; }
        }

        public class Tile
        {
            public Coordinate Coordinate { get; set; }

            public TileType TileType { get; set; }
        }

        public enum TileType
        {
            Empty = 0, // is an empty tile. No game object appears in this tile.
            Wall = 1,  //is a wall tile.Walls are indestructible barriers.
            Block = 2, //is a block tile.Blocks can be broken by the ball.
            Paddle = 3, //is a horizontal paddle tile. The paddle is indestructible.
            Ball = 4 //is a ball tile.The ball moves diagonally and bounces off objects.
        }
       
        public override async Task Part2()
        {
            var paddle = new Coordinate();
            var ball = new Coordinate();

            ComputerMemoryInput[0] = 2;
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 0);
            var score = (long)0;
            var input = (long)0;
            while (true)
            {
                var values = intComputer.StartAndReturn3Outputs(input);
                var tile = new Tile() { Coordinate = new Coordinate() { X = values[0], Y = values[1] }, TileType = (TileType)values[2] };
                if (tile.Coordinate.X == -1 && tile.Coordinate.Y == 0)
                {
                    score = values[2];
                    
                }
                else if (tile.TileType == TileType.Paddle)
                {
                    paddle = new Coordinate() { X = values[0], Y = values[1] };
                }
                else if (tile.TileType == TileType.Ball)
                {
                    ball = new Coordinate() { X = values[0], Y = values[1] };
                }

                input = Math.Max(-1, Math.Min((ball.X - paddle.X), 1));

                if (values[3] == 1)
                {
                    break;
                }
            }
            ResultPart2 = score.ToString();
        }
    }
}


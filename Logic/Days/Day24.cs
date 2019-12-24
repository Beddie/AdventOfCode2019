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
    public class Day24 : AdventBase
    {
        public Day24(int day) : base(day)
        {
            Title = "Planet of Discord";
            //TestInput = @"....#
            //#..#.
            //#..##
            //..#..
            //#....";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day24;
            SolutionPart1 = "3186366";
            SolutionPart2 = "2031";
        }


        public override async Task Part1()
        {
            const int stride = 5;
            var layoutList = new List<Layout[,]>();
            var space = new Space(new int[stride, stride]);
            double? amount = null;
            FillFirstSpaceWithPuzzleInput(space);

            while (true)
            {
                var nextgrid = new int[stride, stride];
                var layoutGrid = new Layout[stride, stride];
                for (int y = 0; y < stride; y++)
                {
                    for (int x = 0; x < stride; x++)
                    {
                        var layout = AmountOfNeighbours(x, y, space.Grid);

                        if (space.Grid[x, y] == 1)
                        {
                            if (layout.TotalBugAmount == 1)
                            {
                                nextgrid[x, y] = 1;
                            }
                            else
                            {
                                //dies
                                nextgrid[x, y] = 0;
                            }
                        }
                        else
                        {
                            if (layout.TotalBugAmount == 1 || layout.TotalBugAmount == 2)
                            {
                                nextgrid[x, y] = 1;
                            }
                            else
                            {
                                nextgrid[x, y] = 0;
                            }
                        }
                        layoutGrid[x, y] = layout;
                    }

                }

                layoutList.Add(layoutGrid);
                //printGrid(layoutGrid);
                amount = EvaluateGrids(layoutList);
                if (amount != null)
                {
                    break;
                }

                space.Grid = nextgrid;
            }
            ResultPart1 = amount.ToString();
        }

        private double? EvaluateGrids(List<Layout[,]> gridList)
        {
            var listString = new List<string>();

            foreach (var grid in gridList)
            {
                var sb = new StringBuilder();
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    for (int x = 0; x < grid.GetLength(0); x++)
                    {
                        sb.Append(grid[x, y].Current == 0 ? '.' : '#');
                    }
                }
                listString.Add(sb.ToString());
            }

            var groupie = listString.GroupBy(w => w).Select(a => new { a.Key, amount = a.Count() });

            var powerof2 = 1d;
            if (groupie.Any(x => x.amount > 1))
            {
                var totAmount = 0d;
                var pickup = groupie.Where(x => x.amount > 1).First().Key;

                for (int tile = 0; tile < pickup.Length; tile++)
                {
                    if (pickup[tile] == '#')
                    {
                        totAmount += Math.Pow(2, tile);
                    }
                    powerof2++;
                }
                return totAmount;

            }

            return null;
        }

        private void printGrid(Layout[,] layoutGrid)
        {
            var puzzleAnswerBuilder = new StringBuilder();
            for (int y = 0; y < layoutGrid.GetLength(1); y++)
            {
                for (int x = 0; x < layoutGrid.GetLength(0); x++)
                {
                    puzzleAnswerBuilder.Append(layoutGrid[x, y].Current == 0 ? '.' : '#');
                }
                puzzleAnswerBuilder.AppendLine();
            }
            Debug.WriteLine(puzzleAnswerBuilder.ToString());
        }

        public struct Coordinate
        {
            public Coordinate(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }
        public struct Layout
        {
            public Coordinate Coordinate { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }
            public int Up { get; set; }
            public int Down { get; set; }
            public int Current { get; set; }
            public int TotalBugAmount => Left + Right + Up + Down;
        }

        private void printGrid(int[,] grid, Space space)
        {
            var puzzleAnswerBuilder = new StringBuilder();
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    puzzleAnswerBuilder.Append(grid[x, y] == 0 ? '.' : '#');
                }
                puzzleAnswerBuilder.AppendLine();
            }
            Debug.WriteLine($"level:{space.Level}");
            Debug.WriteLine(puzzleAnswerBuilder.ToString());
        }

        private Layout AmountOfNeighbours(int x, int y, int[,] grid)
        {
            var layout = new Layout();
            layout.Coordinate = new Coordinate(x, y);
            var stride = grid.GetLength(0);
            //Up
            if (y - 1 >= 0 && grid[x, y - 1] == 1)
            {
                layout.Up = 1;
            }
            //Down
            if (y + 1 < stride && grid[x, y + 1] == 1)
            {
                layout.Down = 1;
            }
            //Left
            if (x - 1 >= 0 && grid[x - 1, y] == 1)
            {
                layout.Left = 1;
            }
            //Right
            if (x + 1 < stride && grid[x + 1, y] == 1)
            {
                layout.Right = 1;
            }

            layout.Current = grid[x, y];
            return layout;
        }

        public class Space
        {
            public int[,] Grid;
            public int[,] NextGrid;
            public int Level;
            public Space(int[,] _grid, int _level = 0)
            {
                Grid = _grid;
                Level = _level;
            }

            public Layout[,] LayoutGrid { get; set; }

            public int AmountofBugs()
            {
                var stride = LayoutGrid.GetLength(0);
                var amount = 0;
                for (int y = 0; y < stride; y++)
                {
                    for (int x = 0; x < stride; x++)
                    {
                        amount += LayoutGrid[x, y].Current == 1 ? 1 : 0;
                    }
                }
                return amount;
            }
        }

        public override async Task Part2()
        {
            const int stride = 5;
            var firstSpace = new Space(new int[stride, stride]);
            var spaceList = new List<Space>() { firstSpace };
            double? amount = 0d;
            const int depth = 250;

            CreateLevels(spaceList, depth);

            FillFirstSpaceWithPuzzleInput(firstSpace);

            for (int i = 0; i < (!string.IsNullOrEmpty(TestInput) ? 10 : 200); i++)
            {
                foreach (var space in spaceList)
                {
                    space.NextGrid = new int[stride, stride];
                    space.LayoutGrid = new Layout[stride, stride];
                    for (int y = 0; y < stride; y++)
                    {
                        for (int x = 0; x < stride; x++)
                        {
                            if (x == 2 && y == 2)
                            {
                                space.NextGrid[x, y] = 0;
                                space.LayoutGrid[x, y] = new Layout() { Current = 0 };
                            }
                            else
                            {
                                var layout = AmountOfNeighboursInfinite(x, y, space, spaceList);

                                if (space.Grid[x, y] == 1)
                                {
                                    if (layout.TotalBugAmount == 1)
                                    {
                                        space.NextGrid[x, y] = 1;
                                    }
                                    else
                                    {
                                        //dies
                                        space.NextGrid[x, y] = 0;
                                    }
                                }
                                else
                                {
                                    if (layout.TotalBugAmount == 1 || layout.TotalBugAmount == 2)
                                    {
                                        space.NextGrid[x, y] = 1;
                                    }
                                    else
                                    {
                                        space.NextGrid[x, y] = 0;
                                    }
                                }
                                layout.Current = space.NextGrid[x, y];
                                space.LayoutGrid[x, y] = layout;
                            }
                        }

                    }
                }

                for (int spaceIndex = 0; spaceIndex < spaceList.Count; spaceIndex++)
                {
                    spaceList[spaceIndex].Grid = spaceList[spaceIndex].NextGrid;
                }
            }

            //foreach (var space in spaceList)
            //{
            //    printGrid(space.LayoutGrid);
            //}
            amount = spaceList.Sum(x => x.AmountofBugs());
            ResultPart2 = amount.ToString();
        }

        private void FillFirstSpaceWithPuzzleInput(Space firstSpace)
        {
            var stride = firstSpace.Grid.GetLength(0);
            var onestring = PuzzleInput.Replace("\r\n", "").Replace(" ", "");
            for (int i = 0; i < onestring.Length; i++)
            {
                var row = (int)(i / stride);
                firstSpace.Grid[i % stride, row] = onestring[i] == '.' ? 0 : 1;
            }
        }

        private Layout AmountOfNeighboursWithoutInfinite(int x, int y, int[,] grid)
        {
            var layout = new Layout();
            layout.Coordinate = new Coordinate(x, y);
            var stride = grid.GetLength(0);
            //Up
            if (y - 1 >= 0 && grid[x, y - 1] == 1)
            {
                layout.Up = 1;
            }
            //Down
            if (y + 1 < stride && grid[x, y + 1] == 1)
            {
                layout.Down = 1;
            }
            //Left
            if (x - 1 >= 0 && grid[x - 1, y] == 1)
            {
                layout.Left = 1;
            }
            //Right
            if (x + 1 < stride && grid[x + 1, y] == 1)
            {
                layout.Right = 1;
            }

            layout.Current = grid[x, y];
            return layout;
        }

        private void CreateLevels(List<Space> spaceList, int depth)
        {
            var initialSpace = spaceList.FirstOrDefault();
            var grid = initialSpace.Grid;
            var stride = grid.GetLength(0);
            for (int lower = 1; lower <= depth; lower++)
            {
                spaceList.Add(new Space(new int[stride, stride], initialSpace.Level + lower));
            }
            for (int higher = 1; higher <= depth; higher++)
            {
                spaceList.Add(new Space(new int[stride, stride], initialSpace.Level - higher));
            }

        }

        private Layout AmountOfNeighboursInfinite(int x, int y, Space space, List<Space> spaceList)
        {
            var centerCoordinate = new Coordinate(2, 2);
            var layout = new Layout();
            var parent = spaceList.FirstOrDefault(x => x.Level == space.Level - 1);
            var child = spaceList.FirstOrDefault(x => x.Level == space.Level + 1);
            var stride = space.Grid.GetLength(0);

            //Up
            if (y == 0 && parent != null)
            {

                layout.Up = parent.Grid[2, 1] == 1 ? 1 : 0;
            }
            else if (y == 3 && x == 2 && child != null)
            {
                for (int xchild = 0; xchild < stride; xchild++)
                {
                    layout.Up += child.Grid[xchild, 4] == 1 ? 1 : 0;
                }
            }
            else if (y - 1 >= 0 && space.Grid[x, y - 1] == 1)
            {
                layout.Up = 1;
            }
            //Down
            if (y == 4 && parent != null)
            {
                layout.Down = parent.Grid[2, 3] == 1 ? 1 : 0;
            }
            else if (y == 1 && x == 2 && child != null)
            {
                for (int xchild = 0; xchild < stride; xchild++)
                {
                    layout.Down += child.Grid[xchild, 0] == 1 ? 1 : 0;
                }
            }
            else if (y + 1 < stride && space.Grid[x, y + 1] == 1)
            {
                layout.Down = 1;
            }
            //Left
            if (x == 0 && parent != null)
            {

                layout.Left = parent.Grid[1, 2] == 1 ? 1 : 0;
            }
            else if (y == 2 && x == 3 && child != null)
            {
                for (int ychild = 0; ychild < stride; ychild++)
                {
                    layout.Left += child.Grid[4, ychild] == 1 ? 1 : 0;
                }
            }
            else if (x - 1 >= 0 && space.Grid[x - 1, y] == 1)
            {
                layout.Left = 1;
            }

            //Right
            if (x == 4 && parent != null)
            {
                layout.Right = parent.Grid[3, 2] == 1 ? 1 : 0;
            }
            else if (y == 2 && x == 1 && child != null)
            {
                for (int ychild = 0; ychild < stride; ychild++)
                {
                    layout.Right += child.Grid[0, ychild] == 1 ? 1 : 0;
                }
            }
            else if (x + 1 < stride && space.Grid[x + 1, y] == 1)
            {
                layout.Right = 1;
            }

            layout.Current = space.Grid[x, y];
            return layout;
        }
    }
}


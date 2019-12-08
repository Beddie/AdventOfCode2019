using Logic.Model;
using Logic.Properties;
using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day8 : AdventBase
    {
        public Day8(int day) : base(day)
        {
            Title = "Space Image Format";
            //TestInput = "123456789012";
            //TestInput = "0222112222120000";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day8;
            SolutionPart1 = "1474";
            SolutionPart2 = "jcrcb";
        }

        public class Layer
        {
            public Layer(int wide, int tall, string pixelString)
            {
                var regex = @$"\d{{{wide}}}";
                var pixelStringList = (from Match m in Regex.Matches(pixelString, regex) select m.Value).ToList();
                Pixels = pixelStringList.Take(tall).Select(x => new Pixel(x)).ToList();
                foreach (var pixel in Pixels)
                {
                    TotalZeroCount += pixel.ZeroCount;
                    TotalOneCount += pixel.OneCount;
                    TotalTwoCount += pixel.TwoCount;
                }
            }

            public int TotalZeroCount { get; set; }
            public int TotalOneCount { get; set; }
            public int TotalTwoCount { get; set; }
            public List<Pixel> Pixels { get; set; }
        }

        public class Pixel
        {
            public Pixel(string val)
            {
                Value = val;
            }
            public string Value { get; private set; }
            public int ZeroCount => CharCount('0');
            public int OneCount => CharCount('1');
            public int TwoCount => CharCount('2');
            private int CharCount(char v)
            {
                return Value.Count(x => x == v);
            }
        }

        public override async Task Part1()
        {
            var tall = 6;
            var width = 25;

            var regex = @$"\d{{{tall * width}}}";
            var pixelStringList = (from Match m in Regex.Matches(PuzzleInput, regex)
                                   select m.Value).ToList();
            var layers = new List<Layer>();
            foreach (var pixelString in pixelStringList)
            {
                var layer = new Layer(width,tall,pixelString);
                layers.Add(layer);
            }

            var layerWithFewestZeroDigits = layers.OrderBy(x => x.TotalZeroCount).FirstOrDefault();
            var amount = layerWithFewestZeroDigits.TotalOneCount * layerWithFewestZeroDigits.TotalTwoCount;
            ResultPart1 = amount.ToString();
        }
        public override async Task Part2()
        {
            var tall = 6;
            var width = 25;

            var regex = @$"\d{{{tall * width}}}";
            var pixelStringList = (from Match m in Regex.Matches(PuzzleInput, regex)
                                   select m.Value).ToList();
            var layers = new List<Layer>();
            foreach (var pixelString in pixelStringList)
            {
                var layer = new Layer(width, tall, pixelString);
                layers.Add(layer);
            }

            var grid = new char[width, tall];

            for (int k = 0; k < tall; k++)
                for (int m = 0; m < width; m++)
                    grid[m, k] = '9';

            for (int i = 0; i < tall; i++)
            {
                var pixels = layers.Select(x => x.Pixels.Skip(i).FirstOrDefault().Value).ToList();
                for (int w = 0; w < width; w++)
                {

                    for (int pi = 0; pi < pixels.Count; pi++)
                    {
                        var testpixel = pixels[pi];
                        if (testpixel[w] == '2')
                        {
                            continue;
                        }
                        else
                        {
                            var currentvalue = grid[w, i];
                            if (currentvalue == '9')
                            {
                                grid[w, i] = testpixel[w];
                            }
                        }

                    }
                }
            }

            var puzzleAnswerBuilder = new StringBuilder();
            for (int x = 0; x < tall; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    puzzleAnswerBuilder.Append(grid[y, x] == '0' ? ' ' : '*');
                }
                puzzleAnswerBuilder.AppendLine();
            }
            Debug.WriteLine(puzzleAnswerBuilder.ToString());
            ResultPart2 = "jcrcb";
        }
    }
}


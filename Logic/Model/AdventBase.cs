using Logic.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Logic.Model
{
    public class AdventBase : AdventInterface
    {
        public AdventBase(int id)
        {
            ID = id;
        }

        public int ID { get; private set; }
        protected string Title { get; set; }
        public string Name => $"Day {ID}: {Title}";
        public string ResultPart1 { get; protected set; }
        public string ResultPart2 { get; protected set; }
        public long ElapsedTime { get; private set; }
        public List<EnumParts> PartsToRender { get; set; } = new List<EnumParts>() { EnumParts.Part1, EnumParts.Part2 };
        public bool IsValid => (!HasPart1 || (HasPart1 && part1isValid)) && (!HasPart2 || (HasPart2 && part2isValid));
        public bool Active => Name.Length > 9;
      
        protected string TestInput { get; set; }
        protected string SolutionPart1 { private get; set; }
        protected string SolutionPart2 { private get; set; }
        protected string PuzzleInput { get; set; }

        private bool HasSolutionPart1 => !string.IsNullOrWhiteSpace(SolutionPart1);
        private bool HasSolutionPart2 => !string.IsNullOrWhiteSpace(SolutionPart2);
        private bool HasPart1 => PartsToRender.Contains(EnumParts.Part1);
        private bool HasPart2 => PartsToRender.Contains(EnumParts.Part2);
        private bool part1isValid => ResultPart1 == SolutionPart1;
        private bool part2isValid => ResultPart2 == SolutionPart2;

        public void PrintResultToDebugWindow()
        {
            if (HasPart1) Debug.WriteLine(GetResult(EnumParts.Part1));
            if (HasPart2) Debug.WriteLine(GetResult(EnumParts.Part2));
        }

        public string GetResult(EnumParts part)
        {
            switch (part)
            {
                case EnumParts.Part1:
                    return $"Day{ID}-Part1 answer ({ElapsedTime}ms): {ResultPart1} {(HasSolutionPart1 ? $"Valid = {part1isValid}" : string.Empty)}";
                case EnumParts.Part2:
                    return $"Day{ID}-Part2 answer ({ElapsedTime}ms): {ResultPart2} {(HasSolutionPart2 ? $"Valid = {part2isValid}" : string.Empty)}";
                case EnumParts.None:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public async Task RenderParts()
        {
            var sw = new Stopwatch();
            sw.Start();
            var runPart1 = (HasPart1) ? Part1() : null;
            var runPart2 = (HasPart2) ? Part2() : null;

            if (HasPart1) await runPart1;
            if (HasPart2) await runPart2;
            sw.Stop();
            ElapsedTime = sw.ElapsedMilliseconds;
        }

        public virtual async Task Part1()
        {
            throw new NotImplementedException();
        }

        public virtual async Task Part2()
        {
            throw new NotImplementedException();
        }
    }
}

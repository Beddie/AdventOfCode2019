using Logic.Model;
using Logic.Properties;
using Logic.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day23 : AdventBase
    {
        public Day23(int day) : base(day)
        {
            Title = "Category Six";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day23;
            SolutionPart1 = "24922";
            SolutionPart2 = "19478";
        }

        public override async Task Part1()
        {
            var computers = Enumerable.Range(0, 50).ToList().Select(x => new IntCodeComputer(PuzzleInput.Split(',').Select(x => Convert.ToInt64(x)).ToList(), x)).ToList();
            
            var queue = new ConcurrentDictionary<long, Queue<Packet>>();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var runAll = new List<Task<long?>>();

            computers.ForEach(computer => runAll.Add(Task.Run(() => computer.RunTillOutput(queue, tokenSource), token)));
           
            await Task.WhenAll(runAll);
            ResultPart1 = runAll.FirstOrDefault(x => x.Result.HasValue).Result.ToString();
        }


        public override async Task Part2()
        {
            var computers = Enumerable.Range(0, 50).ToList().Select(x => new IntCodeComputer(PuzzleInput.Split(',').Select(x => Convert.ToInt64(x)).ToList(), x)).ToList();
            
            var queue = new ConcurrentDictionary<long, Queue<Packet>>();
            var idleStatuss = new ConcurrentDictionary<long, bool>();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var runAll = new List<Task<long?>>();

            computers.ForEach(computer => runAll.Add(Task.Run(() => computer.RunTillOutputWithIdleStatus(queue, idleStatuss, tokenSource), token)));
            
            await Task.WhenAll(runAll);

            ResultPart2 = runAll.FirstOrDefault(x => x.Result.HasValue).Result.ToString();

        }
    }
}


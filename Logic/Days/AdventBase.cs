﻿using Logic.ExtensionMethods;
using Logic.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class AdventBase : AdventInterface
    {
        public bool Active => Name.Length > 9;
        public int ID { get; set; }
        public string Title { get; set; }
        public string Name =>  $"Day {ID}: {Title}";
        public object Answer1 { get; set; }
        public object Answer2 { get; set; }
        public static bool Test { get; set; }
        public string PuzzleInput {get; set;}

        public void WriteDebugAnswers(object className)
        {
            Debug.WriteLine($"{className.GetType().Name} Antwoord 1: {Answer1}");
            Debug.WriteLine($"{className.GetType().Name} Antwoord 2: {Answer2}");
        }

        public void WriteDebug<T>(T write, object className)
        {
            Debug.WriteLine($"{className.GetType().Name}: {write}");
        }
             

        public virtual string[] Solution()
        {
            throw new NotImplementedException();
        }

        public virtual string Part1()
        {
            throw new NotImplementedException();
        }

        public virtual string Part2()
        {
            throw new NotImplementedException();
        }
    }
}

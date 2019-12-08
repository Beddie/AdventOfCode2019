using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day4 : AdventBase
    {
        public Day4(int day) : base(day)
        {
            Title = "Secure Container";
            //TestInput = "112233-670283"; //152085-670283
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day4;
            Day2Integers = PuzzleInput.Split(',').Cast<int>();
            SolutionPart1 = "";
            SolutionPart2 = "";
        }

        private IEnumerable<int> Day2Integers;

        public override async Task Part1()
        {
            var fromNumber = Convert.ToInt32(new string(PuzzleInput.Take(6).ToArray()));
            var toNumber = Convert.ToInt32(new string(PuzzleInput.Skip(7).Take(6).ToArray()));
            var numberRange = Enumerable.Range(fromNumber, toNumber - fromNumber);
            var count = 0;
            var validNumbers = new List<int>();
            foreach (var number in numberRange)
            {
                if (CheckNumberPart1(number))
                {
                    validNumbers.Add(number);
                }
            }

            ResultPart1 = validNumbers.Count.ToString();
        }
       
        private static bool CheckNumberPart1(int number)
        {
            //1806
            var numberstring = number.ToString();
            var isValid = true;
            var checkNumber = (byte)0;
            var hasDOuble = false;
            for (int i = 0; i < numberstring.Length; i++)
            {
                var curNum = Convert.ToByte(numberstring[i].ToString());
                if (checkNumber < curNum)
                {
                    checkNumber = curNum;
                }
                else if (checkNumber == curNum)
                {
                    if (!hasDOuble && Convert.ToByte(numberstring[i - 1].ToString()) == curNum) hasDOuble = true;
                }
                else
                {
                    isValid = false;
                    break;
                }
            }
            return isValid & hasDOuble;
        }

        public override async Task Part2()
        {
            var fromNumber = Convert.ToInt32(new string(PuzzleInput.Take(6).ToArray()));
            var toNumber = Convert.ToInt32(new string(PuzzleInput.Skip(7).Take(6).ToArray()));
            var numberRange = Enumerable.Range(fromNumber, toNumber - fromNumber);
            var count = 0;
            var validNumbers = new List<int>();
            foreach (var number in numberRange)
            {
                if (CheckNumberPart2(number))
                {
                    validNumbers.Add(number);
                }
            }
            ResultPart2 = validNumbers.Count.ToString(); ;
        }

        private static bool CheckNumberPart2(int number)
        {
            //1806
            var numberstring = number.ToString();
            var isValid = true;
            var checkNumber = (byte)0;
            var sameCountDictionary = new Dictionary<int, int>();
            for (int i = 0; i < numberstring.Length; i++)
            {
                var curNum = Convert.ToByte(numberstring[i].ToString());
                if (checkNumber < curNum)
                {
                    checkNumber = curNum;
                }
                else if (checkNumber == curNum)
                {
                    if (!sameCountDictionary.TryAdd(curNum, 2)){
                        sameCountDictionary[curNum] += 1;
                    }
                }
                else
                {
                    isValid = false;
                    break;
                }
            }
            return isValid & sameCountDictionary.ContainsValue(2);
        }

    }
}


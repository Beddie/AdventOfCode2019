using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day22 : AdventBase
    {
        public Day22(int day) : base(day)
        {
            Title = "Slam Shuffle";
            TestInput = @"deal with increment 4";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day22;
            SolutionPart1 = "";
            SolutionPart2 = "";
        }

        public class Deck
        {
            public Deck(int amountCards)
            {
                Cards = Enumerable.Range(0, amountCards).ToList();
            }
            private List<int> Cards { get; set; }
            public List<ShuffleAction> shuffleActions = new List<ShuffleAction>();
            public List<int> CutAction(List<int> cards, int param)
            {
                List<int> cardList;
                if (param < 0)
                {
                    cardList = cards.TakeLast(Math.Abs(param)).ToList();
                    cardList.AddRange(cards.Take(cards.Count + param));
                }
                else
                {
                    cardList = cards.Skip(param).ToList(); ;
                    cardList.AddRange(cards.Take(param));

                }
                return cardList;
            }

            public List<int> IncrementAction(List<int> cards, int param)
            {
                var newlist = new int[cards.Count];
                var cardIndex = 0;
                foreach (var card in cards)
                {
                    newlist[cardIndex] = card;
                    cardIndex += param;

                    if (cardIndex >= cards.Count)
                    {
                        cardIndex -= cards.Count;
                    }
                }

                return newlist.ToList();
            }

            public List<int> NewAction(List<int> cards)
            {
                cards.Reverse();
                return cards;
            }


            public delegate List<int> ShuffleAction(List<int> cards);

            public void ParsePuzzleInput(string puzzleinput)
            {
                var actions = puzzleinput.Split("\r\n");

                foreach (var action in actions)
                {
                    if (action.Contains("cut"))
                    {
                        var cutnumber = Convert.ToInt32(new string(action.Trim().SkipWhile(x => x != '-' && !char.IsNumber(x)).ToArray()));
                        shuffleActions.Add((x) => CutAction(x, cutnumber));
                    }
                    else if (action.Contains("increment"))
                    {
                        var incrementnumber = Convert.ToInt32(new string(action.Trim().SkipWhile(x => x != '-' && !char.IsNumber(x)).ToArray()));
                        shuffleActions.Add((x) => IncrementAction(x, incrementnumber));
                    }
                    else if (action.Contains("new"))
                    {
                        shuffleActions.Add(NewAction);
                    }
                }
            }

            internal void ExecuteActionList()
            {
                foreach (var action in shuffleActions)
                {
                    Cards = action(Cards);
                }

                Debug.WriteLine(string.Join(' ', Cards)); ;
            }

            internal int Get2019Value()
            {
                var index =  Cards.FindIndex(x=> x == 2019);
                var test = Cards[index];
                return index;
            }
        }

        public override async Task Part1()
        {
            var deck = new Deck(10007);
            deck.ParsePuzzleInput(PuzzleInput);
            deck.ExecuteActionList();
            var result = deck.Get2019Value();
            ResultPart1 = result.ToString();
        }

        public override async Task Part2() => ResultPart2 = "";

    }
}


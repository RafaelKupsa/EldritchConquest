using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    [Serializable]
    public class SacrificeGame : MiniGame
    {
        private Monster _monster;
        private int _triesLeft;
        private string[] _solution;
        private List<string> _guess;
	
        private List<string[]> _history = new List<string[]>();
	
        public SacrificeGame(Monster monster)
        {
            _monster = monster;
            _triesLeft = 15;
            _solution = _monster.GetSacrifice();
            _guess = new List<string>();
        }
	
        public bool IsCorrect()
        {	
            return _solution.SequenceEqual(_guess);
        }
	
        public bool IsFailed()
        {
            return _triesLeft <= 0;
        }        
        
        public void Fail()
        {
            _triesLeft = 0;
        }
	
        public override int GetScore()
        {
            return _triesLeft + 1;
        }

        public List<string> GetGuess()
        {
            return _guess;
        }

        public float GetTimeLeft()
        {
            return _triesLeft / 15f;
        }

        public List<List<string>> GetFeedback()
        {
            if (_guess.Count < 5)
            {
                return new List<List<string>> { new List<string> { "o", "o", "o", "?" } };
            }

            if (_guess.Count > 10)
            {
                return new List<List<string>> { new List<string> { "w", "t", "f", "?" } };
            } 
            
            var numCorrectItems = _solution.Distinct().Intersect(_guess.Distinct()).Count();
            var numCorrectPrecise = _solution.Distinct()
                .Select(organ => _solution.Count(x => x == organ) == _guess.Count(x => x == organ) ? 1 : 0).Sum();
            var numCorrectItemsWithoutPrecise = numCorrectItems - numCorrectPrecise;
            var numFalse = _guess.Distinct().Count() - numCorrectItems;
            var numMissing = _solution.Distinct().Count() - numCorrectItems - numFalse;

            var feedback = new List<List<string>>();
            if (numCorrectPrecise != 0)
                feedback = feedback.Concat(Enumerable.Range(0, numCorrectPrecise).Select(i => new List<string> { "ny", "o", "o", "o", "o", "m" })).ToList();
            if (numCorrectItemsWithoutPrecise != 0)
                feedback = feedback.Concat(Enumerable.Range(0, numCorrectItemsWithoutPrecise).Select(i => new List<string> { "ny", "u", "m" } )).ToList();
            if (numFalse != 0)
                feedback = feedback.Concat(Enumerable.Range(0, numFalse).Select(i => new List<string> { "b", "l", "a", "r", "'", "gh" })).ToList();
            if (numMissing != 0)
                feedback = feedback.Concat(Enumerable.Range(0, numMissing).Select(i => new List<string> { "ny", "o", "?" })).ToList();
            return feedback;
        }
        
        public void AddOrgan(string organ)
        {
            _guess.Add(organ);
            var guess = _guess.ToArray();
            Monster.SortSacrifice(guess);
            _guess = guess.ToList();
        }
	
        public void RemoveOrgan(string organ)
        {
            _guess.Remove(organ);
            var guess = _guess.ToArray();
            Monster.SortSacrifice(guess);
            _guess = guess.ToList();
        }
	
        public void Clear()
        {
            _guess = new List<string>();
        }
	
        public void DecreaseTries()
        {
            _triesLeft -= 1;
        }
    }
}
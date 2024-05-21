using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backend
{
    [Serializable]
    public class NameGame : MiniGame
    {
        private Monster _monster;
        private float _totalTime;
        private float _timeLeft;
        private string[] _solution;
        private string[] _guess;
        private List<int> _minusIndices;
        private Transcription _transcription;
        private List<Word> _book;

        private List<string[]> _history = new List<string[]>();
	
        public NameGame(Monster monster)
        {
            _monster = monster;
            var name = Language.WordsToLetterList(_monster.GetName());
            _minusIndices = name.Select(x => x.Count).ToList().GetRange(0, name.Count - 1);
            _solution = name.SelectMany(x => x).ToArray();
            _totalTime = Mathf.Lerp(25f, 100f, _solution.Length / 30f);
            _timeLeft = _totalTime;
            _guess = new string[_solution.Length];
            _transcription = new Transcription();
            _book = Language.GenerateBook(_monster.GetName());
        }

        public string[] GetGuess()
        {
            var result = new List<string>();
            for (var i = 0; i < _guess.Length; i++)
            {
                if (_minusIndices.Contains(i))
                {
                    result.Add("-");
                }
                result.Add(_guess[i]);
            }

            return result.ToArray();
        }

        public List<List<int>> GetNameOriginal()
        {
            return _transcription.Map(Language.WordsToLetterList(_monster.GetName()));
        }

        public List<List<int>> GetBookOriginal()
        {
            return _transcription.Map(Language.WordsToLetterList(_book));
        }

        public List<List<string>> GetBookTranscribed()
        {
            return Language.WordsToLetterList(_book);
        }

        public bool IsCorrect()
        {
            return _solution.SequenceEqual(_guess);
        }

        public void Fail()
        {
            _timeLeft = -1;
        }

        public bool IsFailed()
        {
            return _timeLeft < 0;
        }

        public float GetTimeLeft()
        {
            return _timeLeft / _totalTime;
        }
	
        public override int GetScore()
        {
            return Mathf.RoundToInt(Mathf.Lerp(0f, 15f, _timeLeft / _totalTime));
        }

        public string GetScript()
        {
            return _transcription.GetScript();
        }
	
        public void SetLetter(int index, string letter)
        {
            _guess[index] = letter;
        }
	
        public void RemoveLetter(int index)
        {
            _guess[index] = "";
        }
	
        public void Clear()
        {
            _guess = new string[_solution.Length];
        }
	
        public void DecreaseTime(float number)
        {
            _timeLeft -= number;
        }
    }

    [Serializable]
    public class Transcription
    {
        private static readonly string[] LatinLetters =
        {
            "m", "n", "ny", "ng", "p", "t", "c", "k",
            "b", "d", "g", "f", "v", "s", "z", "h",
            "r", "l", "ly", "y", "w", "'", "mh", "nh", 
            "ngh", "ph", "th", "ch", "kh", "bh", "dh", 
            "gh", "fh", "sh", "zh", "rh", "lh", "'h",
            "x", "a", "e", "i", "o", "u", "aa", "ee", 
            "ii", "oo", "uu", "ai", "au", "oi", "ui"
        };

        private static readonly string[] Scripts =
        {
            "hieroglyphs",
            "abstract",
            "triangular",
            "tech"
        };

        private string _script;
        private Dictionary<string, int> _mapping;

        public Transcription()
        {
            _script = Scripts.Choice();

            var symbolIndices = Enumerable.Range(0, 100).ToList();
            symbolIndices.Shuffle();
            var indices = symbolIndices.GetRange(0, LatinLetters.Length);
            _mapping = LatinLetters.Zip(indices, (l, s) => new { l, s })
                .ToDictionary(pair => pair.l, pair => pair.s);
        }

        public string GetScript()
        {
            return _script;
        }

        public List<List<int>> Map(List<List<string>> letterList)
        {
            return letterList.Select(word => word.Select(l => _mapping[l]).ToList()).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Backend
{
    [Serializable]
    public class Altar
    {
        private Monster _monster = Monster.Generate();
        private Dictionary<string, int> _scores = new Dictionary<string, int>
        {
            {"name-game", -1},
            {"icon-game", -1},
            {"sacrifice-game", -1}
        };

        private MiniGame _miniGame;
        private bool _failed;

        public bool IsComplete()
        {
            return _scores.Values.All(v => v != -1);
        }

        public bool IsFailed()
        {
            return _failed;
        }

        public void Fail()
        {
            _failed = true;
        }

        public void InitMiniGame(string game)
        {
            _miniGame = game switch
            {
                "name-game" => new NameGame(_monster),
                "icon-game" => new IconGame(_monster),
                "sacrifice-game" => new SacrificeGame(_monster),
                _ => _miniGame
            };
        }

        public bool IsMiniGameComplete(string game)
        {
            return _scores[game] != -1;
        }

        public MiniGame GetMiniGame()
        {
            return _miniGame;
        }

        public string[] GetSacrifice()
        {
            return _monster.GetSacrifice();
        }

        public string GetShape()
        {
            return _monster.GetShape();
        }

        public string[] GetFeatures()
        {
            return _monster.GetFeatures();
        }

        public int GetScore()
        {
            return _scores.Values.Sum();
        }

        public List<string> GetNameAsLetterList()
        {
            var letterList = Language.WordsToLetterList(_monster.GetName());
            for (var i = 0; i < letterList.Count - 1; i++)
            {
                letterList[i].Add("-");
            }
            return letterList.SelectMany(x => x).ToList();
        }

        public string GetNameAsString()
        {
            return Language.WordsToString(_monster.GetName(), "-", 1f);
        }

        public string GetTitle()
        {
            return _monster.GetTitle();
        }

        public void CompleteGame(string game)
        {
            _scores[game] = _miniGame.GetScore();
        }

        public void Cheat(string game)
        {
            _scores[game] = Random.Range(0, 16);
        }

        public string TempMonsterAppearance()
        {
            return _monster.ToString();
        }
    }
}
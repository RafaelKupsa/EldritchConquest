using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    [Serializable]
    public class IconGame : MiniGame
    {
        private Monster _monster;
        private float _totalTime;
        private float _timeLeft;
        private Monster _guess;
	
        private List<Monster> _history = new List<Monster>();
	
        public IconGame(Monster monster)
        {
            _monster = monster;
            _totalTime = Mathf.Lerp(3f, 5f, _monster.GetComplexity());
            _timeLeft = _totalTime;
            _guess = new Monster();
        }
	
        public bool IsCorrect()
        {
            return Equals(_monster, _guess);
        }
	
        public bool IsFailed()
        {
            return _timeLeft < 0;
        }
        
        public void Fail()
        {
            _timeLeft = -1;
        }
	
        public override int GetScore()
        {
            return Mathf.RoundToInt(Mathf.Lerp(0f, 15f, _timeLeft / _totalTime));
        }

        public float GetTimeLeft()
        {
            return _timeLeft / _totalTime;
        }

        public string GetShape()
        {
            return _guess.GetShape();
        }
	
        public void SetShape(string shape)
        {
            _guess.SetShape(shape);
        }

        public string GetSkin()
        {
            return _guess.GetSkin();
        }
	
        public void SetSkin(string skin)
        {
            _guess.SetSkin(skin);
        }

        public string[] GetFeatures()
        {
            return _guess.GetFeatures();
        }

        public bool CanSetFeature(int index, string feature)
        {
            return _guess.CanSetFeature(index, feature);
        }
	
        public void SetFeature(int index, string feature)
        {
            _guess.SetFeature(index, feature);
        }
	
        public void RemoveFeature(int index)
        {
            _guess.RemoveFeature(index);
        }
	
        public void Clear()
        {
            _guess = new Monster();
        }
	
        public void DecreaseTime(float number)
        {
            _timeLeft -= number;
        }
    }
}
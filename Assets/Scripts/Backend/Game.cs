using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Backend
{
    [Serializable]
    public class Game
    {
        public static string SavePath = Path.Combine(Application.persistentDataPath, "save.dat");
        
        private Vec2 _size;
        private float _radius;
        private Vec2 _currentPin;
        private Altar _currentAltar;
        private Dictionary<Vec2, Altar> _completedAltars = new Dictionary<Vec2, Altar>();
        private List<Blob> _blobs = new List<Blob>();
        private float _completion;
        private bool[,] _map;
        private float _threshold = 1f;

        public Game(int width = 1499, int height = 764)
        {
            _size = new Vec2(width, height);
            _radius = width / (2f * (float)Math.PI);

            _map = new bool[width, height];
        }

        public static Game Load()
        {
            if (File.Exists(SavePath))
            {
                var bf = new BinaryFormatter();
                var file = File.Open(SavePath, FileMode.Open);
                var game = (Game)bf.Deserialize(file);
                file.Close();
                Debug.Log("Game data loaded!");
                return game;
            }
            
            Debug.Log("There is no save data!");
            return null;
        }

        public void Save()
        {
            var bf = new BinaryFormatter();
            var file = File.Create(SavePath);
            bf.Serialize(file, this);
            file.Close();
            Debug.Log("Game data saved!");
        }

        public bool IsComplete()
        {
            return _completion >= 1f;
        }

        public float GetCompletion()
        {
            return _completion;
        }

        public Vec2 GetSize()
        {
            return _size;
        }

        public bool HasPin()
        {
            return !(_currentPin is null);
        }

        public bool IsPinComplete()
        {
            return _completedAltars.Keys.Contains(_currentPin);
        }

        public bool IsPinEmpty()
        {
            return _currentAltar is null;
        }

        public Vec2 GetPin()
        {
            return _currentPin;
        }

        public Vec2[] GetCompletedAltars()
        {
            return _completedAltars.Keys.ToArray();
        }

        public bool[,] GetMap()
        {
            return _map;
        }

        public void SetPin(Vec2 coords)
        {
            _currentPin = coords;
        }

        public void InitAltar()
        {
            _currentAltar = _completedAltars.Keys.Contains(_currentPin) ? _completedAltars[_currentPin] : new Altar();
        }

        public void FailAltar()
        {
            _currentAltar = null;
        }

        public void DiscardPin()
        {
            _currentPin = null;
        }

        public bool CanSetPin(Vec2 coords)
        {
            if (!(_currentPin is null) && !_completedAltars.ContainsKey(_currentPin)) return false;
            
            if (!(0 < coords.x) || !(coords.x <= _size.x) || !(0f < coords.y) || !(coords.y <= _size.y))
                return false;

            return !_completedAltars.ContainsKey(coords);

        }

        public Altar GetAltar()
        {
            return _currentAltar;
        }

        public bool IsAltarComplete()
        {
            return _currentAltar.IsComplete();
        }

        public void CompleteAltar()
        {
            var score = _currentAltar.GetScore();
            var blob = Blob.Generate(_currentPin, score, _size);
            _AddBlob(blob);
            _UpdateCompletion();
            
            _completedAltars[_currentPin] = _currentAltar;
            _currentPin = null;
            _currentAltar = null;
        }

        public void Cheat()
        {
            _currentAltar.Cheat("name-game");
            _currentAltar.Cheat("icon-game");
            _currentAltar.Cheat("sacrifice-game");
        }

        private void _AddBlob(Blob blob)
        {
            _blobs.Add(blob);

            for (var i = 1; i < _size.x + 1; i++)
            {
                for (var j = 1; j < _size.y + 1; j++)
                {
                    if (_map[i - 1, j - 1]) continue;
                    
                    var pos = new Vec2Float(i, j).ToSpherical(_size);
                    var dd = blob.metaBalls.Select(mb => mb.HaversineDistance(pos, _radius));
                    var ee = Enumerable.Zip(dd, blob.metaBalls, (d, ball) => d == 0 ? _threshold : ball.radius / d / 5);
                    _map[i - 1, j - 1] = ee.Sum() >= _threshold;
                }
            }
        }

        private void _UpdateCompletion()
        {
            _completion = _map.Cast<bool>().Select(x => x ? 1 : 0).Sum() / (float)(_size.x * _size.y);
        }
    }


    [Serializable]
    public class Blob
    {
        public MetaBall[] metaBalls;

        public Blob(MetaBall[] metaBalls)
        {
            this.metaBalls = metaBalls;
        }
        
        public static Blob Generate(Vec2 at, int size, Vec2 bounds)
        {
            size += 5;
            var numCircles = Random.Range(3, 6);
            var area = Mathf.Log(size, 2) * ((bounds.x + bounds.y) / 12f);
            var variance = Mathf.RoundToInt(area / numCircles);

            var metaBalls = new MetaBall[numCircles];
            for (var i = 0; i < numCircles; i++)
            {
                var x = at.x + variance * (Random.value - 0.5f);
                var y = at.y + variance * (Random.value - 0.5f);
                var hv = new Vec2Float(x, y).ToSpherical(bounds);
                var r = Math.Max(3, Random.Range(variance - 5, variance + 5));

                metaBalls[i] = new MetaBall(hv, r);
            }

            return new Blob(metaBalls);
        }
    }


    [Serializable]
    public class MetaBall
    {
        public Vec2Float position;
        public float radius;

        public MetaBall(Vec2Float position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }

        public float HaversineDistance(Vec2Float other, float R)
        {
            var dx = other.x - position.x;
            var dy = other.y - position.y;

            var a = Math.Sin(dy / 2f) * Math.Sin(dy / 2f) +
                    Math.Cos(position.y) * Math.Cos(other.y) * Math.Sin(dx / 2f) * Math.Sin(dx / 2f);

            return R * 2f * (float)Math.Asin(Math.Sqrt(a));
        }
    }


    [Serializable]
    public class Vec2
    {
        public int x;
        public int y;

        public Vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vec2(Vector3 vec) => new Vec2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        public static explicit operator Vector3(Vec2 vec) => new Vector3(vec.x, vec.y, 0f);

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    [Serializable]
    public class Vec2Float
    {
        public float x;
        public float y;

        public Vec2Float(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public Vec2Float ToSpherical(Vec2 bounds)
        {
            var h = x / (bounds.x / (2f * Math.PI)) - Math.PI;
            var v = -(y / (bounds.y / Math.PI) - Math.PI / 2f);
            return new Vec2Float((float)h, (float)v);
        }
    }
}
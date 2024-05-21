using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

namespace Backend
{
	
	[Serializable]
    public class Monster
    {
	    private static readonly string[] Titles = {
		    "", "", "", "", "", "", "", "", "", "",
		    "the Almighty", "King", "Queen", "Prince", "Princess",
		    "the Undying", "the Priest", "the Horrifying", "the Terrifying",
		    "the Mighty", "the Great", "the Awesome", "the All-powerful",
		    "the Prophet", "the Monstrous", "the Harrowing", "the Horrible",
		    "the Terrible", "the Dreadful", "the Hideous", "General", "Lord",
		    "the Honorable", "the Venerable"};
		private static readonly string[] Organs = { "intestines", "brain", "heart",  "stomach", "liver", "kidney"};

		private static readonly string[] Skins = {"none", "scaly", "fleshy", "holes", "gooey", "hairy"};
		private static readonly string[] Extremities = {"arm", "tentacle", "claw", "wing", "bugleg", "gills"};
		private static readonly string[] Eyes = {"humaneye", "octopuseye", "goateye", "bugeye", "lizardeye", "stalkeye"};
		private static readonly string[] Mouths = {"humanmouth", "sealampreymouth", "birdbeak", "cuttlefishmouth", "seaurchinmouth", "anglerfishmouth"};
		
		private static readonly string[] Long = {"arm", "tentacle", "claw", "wing", "bugleg", "gills", "stalkeye"};
		private static readonly string[] Short = {"humaneye", "octopuseye", "goateye", "bugeye", "lizardeye", "humanmouth", "sealampreymouth", "birdbeak", "cuttlefishmouth", "seaurchinmouth", "anglerfishmouth"};
		private static readonly string[] ShortEyes = {"humaneye", "octopuseye", "goateye", "bugeye", "lizardeye"};
		private static readonly string[] LongOrEyes = {"humaneye", "octopuseye", "goateye", "bugeye", "lizardeye", "stalkeye", "arm", "tentacle", "claw", "wing", "bugleg", "gills"};
		private static readonly string[] Legs = {"arm", "tentacle", "bugleg", "gills"};
		
		private static readonly string[][] SphereFeatures = {Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, ShortEyes, ShortEyes, ShortEyes, ShortEyes, Short};
		private static readonly string[][] WormFeatures = {ShortEyes, ShortEyes, ShortEyes, ShortEyes, ShortEyes, ShortEyes, Long, Long, Long, Long, Long, Short};
		private static readonly string[][] SkullFeatures = {Long, Long, Long, Long, Long, Long, Short, Short, Short, Mouths, ShortEyes, ShortEyes};
		private static readonly string[][] PeanutFeatures = {Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Short, ShortEyes, Short};
		private static readonly string[][] StarFeatures = {Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Long, Short, Short, Short};
		private static readonly string[][] BlobFeatures = {Short, Short, Short, Short, Short, Short, Short, Short};
		private static readonly Dictionary<string, string[][]> Shapes = new Dictionary<string, string[][]>{
			{"sphere", SphereFeatures},
			{"worm", WormFeatures},
			{"skull", SkullFeatures},
			{"peanut", PeanutFeatures},
			{"star", StarFeatures},
			{"blob", BlobFeatures}
		};
		
		private string _title;
		private List<Word> _name;
		private string _shape;
		private string _skin;
		private string[] _features;
		private string[] _sacrifice;

		public static void SortSacrifice(string[] sacrifice)
		{
			Array.Sort(sacrifice, (m, n) => Array.IndexOf(Organs, m) - Array.IndexOf(Organs, n));
		}
		
		public static Monster Generate()
		{
			var monster = new Monster
			{
				_title = Titles.Choice(),
				_name = Language.GenerateName(),
				_shape = Shapes.Keys.ToList()[Random.Range(0, Shapes.Count)],
				_skin = Skins[Random.Range(0, Skins.Length)]
			};
			monster._features = new string[Shapes[monster._shape].Length];
			var features = new List<string>();
			for (var i = 0; i < monster._features.Length; i++)
			{
				var slots = Shapes[monster._shape][i];
				var options = features.Where(feature => slots.Contains(feature)).ToList().Repeat(2).ToList();
				var nulls = Enumerable.Repeat("", Mathf.RoundToInt(options.Count * 1.5f));
				var choice = slots.Concat(options).Concat(nulls).ToList().Choice();
				
				features.Add(choice == "" ? null : choice);
				monster._features[i] = choice == "" ? null : choice;
			}

			var numOrgans = Random.Range(5, 11);
			var sacrifice = Enumerable.Range(0, numOrgans).Select(i => Organs[Random.Range(0, Organs.Length)]).ToArray();
			SortSacrifice(sacrifice);
			monster._sacrifice = sacrifice;
			
			return monster;
		}

		public bool IsEmpty(){
			return _shape.Length == 0 && _skin.Length == 0 && _features == null;
		}

		public List<Word> GetName()
		{
			return _name;
		}

		public string GetTitle()
		{
			return _title;
		}

		public string[] GetSacrifice()
		{
			return _sacrifice;
		}

		public string GetShape()
		{
			return _shape;
		}

		public void SetShape(string shape)
		{
			if (_shape == shape) return;
			_shape = shape;
			_features = new string[Shapes[_shape].Length];
		}

		public string GetSkin()
		{
			return _skin;
		}

		public void SetSkin(string skin)
		{
			_skin = skin;
		}

		public string[] GetFeatures()
		{
			return _features;
		}

		public bool CanSetFeature(int index, string feature)
		{
			return Shapes[_shape][index].Contains(feature);
		}

		public void SetFeature(int index, string feature)
		{
			_features[index] = feature;
		}

		public void RemoveFeature(int index)
		{
			_features[index] = null;
		}

		public int GetComplexity()
		{
			return _features.Count(x => !(x is null));
		}
		
		public override bool Equals(object obj)
	    {
		    if (!(obj is Monster other))
			{
				return false;
			}

		    return _shape == other._shape && _features.SequenceEqual(other._features);
		}
		
		public override int GetHashCode()
		{
			return (int)Math.Pow(_shape.GetHashCode() * _skin.GetHashCode(), _features.GetHashCode());
		}

		public override string ToString()
		{
			return $"Title: {_title}\n" +
			       $"Name: {Language.WordsToString(_name, "-", 1f)}\n" +
			       $"Shape: {_shape}\n" +
			       $"Skin: {_skin}\n" +
			       "Features: " + string.Join(", ", _features) + "\n" +
			       "Likes: " + string.Join(", ", _sacrifice);
		}
	}
    
    
}
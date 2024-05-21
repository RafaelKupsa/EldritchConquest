using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Backend
{
public static class DeprLanguage
{
	private static readonly string[][] Onsets = {
		new string[]{}, new[]{"m"}, new[]{"n"}, new[]{"ny"}, new[]{"ng"},
        new[]{"m", "m"}, new[]{"n", "n"},
        new[]{"p"}, new[]{"t"}, new[]{"c"}, new[]{"k"},
        new[]{"p", "p"}, new[]{"t", "t"}, new[]{"c", "c"}, new[]{"k", "k"},
        new[]{"b"}, new[]{"d"}, new[]{"g"},
        new[]{"b", "b"}, new[]{"d", "d"}, new[]{"g", "g"},
        new[]{"f"}, new[]{"v"}, new[]{"s"}, new[]{"z"}, new[]{"h"},
        new[]{"f", "f"}, new[]{"s", "s"}, new[]{"z", "z"},
        new[]{"r"}, new[]{"l"}, new[]{"ly"}, new[]{"y"}, new[]{"w"}, new[]{"'"},
        new[]{"r", "r"}, new[]{"l", "l"},
        new[]{"mh"}, new[]{"nh"}, new[]{"ngh"},
        new[]{"ph"}, new[]{"th"}, new[]{"ch"}, new[]{"kh"},
        new[]{"bh"}, new[]{"dh"}, new[]{"gh"},
        new[]{"fh"}, new[]{"sh"}, new[]{"zh"},
        new[]{"rh"}, new[]{"lh"}, new[]{"'h"},
        new[]{"s", "t"}, new[]{"f", "t"}, new[]{"sh", "t"}, new[]{"fh", "t"},
        new[]{"s", "n"}, new[]{"g", "n"}, new[]{"k", "n"}, new[]{"f", "n"}, new[]{"sh", "n"}, new[]{"fh", "n"},
        new[]{"x"}, new[]{"p", "s"}, new[]{"t", "s"},
        new[]{"p", "l"}, new[]{"t", "l"}, new[]{"k", "l"},
        new[]{"b", "l"}, new[]{"d", "l"}, new[]{"g", "l"},
        new[]{"ph", "l"}, new[]{"th", "l"}, new[]{"kh", "l"},
        new[]{"bh", "l"}, new[]{"dh", "l"}, new[]{"gh", "l"},
        new[]{"p", "r"}, new[]{"t", "r"}, new[]{"k", "r"},
        new[]{"b", "r"}, new[]{"d", "r"}, new[]{"g", "r"},
        new[]{"ph", "r"}, new[]{"th", "r"}, new[]{"kh", "r"},
        new[]{"bh", "r"}, new[]{"dh", "r"}, new[]{"gh", "r"},
        new[]{"p", "th"}, new[]{"c", "th"}, new[]{"k", "th"}
	};
	private static readonly float[] POnsets = new[]{
		5, 5, 5, 5, 4,
        3, 3,
        5, 5, 5, 5,
        3, 3, 3, 3,
        5, 5, 6,
        3, 3, 6,
        4, 2, 5, 4, 6,
        3, 3, 4,
        5, 5, 5, 3, 2, 2,
        3, 3,
        3, 2, 1,
        4, 3, 1, 1,
        1, 1, 5,
        3, 6, 6,
        4, 4, 4,
        1, 1, 1, 1,
        1, 1, 1, 1, 1, 1,
        3, 1, 1,
        1, 1, 1,
        1, 1, 1,
        1, 1, 1,
        1, 1, 1,
        1, 1, 1,
        1, 1, 1,
        1, 1, 1,
        1, 1, 1,
        3, 3, 3
	}.Softmax();
	private static readonly string[][] Nuclei = {
        new[]{"a"}, new[]{"e"}, new[]{"i"}, new[]{"o"}, new[]{"u"},
        new[]{"aa"}, new[]{"ee"}, new[]{"ii"}, new[]{"oo"}, new[]{"uu"},
        new[]{"ai"}, new[]{"au"}, new[]{"ui"}, new[]{"oi"},
        new[]{"y"}, new[]{"w"}, new[]{"'"}, new[]{"m"}, new[]{"n"}, new[]{"ng"}, new[]{"l"}, new[]{"r"}
	};
    private static readonly float[] PNuclei = new[]{
        5, 5, 5, 5, 5,
        3, 3, 2, 2, 2,
        2, 2, 3, 2,
        3, 3, 5, 1, 1, 1, 1, 1
    }.Softmax();
    private static readonly string[][] NucleiStressed = {
        new[]{"a"}, new[]{"e"}, new[]{"i"}, new[]{"o"}, new[]{"u"},
        new[]{"aa"}, new[]{"ee"}, new[]{"ii"}, new[]{"oo"}, new[]{"uu"},
        new[]{"ai"}, new[]{"au"}, new[]{"ui"}, new[]{"oi"}
	};
    private static readonly float[] PNucleiStressed = new[]{
        5, 5, 5, 5, 5,
        3, 3, 2, 2, 2,
        2, 2, 3, 2
    }.Softmax();
    private static readonly string[][] Codas = {new string[]{}, new[]{"r"}, new[]{"l"}};
    private static readonly float[] PCodas = new[]{5, 2, 2}.Softmax();
    private static readonly string[][] Finals = {
        new[]{"m"}, new[]{"n"}, new[]{"ng"},
        new[]{"m", "m"}, new[]{"n", "n"},
        new[]{"p"}, new[]{"t"}, new[]{"c"}, new[]{"k"},
        new[]{"p", "p"}, new[]{"t", "t"}, new[]{"c", "c"}, new[]{"k", "k"},
        new[]{"b"}, new[]{"d"}, new[]{"g"},
        new[]{"b", "b"}, new[]{"d", "d"}, new[]{"g", "g"},
        new[]{"f"}, new[]{"v"}, new[]{"s"}, new[]{"z"}, new[]{"h"},
        new[]{"f", "f"}, new[]{"s", "s"}, new[]{"z", "z"},
        new[]{"r"}, new[]{"l"},
        new[]{"mh"}, new[]{"nh"}, new[]{"ngh"},
        new[]{"ph"}, new[]{"th"}, new[]{"ch"}, new[]{"kh"},
        new[]{"bh"}, new[]{"dh"}, new[]{"gh"},
        new[]{"fh"}, new[]{"sh"}, new[]{"zh"},
        new[]{"rh"}, new[]{"lh"}, new[]{"'h"},
        new[]{"s", "t"}, new[]{"f", "t"}, new[]{"sh", "t"}, new[]{"fh", "t"},
        new[]{"x"},
        new[]{"p", "th"}, new[]{"c", "th"}, new[]{"k", "th"}
	};
    private static readonly float[] PFinals = new[]{
        2, 2,
        5, 5, 5, 5,
        2, 2, 2, 2,
        5, 5, 6,
        2, 2, 2,
        4, 2, 5, 5, 6,
        2, 2, 2,
        3, 3,
        3, 2, 1,
        4, 6, 1, 1,
        1, 1, 4,
        3, 6, 6,
        4, 4, 4,
        1, 1, 1, 1,
        3,
        1, 1, 1
    }.Softmax();
    
    private static Tuple<string[][], float[]> Filter(IEnumerable<string[]> letters, IEnumerable<float> probabilities, string letter)
    {
	    var zipped = letters.Zip(probabilities, (l, p) => new {l, p}).Where(pair => pair.l.Contains(letter)).ToArray();
	    var newLetters = zipped.Select(pair => pair.l).ToArray();
	    var newP = zipped.Select(pair => pair.p).ToArray().Softmax();
	    return Tuple.Create(newLetters, newP);
    }
    
    public static List<List<string[]>> GenerateWord(int numSyl, bool forceFinal = false, string wLetter = "")
	{
		var word = new List<List<string[]>>();
		var stressed = numSyl == 1 || UnityEngine.Random.value < 0.5;
		var onsetReduplicationChance = 0.25f;
		
		var (onsetsWLetter, pOnsetsWLetter) = Filter(Onsets, POnsets, wLetter);
		var (nucleiWLetter, pNucleiWLetter) = Filter(Nuclei, PNuclei, wLetter);
		var (codasWLetter, pCodasWLetter) = Filter(Codas, PCodas, wLetter);
		
		for (var i = 0; i < numSyl; i++)
		{
			string[] onset;
			string[] nucleus;
			string[] coda;
			
			
			// ONSET
			if (wLetter != "" && Onsets.Any(x => x.Contains(wLetter)) 
			                       && UnityEngine.Random.value < (numSyl == 0 ? 1 : Mathf.Lerp(0.5f, 1f, i / (numSyl - 1f))))
			{
				onset = onsetsWLetter.Choice(pOnsetsWLetter);
				wLetter = "";
			}
			else if (i != 0 && UnityEngine.Random.value < onsetReduplicationChance)
			{
				onset = word.Choice()[0];
				onsetReduplicationChance -= 0.05f;
			}
			else
			{
				onset = Onsets.Choice(POnsets);
			}
			
			// NUCLEUS
			if (stressed)
			{
				if (wLetter != "" && NucleiStressed.Any(x => x.Contains(wLetter)) 
				                  && UnityEngine.Random.value < (numSyl == 0 ? 1 : Mathf.Lerp(0.5f, 1f, i / (numSyl - 1f))))
				{
					nucleus = nucleiWLetter.Choice(pNucleiWLetter);
					wLetter = "";
				}
				else if (i != 0 && UnityEngine.Random.value < 0.2f)
				{
					var possNucleus = word.Choice()[1];
					nucleus = NucleiStressed.Any(x => x.SequenceEqual(possNucleus)) 
						? possNucleus 
						: NucleiStressed.Choice(PNucleiStressed);
				}
				else
				{
					nucleus = NucleiStressed.Choice(PNucleiStressed);
				}
			}
			else
			{
				if (wLetter != "" && Nuclei.Any(x => x.Contains(wLetter)) 
				                  && UnityEngine.Random.value < (numSyl == 0 ? 1 : Mathf.Lerp(0.5f, 1f, i / (numSyl - 1f))))
				{
					nucleus = nucleiWLetter.Choice(pNucleiWLetter);
					wLetter = "";
				}
				else if (i != 0 && UnityEngine.Random.value < 0.2f)
				{
					nucleus = word.Choice()[1];
				}
				else
				{
					nucleus = Nuclei.Choice(PNuclei);
				}
			}
			
			// CODA
			if (wLetter != "" && Codas.Any(x => x.Contains(wLetter)) 
			                  && UnityEngine.Random.value < (numSyl == 0 ? 1 : Mathf.Lerp(0.5f, 1f, i / (numSyl - 1f))))
			{
				coda = codasWLetter.Choice(pCodasWLetter);
				wLetter = "";
			}
			else
			{
				coda = Codas.Choice(PCodas);
			}

			word.Add(new List<string[]>{onset, nucleus, coda});
			stressed = !stressed;
		}
		
		// FINAL
		if (forceFinal || UnityEngine.Random.value < 0.5f)
		{
			var randomOnset = word.Choice()[0];
			if (Finals.Any(x => randomOnset.SequenceEqual(x)) 
			    && UnityEngine.Random.value < onsetReduplicationChance + 0.25f)
			{
				word.Add(new List<string[]>{randomOnset});
			}
			else
			{
				var final = Finals.Choice(PFinals);
				word.Add(new List<string[]>{final});
			}
		}
		
		// APOSTROPHE
		foreach (var syl in word)
		{
			if (syl.Count > 1 && UnityEngine.Random.value < 0.2)
			{
				syl[0] = syl[0].ToList().Append("'").ToArray();
			}
		}

		// CLEANING
		foreach (var syl in word)
		{
			if (syl[0].Length == 0)
			{
				syl.RemoveAt(0);
			}

			if (syl.Count > 1 && syl[2].Length == 0)
			{
				syl.RemoveAt(2);
			}
		}
		
		
		var doubles = new[] { "y", "w", "'" };
		for (var i = 1; i < word.Count; i++)
		{
			var pEnd = word[i - 1][word[i - 1].Count - 1];
			var nStart = word[i][0];

			if (pEnd[pEnd.Length - 1] == nStart[0] && doubles.Contains(pEnd[pEnd.Length - 1]))
			{
				nStart.CopyTo(word[i][0], 1);
			}
		}
		
		if (word[0][0].Length > 1 && word[0][0][0] == word[0][0][1])
		{
			word[0][0].CopyTo(word[0][0], 1);
		}

		if (word[0][0][0] == "'")
		{
			if (word[0][0].Length == 1)
			{
				word[0].RemoveAt(0);
			}
			else
			{
				word[0][0].CopyTo(word[0][0], 1);
			}
		}

		return word;
	}

    public static List<List<List<string[]>>> GenerateName()
    {
	    while (true)
	    {
		    var p = UnityEngine.Random.value;
		    List<List<List<string[]>>> name;

		    if (p < 0.375f)
		    {
			    name = new List<List<List<string[]>>> { GenerateWord(1), GenerateWord(UnityEngine.Random.Range(2, 4), true) };
		    }
		    else if (p < 0.575f)
		    {
			    name = new List<List<List<string[]>>> { GenerateWord(UnityEngine.Random.Range(2, 4)), GenerateWord(1, true) };
		    }
		    else
		    {
			    name = new List<List<List<string[]>>> { GenerateWord(UnityEngine.Random.Range(2, 4), true) };
		    }

		    if (NameToString(name).Length <= 3) continue;
		    return name;
	    }
    }

    public static string NameToString(List<List<List<string[]>>> name)
    {
	    return string.Join("-", name
		    .Select(word => word
			    .Select(syl => syl
				    .Select(@let => string.Join("", @let)))
			    .Select(s => string.Join("", s)))
		    .Select(w => string.Join("", w).Capitalize()));
    }

    public static List<List<string>> WordsToSyllableList(List<List<List<string[]>>> words)
    {
	    var output = new List<List<string>>();
	    foreach (var word in words)
	    {
		    var w = new List<string>();
		    foreach (var sylPart in word.SelectMany(syl => syl))
		    {
			    w.AddRange(sylPart);
		    }
		    output.Add(w);
	    }

	    return output;
    }

    public static List<string> WordsToLetterList(List<List<List<string[]>>> words)
    {
	    var output = new List<string>();
	    foreach (var sylPart in words.SelectMany(word => word.SelectMany(syllable => syllable)))
	    {
		    output.AddRange(sylPart);
	    }

	    return output;
    }

    public static List<List<List<string[]>>> GenerateBook(List<List<List<string[]>>> monsterName)
    {
	    var syllablesPerWord = Util.RandomChunks(50, new List<int> { 1, 2, 3, 4 }, new List<int> { 5, 4, 4, 4 }.Softmax()).ToList();
	    var letters = monsterName.SelectMany(x => x.SelectMany(y => y.SelectMany(z => z))).Distinct().ToList();
	    var lettersPerWord = letters.Concat(Enumerable.Range(0, syllablesPerWord.Count - letters.Count).Select(i => "")).ToList();
	    lettersPerWord.Shuffle();

	    return syllablesPerWord.Zip(lettersPerWord, (numSyl, letter) => GenerateWord(numSyl, false, letter)).ToList();
    }

    public static string BookToString(List<List<List<string[]>>> scroll)
    {
	    return string.Join(" ", scroll
		    .Select(word => word
			    .Select(syl => syl
				    .Select(@let => string.Join("", @let)))
			    .Select(s => string.Join("", s)))
		    .Select(w => Random.value < 0.1 ? string.Join("", w).Capitalize() : string.Join("", w))).Capitalize();
    }
}

public static class Language
{
	private static readonly Segment[] Onsets = {
		new Segment(), 
		new Segment("m"), 
		new Segment("n"), 
		new Segment("ny"), 
		new Segment("ng"),
        new Segment("m", "m"), 
        new Segment("n", "n"),
        new Segment("p"), 
        new Segment("t"), 
        new Segment("c"), 
        new Segment("k"),
        new Segment("p", "p"), 
        new Segment("t", "t"), 
        new Segment("c", "c"), 
        new Segment("k", "k"),
        new Segment("b"), 
        new Segment("d"), 
        new Segment("g"),
        new Segment("b", "b"), 
        new Segment("d", "d"), 
        new Segment("g", "g"),
        new Segment("f"), 
        new Segment("v"), 
        new Segment("s"), 
        new Segment("z"), 
        new Segment("h"),
        new Segment("f", "f"), 
        new Segment("s", "s"), 
        new Segment("z", "z"),
        new Segment("r"),
        new Segment("l"), 
        new Segment("ly"), 
        new Segment("y"), 
        new Segment("w"), 
        new Segment("'"),
        new Segment("r", "r"), 
        new Segment("l", "l"),
        new Segment("mh"), 
        new Segment("nh"),
        new Segment("ngh"),
        new Segment("ph"), 
        new Segment("th"), 
        new Segment("ch"), 
        new Segment("kh"),
        new Segment("bh"),
        new Segment("dh"), 
        new Segment("gh"),
        new Segment("fh"),
        new Segment("sh"), 
        new Segment("zh"),
        new Segment("rh"), 
        new Segment("lh"), 
        new Segment("'h"),
        new Segment("s", "t"), 
        new Segment("f", "t"), 
        new Segment("sh", "t"), 
        new Segment("fh", "t"),
        new Segment("s", "n"),
        new Segment("g", "n"), 
        new Segment("k", "n"), 
        new Segment("f", "n"),
        new Segment("sh", "n"), 
        new Segment("fh", "n"),
        new Segment("x"), 
        new Segment("p", "s"),
        new Segment("t", "s"),
        new Segment("p", "l"), 
        new Segment("t", "l"),
        new Segment("k", "l"),
        new Segment("b", "l"),
        new Segment("d", "l"), 
        new Segment("g", "l"),
        new Segment("ph", "l"),
        new Segment("th", "l"),
        new Segment("kh", "l"),
        new Segment("bh", "l"), 
        new Segment("dh", "l"), 
        new Segment("gh", "l"),
        new Segment("p", "r"), 
        new Segment("t", "r"), 
        new Segment("k", "r"),
        new Segment("b", "r"), 
        new Segment("d", "r"), 
        new Segment("g", "r"),
        new Segment("ph", "r"), 
        new Segment("th", "r"),
        new Segment("kh", "r"),
        new Segment("bh", "r"), 
        new Segment("dh", "r"), 
        new Segment("gh", "r"),
        new Segment("p", "th"),
        new Segment("c", "th"), 
        new Segment("k", "th")
	};
	private static readonly float[] POnsets = new[]{
		5, 5, 5, 5, 4, 3, 3, 5, 5, 5, 5, 3, 3, 3, 3, 5, 5, 6,
        3, 3, 6, 4, 2, 5, 4, 6, 3, 3, 4, 5, 5, 5, 3, 2, 2, 3, 
        3, 3, 2, 1, 4, 3, 1, 1, 1, 1, 5, 3, 6, 6, 4, 4, 4, 1, 
        1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        3, 3, 3
	}.Softmax();
	private static readonly Segment[] Nuclei = {
        new Segment("a"),
        new Segment("e"), 
        new Segment("i"), 
        new Segment("o"),
        new Segment("u"),
        new Segment("aa"), 
        new Segment("ee"), 
        new Segment("ii"),
        new Segment("oo"), 
        new Segment("uu"),
        new Segment("ai"),
        new Segment("au"), 
        new Segment("ui"), 
        new Segment("oi"),
        new Segment("y"), 
        new Segment("w"), 
        new Segment("'"), 
        new Segment("m"), 
        new Segment("n"), 
        new Segment("ng"), 
        new Segment("l"), 
        new Segment("r")
	};
    private static readonly float[] PNuclei = new []{
        5, 5, 5, 5, 5, 3, 3, 2, 2, 2, 2, 2, 3, 2,
        3, 3, 5, 1, 1, 1, 1, 1
    }.Softmax();
    private static readonly Segment[] NucleiStressed = {
        new Segment("a"), 
        new Segment("e"), 
        new Segment("i"), 
        new Segment("o"), 
        new Segment("u"),
        new Segment("aa"), 
        new Segment("ee"), 
        new Segment("ii"), 
        new Segment("oo"),
        new Segment("uu"),
        new Segment("ai"), 
        new Segment("au"), 
        new Segment("ui"), 
        new Segment("oi")
	};
    private static readonly float[] PNucleiStressed = new []{
        5, 5, 5, 5, 5, 3, 3, 2, 2, 2, 2, 2, 3, 2
    }.Softmax();
    private static readonly Segment[] Codas = {
	    new Segment(), 
	    new Segment("r"),
	    new Segment("l")
    };
    private static readonly float[] PCodas = new [] {
	    5, 2, 2
    }.Softmax();
    private static readonly Segment[] Finals = {
        new Segment("m"), 
        new Segment("n"), 
        new Segment("ng"),
        new Segment("m", "m"),
        new Segment("n", "n"),
        new Segment("p"), 
        new Segment("t"), 
        new Segment("c"), 
        new Segment("k"),
        new Segment("p", "p"), 
        new Segment("t", "t"), 
        new Segment("c", "c"), 
        new Segment("k", "k"),
        new Segment("b"), 
        new Segment("d"), 
        new Segment("g"),
        new Segment("b", "b"), 
        new Segment("d", "d"), 
        new Segment("g", "g"),
        new Segment("f"), 
        new Segment("v"), 
        new Segment("s"), 
        new Segment("z"), 
        new Segment("h"),
        new Segment("f", "f"),
        new Segment("s", "s"), 
        new Segment("z", "z"),
        new Segment("r"), 
        new Segment("l"),
        new Segment("mh"), 
        new Segment("nh"), 
        new Segment("ngh"),
        new Segment("ph"), 
        new Segment("th"), 
        new Segment("ch"), 
        new Segment("kh"),
        new Segment("bh"), 
        new Segment("dh"), 
        new Segment("gh"),
        new Segment("fh"), 
        new Segment("sh"), 
        new Segment("zh"),
        new Segment("rh"), 
        new Segment("lh"), 
        new Segment("'h"),
        new Segment("s", "t"), 
        new Segment("f", "t"), 
        new Segment("sh", "t"), 
        new Segment("fh", "t"),
        new Segment("x"),
        new Segment("p", "th"), 
        new Segment("c", "th"), 
        new Segment("k", "th")
	};
    private static readonly float[] PFinals = new[]{
        2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 6, 2, 2, 2, 4,
        2, 5, 5, 6, 2, 2, 2, 3, 3, 3, 2, 1, 4, 6, 1, 1, 1, 
        1, 4, 3, 6, 6, 4, 4, 4, 1, 1, 1, 1, 3, 1, 1, 1
    }.Softmax();
    
    private static Tuple<Segment[], float[]> Filter(IEnumerable<Segment> segments, IEnumerable<float> probabilities, string letter)
    {
	    var zipped = segments.Zip(probabilities, (s, p) => new { s, p}).Where(pair => pair.s.Contains(letter)).ToArray();
	    var newSegments = zipped.Select(pair => pair.s).ToArray();
	    var newProbabilities = zipped.Select(pair => pair.p).ToArray().Softmax();
	    return Tuple.Create(newSegments, newProbabilities);
    }

    private static bool ShouldUseLetter(int numSyl, int currentSyl)
    {
	    return Random.value < (numSyl - 1 == 0 ? 1 : Mathf.Lerp(0.5f, 1f, currentSyl / (numSyl - 1f)));
    }
    
    public static Word GenerateWord(int numSyl, bool forceFinal = false, string wLetter = "")
    {
	    var word = new Word();
		var stressed = numSyl == 1 || Random.value < 0.5;
		var onsetReduplicationChance = 0.25f;
		
		var (onsetsWLetter, pOnsetsWLetter) = Filter(Onsets, POnsets, wLetter);
		var (nucleiWLetter, pNucleiWLetter) = Filter(Nuclei, PNuclei, wLetter);
		var (codasWLetter, pCodasWLetter) = Filter(Codas, PCodas, wLetter);
		
		for (var i = 0; i < numSyl; i++)
		{
			var syllable = new Syllable();

			// ONSET
			if (wLetter != "" && Onsets.Any(x => x.Contains(wLetter)) && ShouldUseLetter(numSyl, i))
			{
				syllable.SetOnset(onsetsWLetter.Choice(pOnsetsWLetter).Clone());
				wLetter = "";
			}
			else if (i != 0 && Random.value < onsetReduplicationChance)
			{
				syllable.SetOnset(word.GetRandomOnset().Clone());
				onsetReduplicationChance -= 0.05f;
			}
			else
			{
				syllable.SetOnset(Onsets.Choice(POnsets).Clone());
			}
			
			// NUCLEUS
			if (stressed)
			{
				if (wLetter != "" && NucleiStressed.Any(x => x.Contains(wLetter)) && ShouldUseLetter(numSyl, i))
				{
					syllable.SetNucleus(nucleiWLetter.Choice(pNucleiWLetter).Clone());
					wLetter = "";
				}
				else if (i != 0 && Random.value < 0.2f)
				{
					var possNucleus = word.GetRandomNucleus().Clone();
					syllable.SetNucleus(NucleiStressed.Any(x => x.Equals(possNucleus)) 
						? possNucleus 
						: NucleiStressed.Choice(PNucleiStressed).Clone());
				}
				else
				{
					syllable.SetNucleus(NucleiStressed.Choice(PNucleiStressed).Clone());
				}
			}
			else
			{
				if (wLetter != "" && Nuclei.Any(x => x.Contains(wLetter)) && ShouldUseLetter(numSyl, i))
				{
					syllable.SetNucleus(nucleiWLetter.Choice(pNucleiWLetter).Clone());
					wLetter = "";
				}
				else if (i != 0 && Random.value < 0.2f)
				{
					syllable.SetNucleus(word.GetRandomNucleus().Clone());
				}
				else
				{
					syllable.SetNucleus(Nuclei.Choice(PNuclei).Clone());
				}
			}
			
			// CODA
			if (wLetter != "" && Codas.Any(x => x.Contains(wLetter)) && ShouldUseLetter(numSyl, i))
			{
				syllable.SetCoda(codasWLetter.Choice(pCodasWLetter).Clone());
				wLetter = "";
			}
			else
			{
				syllable.SetCoda(Codas.Choice(PCodas).Clone());
			}

			word.AddSyllable(syllable);

			stressed = !stressed;
		}
		
		// FINAL
		if (forceFinal || Random.value < 0.5f)
		{
			var randomOnset = word.GetRandomOnset();
			if (Finals.Any(x => x.Equals(randomOnset)) && Random.value < onsetReduplicationChance + 0.25f)
			{
				word.AddSyllable(new Syllable(randomOnset.Clone()));
			}
			else
			{
				word.AddSyllable(new Syllable(Finals.Choice(PFinals).Clone()));
			}
		}
		
		// APOSTROPHE
		foreach (var syllable in word.GetSyllables())
		{
			if (syllable.GetOnset().Count() > 1 && Random.value < 0.2)
			{
				syllable.GetOnset().Add("'");
			}
		}

		// CLEANING
		word.DeleteDoubleChars('y');
		word.DeleteDoubleChars('w');
		word.DeleteDoubleChars('\'');

		word.DeleteInitialDoubleChars();
		word.DeleteInitialChars('\'');
		
		return word;
	}

    public static List<Word> GenerateName()
    {
	    while (true)
	    {
		    var p = Random.value;
		    List<Word> name;

		    if (p < 0.375f)
		    {
			    name = new List<Word> { GenerateWord(1), GenerateWord(Random.Range(2, 4), true) };
		    }
		    else if (p < 0.575f)
		    {
			    name = new List<Word> { GenerateWord(Random.Range(2, 4)), GenerateWord(1, true) };
		    }
		    else
		    {
			    name = new List<Word> { GenerateWord(Random.Range(2, 4), true) };
		    }

		    if (WordsToString(name, "-", 1).Length <= 3) continue;
		    return name;
	    }
    }

    public static List<Word> GenerateBook(List<Word> monsterName)
    {
	    while (true)
	    {
		    var letters = monsterName.Select(w => w.ToLetterList()).SelectMany(x => x).Distinct().ToList();
		    List<int> syllablesPerWord;
		    while (true)
		    {
			    syllablesPerWord =
				    Util.RandomChunks(15, new List<int> { 1, 2, 3, 4 }, new List<int> { 5, 4, 4, 3 }.Softmax()).ToList();
			    if (letters.Count <= syllablesPerWord.Count)
				    break;
		    }
		    var lettersPerWord = letters
			    .Concat(Enumerable.Range(0, syllablesPerWord.Count - letters.Count).Select(i => "")).ToList();
		    lettersPerWord.Shuffle();

		    var words = new List<Word>();
		    foreach (var x in syllablesPerWord.Zip(lettersPerWord, (numSyl, letter) => new {numSyl, letter}))
		    {
			    while (true)
			    {
				    var word = GenerateWord(x.numSyl, false, x.letter);
				    if (word.ToString().Length <= 8)
				    {
					    words.Add(word);
					    break;
				    }
			    }
		    }

		    if (WordsToString(words, " ", 0.1f).Length < 64)
		    {
			    return words;
		    }
	    }
    }

    public static string WordsToString(IEnumerable<Word> words, string separator, float pCapitalize)
    {
	    return string.Join(separator, words
		    .Select(w => w.ToString())
		    .Select(w => Random.value < pCapitalize ? w.Capitalize() : w));
    }

    public static List<List<List<string>>> WordsToSyllableList(IEnumerable<Word> words)
    {
	    return words.Select(w => w.ToSyllableList()).ToList();
    }

    public static List<List<string>> WordsToLetterList(IEnumerable<Word> words)
    {
	    return words.Select(w => w.ToLetterList()).ToList();
    }
}

[Serializable]
public class Word
{
	private List<Syllable> _syllables = new List<Syllable>();

	public void AddSyllable(Syllable syllable)
	{
		_syllables.Add(syllable);
	}

	public Segment GetRandomOnset()
	{
		return _syllables.Choice().GetOnset();
	}

	public Segment GetRandomNucleus()
	{
		return _syllables.Choice().GetNucleus();
	}

	public IEnumerable<Syllable> GetSyllables()
	{
		return _syllables;
	}

	public int Count()
	{
		return _syllables.Select(syl => syl.Count()).Sum();
	}

	public override string ToString()
	{
		return string.Join("", _syllables.Select(s => s.ToString()));
	}

	public List<List<string>> ToSyllableList()
	{
		return _syllables.Select(s => s.ToLetterList()).ToList();
	}

	public List<string> ToLetterList()
	{
		return _syllables.Select(s => s.ToLetterList()).SelectMany(x => x).ToList();
	}

	public void DeleteDoubleChars(char chr)
	{
		for (var i = 0; i < _syllables.Count; i++)
		{
			_syllables[i].DeleteDoubleChars(chr);
			if (i == 0) continue;

			if (_syllables[i - 1].Count() > 0 && _syllables[i].Count() > 0)
			{
				var j = _syllables[i - 1].GetLastChar();
				var k = _syllables[i].GetFirstChar();
				if (j == k && j == chr)
				{
					_syllables[i].DeleteFirstChar();
				}
			}
		}
	}

	public void DeleteInitialDoubleChars()
	{
		if (Count() <= 1) return;
		for (var i = 0; i < _syllables.Count; i++)
		{
			if (_syllables[i].Count() > 1)
			{
				_syllables[i].DeleteInitialDoubleChars();
				break;
			}
			if (_syllables[i].Count() == 1 && i + 1 < _syllables.Count)
			{
				var j = _syllables[i].GetLastChar();
				var k = _syllables[i+1].GetFirstChar();
				if (j == k)
				{
					_syllables[i+1].DeleteFirstChar();
				}
				break;
			}
		}
	}

	public void DeleteInitialChars(char chr)
	{
		while (Count() > 0 && GetFirstChar() == chr)
		{
			DeleteFirstChar();
		}
	}

	public char GetFirstChar()
	{
		return _syllables.First(syl => syl.Count() > 0).GetFirstChar();
	}

	public void DeleteFirstChar()
	{
		_syllables.First(syl => syl.Count() > 0).DeleteFirstChar();
	}
}

[Serializable]
public class Syllable
{
	private Segment _onset;
	private Segment _nucleus;
	private Segment _coda;

	public Syllable(Segment onset = null, Segment nucleus = null, Segment coda = null)
	{
		_onset = onset ?? new Segment();
		_nucleus = nucleus ?? new Segment();
		_coda = coda ?? new Segment();
	}

	public Segment GetOnset()
	{
		return _onset;
	}
	public void SetOnset(Segment onset)
	{
		_onset = onset;
	}
	
	public Segment GetNucleus()
	{
		return _nucleus;
	}
	
	public void SetNucleus(Segment nucleus)
	{
		_nucleus = nucleus;
	}
	
	public Segment GetCoda()
	{
		return _coda;
	}

	public void SetCoda(Segment coda)
	{
		_coda = coda;
	}
	
	public override string ToString()
	{
		return _onset.ToString() + _nucleus + _coda;
	}

	public List<string> ToLetterList()
	{
		return _onset.GetLetters().Concat(_nucleus.GetLetters()).Concat(_coda.GetLetters()).ToList();
	}

	public void DeleteDoubleChars(char chr)
	{
		_onset.DeleteDoubleChars(chr);
		_nucleus.DeleteDoubleChars(chr);
		_coda.DeleteDoubleChars(chr);

		if (_onset.Count() > 0 && _nucleus.Count() > 0)
		{
			var j = _onset.GetLastChar();
			var k = _nucleus.GetFirstChar();
			if (j == k && j == chr)
			{
				_onset.DeleteLastChar();
			}
		}

		if (_nucleus.Count() > 0 && _coda.Count() > 0)
		{
			var j = _nucleus.GetLastChar();
			var k = _coda.GetFirstChar();
			if (j == k && j == chr)
			{
				_coda.DeleteFirstChar();
			}
		}
	}

	public void DeleteInitialDoubleChars()
	{
		if (_onset.Count() > 1)
		{
			_onset.DeleteInitialDoubleChars();
		}
		else if (_onset.Count() == 1)
		{
			var j = _onset.GetLastChar();
			var k = _nucleus.Count() > 0 ? _nucleus.GetFirstChar() : _coda.GetFirstChar();
			if (j == k)
			{
				_onset.DeleteLastChar();
			}
		}
		else if (_nucleus.Count() > 1)
		{
			_nucleus.DeleteInitialDoubleChars();
		}
		else if (_nucleus.Count() == 1)
		{
			var j = _nucleus.GetLastChar();
			var k = _coda.GetFirstChar();
			if (j == k)
			{
				_nucleus.DeleteLastChar();
			}
		}
		else if (_coda.Count() > 1)
		{
			_coda.DeleteInitialDoubleChars();
		}
	}

	public void DeleteFirstChar()
	{
		if (_onset.Count() > 0)
		{
			_onset.DeleteFirstChar();
		} 
		if (_nucleus.Count() > 0)
		{
			_nucleus.DeleteFirstChar();
		}
		if (_coda.Count() > 0)
		{
			_coda.DeleteFirstChar();
		}
	}

	public char GetLastChar()
	{
		if (_coda.Count() > 0)
		{
			return _coda.GetLastChar();
		}
		if (_nucleus.Count() > 0)
		{
			return _nucleus.GetLastChar();
		}
		if (_onset.Count() > 0)
		{
			return _onset.GetLastChar();
		} 

		return new char();
	}

	public char GetFirstChar()
	{
		if (_onset.Count() > 0)
		{
			return _onset.GetFirstChar();
		} 
		if (_nucleus.Count() > 0)
		{
			return _nucleus.GetFirstChar();
		}
		if (_coda.Count() > 0)
		{
			return _coda.GetFirstChar();
		}

		return new char();
	}

	public int Count()
	{
		return _onset.Count() + _nucleus.Count() + _coda.Count();
	}
}

[Serializable]
public class Segment
{
	private List<string> _letters;

	public Segment(params string[] letters)
	{
		_letters = letters.ToList();
	}

	public Segment Clone()
	{
		return new Segment(_letters.ToArray());
	}

	public List<string> GetLetters()
	{
		return _letters.Where(l => l != "").ToList();
	}

	public bool Contains(object o)
	{
		return _letters.Contains(o);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Segment segment))
		{
			return false;
		}

		return _letters.SequenceEqual(segment._letters);
	}

	public override int GetHashCode()
	{
		return _letters.GetHashCode();
	}

	public void Add(string letter)
	{
		_letters.Add(letter);
	}
	
	public override string ToString()
	{
		return string.Join("", _letters);
	}

	public void DeleteDoubleChars(char chr)
	{
		for (var i = 0; i < _letters.Count; i++)
		{
			_letters[i] = _letters[i].Replace((chr + chr).ToString(), chr.ToString());
			if (i == 0) continue;
			if (_letters[i].Length < 2) continue;
			
			var j = _letters[i-1].Last();
			var k = _letters[i].First();
			if (j == k && j == chr)
			{
				_letters[i] = _letters[i].Substring(0, _letters[i].Length - 1);
			}
		}
	}

	public void DeleteInitialDoubleChars()
	{
		for (var i = 0; i < _letters.Count; i++)
		{
			if (_letters[i].Length > 1)
			{
				if (_letters[i][0] == _letters[i][1])
				{
					_letters[i] = _letters[i].Substring(1);
				}
				break;
			}
			if (_letters[i].Length == 1 && i + 1 < _letters.Count && _letters[i+1].Length > 0)
			{
				if (_letters[i][0] == _letters[i+1][0])
				{
					_letters[i] = "";
				}
				break;
			}
		}
	}

	public char GetFirstChar()
	{
		return _letters.First(x => x.Length > 0).First();
	}

	public char GetLastChar()
	{
		return _letters.Last(x => x.Length > 0).Last();
	}

	public void DeleteFirstChar()
	{
		var index = _letters.Select((x, i) => new { x, i }).First(y => y.x.Length > 0).i;
		_letters[index] = _letters[index].Substring(1);
	}

	public void DeleteLastChar()
	{
		var index = _letters.Select((x, i) => new { x, i }).Last(y => y.x.Length > 0).i;
		_letters[index] = _letters[index].Substring(0, _letters[index].Length - 1);
	}

	public int Count()
	{
		return string.Join("", _letters).Length;
	}
}

}
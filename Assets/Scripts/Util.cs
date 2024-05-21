using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class Util
{
    public static Tuple<float, float, float, float> GetScreenBorders()
    {
        var screenHeight = Camera.main.orthographicSize * 2f;
        var screenWidth = screenHeight * Camera.main.aspect;
        return Tuple.Create(-screenWidth / 2f, screenWidth / 2f, -screenHeight / 2f, screenHeight / 2f);
    }
    
    public static Tuple<float, float, float, float> GetScreenBordersInWorld()
    {
        var screenBorders = GetScreenBorders();
        var ll = Camera.main.ScreenToWorldPoint(new Vector3(screenBorders.Item1, screenBorders.Item3, 0f));
        var ur = Camera.main.ScreenToWorldPoint(new Vector3(screenBorders.Item2, screenBorders.Item4, 0f));

        return Tuple.Create(ll.x, ur.x, ll.y, ur.y); 
    }

    public static Vector3 EDivide(this Vector3 p, Vector3 q) {
        return Vector3.Scale(p, new Vector3(1f / q.x, 1f / q.y, 1f / q.z));
    }

    public static Vector3 EMultiply(this Vector3 p, Vector3 q)
    {
        return Vector3.Scale(p, q);
    }

    public static T[,] Transpose<T>(this T[,] input)
    {
        var output = new T[input.GetLength(1), input.GetLength(0)];
        for (var i = 0; i < input.GetLength(0); i++)
        for (var j = 0; j < input.GetLength(1); j++)
            output[j, i] = input[i, j];

        return output;
    }
    
    public static float[] Softmax(this IEnumerable<int> input)
    {
        var tmp = input.Select(i => (float)Math.Exp((float)i)).ToArray();
        var sum = tmp.Sum();
        return tmp.Select(i => i / sum).ToArray();
    }
	
    public static float[] Softmax(this IEnumerable<float> input)
    {
        var tmp = input.Select(i => (float)Math.Exp(i)).ToArray();
        var sum = tmp.Sum();
        return tmp.Select(i => i / sum).ToArray();
    }

    public static T Choice<T>(this IList<T> sequence, IEnumerable<float> distribution) {
        double sum = 0;
        var cumulative = distribution.Select(c => {
            var result = c + sum;
            sum += c;
            return result;
        }).ToList();
		
        var r = Random.value;
        var idx = cumulative.BinarySearch(r);
        if (idx < 0)
            idx = ~idx; 
        if (idx > cumulative.Count - 1)
            idx = cumulative.Count - 1;
        return sequence[idx];
    }

    public static T Choice<T>(this IList<T> sequence)
    {
        return sequence[Random.Range(0, sequence.Count)];
    }

    public static IEnumerable<int> RandomChunks(int elements, IList<int> lengths, IList<float> distribution)
    {
        while (true)
        {
            var chunk = lengths.Choice(distribution);
            if (elements - chunk <= 0)
            {
                yield return elements;
                break;
            }
            yield return chunk;
            elements -= chunk;
        }
    }

    public static IEnumerable<T> Repeat<T>(this IList<T> sequence, int n)
    {
        for (var i = 0; i < n; i++)
        {
            foreach (var elem in sequence)
            {
                yield return elem;
            }
        }
    }

    public static void Shuffle<T>(this IList<T> list)  
    {  
        var n = list.Count;  
        while (n > 1) {  
            n--;  
            var k = Random.Range(0, n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }

    public static string Capitalize(this string input)
    {
        return string.IsNullOrEmpty(input) 
            ? input
            : input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
    }

    public static void SetAlpha(this SpriteRenderer renderer, float alpha)
    {
        var color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }
    
    public static void SetAlpha(this Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public static void IncreaseAlpha(this SpriteRenderer renderer, float alpha)
    {
        var color = renderer.color;
        color.a += alpha;
        renderer.color = color;
    }
    
    public static void IncreaseAlpha(this Image image, float alpha)
    {
        var color = image.color;
        color.a += alpha;
        image.color = color;
    }
    
    public static void CenterSpriteOnPivotY(this Image image)
    {
        var offsetY = image.rectTransform.rect.height * Mathf.Abs((image.sprite.pivot.y / image.sprite.rect.height) - .5f);
     
        image.rectTransform.offsetMin = new Vector2(image.rectTransform.offsetMin.x, -offsetY);
        image.rectTransform.offsetMax = new Vector2(image.rectTransform.offsetMax.x, -offsetY);
    }

}

[Serializable]
public class Pair<T1, T2> {
    public T1 key;
    public T2 val;

    public Pair(T1 key)
    {
        this.key = key;
    }
}
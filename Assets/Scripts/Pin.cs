using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class Pin : MonoBehaviour
{
    public Sprite uncorruptedSprite;
    public Sprite corruptedSprite;
    public Vec2 save;

    public void Init(Vec2 loc, bool corrupted)
    {
        save = loc;
        GetComponent<SpriteRenderer>().sprite = corrupted ? corruptedSprite : uncorruptedSprite;
    }

    public void SetCorrupted()
    {
        GetComponent<SpriteRenderer>().sprite = corruptedSprite;
    }
}

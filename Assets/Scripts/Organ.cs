using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organ : MonoBehaviour
{
    public string id;
    private SacrificeGameUI _sacrificeGameUI;

    public void Start()
    {
        _sacrificeGameUI = FindObjectOfType<SacrificeGameUI>();
    }

    public void Select()
    {
        _sacrificeGameUI.SelectFromPlate(id);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AltarUI : MonoBehaviour
{
    public GameObject mistPrefab;
    public GameObject letterPrefab;
    public GameObject[] layoutPrefabs;
    public GameObject organPrefab;
    public GameObject shortFeaturePrefab;
    public GameObject longFeaturePrefab;
    public GameObject[] shapePrefabs;
    
    public GameObject nameBar;
    public GameObject book;
    public GameObject icon;
    public GameObject monster;
    public GameObject sacrifice;
    
    public GameObject backButton;
    public GameObject cancelButton;
    public GameObject nameGameButton;
    public GameObject iconGameButton;
    public GameObject sacrificeGameButton;

    private GameObject[] _mistClouds = new GameObject[50];
    private Dictionary<string, Sprite> _organSprites;
    private Dictionary<string, Sprite> _letterSprites;
    private Dictionary<string, Sprite> _featureSprites;
    private Dictionary<string, GameObject> _shapePrefabs;
    
    private readonly string[] _long = { "stalkeye", "wing", "bugleg", "tentacle", "arm", "gills", "claw" };

    private void Awake()
    {
        _organSprites = new List<string>
        {
            "brain", "intestines", "stomach", "liver", "heart", "kidney"
        }.ToDictionary(x => x, x => Resources.Load<Sprite>($"Sprites/loc_organ_{x}"));
        
        _letterSprites = new List<string>
            {
                "A", "a", "B", "b", "C", "c", "D", "d", "E", "e", "F", "f", "G", "g", "H", "h",
                "I", "i", "J", "j", "K", "k", "L", "l", "M", "m", "N", "n", "O", "o", "P", "p",
                "Q", "q", "R", "r", "S", "s", "T", "t", "U", "u", "V", "v", "W", "w", "X", "x",
                "Y", "y", "Z", "z", "-", "'", ",", ".", "?", "!", "%", "0", "1", "2", "3", "4", 
                "5", "6", "7", "8", "9", "AA", "AI", "AU", "BH", "CH", "DH", "EE", "FH", "GH",
                "II", "KH", "LH", "LY", "MH", "NG", "NGH", "NH", "NY", "OO", "OI", "PH", "RH", 
                "SH", "TH", "UU", "UI", "ZH", "'H"
            }
            .Zip(Resources.LoadAll<Sprite>($"Sprites/letters_single").Concat(Resources.LoadAll<Sprite>("Sprites/letters_multiple")), 
                (l, s) => new {l, s}).ToDictionary(pair => pair.l, pair => pair.s);
        
        _featureSprites = new Dictionary<string, string>
        {
            { "blob", "shapes_blob" }, { "sphere", "shapes_sphere" }, { "peanut", "shapes_peanut" }, 
            { "skull", "shapes_skull" }, { "worm", "shapes_worm" }, { "star", "shapes_star" },
            { "arm", "extremities_arm" }, { "wing", "extremities_wing" }, { "bugleg", "extremities_bugleg" },
            { "tentacle", "extremities_tentacle" }, { "gills", "extremities_gills" }, { "claw", "extremities_claw" },
            { "octopuseye", "eyes_octopuseye" }, { "humaneye", "eyes_humaneye" }, { "bugeye", "eyes_bugeye" },
            { "goateye", "eyes_goateye" }, { "lizardeye", "eyes_lizardeye" }, { "stalkeye", "eyes_stalkeye" }, 
            { "cuttlefishmouth", "mouths_cuttlefishmouth" }, { "humanmouth", "mouths_humanmouth" }, { "sealampreymouth", "mouths_sealampreymouth" },
            { "anglerfishmouth", "mouths_anglerfishmouth" }, { "seaurchinmouth", "mouths_seaurchinmouth" }, { "birdbeak", "mouths_birdbeak" }
        }.ToDictionary(x => x.Key, x => Resources.Load<Sprite>($"Sprites/loc_{x.Value}"));

        _shapePrefabs = new List<string>
        {
            "blob", "peanut", "skull", "sphere", "star", "worm"
        }.Zip(shapePrefabs, (s, o) => new { s, o }).ToDictionary(x => x.s, x => x.o);
    }

    void Start()
    {
        var altarComplete = Global.Altar.IsComplete();
        backButton.SetActive(altarComplete);
        cancelButton.SetActive(!altarComplete);

        InitMist();
        
        UpdateName();
        UpdateIcon();
        UpdateSacrifice();
        UpdateButtons();
    }
    
    void InitMist()
    {
        var screenBorders = Util.GetScreenBorders();

        for (var i = 0; i < _mistClouds.Length; i++)
        {
            var x = Mathf.Lerp(screenBorders.Item1, screenBorders.Item2, Random.value);
            var y = Mathf.Lerp(screenBorders.Item3, screenBorders.Item4, Random.value);
            _mistClouds[i] = Instantiate(mistPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    void UpdateName()
    {
        var nameGameComplete = Global.Altar.IsMiniGameComplete("name-game");
        nameBar.SetActive(nameGameComplete);
        book.SetActive(nameGameComplete);
        if (!nameGameComplete) return;
        
        // POPULATE NAME
        foreach (var l in Global.Altar.GetNameAsLetterList())
        {
            var letter = Instantiate(letterPrefab, nameBar.transform, false);
            letter.GetComponent<Image>().sprite = _letterSprites[l.ToUpper()];
        }
    }

    void UpdateIcon()
    {
        var iconGameComplete = Global.Altar.IsMiniGameComplete("icon-game");
        icon.transform.parent.gameObject.SetActive(iconGameComplete);
        if (!iconGameComplete) return;
        
        foreach (var mist in _mistClouds)
        {
            mist.GetComponent<Mist>().Reveal();
        }
            
        monster.GetComponent<MonsterUI>().SetVisible();
            
        // POPULATE ICON
        var shape = Global.Altar.GetShape();
        if (!(shape is null))
        {
            var layout = Instantiate(_shapePrefabs[shape], icon.transform, false);
            var features = Global.Altar.GetFeatures();
            for (var i = 0; i < features.Length; i++)
            {
                if (features[i] is null) continue;
                var feature = Instantiate(_long.Contains(features[i]) ? longFeaturePrefab : shortFeaturePrefab, layout.transform.GetChild(i + 1), false);
                feature.GetComponent<Image>().sprite = _featureSprites[features[i]];
                feature.GetComponent<Image>().SetNativeSize();
                feature.transform.localScale *= 0.9f;
            }
        }
    }

    void UpdateSacrifice()
    {
        var sacrificeGameComplete = Global.Altar.IsMiniGameComplete("sacrifice-game");
        sacrifice.SetActive(sacrificeGameComplete);
        if (!sacrificeGameComplete) return;
        
        // POPULATE PLATE
        var organs = Global.Altar.GetSacrifice();
        var layout = Instantiate(layoutPrefabs[organs.Length - 1], sacrifice.transform.GetChild(0), false);
        for (var i = 0; i < organs.Length; i++)
        {
            var organ = Instantiate(organPrefab, layout.transform.GetChild(i), false);
            organ.GetComponent<Image>().sprite = _organSprites[organs[i]];
            organ.GetComponent<Image>().SetNativeSize();
        }
    }

    void UpdateButtons()
    {
        nameGameButton.SetActive(!Global.Altar.IsMiniGameComplete("name-game"));
        iconGameButton.SetActive(!Global.Altar.IsMiniGameComplete("icon-game"));
        sacrificeGameButton.SetActive(!Global.Altar.IsMiniGameComplete("sacrifice-game"));
    }

    public void Back()
    {
        var name = Global.Altar.GetNameAsString();
        var title = Global.Altar.GetTitle() + " ";
        Global.LoadingMessage = Global.Altar.IsComplete() 
            ? $"     You have convinced {title}{name} to join you on your conquest!    " 
            : $"     You have failed to convince {title}{name} to join you on your conquest.    ";

        Global.NextScene = "Map";
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void InitNameGame()
    {
        Global.LoadingMessage = "     Translate Their name with the help of the nefarious Necronomicon.    ";
        Global.NextScene = "NameGame";
        Global.Altar.InitMiniGame("name-game");
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void InitIconGame()
    {
        Global.LoadingMessage = "     Build a flattering icon to worship Their godly appearance.     ";
        Global.NextScene = "IconGame";
        Global.Altar.InitMiniGame("icon-game");
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void InitSacrificeGame()
    {
        Global.LoadingMessage = "     Serve Them a Sacrifice worthy of their mighty appetite.     ";
        Global.NextScene = "SacrificeGame";
        Global.Altar.InitMiniGame("sacrifice-game");
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }
}

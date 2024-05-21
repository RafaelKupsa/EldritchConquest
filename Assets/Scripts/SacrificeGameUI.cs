using System.Collections.Generic;
using System.Linq;
using Backend;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SacrificeGameUI : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject letterPrefab;
    public GameObject spacePrefab;
    public GameObject selectedPrefab;
    public GameObject[] layoutPrefabs;
    public GameObject organPrefab;
    
    public GameObject timerBar;
    public GameObject timerBarBar;
    public GameObject timerBarBorder;
    public GameObject speechBubble;
    public GameObject plate;
    public GameObject organsMenu;
    public Canvas selectionCanvas;

    private const int MaxLineLength = 15;
    private Dictionary<string, Sprite> _letterSprites;
    private Dictionary<string, Sprite> _organSprites;
    private float _timerSize;

    private void Awake()
    {
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
        
        _organSprites = new List<string>
        {
            "brain", "intestines", "stomach", "liver", "heart", "kidney"
        }.ToDictionary(x => x, x => Resources.Load<Sprite>($"Sprites/sg_organ_{x}"));
    }

    void Start()
    {
        UpdatePlate();
        speechBubble.SetActive(false);
        _timerSize = timerBar.GetComponent<RectTransform>().rect.width * timerBar.transform.localScale.x;
        UpdateTimer();
    }
    
    void Update()
    {
        if (Input.GetMouseButton(0) && !(Global.Selected is null))
        {
            Global.Selected.transform.position = Input.mousePosition;
        }
    }

    void LateUpdate()
    {
        if (!Input.GetMouseButton(0) && !(Global.Selected is null))
        {
            Destroy(Global.Selected);
            Global.Selected = null;
            Global.SelectedID = null;
            
            if (plate.transform.childCount > 0)
            {
                foreach (Transform childTransform in plate.transform.GetChild(0))
                {
                    childTransform.GetChild(0).GetComponent<Image>().raycastTarget = true;
                }
            }
        }
    }
    
    public void Select(string id)
    {
        Global.Selected = Instantiate(selectedPrefab, Input.mousePosition, Quaternion.identity, selectionCanvas.transform);
        Global.Selected.GetComponent<Image>().sprite = _organSprites[id];
        Global.Selected.GetComponent<Image>().SetNativeSize();
        Global.Selected.transform.localScale *= 1.1f;
        Global.Selected.GetComponent<Image>().raycastTarget = false;
        Global.SelectedID = id;

        if (plate.transform.childCount > 0)
        {
            foreach (Transform childTransform in plate.transform.GetChild(0))
            {
                childTransform.GetChild(0).GetComponent<Image>().raycastTarget = false;
            }
        }
    }

    public void SelectFromPlate(string id)
    {
        ((SacrificeGame)Global.MiniGame).RemoveOrgan(id);
        UpdatePlate();
        Select(id);
    }

    public void UpdatePlate()
    {
        foreach (Transform childTransform in plate.transform)
        {
            Destroy(childTransform.gameObject);
        }
        
        var organs = ((SacrificeGame)Global.MiniGame).GetGuess();
        if (organs.Count > 0)
        {
            var layout = Instantiate(layoutPrefabs[organs.Count - 1], plate.transform, false);
            for (var i = 0; i < organs.Count; i++)
            {
                var organ = Instantiate(organPrefab, layout.transform.GetChild(i), false);
                organ.GetComponent<Organ>().id = organs[i];
                organ.GetComponent<Image>().sprite = _organSprites[organs[i]];
                organ.GetComponent<Image>().SetNativeSize();
            }
        }

        foreach (Transform childTransform in organsMenu.transform)
        {
            childTransform.gameObject.SetActive(organs.Count < 10);
        }
    }

    void UpdateTimer()
    {
        timerBarBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _timerSize * ((SacrificeGame)Global.MiniGame).GetTimeLeft());
        timerBarBorder.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _timerSize * ((SacrificeGame)Global.MiniGame).GetTimeLeft());
    }

    void UpdateSpeechbubble()
    {
        speechBubble.SetActive(true);

        foreach (Transform childTransform in speechBubble.transform.GetChild(0))
        {
            Destroy(childTransform.gameObject);
        }

        var feedback = ((SacrificeGame)Global.MiniGame).GetFeedback();
        
        var currentLineLength = 0;
        var line = Instantiate(linePrefab, speechBubble.transform.GetChild(0), false);
        foreach (var word in feedback)
        {
            if (currentLineLength + word.Count < MaxLineLength)
            {
                foreach (var l in word)
                {
                    var letter = Instantiate(letterPrefab, line.transform, false);
                    letter.GetComponent<Image>().sprite = _letterSprites[l.ToUpper()];
                }
                Instantiate(spacePrefab, line.transform, false);
                currentLineLength += word.Count + 1;
            }
            else
            {
                line = Instantiate(linePrefab, speechBubble.transform.GetChild(0), false);
                foreach (var l in word)
                {
                    var letter = Instantiate(letterPrefab, line.transform, false);
                    letter.GetComponent<Image>().sprite = _letterSprites[l.ToUpper()];
                }
                Instantiate(spacePrefab, line.transform, false);
                currentLineLength = word.Count + 1;
            }
        }
    }
    
    public void Cancel()
    {
        ((SacrificeGame)Global.MiniGame).Fail();
        Return();
    }
    
    public void Submit()
    {
        if (((SacrificeGame)Global.MiniGame).IsCorrect())
        {
            Return();
        }
        else
        {
            ((SacrificeGame)Global.MiniGame).DecreaseTries();
            UpdateSpeechbubble();
            UpdateTimer();
            if (((SacrificeGame)Global.MiniGame).IsFailed())
            {
                Return();
            }
        }
    }
    
    public void Reset()
    {
        ((SacrificeGame)Global.MiniGame).Clear();
        UpdatePlate();
    }
    
    void Return()
    {
        var name = Global.Altar.GetNameAsString();
        var title = Global.Altar.GetTitle() + " ";

        if (((SacrificeGame)Global.MiniGame).IsFailed())
        {
            Global.Altar.Fail();
            Global.LoadingMessage = $"     {title.Capitalize()}{name} rejects your miserable attempt at a meal and vanishes!    ";
            Global.NextScene = "Map";
            SceneManager.LoadScene("Scenes/LoadingScreen");
        }
        else
        {
            Global.Altar.CompleteGame("sacrifice-game");
            Global.LoadingMessage = $"    The Old One is eagerly devouring your offering.      ";
            Global.NextScene = "Altar";
            SceneManager.LoadScene("Scenes/LoadingScreen");
        }
    }
    
}

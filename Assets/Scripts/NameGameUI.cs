using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Backend;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameGameUI : MonoBehaviour
{
    public GameObject selectedPrefab;
    public GameObject letterSlotPrefab;
    public GameObject minusPrefab;
    public GameObject symbolPrefab;
    public GameObject spacePrefab;
    public GameObject bookLetterPrefab;
    public GameObject bookSymbolPrefab;
    public GameObject bookOriginalLinePrefab;
    public GameObject bookTranscribedLinePrefab;
    public GameObject bookOriginalSpacePrefab;
    public GameObject bookTranscribedSpacePrefab;

    public GameObject timerBar;
    public GameObject timerBarBar;
    public GameObject timerBarBorder;
    public GameObject inputBar;
    public GameObject keyboard1;
    public GameObject keyboard2;
    public GameObject namePanel;
    public GameObject bookOriginalPanel;
    public GameObject bookTranscribedPanel;
    public Canvas selectionCanvas;

    private bool _shifted;
    private const int MaxLineLength = 9;

    private Dictionary<string, Sprite> _letterSprites;
    private Dictionary<string, Sprite[]> _scriptSprites;

    private float _timerSize;
    private Coroutine _timer;

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

        _scriptSprites = new Dictionary<string, Sprite[]>
        {
            {"abstract", Resources.LoadAll<Sprite>($"Sprites/ng_script_abstract")},
            {"hieroglyphs", Resources.LoadAll<Sprite>($"Sprites/ng_script_hieroglyphs")},
            {"triangular", Resources.LoadAll<Sprite>($"Sprites/ng_script_triangular")},
            {"tech", Resources.LoadAll<Sprite>($"Sprites/ng_script_tech")}
        };
    }

    void Start()
    {
        InitName();
        InitBook();
        UpdateInput();
        keyboard2.SetActive(false);
        _timerSize = timerBar.GetComponent<RectTransform>().rect.width * timerBar.transform.localScale.x;
        _timer = StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while (!((NameGame)Global.MiniGame).IsFailed())
        {
            ((NameGame)Global.MiniGame).DecreaseTime(0.01f);
            timerBarBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _timerSize * ((NameGame)Global.MiniGame).GetTimeLeft());
            timerBarBorder.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _timerSize * ((NameGame)Global.MiniGame).GetTimeLeft());
            yield return new WaitForSeconds(0.01f);
        }
        Return();
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
        }
    }

    public void Select(string id)
    {
        Global.Selected = Instantiate(selectedPrefab, Input.mousePosition, Quaternion.identity, selectionCanvas.transform);
        Global.Selected.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/ng_letter_{Global.SelectedID}");
        Global.Selected.GetComponent<Image>().raycastTarget = false;
        Global.Selected.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
        Global.SelectedID = id;
    }

    void InitName()
    {
        var script = ((NameGame)Global.MiniGame).GetScript();
        foreach (var word in ((NameGame)Global.MiniGame).GetNameOriginal())
        {
            foreach (var i in word)
            {
                var symbol = Instantiate(symbolPrefab, namePanel.transform, false);
                symbol.GetComponent<Image>().sprite = _scriptSprites[script][i];
            }
            Instantiate(spacePrefab, namePanel.transform, false);
        }
    }

    void InitBook()
    {
        var script = ((NameGame)Global.MiniGame).GetScript();
        var bookOriginal = ((NameGame)Global.MiniGame).GetBookOriginal();
        
        var currentLineLength = 0;
        var line = Instantiate(bookOriginalLinePrefab, bookOriginalPanel.transform, false);
        foreach (var word in bookOriginal)
        {
            if (currentLineLength + word.Count < MaxLineLength)
            {
                foreach (var l in word)
                {
                    var symbol = Instantiate(bookSymbolPrefab, line.transform, false);
                    symbol.GetComponent<Image>().sprite = _scriptSprites[script][l];
                }
                Instantiate(bookOriginalSpacePrefab, line.transform, false);
                currentLineLength += word.Count + 1;
            }
            else
            {
                line = Instantiate(bookOriginalLinePrefab, bookOriginalPanel.transform, false);
                foreach (var l in word)
                {
                    var symbol = Instantiate(bookSymbolPrefab, line.transform, false);
                    symbol.GetComponent<Image>().sprite = _scriptSprites[script][l];
                }
                Instantiate(bookOriginalSpacePrefab, line.transform, false);
                currentLineLength = word.Count + 1;
            }
        }
        
        var bookTranscribed = ((NameGame)Global.MiniGame).GetBookTranscribed();
        
        currentLineLength = 0;
        line = Instantiate(bookTranscribedLinePrefab, bookTranscribedPanel.transform, false);
        foreach (var word in bookTranscribed)
        {
            if (currentLineLength + word.Count < MaxLineLength)
            {
                foreach (var l in word)
                {
                    var symbol = Instantiate(bookLetterPrefab, line.transform, false);
                    symbol.GetComponent<Image>().sprite = _letterSprites[l.ToUpper()];
                }
                Instantiate(bookTranscribedSpacePrefab, line.transform, false);
                currentLineLength += word.Count + 1;
            }
            else
            {
                line = Instantiate(bookTranscribedLinePrefab, bookTranscribedPanel.transform, false);
                foreach (var l in word)
                {
                    var symbol = Instantiate(bookLetterPrefab, line.transform, false);
                    symbol.GetComponent<Image>().sprite = _letterSprites[l.ToUpper()];
                }
                Instantiate(bookOriginalSpacePrefab, line.transform, false);
                currentLineLength = word.Count + 1;
            }
        }
    }

    void UpdateInput()
    {
        foreach (Transform childTransform in inputBar.transform)
        {
            Destroy(childTransform.gameObject);
        }
        
        var index = 0;
        foreach (var l in ((NameGame)Global.MiniGame).GetGuess())
        {
            if (l is null || l != "-")
            {
                var slot = Instantiate(letterSlotPrefab, inputBar.transform, false);
                slot.GetComponent<LetterSlot>().index = index;
                slot.transform.GetChild(0).gameObject.SetActive(!(l is null));
                if (!(l is null))
                {
                    slot.transform.GetChild(0).GetComponent<Image>().sprite = _letterSprites[l.ToUpper()];
                }
            }
            else
            {
                var minus = Instantiate(minusPrefab, inputBar.transform, false);
            }

            if (l != "-")
                index += 1;
        }
    }

    public void Cancel()
    {
        StopCoroutine(_timer);
        ((NameGame)Global.MiniGame).Fail();
        Return();
    }

    public void Submit()
    {
        if (!((NameGame)Global.MiniGame).IsCorrect()) return;
        Return();
    }

    public void Reset()
    {
        ((NameGame)Global.MiniGame).Clear();
        UpdateInput();
    }

    void Return()
    {
        StopCoroutine(_timer);
        
        
        var name = Global.Altar.GetNameAsString();
        var title = Global.Altar.GetTitle() + " ";

        if (((NameGame)Global.MiniGame).IsFailed())
        {
            
            Global.Altar.Fail();
            Global.LoadingMessage = $"     {title.Capitalize()}{name} got tired of your pathetic prayers!    ";
            Global.NextScene = "Map";
            SceneManager.LoadScene("Scenes/LoadingScreen");
        }
        else
        {
            Global.Altar.CompleteGame("name-game");
            Global.LoadingMessage = $"    {title.Capitalize()}{name} heard your prayers!      ";
            Global.NextScene = "Altar";
            SceneManager.LoadScene("Scenes/LoadingScreen");
        }
    }

    public void Shift()
    {
        keyboard1.SetActive(_shifted);
        keyboard2.SetActive(!_shifted);
        _shifted = !_shifted;
    }
}

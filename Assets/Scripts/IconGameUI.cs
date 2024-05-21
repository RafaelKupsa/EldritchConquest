using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Backend;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IconGameUI : MonoBehaviour
{
    public GameObject selectedPrefab;
    public GameObject shortFeaturePrefab;
    public GameObject longFeaturePrefab;
    public GameObject[] shapePrefabs;

    public GameObject timerBar;
    public GameObject timerBarBar;
    public GameObject timerBarBorder;
    public GameObject monster;
    public GameObject pedestal;
    public GameObject shapesMenu;
    public GameObject extremitiesMenu;
    public GameObject eyesMenu;
    public GameObject mouthsMenu;
    public GameObject eyeOpen;
    public GameObject eyeClosed;
    public GameObject ui;
    public GameObject hiddenUI;
    public Canvas selectionCanvas;

    private GameObject monsterLayout;
    
    private bool _looking;
    private Dictionary<string, Sprite> _selectedFeatureSprites;
    private Dictionary<string, Sprite> _iconFeatureSprites;
    private Dictionary<string, GameObject> _shapePrefabs;
    
    private float _timerSize;
    private Coroutine _timer;

    private readonly string[] _shapes = { "blob", "peanut", "skull", "sphere", "star", "worm" };
    private readonly string[] _long = { "stalkeye", "wing", "bugleg", "tentacle", "arm", "gills", "claw" };

    private string _select;

    private void Awake()
    {
        _selectedFeatureSprites = new Dictionary<string, string>
        {
            { "blob", "shapes_blob_menu" }, { "sphere", "shapes_sphere_menu" }, { "peanut", "shapes_peanut_menu" }, 
            { "skull", "shapes_skull_menu" }, { "worm", "shapes_worm_menu" }, { "star", "shapes_star_menu" },
            { "arm", "extremities_arm_menu" }, { "wing", "extremities_wing_menu" }, { "bugleg", "extremities_bugleg_menu" },
            { "tentacle", "extremities_tentacle_menu" }, { "gills", "extremities_gills_menu" }, { "claw", "extremities_claw_menu" },
            { "octopuseye", "eyes_octopuseye" }, { "humaneye", "eyes_humaneye" }, { "bugeye", "eyes_bugeye" },
            { "goateye", "eyes_goateye" }, { "lizardeye", "eyes_lizardeye" }, { "stalkeye", "eyes_stalkeye_menu" }, 
            { "cuttlefishmouth", "mouths_cuttlefishmouth" }, { "humanmouth", "mouths_humanmouth" }, { "sealampreymouth", "mouths_sealampreymouth" },
            { "anglerfishmouth", "mouths_anglerfishmouth" }, { "seaurchinmouth", "mouths_seaurchinmouth" }, { "birdbeak", "mouths_birdbeak" }
        }.ToDictionary(x => x.Key, x => Resources.Load<Sprite>($"Sprites/ig_{x.Value}"));

        _iconFeatureSprites = new Dictionary<string, string>
        {
            { "blob", "shapes_blob" }, { "sphere", "shapes_sphere" }, { "peanut", "shapes_peanut" }, 
            { "skull", "shapes_skull" }, { "worm", "shapes_worm" }, { "star", "shapes_star" },
            { "arm", "extremities_arm" }, { "wing", "extremities_wing" }, { "bugleg", "extremities_bugleg" },
            { "tentacle", "extremities_tentacle" }, { "gills", "extremities_gills" }, { "claw", "extremities_claw" },
            { "octopuseye", "eyes_octopuseye" }, { "humaneye", "eyes_humaneye" }, { "bugeye", "eyes_bugeye" },
            { "goateye", "eyes_goateye" }, { "lizardeye", "eyes_lizardeye" }, { "stalkeye", "eyes_stalkeye" }, 
            { "cuttlefishmouth", "mouths_cuttlefishmouth" }, { "humanmouth", "mouths_humanmouth" }, { "sealampreymouth", "mouths_sealampreymouth" },
            { "anglerfishmouth", "mouths_anglerfishmouth" }, { "seaurchinmouth", "mouths_seaurchinmouth" }, { "birdbeak", "mouths_birdbeak" }
        }.ToDictionary(x => x.Key, x => Resources.Load<Sprite>($"Sprites/ig_{x.Value}"));

        _shapePrefabs = new List<string>
        {
            "blob", "peanut", "skull", "sphere", "star", "worm"
        }.Zip(shapePrefabs, (s, o) => new { s, o }).ToDictionary(x => x.s, x => x.o);
    }

    void Start()
    {
        UpdateIcon();
        extremitiesMenu.SetActive(false);
        eyesMenu.SetActive(false);
        mouthsMenu.SetActive(false);

        hiddenUI.SetActive(false);
        _timerSize = timerBar.GetComponent<RectTransform>().rect.width * timerBar.transform.localScale.x;
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
            
            if (pedestal.transform.childCount > 0)
            {
                for (var i = 1; i < pedestal.transform.GetChild(0).childCount; i++)
                {
                    var childTransform = pedestal.transform.GetChild(0).GetChild(i);
                    childTransform.GetComponent<Image>().raycastTarget = false;
                    childTransform.GetComponent<Image>().SetAlpha(0f);          
                    if (childTransform.childCount > 0)
                    {
                        childTransform.GetChild(0).GetComponent<Image>().raycastTarget = true;
                    }
                }
            }
        }
    }
    
    IEnumerator Timer()
    {
        while (!((IconGame)Global.MiniGame).IsFailed())
        {
            ((IconGame)Global.MiniGame).DecreaseTime(0.01f);
            timerBarBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _timerSize * ((IconGame)Global.MiniGame).GetTimeLeft());
            timerBarBorder.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _timerSize * ((IconGame)Global.MiniGame).GetTimeLeft());
            yield return new WaitForSeconds(0.01f);
        }
        Return();
    }

    public void Select(string id)
    {
        Global.Selected = Instantiate(selectedPrefab, Input.mousePosition, Quaternion.identity, selectionCanvas.transform);
        Global.Selected.GetComponent<Image>().sprite = _selectedFeatureSprites[id];
        Global.Selected.GetComponent<Image>().SetNativeSize();
        Global.Selected.transform.localScale *= 1.1f;
        Global.Selected.GetComponent<Image>().raycastTarget = false;
        Global.SelectedID = id;

        if (_shapes.Contains(id))
        {
            pedestal.transform.GetComponent<Image>().raycastTarget = true;
            if (pedestal.transform.childCount > 0)
            {
                for (var i = 1; i < monsterLayout.transform.childCount; i++)
                {
                    var childTransform = monsterLayout.transform.GetChild(i);
                    childTransform.GetComponent<Image>().raycastTarget = false;
                    if (childTransform.childCount > 0)
                    {
                        childTransform.GetChild(0).GetComponent<Image>().raycastTarget = false;
                    }
                }
            }
        }
        else
        {
            pedestal.transform.GetComponent<Image>().raycastTarget = false;
            if (pedestal.transform.childCount > 0)
            {
                for (var i = 1; i < monsterLayout.transform.childCount; i++)
                {
                    var childTransform = monsterLayout.transform.GetChild(i);
                    var canSetFeature = ((IconGame)Global.MiniGame).CanSetFeature(i - 1, Global.SelectedID);
                    childTransform.GetComponent<Image>().raycastTarget = canSetFeature;
                    childTransform.GetComponent<Image>().SetAlpha(canSetFeature ? 0.25f: 0f);
                    if (childTransform.childCount > 0)
                    {
                        childTransform.GetChild(0).GetComponent<Image>().raycastTarget = false;
                    }
                }
                
            }
        }
    }

    public void SelectFromIcon(string id, int index)
    {
        ((IconGame)Global.MiniGame).RemoveFeature(index);
        Select(id);
    }
    
    public void UpdateIcon()
    {
        if (pedestal.transform.childCount > 0)
        {
            foreach (Transform childTransform in pedestal.transform)
            {
                Destroy(childTransform.gameObject);
            }
        }
        
        var shape = ((IconGame)Global.MiniGame).GetShape();
        if (!(shape is null))
        {
            pedestal.transform.GetComponent<Image>().raycastTarget = false;
            monsterLayout = Instantiate(_shapePrefabs[shape], pedestal.transform, false);
            var features = ((IconGame)Global.MiniGame).GetFeatures();
            for (var i = 0; i < features.Length; i++)
            {
                if (features[i] is null) continue;
                var feature = Instantiate(_long.Contains(features[i]) ? longFeaturePrefab : shortFeaturePrefab, monsterLayout.transform.GetChild(i+1), false);
                feature.GetComponent<Feature>().id = features[i];
                feature.GetComponent<Feature>().index = i;
                feature.GetComponent<Image>().sprite = _iconFeatureSprites[features[i]];
                feature.GetComponent<Image>().SetNativeSize();
            }
        }
    }
    
    public void Cancel()
    {
        ((IconGame)Global.MiniGame).Fail();
        Return();
    }
    
    public void Submit()
    {
        if (((IconGame)Global.MiniGame).IsCorrect())
        {
            Return();
        }
    }
    
    public void Reset()
    {
        ((IconGame)Global.MiniGame).Clear();
        UpdateIcon();
    }
    
    void Return()
    {

        var name = Global.Altar.GetNameAsString();
        var title = Global.Altar.GetTitle() + " ";

        if (((IconGame)Global.MiniGame).IsFailed())
        {
            Global.Altar.Fail();
            Global.LoadingMessage = $"     {title.Capitalize()}{name} is offended by your pitiful attempt at an icon!    ";
            Global.NextScene = "Map";
            SceneManager.LoadScene("Scenes/LoadingScreen");
        }
        else
        {
            Global.Altar.CompleteGame("icon-game");
            Global.LoadingMessage = $"    The Old One is honored by your masterpiece.      ";
            Global.NextScene = "Altar";
            SceneManager.LoadScene("Scenes/LoadingScreen");
        }
    }

    public void Shift(string menu)
    {
        shapesMenu.SetActive(menu == "shapes");
        extremitiesMenu.SetActive(menu == "extremities");
        eyesMenu.SetActive(menu == "eyes");
        mouthsMenu.SetActive(menu == "mouths");
    }

    public void Look()
    {
        _looking = !_looking;
        eyeOpen.SetActive(!_looking);
        if (pedestal.transform.childCount > 0)
        {
            for (var i = 1; i < pedestal.transform.GetChild(0).childCount; i++)
            {
                var childTransform = pedestal.transform.GetChild(0).GetChild(i);
                if (childTransform.childCount > 0)
                {
                    childTransform.GetChild(0).GetComponent<Image>().raycastTarget = !_looking;
                }
            }
        }
        
        StartCoroutine(StartLook());
        StartCoroutine(monster.GetComponent<MonsterUI>().Fade(_looking, 0.3f));
    }

    IEnumerator StartLook()
    {
        if (_looking) 
            hiddenUI.SetActive(true);

        if (!_looking)
            StopCoroutine(_timer);

        eyeOpen.GetComponent<Image>().raycastTarget = false;
        eyeClosed.GetComponent<Image>().raycastTarget = false;

        var direction = _looking ? Vector3Int.down : Vector3Int.up;

        var minY = direction.y == -1 ? -30 / Camera.main.orthographicSize : 0;
        while (true)
        {
            ui.transform.position += (Vector3)direction * 0.3f;
            var x = direction.y == -1 ? ui.transform.position.y <= minY : ui.transform.position.y >= minY;
            if (x) break;
            yield return new WaitForSeconds(0.01f);
        }

        ui.transform.position = new Vector3(ui.transform.position.x, minY, ui.transform.position.z);
        
        if (!_looking)
        {
            hiddenUI.SetActive(false);
            eyeOpen.GetComponent<Image>().raycastTarget = true;
        }

        if (_looking)
        {
            eyeClosed.GetComponent<Image>().raycastTarget = true;
            _timer = StartCoroutine(Timer());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Backend;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUI : MonoBehaviour
{
    public GameObject pinPrefab;
    
    public GameObject map;
    public GameObject blackMap;
    public GameObject eye;
    public GameObject completionBar;
    public GameObject completionBarCorruption;
    public GameObject completionSlider;
    public GameObject restartButton;
    public GameObject submitButton;
    public GameObject cancelButton;
    
    public Sprite[] eyeSprites;
    
    public float zoomFactor = 0.0005f;
    public float panFactor = 0.1f;
    
    private GameObject _currentPin;
    private List<GameObject> _allPins = new List<GameObject>();
    private SpriteMask _mapMask;
    private Coroutine _blinking;

    private Vector3 _touchStart;
    private bool _multiTouch;

    private float _scaleMin;
    private float _scaleMax;
    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;
    private const float LeftBorder = 0.2f;
    private const float RightBorder = 0.2f;
    private const float TopBorder = 0.2f;
    private const float BottomBorder = 0.2f;

    private Color _defaultBGColor;
    private bool _completionOngoing;

    void Start()
    {
        var screenHeight = Camera.main.orthographicSize * 2f;
        var screenWidth = screenHeight * Camera.main.aspect;
        var mapHeight = map.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        var mapWidth = map.GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        _scaleMin = (screenWidth - RightBorder - LeftBorder) / mapWidth;
        _scaleMax = (screenHeight - TopBorder - BottomBorder) / mapHeight;
        _xMin = -screenWidth / 2f + LeftBorder;
        _xMax = screenWidth / 2f - RightBorder;
        _yMin = -screenHeight / 2f + BottomBorder;
        _yMax = screenHeight / 2f - TopBorder;

        _defaultBGColor = Camera.main.backgroundColor;

        foreach (var loc in Global.Game.GetCompletedAltars())
        {
            var point = GameToWorldPoint(loc);
            var pin = Instantiate(pinPrefab, new Vector3(point.x, point.y, map.transform.position.z-1), Quaternion.identity);
            pin.GetComponent<Pin>().Init(loc, true);
            _allPins.Add(pin);
        }

        if (Global.Game.HasPin() && !Global.Game.IsPinComplete())
        {
            var point = GameToWorldPoint(Global.Game.GetPin());
            _currentPin = Instantiate(pinPrefab, new Vector3(point.x, point.y, map.transform.position.z-2), Quaternion.identity);
            _currentPin.GetComponent<Pin>().Init(Global.Game.GetPin(), false);
        }

        ResetZoom();
        ResetPosition();

        _mapMask = map.GetComponentInChildren<SpriteMask>();
        UpdateMask();
        
        blackMap.SetActive(false);
        eye.SetActive(false);

        UpdateCompletionBar();

        if (Global.Game.IsComplete())
        {
            _completionOngoing = true;
            StartCoroutine(FadeOutMap());
        }
        
        UpdateButtons();
    }
    
    void Update()
    {
        if (Global.Game.IsComplete()) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            _touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _multiTouch = false;
        }

        if (Input.touchCount == 2)
        {
            _multiTouch = true;
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);
            
            var touch0Prev = touch0.position - touch0.deltaPosition;
            var touch1Prev = touch1.position - touch1.deltaPosition;

            var magnitudePrev = (touch0Prev - touch1Prev).magnitude;
            var magnitude = (touch0.position - touch1.position).magnitude;
            
            Zoom((magnitude - magnitudePrev) * zoomFactor);
        }
        else if (Input.GetMouseButton(0) && !_multiTouch)
        {
            var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _touchStart;
            Pan(direction * panFactor * map.transform.localScale.x);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = touch - _touchStart;
            if (direction.magnitude < 0.001)
            {
                if (!map.GetComponent<Collider2D>().OverlapPoint(touch) && !(_currentPin is null))
                {
                    if (Global.Game.IsPinEmpty())
                    {
                        Global.Game.DiscardPin();
                        DestroyCurrentPin();
                        _currentPin = null;
                        UpdateButtons();
                    }
                }
                else
                {
                    var coll = Physics2D.OverlapPoint(touch);
                    if (!(coll is null) && coll.GetComponent<Pin>())
                    {
                        var gamePin = coll.GetComponent<Pin>().save;
                        Global.Game.SetPin(gamePin);

                        if (!(_currentPin is null))
                        {
                            DestroyCurrentPin();
                        }

                        _currentPin = coll.gameObject;
                        UpdateButtons();
                    }
                    else if (!Global.Game.IsComplete())
                    {
                        var touchInGame = WorldToGamePoint(touch);
                        if (Global.Game.CanSetPin(touchInGame))
                        {
                            Global.Game.SetPin(touchInGame);
                            _currentPin = Instantiate(pinPrefab, new Vector3(touch.x, touch.y, transform.position.z),
                                Quaternion.identity);
                            _currentPin.GetComponent<Pin>().Init(Global.Game.GetPin(), false);
                            UpdateButtons();
                        }
                    }
                }
            }
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void UpdateCompletionBar()
    {
        var completionBarSize = completionBar.GetComponent<RectTransform>().rect.width * completionBar.transform.localScale.x;
        Debug.Log("ICONGAME: " + completionBarSize);
        completionBarCorruption.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, completionBarSize * Global.Game.GetCompletion());
        completionSlider.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, completionBarSize * Global.Game.GetCompletion());
    }

    IEnumerator FadeOutMap()
    {
        blackMap.SetActive(true);
        blackMap.GetComponent<SpriteRenderer>().SetAlpha(0f);

        var alpha = 0f;
        var speed = 0.1f;
        while (true)
        {
            alpha = Mathf.Clamp(alpha + speed, 0f, 1f);
            
            blackMap.GetComponent<SpriteRenderer>().SetAlpha(alpha);
            foreach (var pin in _allPins)
            {   
                pin.GetComponent<SpriteRenderer>().SetAlpha(1 - alpha);
            }

            if (alpha >= 1f) break;
            yield return new WaitForSeconds(0.005f);
        }
        
        StartCoroutine(FadeInEye());
    }

    IEnumerator FadeInEye()
    {
        eye.SetActive(true);
        eye.GetComponent<SpriteRenderer>().SetAlpha(0f);
        
        var alpha = 0f;
        var speed = 0.1f;
        
        while (true)
        {
            alpha = Mathf.Clamp(alpha + speed, 0f, 1f);
            
            eye.GetComponent<SpriteRenderer>().SetAlpha(alpha);
            Camera.main.backgroundColor = _defaultBGColor * (1 - alpha);

            if (alpha >= 1f) break;
            yield return new WaitForSeconds(0.005f);
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(OpenEye());
    }

    IEnumerator OpenEye()
    {
        var spriteIndex = 0;
        while (spriteIndex < 4)
        {
            spriteIndex += 1;
            eye.GetComponent<SpriteRenderer>().sprite = eyeSprites[spriteIndex];
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1f);
        
        _completionOngoing = false;
        UpdateButtons();
        
        _blinking = StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.value * 5f);

            var spriteIndex = 4;
            while (spriteIndex > 0)
            {
                spriteIndex -= 1;
                eye.GetComponent<SpriteRenderer>().sprite = eyeSprites[spriteIndex];
                yield return new WaitForSeconds(0.005f);
            }

            while (spriteIndex < 4)
            {
                spriteIndex += 1;
                eye.GetComponent<SpriteRenderer>().sprite = eyeSprites[spriteIndex];
                yield return new WaitForSeconds(0.005f);
            }
        }
    }

    public void UpdateButtons()
    {
        restartButton.SetActive(!_completionOngoing);
        submitButton.SetActive(Global.Game.HasPin());
        cancelButton.SetActive(Global.Game.HasPin() && !Global.Game.IsPinComplete());
    }

    public void Restart()
    {
        GameManager.Restart();
        
        if (_currentPin)
        {
            Destroy(_currentPin);
        }

        foreach (var pin in _allPins)
        {
            Destroy(pin);
        }
        _allPins = new List<GameObject>();
        
        if (!(_blinking is null))
            StopCoroutine(_blinking);
        
        eye.SetActive(false);
        blackMap.SetActive(false);
        Camera.main.backgroundColor = _defaultBGColor;

        ResetZoom();
        ResetPosition();

        UpdateMask();
        UpdateCompletionBar();
        UpdateButtons();
    }

    public void Submit()
    {
        Global.Game.InitAltar();
        Global.LoadingMessage = "    You have summoned an ancient Evil to the World ...    ";
        Global.NextScene = "Altar";
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void Cancel()
    {
        Global.Game.DiscardPin();
        DestroyCurrentPin();
        UpdateButtons();
    }

    void ResetPosition()
    {
        var position = new Vector3(0, 0, map.transform.position.z);
        map.transform.position = position;
    }

    void ResetZoom()
    {
        var scale = new Vector3(_scaleMin, _scaleMin, map.transform.localScale.z);
        map.transform.localScale = scale;
    }

    void Pan(Vector3 direction)
    {
        map.transform.position += direction;

        ClampMap();
        ResetPins();
    }

    void Zoom(float increment)
    {
        if (Mathf.Abs(increment) < 1e-15) return;
        
        var scaleFactor = Mathf.Clamp(map.transform.localScale.x + increment, _scaleMin, _scaleMax);
        var scale = new Vector3(scaleFactor, scaleFactor, map.transform.localScale.z);
        map.transform.localScale = scale;

        ClampMap();
        ResetPins();
    }

    void ClampMap()
    {
        var mapPos = map.transform.position;
        var mapWidth = map.GetComponent<SpriteRenderer>().sprite.bounds.size.x * map.transform.localScale.x;
        var mapHeight = map.GetComponent<SpriteRenderer>().sprite.bounds.size.y * map.transform.localScale.y;

        var x = Mathf.Clamp(
            mapPos.x, 
            _xMax - mapWidth / 2f,
            _xMin + mapWidth / 2f);
        var y = Mathf.Clamp(
            mapPos.y, 
            _yMin + mapHeight / 2f,  
            _yMax - mapHeight / 2f);

        var position = new Vector3(x, y, mapPos.z);
        map.transform.position = position;
    }

    public void DestroyCurrentPin()
    {
        Destroy(_currentPin);
    }
    
    void UpdateMask()
    {
        var transparent = new Color(0, 0, 0, 0);
        var solid = new Color(0, 0, 0, 1);
        var map = Global.Game.GetMap().Transpose();
        _mapMask.sprite.texture.SetPixels(map.Cast<bool>().Select(x => x ? transparent : solid).ToArray());
        _mapMask.sprite.texture.Apply();
    }

    void ResetPins()
    {
        foreach (var pin in _allPins)
        {
            var point = GameToWorldPoint(pin.GetComponent<Pin>().save);
            pin.transform.position = new Vector3(point.x, point.y, map.transform.position.z-1);
        }
        
        if (_currentPin)
        {
            var point = GameToWorldPoint(_currentPin.GetComponent<Pin>().save);
            _currentPin.transform.position = new Vector3(point.x, point.y, map.transform.position.z-2);
        }
    }

    Vector3 GameToWorldPoint(Vec2 gamePoint)
    {
        var mapPos = map.transform.position;
        var mapSize = map.GetComponent<SpriteRenderer>().sprite.bounds.size.EMultiply(map.transform.localScale);
        var gameSize = Global.Game.GetSize();

        var gamePointInWorld = ((Vector3)gamePoint).EDivide((Vector3)gameSize).EMultiply(mapSize);
        return gamePointInWorld + mapPos - mapSize / 2f;
    }

    Vec2 WorldToGamePoint(Vector3 worldPoint)
    {
        var mapPos = map.transform.position;
        var mapSize = map.GetComponent<SpriteRenderer>().sprite.bounds.size.EMultiply(map.transform.localScale);
        var gameSize = Global.Game.GetSize();

        var pointRelative = worldPoint - mapPos + mapSize / 2f;
        return pointRelative.EDivide(mapSize).EMultiply((Vector3)gameSize);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;
using Backend;

public class MapController : MonoBehaviour
{
    public MapUI mapUI;
    public SpriteMask mask;
    public float zoomFactor = 0.0005f;
    public float panFactor = 0.1f;

    public GameObject pinPrefab;
    private GameObject currentPin;
    private List<GameObject> allPins = new List<GameObject>();

    private Vector3 touchStart;
    private bool multiTouch;

    private float scaleMin;
    private float scaleMax;
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;
    private float leftBorder = 0.2f;
    private float rightBorder = 0.2f;
    private float topBorder = 0.2f;
    private float bottomBorder = 0.2f;
    
    void Start()
    {
        var screenHeight = Camera.main.orthographicSize * 2f;
        var screenWidth = screenHeight * Camera.main.aspect;
        var mapHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        var mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        scaleMin = (screenWidth - rightBorder - leftBorder) / mapWidth;
        scaleMax = (screenHeight - topBorder - bottomBorder) / mapHeight;
        xMin = -screenWidth / 2f + leftBorder;
        xMax = screenWidth / 2f - rightBorder;
        yMin = -screenHeight / 2f + bottomBorder;
        yMax = screenHeight / 2f - topBorder;
        
        
        foreach (var loc in Global.Game.GetCompletedAltars())
        {
            var point = GameToWorldPoint(loc);
            var pin = Instantiate(pinPrefab, new Vector3(point.x, point.y, transform.position.z-1), Quaternion.identity);
            pin.GetComponent<Pin>().Init(loc, true);
            allPins.Add(pin);
        }

        if (Global.Game.HasPin() && !Global.Game.IsPinComplete())
        {
            var point = GameToWorldPoint(Global.Game.GetPin());
            currentPin = Instantiate(pinPrefab, new Vector3(point.x, point.y, transform.position.z-2), Quaternion.identity);
            currentPin.GetComponent<Pin>().Init(Global.Game.GetPin(), false);
        }

        ResetZoom();
        ResetPosition();

        UpdateMask();

        if (Global.Game.IsComplete())
        {
            Debug.Log("You won!");
        }
        else
        {
            Debug.Log($"Completion is at {Mathf.RoundToInt(Global.Game.GetCompletion() * 100)}%!");
        }
    }

    public void Restart()
    {
        if (currentPin)
        {
            Destroy(currentPin);
        }

        foreach (var pin in allPins)
        {
            Destroy(pin);
        }

        ResetZoom();
        ResetPosition();

        UpdateMask();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            multiTouch = false;
        }

        if (Input.touchCount == 2)
        {
            multiTouch = true;
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);
            
            var touch0Prev = touch0.position - touch0.deltaPosition;
            var touch1Prev = touch1.position - touch1.deltaPosition;

            var magnitudePrev = (touch0Prev - touch1Prev).magnitude;
            var magnitude = (touch0.position - touch1.position).magnitude;
            
            Zoom((magnitude - magnitudePrev) * zoomFactor);
        }
        else if (Input.GetMouseButton(0) && !multiTouch)
        {
            var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - touchStart;
            Pan(direction * panFactor * transform.localScale.x);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = touch - touchStart;
            if (direction.magnitude < 0.001)
            {
                if (!GetComponent<Collider2D>().OverlapPoint(touch) && !(currentPin is null))
                {
                    if (Global.Game.IsPinEmpty())
                    {
                        Global.Game.DiscardPin();
                        DestroyCurrentPin();
                        currentPin = null;
                        mapUI.UpdateButtons();
                    }
                }
                else
                {
                    var coll = Physics2D.OverlapPoint(touch);
                    Debug.Log($"Collision with {coll}!");
                    if (!(coll is null) && coll.GetComponent<Pin>())
                    {
                        Debug.Log("Collider is Pin!");
                        var gamePin = coll.GetComponent<Pin>().save;
                        Global.Game.SetPin(gamePin);

                        if (!(currentPin is null))
                        {
                            DestroyCurrentPin();
                        }

                        currentPin = coll.gameObject;
                        mapUI.UpdateButtons();
                    }
                    else if (!Global.Game.IsComplete())
                    {
                        var touchInGame = WorldToGamePoint(touch);
                        if (Global.Game.CanSetPin(touchInGame))
                        {
                            Global.Game.SetPin(touchInGame);
                            currentPin = Instantiate(pinPrefab, new Vector3(touch.x, touch.y, transform.position.z),
                                Quaternion.identity);
                            currentPin.GetComponent<Pin>().Init(Global.Game.GetPin(), false);
                            mapUI.UpdateButtons();
                        }
                    }
                }
            }
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void ResetPosition()
    {
        var position = new Vector3(0, 0, transform.position.z);
        transform.position = position;
    }

    void ResetZoom()
    {
        var scale = new Vector3(scaleMin, scaleMin, transform.localScale.z);
        transform.localScale = scale;
    }

    void Pan(Vector3 direction)
    {
        transform.position += direction;

        ClampMap();
        ResetPins();
    }

    void Zoom(float increment)
    {
        if (Mathf.Abs(increment) < 1e-15) return;
        
        var scaleFactor = Mathf.Clamp(transform.localScale.x + increment, scaleMin, scaleMax);
        var scale = new Vector3(scaleFactor, scaleFactor, transform.localScale.z);
        transform.localScale = scale;

        ClampMap();
        ResetPins();
    }

    void ClampMap()
    {
        var mapPos = transform.position;
        var mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x;
        var mapHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.localScale.y;

        var x = Mathf.Clamp(
            mapPos.x, 
            xMax - mapWidth / 2f,
            xMin + mapWidth / 2f);
        var y = Mathf.Clamp(
            mapPos.y, 
            yMin + mapHeight / 2f,  
            yMax - mapHeight / 2f);

        var position = new Vector3(x, y, mapPos.z);
        transform.position = position;
    }

    public void DestroyCurrentPin()
    {
        Destroy(currentPin);
    }

    public void TempCompletePin()
    {
        currentPin.GetComponent<Pin>().SetCorrupted();
        allPins.Add(currentPin);
        currentPin = null;
        UpdateMask();
    }

    void UpdateMask()
    {
        var transparent = new Color(0, 0, 0, 0);
        var solid = new Color(0, 0, 0, 1);
        var map = Global.Game.GetMap().Transpose();
        mask.sprite.texture.SetPixels(map.Cast<bool>().Select(x => x ? transparent : solid).ToArray());
        mask.sprite.texture.Apply();
    }

    void ResetPins()
    {
        foreach (var pin in allPins)
        {
            var point = GameToWorldPoint(pin.GetComponent<Pin>().save);
            pin.transform.position = new Vector3(point.x, point.y, transform.position.z-1);
        }
        
        if (currentPin)
        {
            var point = GameToWorldPoint(currentPin.GetComponent<Pin>().save);
            currentPin.transform.position = new Vector3(point.x, point.y, transform.position.z-2);
        }
    }

    Vector3 GameToWorldPoint(Vec2 gamePoint)
    {
        var mapPos = transform.position;
        var mapSize = GetComponent<SpriteRenderer>().sprite.bounds.size.EMultiply(transform.localScale);
        var gameSize = Global.Game.GetSize();

        var gamePointInWorld = ((Vector3)gamePoint).EDivide((Vector3)gameSize).EMultiply(mapSize);
        return gamePointInWorld + mapPos - mapSize / 2f;
    }

    Vec2 WorldToGamePoint(Vector3 worldPoint)
    {
        var mapPos = transform.position;
        var mapSize = GetComponent<SpriteRenderer>().sprite.bounds.size.EMultiply(transform.localScale);
        var gameSize = Global.Game.GetSize();

        var pointRelative = worldPoint - mapPos + mapSize / 2f;
        return pointRelative.EDivide(mapSize).EMultiply((Vector3)gameSize);
    }
}

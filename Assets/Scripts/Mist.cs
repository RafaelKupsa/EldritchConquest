using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mist : MonoBehaviour
{
    public Sprite[] sprites;

    private Vector3Int _direction;
    private float _speed;

    private bool _wiggle = true;
    private float _maxOffset;
    private Vector3 _defaultPosition;
    private float _xFrequency;
    private float _yFrequency;

    private float _screenLeftBorder;
    private float _screenRightBorder;
    
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites.Choice();
        var scale = Random.value;
        transform.localScale *= Mathf.Lerp(0.5f, 3f, scale);
        GetComponent<SpriteRenderer>().SetAlpha(Mathf.Lerp(0.4f, 0.2f, scale));
        if (Random.value < 0.5)
        {
            transform.localScale = transform.localScale.EMultiply(new Vector3(-1, 1, 1));
        }
        
        _defaultPosition = transform.position;
        _maxOffset = Random.value / 10;
        _xFrequency = Random.value / 100;
        _yFrequency = Random.value / 100;
        
        var screenHeight = Camera.main.orthographicSize * 2f;
        var screenWidth = screenHeight * Camera.main.aspect;
        _screenLeftBorder = -screenWidth / 2f;
        _screenRightBorder = screenWidth / 2f;
    }

    void Update()
    {
        if (_wiggle)
        {
            var x = _defaultPosition.x + _maxOffset * Math.Sin(_xFrequency * Time.frameCount);
            var y = _defaultPosition.y + _maxOffset * Math.Sin(_yFrequency * Time.frameCount);
            transform.position = new Vector3((float)x, (float)y, transform.position.z);
        }
        else
        {
            if (_direction.x == -1)
            {
                var rightBorder = GetComponent<SpriteRenderer>().bounds.max.x;
                if (rightBorder > _screenLeftBorder)
                {
                    transform.position += (Vector3)_direction * _speed;
                }
            }
            else if (_direction.x == 1)
            {
                var leftBorder = GetComponent<SpriteRenderer>().bounds.min.x;
                if (leftBorder < _screenRightBorder)
                {
                    transform.position += (Vector3)_direction * _speed;
                }
            }
        }
    }

    public void Reveal()
    {
        _direction = Random.value < 0.5 ? Vector3Int.left : Vector3Int.right;
        _speed = Mathf.Lerp( 0.005f, 0.02f,Random.value);
        _wiggle = false;
    }
}

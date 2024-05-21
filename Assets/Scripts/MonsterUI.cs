using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MonsterUI : MonoBehaviour
{
    public GameObject shortFeaturePrefab;
    public GameObject longFeaturePrefab;
    public GameObject[] shapePrefabs;

    private Dictionary<string, Sprite> _featureSprites;
    private Dictionary<string, GameObject> _shapePrefabs;
    
    private float _maxOffset;
    private Vector3 _defaultPosition;
    private float _frequency;

    private bool _visible;
    
    private readonly string[] _long = { "stalkeye", "wing", "bugleg", "tentacle", "arm", "gills", "claw" };

    private void Awake()
    {
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
        }.ToDictionary(x => x.Key, x => Resources.Load<Sprite>($"Sprites/monster_{x.Value}"));

        _shapePrefabs = new List<string>
        {
            "blob", "peanut", "skull", "sphere", "star", "worm"
        }.Zip(shapePrefabs, (s, o) => new { s, o }).ToDictionary(x => x.s, x => x.o);
    }

    void Start()
    {
        UpdateMonster();
        if (_visible)
        {
            StartCoroutine(Fade(true, 0.05f));
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().SetAlpha(0f);
            for (var i = 1; i < transform.GetChild(0).childCount; i++)
            {
                var childTransform = transform.GetChild(0).GetChild(i);
                if (childTransform.childCount > 0)
                    childTransform.GetChild(0).GetComponent<Image>().SetAlpha(0f);
            }
        }
            
        _defaultPosition = transform.GetChild(0).localPosition;
        _maxOffset = Random.value * 10 + 10;
        _frequency = Random.value / 100;
    }

    void Update()
    {
        var y = _defaultPosition.y + _maxOffset * Math.Sin(_frequency * Time.frameCount);
        transform.GetChild(0).localPosition = new Vector3(transform.GetChild(0).localPosition.x, (float)y, transform.GetChild(0).localPosition.z);
    }

    void UpdateMonster()
    {
        var shape = Global.Altar.GetShape();
        if (!(shape is null))
        {
            var layout = Instantiate(_shapePrefabs[shape], transform, false);
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

    public void SetVisible()
    {
        _visible = true;
    }

    public IEnumerator Fade(bool revealing, float speed)
    {
        var alpha = revealing ? 0f : 1f;
        while (true)
        {
            var newAlpha = revealing ? alpha + speed : alpha - speed;
            alpha = Mathf.Clamp(newAlpha, 0f, 1f);
            transform.GetChild(0).GetChild(0).GetComponent<Image>().SetAlpha(alpha);
            for (var i = 1; i < transform.GetChild(0).childCount; i++)
            {
                var childTransform = transform.GetChild(0).GetChild(i);
                if (childTransform.childCount > 0)
                    childTransform.GetChild(0).GetComponent<Image>().SetAlpha(alpha);
            }
            var x = revealing ? alpha >= 1f : alpha <= 0f;
            if (x) break;
            yield return new WaitForSeconds(0.1f);
        }
    }
}

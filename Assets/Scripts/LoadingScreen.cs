using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingScreen : MonoBehaviour
{
    public Image decoration;
    public Text text;
    public GameObject loadingIcon;
    public GameObject continueButton;
    public GameObject infoButton;
    public GameObject infoText;
    
    public Sprite[] decorationSprites;

    private void Awake()
    {
        decoration.sprite = decorationSprites.Choice();
        decoration.SetNativeSize();
        /*
        if (decoration.sprite.name.Contains("extremities") || decoration.sprite.name.Contains("stalkeye"))
        {
            decoration.transform.Rotate(new Vector3(0, 0, Random.value < 0.5 ? -90 : 90));
        }
        */
        infoText.SetActive(false);
    }

    void Start()
    {
        loadingIcon.SetActive(false);
        
        if (Global.NextScene == "Map" && !(Global.Altar is null) && Global.Altar.IsFailed())
        {
            Global.Game.FailAltar();
        }

        if (Global.NextScene == "Map" && !(Global.Altar is null) && Global.Altar.IsComplete())
        {
            continueButton.SetActive(false);
            loadingIcon.SetActive(true);
            StartCoroutine(LoadMap());
        }

        text.text = Global.LoadingMessage;

        InitInfoText();
    }

    IEnumerator LoadMap()
    {
        yield return new WaitForSeconds(0.5f);
        Global.Game.CompleteAltar();
        
        loadingIcon.SetActive(false);
        continueButton.SetActive(true);
    }

    void InitInfoText()
    {
        var txt = infoText.GetComponent<Text>();
        switch (Global.NextScene)
        {
            case "Altar":
                txt.text = "To convince the Old One to join you on your conquest of Earth, complete all of the three tasks. If you fail even at one, the Old One will leave.";
                break;
            case "NameGame":
                txt.text = "Decode the Old One's name on the front of the altar by finding out the letter correspondences between the text and its transcription in the book. Pull the letters into the empty slots. Beware of the time!";
                break;
            case "IconGame":
                txt.text = "Build an exact copy of the Old One by dragging a shape and its extremities, eyes and mouths onto the pedestal. You can look at the monster by pressing the eye but be aware that the time will go down if you do!";
                break;
            case "SacrificeGame":
                txt.text = "Find out the Old One's preferred combination of organs on the plate. Each time you press the checkmark button, the Old One will give you feedback:\n NYOOOOM = I like one of the organs and also its amount.\n NYUM = I like one of the organs but not its amount.\n BLAR'GH = I don't like one of the organs.\n NYO? = There is an organ missing.\n OOO? = I want at least 5 organs.";
                break;
            default:
                txt.text = "";
                infoButton.SetActive(false);
                break;
        }
    }

    public void Continue()
    {
        SceneManager.LoadScene($"Scenes/{Global.NextScene}");
    }

    public void Info()
    {
        infoText.SetActive(!infoText.activeSelf);
    }
}

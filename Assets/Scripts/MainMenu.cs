using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] allLevels = null;
    [SerializeField] private GameObject menu = null;
    [SerializeField] private GameObject levelsBar = null;

    private void Start()
    {
        PlayerPrefsOnStart();
        InteractibleLevels();
    }

    #region Button Methods
    public void NewGame()
    {
        GetComponent<Fade>().FadeAppear("Plansza2");
    }

    public void LoadGame()
    {
        menu.GetComponent<Animation>().clip = menu.GetComponent<Animation>().GetClip("MainMenuDisappear");
        menu.GetComponent<Animation>().Play();
        levelsBar.GetComponent<Animation>().clip = levelsBar.GetComponent<Animation>().GetClip("LevelsBarAppear");
        levelsBar.GetComponent<Animation>().Play();

        GetComponent<Fade>().MainMenuFadeAppear();
    }

    public void Back()
    {
        menu.GetComponent<Animation>().clip = menu.GetComponent<Animation>().GetClip("MainMenuAppear");
        menu.GetComponent<Animation>().Play();
        levelsBar.GetComponent<Animation>().clip = levelsBar.GetComponent<Animation>().GetClip("LevelsBarDisappear");
        levelsBar.GetComponent<Animation>().Play();

        GetComponent<Fade>().MainMenuFadeDisappear();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    private void PlayerPrefsOnStart() // 0 - false, 1 - true
    {
        if (!PlayerPrefs.HasKey("Plansza3"))
        {
            PlayerPrefs.SetInt("Plansza3", 1);
            PlayerPrefs.SetInt("Plansza4", 1);
            PlayerPrefs.SetInt("Plansza5", 1);
            PlayerPrefs.SetInt("Plansza6", 1);
            PlayerPrefs.SetInt("Plansza7", 1);
            PlayerPrefs.SetInt("Plansza8", 1);
            PlayerPrefs.SetInt("Plansza9", 1);
            PlayerPrefs.SetInt("Plansza10", 1);
            PlayerPrefs.SetInt("Plansza11", 1);
            PlayerPrefs.SetInt("Plansza12", 1);
            PlayerPrefs.SetInt("Plansza13", 1);
            PlayerPrefs.SetInt("Plansza14", 1);
            PlayerPrefs.SetInt("Plansza15", 1); // Removed
            PlayerPrefs.SetInt("Plansza16", 1);
            PlayerPrefs.SetInt("Plansza17", 1);
            PlayerPrefs.SetInt("Plansza18", 1);
            PlayerPrefs.SetInt("Plansza19", 1);
            PlayerPrefs.SetInt("Plansza20", 1);
            PlayerPrefs.SetInt("Plansza21", 1);
            PlayerPrefs.SetInt("Plansza22", 1);
            PlayerPrefs.SetInt("Plansza23", 1);
            PlayerPrefs.SetInt("Plansza24", 1);
            PlayerPrefs.SetInt("Plansza26", 1);
            PlayerPrefs.SetInt("Plansza27", 1);
        }
    }

    private void InteractibleLevels()
    {
        foreach (var level in allLevels)
        {
            if (PlayerPrefs.GetInt("Plansza" + level.GetComponent<SaveButton>().levelNumber) == 0)
            {
                level.GetComponent<SaveButton>().GetLockImage.gameObject.SetActive(true);
                level.GetComponent<Button>().interactable = false;
            }
            else
            {
                level.GetComponent<SaveButton>().GetLockImage.gameObject.SetActive(false);
                level.GetComponent<Button>().interactable = true;
            }
        }
    }
}

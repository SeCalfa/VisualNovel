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
        GetComponent<Fade>().FadeAppear("IntroScene");
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
            PlayerPrefs.SetInt("Plansza3", 0);
            PlayerPrefs.SetInt("Plansza4", 0);
            PlayerPrefs.SetInt("Plansza5", 0);
            PlayerPrefs.SetInt("Plansza6", 0);
            PlayerPrefs.SetInt("Plansza7", 0);
            PlayerPrefs.SetInt("Plansza8", 0);
            PlayerPrefs.SetInt("Plansza9", 0);
            PlayerPrefs.SetInt("Plansza10", 0);
            PlayerPrefs.SetInt("Plansza11", 0);
            PlayerPrefs.SetInt("Plansza12", 0);
            PlayerPrefs.SetInt("Plansza13", 0);
            PlayerPrefs.SetInt("Plansza14", 0);
            PlayerPrefs.SetInt("Plansza15", 0);
            PlayerPrefs.SetInt("Plansza16", 0);
            PlayerPrefs.SetInt("Plansza17", 0);
        }
    }

    private void InteractibleLevels()
    {
        for (int i = 3; i < 17; i++)
        {
            if (PlayerPrefs.GetInt("Plansza" + i.ToString()) == 0)
            {
                allLevels[i - 3].GetComponent<SaveButton>().GetLockImage.gameObject.SetActive(true);
                allLevels[i - 3].GetComponent<Button>().interactable = false;
            }
            else
            {
                allLevels[i - 3].GetComponent<SaveButton>().GetLockImage.gameObject.SetActive(false);
                allLevels[i - 3].GetComponent<Button>().interactable = true;
            }
        }
    }
}

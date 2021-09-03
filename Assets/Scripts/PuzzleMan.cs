using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleMan : MonoBehaviour
{

    public static PuzzleMan self;

    internal List<bool> plantedPhotos = new List<bool>() { false, false, false, false, false };
    internal List<bool> plantedCorrect = new List<bool>() { false, false, false, false, false };

    [SerializeField]
    private GameObject NextButton;

    private void Awake()
    {
        self = this;
        NextButton.SetActive(false);
    }

    internal void PlantPhoto(int number)
    {
        int totalPlanted = 0;
        plantedPhotos[number] = true;

        foreach (var p in plantedPhotos)
        {
            if(p == true)
            {
                totalPlanted += 1;

                if (totalPlanted == 5)
                    NextButton.SetActive(true);
            }
        }
    }

    internal void ReturnPhoto()
    {
        int totalPlanted = 0;

        foreach (var p in plantedPhotos)
        {
            if (p == true)
            {
                totalPlanted += 1;

                if (totalPlanted != 5)
                    NextButton.SetActive(false);
            }
        }
    }

    public void NextLevel()
    {
        int totalCorrectPhotos = 0;

        foreach (var p in plantedCorrect)
        {
            if (p)
                totalCorrectPhotos += 1;
        }

        if (totalCorrectPhotos == 5)
        {
            PlayerPrefs.SetInt("IsAlive", 2);
            SceneManager.LoadScene("Plansza27_Correct");
        }
        else
        {
            PlayerPrefs.SetInt("IsAlive", 1);
            SceneManager.LoadScene("Plansza27_Incorrect");
        }
    }

}

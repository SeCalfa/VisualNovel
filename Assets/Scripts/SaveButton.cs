using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private Image lockImage = null;
    [SerializeField] internal int levelNumber;

    public Image GetLockImage { get { return lockImage; } }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Plansza" + levelNumber);
    }
}

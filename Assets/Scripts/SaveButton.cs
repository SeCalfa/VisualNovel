using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private Image picture;
    [SerializeField] internal int levelNumber;

    public Image GetPicture { get { return picture; } }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Plansza" + levelNumber);
    }
}

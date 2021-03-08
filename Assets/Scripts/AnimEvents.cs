using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimEvents : MonoBehaviour
{
    [SerializeField] private Dialogs dialogs = null;

    public IEnumerator LastPaperDropped()
    {
        yield return new WaitForSeconds(3f);
        dialogs.LastPaperDropped();
    }

    public void LastPaper() // After first paper / Plansza 8
    {
        if(SceneManager.GetActiveScene().name == "Plansza8")
        {
            StartCoroutine(LastPaperDropped());
            PlayerPrefs.SetInt("Plansza8", 1);
        }
    }
}

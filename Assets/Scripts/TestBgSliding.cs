using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestBgSliding : MonoBehaviour
{

    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private Image bg1;
    [SerializeField]
    private Image bg2;
    [SerializeField]
    private Sprite[] allBG;

    private bool isContinueActive = false;
    private float timer = 2.0f;
    private int currentSprite = 0;

    #region Unity Callbacks

    private void Start()
    {
        StartCoroutine(FadeDisappear());
    }

    private void Update()
    {
        NextBackground();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    #endregion

    private void NextBackground()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            isContinueActive = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isContinueActive)
        {
            if(currentSprite == 22)
            {
                StartCoroutine(FadeAppear());

                return;
            }

            if(bg1.GetComponent<Canvas>().sortingOrder > bg2.GetComponent<Canvas>().sortingOrder)
            {
                StartCoroutine(NewImageAppear(bg2));
                bg2.GetComponent<Canvas>().sortingOrder += 2;
                bg2.sprite = allBG[currentSprite];
            }
            else
            {
                StartCoroutine(NewImageAppear(bg1));
                bg1.GetComponent<Canvas>().sortingOrder += 2;
                bg1.sprite = allBG[currentSprite];
            }

            timer = 2.0f;
            isContinueActive = false;
            currentSprite += 1;
        }
    }


    #region Corutines

    private IEnumerator FadeAppear()
    {
        Reset:
        fadeImage.color = new Color(1, 1, 1, fadeImage.color.a + 0.02f);
        yield return new WaitForSeconds(0.01f);
        if (fadeImage.color.a < 1)
            goto Reset;

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FadeDisappear()
    {
        fadeImage.color = new Color(1, 1, 1, 1);

    Reset:
        fadeImage.color = new Color(1, 1, 1, fadeImage.color.a - 0.02f);
        yield return new WaitForSeconds(0.01f);
        if (fadeImage.color.a > 0)
            goto Reset;
    }

    private IEnumerator NewImageAppear(Image newImage)
    {
        newImage.color = new Color(1, 1, 1, 0);

    Reset:
        newImage.color = new Color(1, 1, 1, newImage.color.a + 0.02f);
        yield return new WaitForSeconds(0.01f);
        if (newImage.color.a < 1)
            goto Reset;
    }

    #endregion

}

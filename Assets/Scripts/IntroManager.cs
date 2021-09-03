using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{

    private VideoPlayer player;
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        player = GetComponent<VideoPlayer>();
        player.loopPointReached += OnIntroEnd;

        StartCoroutine(FadeDisappear());
    }

    private void OnIntroEnd(VideoPlayer player)
    {
        StartCoroutine(FadeStart());
    }

    private IEnumerator FadeStart()
    {
        Reset:
        yield return new WaitForSecondsRealtime(0.01f);
        fadeImage.color = new Color(0, 0, 0, fadeImage.color.a + 0.01f);
        if (fadeImage.color.a < 1)
            goto Reset;

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FadeDisappear()
    {
        fadeImage.color = new Color(0, 0, 0, 1);

    Reset:
        yield return new WaitForSecondsRealtime(0.01f);
        fadeImage.color = new Color(0, 0, 0, fadeImage.color.a - 0.025f);
        if (fadeImage.color.a > 0)
            goto Reset;
    }

}

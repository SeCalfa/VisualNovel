using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{

    [SerializeField] internal Image fade;
    [SerializeField] private bool isMainMenu = false;
    [SerializeField] private Transform fadeAfterGameStart = null;

    [SerializeField]
    private Image fadeOnStart;

    private float fadeAlpha = 0;

    internal bool isFadeDisappeared = false;
    internal bool isNextLevelActive = true;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            StartCoroutine(FadeOnStart());
    }

    internal void FadeAppear(string sceneName)
    {
        if (isMainMenu)
            fade.transform.SetParent(fadeAfterGameStart);
        StartCoroutine(WhiteFadeAppear(sceneName));
    }

    internal void BlackFadeAppear(bool isLoadLevel, string scene)
    {
        StartCoroutine(BlackFadeAppearCor(isLoadLevel, scene));
    }

    internal void BlackFadeDisappear()
    {
        StartCoroutine(BlackFadeDisappearCor());
    }

    internal void FadeDisappear()
    {
        StartCoroutine(WhiteFadeDisappear());
    }

    internal void MainMenuFadeAppear()
    {
        StopAllCoroutines();
        StartCoroutine(MMenuFadeAppear());
    }

    internal void MainMenuFadeDisappear()
    {
        StartCoroutine(MMenuFadeDisappear());
    }

    private IEnumerator BlackFadeAppearCor(bool isLoadLevel, string scene)
    {
        fadeAlpha = 0;
        fade.raycastTarget = true;
    Reset:
        fadeAlpha += 0.02f;
        fade.color = new Color(0, 0, 0, fadeAlpha);
        yield return new WaitForSeconds(0.05f);
        if (fadeAlpha < 1.5f)
            goto Reset;

        if(isLoadLevel)
            SceneManager.LoadScene(scene);
    }

    private IEnumerator BlackFadeDisappearCor()
    {
        fadeAlpha = 1;
        fade.raycastTarget = true;
    Reset:
        fadeAlpha -= 0.02f;
        fade.color = new Color(0, 0, 0, fadeAlpha);
        yield return new WaitForSeconds(0.05f);
        if (fadeAlpha > 0.0f)
            goto Reset;
    }

    private IEnumerator WhiteFadeAppear(string scene)
    {
        fadeAlpha = 0;
        fade.raycastTarget = true;
    Reset:
        fadeAlpha += 0.02f;
        fade.color = new Color(1, 1, 1, fadeAlpha);
        yield return new WaitForSeconds(0.05f);
        if (fadeAlpha < 1.5f)
            goto Reset;

        SceneManager.LoadScene(scene);
    }

    private IEnumerator WhiteFadeDisappear()
    {
        fadeAlpha = 1;
        fade.raycastTarget = true;
    Reset:
        fadeAlpha -= 0.02f;
        fade.color = new Color(1, 1, 1, fadeAlpha);
        yield return new WaitForSeconds(0.05f);
        if (fadeAlpha > 0)
            goto Reset;

        isFadeDisappeared = true;
    }

    private IEnumerator MMenuFadeAppear()
    {
        fadeAlpha = 0;
    Reset:
        fadeAlpha += 0.028f;
        fade.color = new Color(0, 0, 0, fadeAlpha);
        yield return new WaitForSeconds(0.01f);
        if (fadeAlpha < 0.7f)
            goto Reset;
    }

    private IEnumerator MMenuFadeDisappear()
    {
        fadeAlpha = 0.7f;
    Reset:
        fadeAlpha -= 0.028f;
        fade.color = new Color(0, 0, 0, fadeAlpha);
        yield return new WaitForSeconds(0.01f);
        if (fadeAlpha > 0)
            goto Reset;
    }

    // --------

    private IEnumerator FadeOnStart()
    {
        fadeOnStart.color = new Color(0, 0, 0, 1);

    Reset:
        yield return new WaitForSecondsRealtime(0.01f);
        fadeOnStart.color = new Color(0, 0, 0, fadeOnStart.color.a - 0.01f);
        if (fadeOnStart.color.a > 0)
            goto Reset;

        Destroy(fadeOnStart);
    }

}

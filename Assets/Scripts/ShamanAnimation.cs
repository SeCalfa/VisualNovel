using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShamanAnimation : MonoBehaviour
{
    [SerializeField]
    private Image backgroundFront;
    [Space]
    [SerializeField]
    private Sprite[] shamanBackgrounds;
    [Space]
    [SerializeField]
    private float frameRate = 1.0f;

    private int currentState = 0;
    private int pastState = 0;
    private int maxState = 6;

    private float currentTime = 1.0f;

    private float fadeTime = 0;
    private bool fadeStart = false;

    private void Update()
    {
        Timer();
        Fade();
    }

    private void Timer()
    {
        currentTime -= Time.deltaTime;
        if(currentTime <= 0)
        {
            currentTime = frameRate;
            pastState = currentState;
            currentState += 1;
            if(currentState > maxState)
            {
                currentState = 0;
            }

            GetComponent<Image>().sprite = shamanBackgrounds[pastState];
            backgroundFront.sprite = shamanBackgrounds[currentState];

            fadeStart = true;
            fadeTime = 0;
        }
    }

    private void Fade()
    {
        if (fadeStart)
        {
            fadeTime += Time.deltaTime;

            float alpha = Mathf.Lerp(0, 1, fadeTime / frameRate);
            print(alpha);
            backgroundFront.color = new Color(1, 1, 1, alpha);

            if (fadeTime >= frameRate)
            {
                fadeStart = false;
            }
        }
    }
    
}

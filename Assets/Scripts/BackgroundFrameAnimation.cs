using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFrameAnimation : MonoBehaviour
{
    [SerializeField]
    private Image backgroundFront;
    [Space]
    [SerializeField]
    private Sprite[] backgrounds;
    [Space]
    [SerializeField]
    private float frameRate = 1.0f;

    [Space]
    [SerializeField]
    private bool isFrameFadeFromStart = true;

    public enum FadingType
    {
        Loop,
        LoopFull,
        Single,
        DoubleSide,
        Plansza40
    }

    [Space]
    [SerializeField]
    private FadingType fadeType;

    internal bool singleFadeActivate = false;

    private int currentState = 0;
    private int pastState = 0;
    private int maxState;

    private float currentTime = 1.0f;

    private float fadeTime = 0;
    private bool fadeStart = false;
    private int direction = 1;

    internal bool frameFadeStart = true;

    private void Awake()
    {
        currentTime = frameRate;
        maxState = backgrounds.Length - 1;

        if (!isFrameFadeFromStart)
            frameFadeStart = false;
    }

    private void Update()
    {
        TypeController();
    }

    private void TypeController()
    {
        if (frameFadeStart)
        {
            if(fadeType == FadingType.Single)
            {
                TimerSingle();
                FadeSingle();
            }
            else if(fadeType == FadingType.Loop)
            {
                TimerMulty();
                Loop();
            }
            else if (fadeType == FadingType.LoopFull)
            {
                TimerMulty();
                LoopFull();
            }
            else if(fadeType == FadingType.DoubleSide)
            {
                DoubleFade();
            }
            else if(fadeType == FadingType.Plansza40)
            {
                Plansza40Fade();
                Loop();
            }
        }
    }

    private void Plansza40Fade()
    {
        currentTime -= Time.deltaTime; // alpha down
        float alphaGrown = 1 - currentTime;

        if(currentTime <= 0)
        {
            currentTime = frameRate;
            pastState = currentState;
            currentState += 1;

            if (currentState == 3)
            {
                frameRate = 0.5f;
            }

            if (currentState == maxState)
            {
                currentState = 3;
            }

            GetComponent<Image>().sprite = backgrounds[pastState];
            backgroundFront.sprite = backgrounds[currentState];

            fadeStart = true;
            fadeTime = 0;
        }
    }

    private void TimerMulty()
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

            GetComponent<Image>().sprite = backgrounds[pastState];
            backgroundFront.sprite = backgrounds[currentState];

            fadeStart = true;
            fadeTime = 0;
        }
    }

    private void TimerSingle()
    {
        if (singleFadeActivate)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = frameRate;
                pastState = currentState;
                currentState += 1;
                if (currentState > maxState)
                {
                    singleFadeActivate = false;
                    fadeStart = false;
                    return;
                }

                GetComponent<Image>().sprite = backgrounds[pastState];
                backgroundFront.sprite = backgrounds[currentState];

                fadeStart = true;
                fadeTime = 0;
            }
        }
    }

    private void Loop()
    {
        if (fadeStart)
        {
            fadeTime += Time.deltaTime;

            float alpha = Mathf.Lerp(0, 1, fadeTime / frameRate);

            backgroundFront.color = new Color(1, 1, 1, alpha);
            GetComponent<Image>().color = new Color(1, 1, 1, 1 - alpha);

            if (fadeTime >= frameRate)
            {
                fadeStart = false;
            }
        }
    }

    private void LoopFull()
    {
        if (fadeStart)
        {
            fadeTime += Time.deltaTime;

            float alpha = Mathf.Lerp(0, 1, fadeTime / frameRate);

            backgroundFront.color = new Color(1, 1, 1, alpha);

            if (fadeTime >= frameRate)
            {
                fadeStart = false;
            }
        }
    }

    private void FadeSingle()
    {
        if (fadeStart)
        {
            fadeTime += Time.deltaTime;

            float alpha = Mathf.Lerp(0, 1, fadeTime / frameRate);
            
            backgroundFront.color = new Color(1, 1, 1, alpha);
            GetComponent<Image>().color = new Color(1, 1, 1, 1 - alpha);

            if (fadeTime >= frameRate)
            {
                fadeStart = false;
            }
        }
    }

    private void DoubleFade()
    {
        fadeTime += Time.deltaTime * direction;

        float alpha = Mathf.Lerp(0, 1, fadeTime / frameRate);

        GetComponent<Image>().color = new Color(1, 1, 1, alpha);

        if (alpha >= 1)
            direction = -1;
        else if (alpha <= 0)
            direction = 1;
    }
    
}

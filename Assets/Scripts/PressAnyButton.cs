using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressAnyButton : MonoBehaviour
{
    [SerializeField] private Image title;
    [SerializeField] private GameObject pressAnyButtonText;
    [SerializeField] private Animation menu;

    private float apperaingTime = 0;
    private bool isFirstFrameWas = false;
    private float timeToStart = 1.0f;
    private bool canPressAnyButton = false;
    private bool isAnyKeyPressed = false;

    private void Update()
    {
        SmileAppearing();
        AnyKey();
    }

    private void SmileAppearing()
    {
        if (!isAnyKeyPressed)
        {
            timeToStart -= Time.deltaTime;
            if (!isFirstFrameWas)
            {
                title.material.SetFloat("_Fade", 0);
                isFirstFrameWas = true;
            }

            if (timeToStart > 0)
                return;

            apperaingTime += Time.deltaTime / 7;
            if (apperaingTime <= 1)
            {
                Mathf.Clamp(apperaingTime, 0, 1);
                title.material.SetFloat("_Fade", apperaingTime);
            }
            else
            {
                pressAnyButtonText.gameObject.SetActive(true);
                canPressAnyButton = true;
            }
        }
    }

    private void AnyKey()
    {
        if (canPressAnyButton && !isAnyKeyPressed)
        {
            if (Input.anyKey)
            {
                title.gameObject.SetActive(false);
                pressAnyButtonText.gameObject.SetActive(false);
                menu.Play();
                isAnyKeyPressed = true;
            }
        }
    }

}

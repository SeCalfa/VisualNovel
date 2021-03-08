using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteElementFade : MonoBehaviour
{

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    internal void ImageAppear()
    {
        StartCoroutine(AppearImg());
    }

    internal void ImageDisappear()
    {
        StartCoroutine(DisappearImg());
    }

    private IEnumerator AppearImg()
    {
    Reset:
        yield return new WaitForSeconds(0.01f);
        img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.01f);

        if (img.color.a < 1)
            goto Reset;
    }

    private IEnumerator DisappearImg()
    {
    Reset:
        yield return new WaitForSeconds(0.01f);
        img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - 0.01f);

        if (img.color.a < 1)
            goto Reset;
    }

}

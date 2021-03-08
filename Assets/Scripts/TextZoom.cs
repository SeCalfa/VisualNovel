using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class TextZoom : Button
{
    private Vector3 BookEndPosition1 = new Vector3(600, 400, 90); // Текст в левом нижнем углу

    private GameObject book;

    private Vector3 currentScale;

    protected override void Awake()
    {
        book = GameObject.Find("Book");
        currentScale = book.transform.localScale;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        StopAllCoroutines();
        StartCoroutine(Zoom(0, 1));
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        StopAllCoroutines();
        StartCoroutine(Zoom(1, -1));
    }

    private IEnumerator Zoom(float alpha, float direction)
    {
    Reset:
        yield return new WaitForSeconds(0.01f);
        alpha += 0.025f * direction;
        book.transform.localScale = Vector3.Lerp(currentScale, new Vector3(100, 100, 1), alpha);
        book.transform.localPosition = Vector3.Lerp(new Vector3(0, 0, 90), BookEndPosition1, alpha);
        if (alpha < 1)
            goto Reset;
    }

}

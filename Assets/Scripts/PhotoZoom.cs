using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoZoom : MonoBehaviour
{

    [SerializeField]
    private Transform Target;
    [SerializeField]
    private Image Fade = null;

    private Button photoZoom;
    private bool isPhotoZoomed = false;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 currentScale;

    private Vector3 targetPosition = new Vector3(0, 0, 0);
    private Vector3 targetScale = new Vector3(1.6f, 1.6f, 1);

    private void Awake()
    {
        photoZoom = GetComponent<Button>();
        photoZoom.onClick.AddListener(PhotoOpen);

        SetCurrentTransform(); // Нужно будет менять из скрипта переварачивающего страницы
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPhotoZoomed)
        {
            PhotoClose();
        }
    }

    internal void SetCurrentTransform()
    {
        currentPosition = transform.localPosition;
        currentRotation = transform.rotation;
        currentScale = transform.localScale;
    }

    private void PhotoOpen()
    {
        if (!isPhotoZoomed)
        {
            isPhotoZoomed = true;
            StopAllCoroutines();
            StartCoroutine(ZoomOn(1, 0));
        }
    }

    private void PhotoClose()
    {
        if (isPhotoZoomed)
        {
            isPhotoZoomed = false;
            StopAllCoroutines();
            StartCoroutine(ZoomOn(-1, 1));
        }
    }

    private IEnumerator ZoomOn(float direction, float alpha)
    {
    Reset:
        transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, alpha);
        transform.rotation = Quaternion.Lerp(currentRotation, Target.rotation, alpha);
        transform.localScale = Vector3.Lerp(currentScale, targetScale, alpha);
        Fade.color = new Color(0, 0, 0, alpha / 2);
        yield return new WaitForSeconds(0.01f);
        alpha += 0.025f * direction;
        if (alpha < 1)
            goto Reset;

    }

}

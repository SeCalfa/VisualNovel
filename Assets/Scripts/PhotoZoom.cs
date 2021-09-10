using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoZoom : MonoBehaviour
{

    [SerializeField]
    private Transform Target;
    [SerializeField]
    private Image Fade;
    [SerializeField]
    private NoteBook noteBook;
    [SerializeField]
    private GameObject upParent;
    [SerializeField]
    private Transform photos;

    [HideInInspector]
    public float alpha = 0;

    private bool isPhotoZoomed = false;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 currentScale;

    private Vector3 targetScale = new Vector3(3.5f, 3.5f, 1);

    public GameObject GetUpParent { get { return upParent; } }

    private void Awake()
    {
        SetCurrentTransform(); // Нужно будет менять из скрипта переварачивающего страницы
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPhotoZoomed)
        {
            PhotoClose();
        }

        ParametresUpdating();
    }

    private void ParametresUpdating()
    {
        Fade.color = new Color(0, 0, 0, noteBook.alpha / 2); // Fade after notebook open

        transform.localPosition = Vector3.Lerp(currentPosition, Target.localPosition, alpha);
        transform.rotation = Quaternion.Lerp(currentRotation, Target.rotation, alpha);
        transform.localScale = Vector3.Lerp(currentScale, targetScale, alpha);
    }

    internal void SetCurrentTransform()
    {
        currentPosition = transform.localPosition;
        currentRotation = transform.rotation;
        currentScale = transform.localScale;
    }

    public void PhotoOpen()
    {
        if (!isPhotoZoomed && !noteBook.isActivePhotoNow)
        {
            isPhotoZoomed = true;
            GetComponent<Animation>().clip = GetComponent<Animation>().GetClip("AlphaForPhotosUp");
            GetComponent<Animation>().Play();
            transform.SetParent(upParent.transform);
        }
    }

    private void PhotoClose()
    {
        if (isPhotoZoomed)
        {
            isPhotoZoomed = false;
            GetComponent<Animation>().clip = GetComponent<Animation>().GetClip("AlphaForPhotosDown");
            GetComponent<Animation>().Play();
            transform.SetParent(photos);
        }
    }

    public void SetPhotoOnDefaultPlace()
    {
        transform.SetAsFirstSibling();
        noteBook.isActivePhotoNow = false;
        noteBook.activePhoto = null;
    }

    public void ActivatePhoto()
    {
        noteBook.isActivePhotoNow = true;
        noteBook.activePhoto = this;
    }
}

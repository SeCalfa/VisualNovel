using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteBook : MonoBehaviour
{
    [SerializeField] private NoteBookSO noteBookSO;

    internal bool isNoteBookActive = false;
    [HideInInspector]
    public bool isCanOpenOrClose = true;
    [HideInInspector]
    public float alpha = 0;

    [SerializeField]
    private PauseMenu pauseMenu;

    internal bool isActivePhotoNow = false;
    internal PhotoZoom activePhoto = null;

    private void Awake()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && isCanOpenOrClose && !pauseMenu.isPauseActive)
        {
            BookInteract();
            isNoteBookActive = !isNoteBookActive;
        }

        PhotoFadeUpdating();
    }

    private void PhotoFadeUpdating()
    {
        if(activePhoto != null)
        {
            print(activePhoto.alpha);
            activePhoto.GetUpParent.GetComponent<Image>().color = new Color(0, 0, 0, activePhoto.alpha / 2);
        }
    }

    private void BookInteract()
    {
        if (isNoteBookActive)
        {
            GetComponent<Animator>().SetTrigger("Disappear");
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Appear");
        }
    }
}

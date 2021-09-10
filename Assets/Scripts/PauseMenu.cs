using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    internal bool isPauseActive = false;
    private Animator animator;

    [HideInInspector]
    public bool isCanOpenOrClose = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isCanOpenOrClose)
        {
            Pause();
            isPauseActive = !isPauseActive;
        }
    }

    private void Pause()
    {
        if (isPauseActive)
        {
            animator.SetTrigger("Disappear");
        }
        else
        {
            Time.timeScale = 0;
            animator.SetTrigger("Appear");
        }
    }

    public void TimeScaleToNormal()
    {
        Time.timeScale = 1;
    }

    public void Resume()
    {
        Pause();
        isPauseActive = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

}

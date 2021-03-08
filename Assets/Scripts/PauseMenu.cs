using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Animator pauseAnimation;
    private bool isPauseActive = false;

    private void Awake()
    {
        pauseAnimation = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPauseActive = !isPauseActive;
            Pause();
        }
    }

    private void Pause()
    {
        if (isPauseActive)
        {
            Time.timeScale = 0;
            pauseAnimation.Play("PauseMenuAppear");
        }
        else
        {
            Time.timeScale = 1;
            pauseAnimation.Play("PauseMenuDisappear");
        }
    }

    public void Resume()
    {
        isPauseActive = false;
        Pause();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

}

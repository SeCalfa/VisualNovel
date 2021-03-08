using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuEvents : MonoBehaviour
{
    [SerializeField] private GameObject backButton = null;

    private void LevelsListAppear() => backButton.SetActive(true);
    private void LevelsListDisappear() => backButton.SetActive(false);
}

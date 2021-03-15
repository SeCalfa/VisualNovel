using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleManager : MonoBehaviour
{

    public static PuzzleManager self;

    [SerializeField]
    internal PuzzleElements[] Photos = null;
    [SerializeField]
    internal PuzzleElements[] Places = null;
    [SerializeField]
    private GameObject NextButton = null;

    internal List<bool> PlantedList = new List<bool>() { false, false, false, false, false };
    internal List<int> PhotosPlace = new List<int>() { -1, -1, -1, -1, -1 };

    internal GameObject takenPhoto = null;
    internal bool isHovered = false;
    internal int number = -1;

    private int photosDone = 0;

    private void Awake()
    {
        self = this;
        NextButton.SetActive(false);
    }

    private void Update()
    {
        
    }

    public void QuessionHovered(int number)
    {
        if(takenPhoto != null && !PlantedList[number])
        {
            isHovered = true;
            this.number = number;
            Vector2 startPos = takenPhoto.transform.localPosition;

            takenPhoto.GetComponent<PuzzleElements>().PhotoTargetVoid(startPos, Places[number].gameObject);
            takenPhoto.GetComponent<PuzzleElements>().number = number;
            PlantedList[number] = true;
        }
    }

    public void QuessionUnHovered(int number)
    {
        isHovered = false;
        this.number = -1;
        if (takenPhoto != null)
        {
            PlantedList[number] = false;
        }
    }

    internal void TestButtonAppearence(int num)
    {
        photosDone += num;

        print(photosDone);
        if (photosDone == 5)
            NextButton.SetActive(true);
        else
            NextButton.SetActive(false);
    }

}

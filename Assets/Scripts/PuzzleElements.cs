using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElements : MonoBehaviour
{

    [SerializeField]
    internal int PlacedElementNumber;

    public enum ElementType
    {
        Photo,
        Place
    }

    [SerializeField]
    private ElementType Type;

    public Vector2 finalPlacePosition { get; private set; }

    private PuzzleManager puzzleManager;
    List<bool> allEndPoints = new List<bool>() { false, false, false, false, false };
    private Vector2 endPoint;
    private GameObject targetPlace;
    private float startDistance = 150f;
    private Vector2 startDrugPos;
    private Vector2 currentDrugPos;
    private Vector2 endDrugPos;
    private bool isPlanted = false;

    private void Awake()
    {
        puzzleManager = FindObjectOfType<PuzzleManager>();

        if(Type == ElementType.Place)
        {
            finalPlacePosition = transform.localPosition;
        }
    }

    private void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if(Type == ElementType.Photo && !isPlanted)
        {
            GetComponent<Canvas>().sortingOrder += 10;
            GetComponent<Animator>().SetTrigger("Hovered");
        }
    }

    private void OnMouseExit()
    {
        if (Type == ElementType.Photo && !isPlanted)
        {
            GetComponent<Canvas>().sortingOrder -= 10;
            GetComponent<Animator>().SetTrigger("UnHovered");
        }
    }

    private void OnMouseDown()
    {
        if (!isPlanted)
        {
            startDrugPos = transform.localPosition;
        }
    }

    private void OnMouseUp()
    {
        if (!isPlanted)
        {
            endDrugPos = transform.localPosition;

            if (startDrugPos == endDrugPos) // Click event
            {

            }
            else // Drug and drop event
            {
                if (CalculatingEndPoint())
                {
                    StartCoroutine(ReturnPosition(endPoint, true));
                    GetComponent<Animator>().SetTrigger("UnHovered");
                    isPlanted = true;

                    puzzleManager.PlantedList[targetPlace.GetComponent<PuzzleElements>().PlacedElementNumber] = true;
                    GetComponent<Canvas>().sortingOrder -= 10;
                }
                else
                {
                    StartCoroutine(ReturnPosition(startDrugPos, false));
                }
            }
        }
    }

    private void OnMouseDrag()
    {
        if (!isPlanted)
        {
            Vector2 mousePos = Input.mousePosition;
            currentDrugPos = Camera.main.ScreenToWorldPoint(mousePos);

            transform.position = currentDrugPos;
        }
    }

    private bool CalculatingEndPoint()
    {
        int pointsT = 0;

        for (int i = 0; i < puzzleManager.Places.Length; i++)
        {
            if(Vector2.Distance(endDrugPos, puzzleManager.Places[i].transform.localPosition) <= 150)
            {
                allEndPoints[i] = true;
            }
        }

        foreach (var point in allEndPoints)
        {
            if (point == true)
                pointsT += 1;
        }

        if (pointsT == 0)
            return false;
        else
        {
            for (int i = 0; i < allEndPoints.Count; i++)
            {
                if (allEndPoints[i] == true && Vector2.Distance(endDrugPos, puzzleManager.Places[i].transform.localPosition) < startDistance && !puzzleManager.PlantedList[i])
                {
                    startDistance = Vector2.Distance(endDrugPos, puzzleManager.Places[i].transform.localPosition);
                    endPoint = puzzleManager.Places[i].transform.localPosition;
                    targetPlace = puzzleManager.Places[i].gameObject;
                }
                else if (allEndPoints[i] == true && Vector2.Distance(endDrugPos, puzzleManager.Places[i].transform.localPosition) < startDistance && puzzleManager.PlantedList[i])
                {
                    return false;
                }
            }
            return true;
        }
    }


    
    private IEnumerator ReturnPosition(Vector2 targetPos, bool isChangeRotation)
    {
        float alpha = 0;
        Quaternion currentRot = Quaternion.identity;
        Vector2 currentScale = transform.localScale;
    Reset:
        yield return new WaitForSeconds(0.01f);
        alpha += 0.04f;
        transform.localPosition = Vector2.Lerp(endDrugPos, targetPos, alpha);

        if (isChangeRotation)
            transform.localRotation = Quaternion.Lerp(currentRot, targetPlace.transform.localRotation, alpha);

        if (alpha < 1)
            goto Reset;
    }

}

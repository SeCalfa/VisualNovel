using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElements : MonoBehaviour
{
    public enum ElementType
    {
        Photo,
        Place
    }

    [SerializeField]
    private ElementType Type;

    private bool isPlanted = false;
    private Vector2 posOnAwake;
    private Vector2 startDragPos;
    private Vector2 currentDragPos;
    private Vector2 endPos;
    private GameObject targetPlace = null;
    internal int number;

    #region UnityCallbacks

    private void Start()
    {
        posOnAwake = transform.localPosition;
    }

    private void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if (Type == ElementType.Photo && !isPlanted)
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
        if (Type == ElementType.Photo && !isPlanted)
        {
            PuzzleManager.self.takenPhoto = gameObject;
            startDragPos = transform.localPosition;
        }
    }

    private void OnMouseDrag()
    {
        if (Type == ElementType.Photo && !isPlanted)
        {
            if (!PuzzleManager.self.isHovered)
            {
                Vector2 mousePos = Input.mousePosition;
                currentDragPos = Camera.main.ScreenToWorldPoint(mousePos);

                transform.position = currentDragPos;
            }
        }
    }

    private void OnMouseUp()
    {
        if (Type == ElementType.Photo)
        {
            PuzzleManager.self.takenPhoto = null;
            endPos = transform.localPosition;

            if (!isPlanted)
            {
                if (PuzzleManager.self.number != -1) // target = photo
                {
                    //StartCoroutine(PhotoTargeted(endPos, targetPlace.gameObject));
                    PuzzleManager.self.PlantedList[PuzzleManager.self.number] = true;
                    isPlanted = true;
                    GetComponent<Animator>().SetTrigger("UnHovered");
                    PuzzleManager.self.TestButtonAppearence(1);
                }
                else // target = start pos
                {
                    StartCoroutine(ReturnPos(endPos, startDragPos));
                }
            }
            else
            {
                PuzzleManager.self.PlantedList[number] = false;
                StartCoroutine(ReturnPos(endPos, posOnAwake));
                PuzzleManager.self.TestButtonAppearence(-1);
            }
        }
    }

    #endregion


    #region Custom Callbacks

    internal void PhotoTargetVoid(Vector2 endDragPos, GameObject targetObj)
    {
        StopAllCoroutines();
        StartCoroutine(PhotoTargeted(endDragPos, targetObj));
    }

    private IEnumerator PhotoTargeted(Vector2 endDragPos, GameObject targetObj)
    {
        float alpha = 0;
        Quaternion currentRot = Quaternion.identity;
    Reset:
        yield return new WaitForSeconds(0.01f);
        alpha += 0.04f;

        transform.localPosition = Vector3.Lerp(endDragPos, new Vector3(targetObj.transform.localPosition.x, targetObj.transform.localPosition.y, -10), alpha);
        transform.localRotation = Quaternion.Lerp(currentRot, targetObj.transform.localRotation, alpha);

        if (alpha < 1)
            goto Reset;
    }

    // --------------

    private IEnumerator ReturnPos(Vector2 endDragPos, Vector2 targetObj)
    {
        float alpha = 0;
        Quaternion currentRot = Quaternion.identity;
    Reset:
        yield return new WaitForSeconds(0.01f);
        alpha += 0.04f;

        transform.localPosition = Vector3.Lerp(endDragPos, new Vector3(targetObj.x, targetObj.y, -10), alpha);

        if (alpha < 1)
            goto Reset;

        isPlanted = false;
    }

    // --------------

    #endregion

}

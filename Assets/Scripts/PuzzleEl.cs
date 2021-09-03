using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleEl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IDragHandler
{
    private enum State
    {
        NotTaken,
        Taken,
        OnTargetPlace
    }
    private State PhotoState = State.NotTaken;

    public enum Type
    {
        Photo,
        Place
    }
    [SerializeField]
    private Type ElementType;

    [SerializeField]
    private Transform upParent;
    [SerializeField]
    private Transform background;

    [Space]
    [SerializeField]
    internal int number;

    private int currentNumberPlanted = -1;
    private Collider2D currentPlant;
    private bool IsPointerWasDown = false;
    private bool Douned = false;
    private Vector3 currentDragPos;
    private Vector2 startPos;
    Quaternion startRot;
    private float startTime;
    private float duration = 4f;

    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.rotation;
        startTime = Time.time;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ElementType == Type.Photo && PhotoState != State.OnTargetPlace)
        {
            GetComponent<Animator>().SetTrigger("Hovered");
            transform.SetParent(upParent);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ElementType == Type.Photo)
        {
            if (PhotoState == State.NotTaken)
            {
                transform.SetParent(background);

                if (!IsPointerWasDown)
                {
                    GetComponent<Animator>().SetTrigger("UnHovered");
                }
                IsPointerWasDown = false;
            }
            else if (PhotoState == State.OnTargetPlace && Douned)
            {
                IsPointerWasDown = true;
                PhotoState = State.Taken;
                PuzzleMan.self.plantedPhotos[currentPlant.GetComponent<PuzzleEl>().number - 1] = false;
                PuzzleMan.self.ReturnPhoto();
                //print("number: " + currentPlant.GetComponent<PuzzleEl>().number);
            }

            if (PhotoState != State.OnTargetPlace && Douned && currentPlant != null && !PuzzleMan.self.plantedPhotos[currentPlant.GetComponent<PuzzleEl>().number - 1])
            {
                print("er");
                PuzzleMan.self.plantedPhotos[currentPlant.GetComponent<PuzzleEl>().number - 1] = false;
                PuzzleMan.self.ReturnPhoto();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ElementType == Type.Photo)
        {
            if (PhotoState == State.OnTargetPlace)
            {
                PuzzleMan.self.plantedPhotos[currentPlant.GetComponent<PuzzleEl>().number - 1] = false;
                PuzzleMan.self.ReturnPhoto();
            }

            Douned = true;
            IsPointerWasDown = true;
            PhotoState = State.Taken;

            Vector3 mousePos = Input.mousePosition;
            currentDragPos = Camera.main.ScreenToWorldPoint(mousePos);

            transform.position = new Vector2(currentDragPos.x, currentDragPos.y);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 10);

            //Poligon.enabled = false;
            //Circle.enabled = true; // добавить это после стабилизации в начальной точке
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ElementType == Type.Photo)
        {
            Douned = false;

            if (PhotoState != State.OnTargetPlace)
            {
                PhotoState = State.NotTaken;
                GetComponent<Animator>().SetTrigger("UnHovered");
            }

            if((Vector2)transform.localPosition != startPos && PhotoState != State.OnTargetPlace)
            {
                StartCoroutine(ReturnPos());
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ElementType == Type.Photo && PhotoState != State.OnTargetPlace)
        {
            Vector3 mousePos = Input.mousePosition;
            currentDragPos = Camera.main.ScreenToWorldPoint(mousePos);

            transform.position = new Vector2(currentDragPos.x, currentDragPos.y);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 10);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ElementType == Type.Photo && !PuzzleMan.self.plantedPhotos[collision.GetComponent<PuzzleEl>().number - 1])
        {
            PhotoTargetVoid(collision.transform.localPosition, collision.gameObject);
            PhotoState = State.OnTargetPlace;
            GetComponent<Animator>().SetTrigger("UnHovered");

            currentNumberPlanted = collision.GetComponent<PuzzleEl>().number - 1;
            PuzzleMan.self.PlantPhoto(collision.GetComponent<PuzzleEl>().number - 1);

            currentPlant = collision;
        }
    }


    // ---------------------

    private void PhotoTargetVoid(Vector2 endDragPos, GameObject targetObj)
    {
        if (number - 1 == targetObj.GetComponent<PuzzleEl>().number - 1)
            PuzzleMan.self.plantedCorrect[targetObj.GetComponent<PuzzleEl>().number - 1] = true;
        else
            PuzzleMan.self.plantedCorrect[targetObj.GetComponent<PuzzleEl>().number - 1] = false;

        StartCoroutine(PhotoTargeted(endDragPos, targetObj));
    }

    private IEnumerator PhotoTargeted(Vector2 endDragPos, GameObject targetObj)
    {
        //GetComponent<Canvas>().sortingOrder -= 10;
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

    private IEnumerator ReturnPos()
    {
        float alpha = 0;
        Quaternion currentRot = transform.rotation;
        Vector2 currentPos = transform.localPosition;

        if(currentNumberPlanted != -1)
        {
            PuzzleMan.self.plantedPhotos[currentPlant.GetComponent<PuzzleEl>().number - 1] = false;
            PuzzleMan.self.ReturnPhoto();
        }

    Reset:
        yield return new WaitForSeconds(0.01f);
        alpha += 0.04f;

        transform.localPosition = Vector2.Lerp(currentPos, startPos, alpha);
        transform.rotation = Quaternion.Lerp(currentRot, startRot, alpha);

        if (alpha < 1)
            goto Reset;

        currentNumberPlanted = -1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersController : MonoBehaviour
{
    [SerializeField] private Emotions Emotion;

    private Vector3 startLocation;
    private Vector3 startScale;
    internal Vector3 endLocation;
    private Vector3 activeCharacterPos = new Vector3(0, 20, 0); // Прибавление позиции во время активации персонажа
    private Vector3 activeCharacterScale = new Vector3(0.1f, 0.1f, 0); // Прибавление размера во время активации персонажа
    private Vector3 endLocLeft1 = new Vector3(850, 0, 0);
    private Vector3 endLocRight1 = new Vector3(-850, 0, 0);
    private Vector3 endLocLeftMid = new Vector3(600, 0, 0);
    private Vector3 endLocRightMid = new Vector3(-600, 0, 0);
    private Vector3 endLocationWhileTalking;
    private Vector3 endScaleWhileTalking;

    private float alpha = 0.0f;
    private float sizeAlpha = 0.0f;
    private float newLocationAlpha = 0.0f;
    private float leaveAlpha = 0.0f;
    private float setMiddleAlpha = 0.0f;
    private bool isOnPosition = false;
    internal bool isTalking = false;
    internal bool isLeave = false;
    internal bool isNewLocationActive = false;
    internal bool isLeftSide = false;
    internal bool isMiddleReturn = false;

    private void Start()
    {
        startLocation = transform.localPosition;
        startScale = transform.localScale;
        endLocationWhileTalking = endLocation + activeCharacterPos;

        if (startScale.x > 0)
            endScaleWhileTalking = transform.localScale + activeCharacterScale;
        else
            endScaleWhileTalking = transform.localScale + new Vector3(-activeCharacterScale.x, activeCharacterScale.y, activeCharacterScale.z);
    }

    private void Update()
    {
        ChangeLocation();
        Talking();
        NewLocation();
        NewLocation2();
        Leave();
    }

    private void ChangeLocation()
    {
        if (alpha < 1.0f)
        {
            alpha += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startLocation, endLocation, Mathf.Clamp(alpha, 0, 1));
        }
        else
            isOnPosition = true;
    }

    private void Leave() // Персонаж уходит
    {
        if (leaveAlpha < 1.0f && isLeave)
        {
            leaveAlpha += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(endLocation, startLocation, Mathf.Clamp(leaveAlpha, 0, 1));
        }
        else if (leaveAlpha > 1.0f && isLeave)
            Destroy(gameObject);
    }

    private void Talking()
    {
        if(sizeAlpha < 1.0f && isOnPosition && isTalking)
        {
            sizeAlpha += Time.deltaTime * 5;
            transform.localPosition = Vector3.Lerp(endLocation, endLocationWhileTalking, Mathf.Clamp(sizeAlpha, 0, 1));
            transform.localScale = Vector3.Lerp(startScale, endScaleWhileTalking, Mathf.Clamp(sizeAlpha, 0, 1));
        }
        else if (sizeAlpha > 0f && isOnPosition && !isTalking)
        {
            sizeAlpha -= Time.deltaTime * 5;
            transform.localPosition = Vector3.Lerp(endLocation, endLocationWhileTalking, Mathf.Clamp(sizeAlpha, 0, 1));
            transform.localScale = Vector3.Lerp(startScale, endScaleWhileTalking, Mathf.Clamp(sizeAlpha, 0, 1));

            //GetComponent<Image>().sprite = Emotion.EmotionTypes[0]; // Return to default emotion
        }
    }

    internal void SetEmotion(int emotionNumber) // Set new emotion
    {
        GetComponent<Image>().sprite = Emotion.EmotionTypes[emotionNumber];
    }

    private void NewLocation() // Перемещение персонажа когда появляется еще один с его стороны
    {
        Vector3 endLoc;
        if (isLeftSide)
            endLoc = endLocLeft1;
        else
            endLoc = endLocRight1;

        if (newLocationAlpha < 1 && isNewLocationActive)
        {
            newLocationAlpha += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(endLocation, endLoc, Mathf.Clamp(newLocationAlpha, 0, 1));
            if (transform.localPosition == endLoc)
            {
                endLocation = endLoc;
                endLocationWhileTalking = endLocation + activeCharacterPos;
            }
        }
    }

    private void NewLocation2() // Перемещение персонажа когда уходит один персонаж с его стороны
    {
        Vector3 endLoc;
        if (isLeftSide)
            endLoc = endLocLeftMid;
        else
            endLoc = endLocRightMid;

        if (setMiddleAlpha < 1 && isMiddleReturn)
        {
            setMiddleAlpha += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(endLocation, endLoc, Mathf.Clamp(setMiddleAlpha, 0, 1));
            if (transform.localPosition == endLoc)
            {
                endLocation = endLoc;
                endLocationWhileTalking = endLocation + activeCharacterPos;
            }
        }
    }

    [System.Serializable]
    private struct Emotions
    {
        public Sprite[] EmotionTypes;
    }

}

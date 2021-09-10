using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingElement : MonoBehaviour
{
    private float alpha = 0; // 0 to 1
    private int direction; // 1 or -1
    private float maxYpos;
    private float minYpos;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float speed;

    public enum TypeOfFloating
    {
        UpDown,
        ToUp,
        ToDown
    }

    [System.Serializable]
    public struct Parametres
    {
        public float distanceMin;
        public float distanceMax;
        public float speedMin;
        public float speedMax;
    }

    [SerializeField]
    private TypeOfFloating floatType;
    [SerializeField]
    private Parametres parametres;

    private void Awake()
    {
        SettingsOnStart();
    }

    private void Update()
    {
        Move();
    }

    private void SettingsOnStart()
    {
        speed = Random.Range(parametres.speedMin, parametres.speedMax);
        distance = Random.Range(parametres.distanceMin, parametres.distanceMax);

        if (floatType == TypeOfFloating.ToDown)
        {
            direction = -1;
            maxYpos = transform.localPosition.y - distance;
            minYpos = transform.localPosition.y;
        }
        else if (floatType == TypeOfFloating.ToUp)
        {
            direction = 1;
            maxYpos = transform.localPosition.y + distance;
            minYpos = transform.localPosition.y;
        }
        else if (floatType == TypeOfFloating.UpDown)
        {
            direction = 1;
            maxYpos = transform.localPosition.y + distance;
            minYpos = transform.localPosition.y - distance;
        }
    }

    private void Move()
    {
        alpha += Time.deltaTime * direction * speed;
        transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(minYpos, maxYpos, alpha));

        if (alpha > 1)
        {
            direction = -1;
            speed = Random.Range(parametres.speedMin, parametres.speedMax);
        }
        else if (alpha < 0)
        {
            direction = 1;
            speed = Random.Range(parametres.speedMin, parametres.speedMax);
        }
    }

}

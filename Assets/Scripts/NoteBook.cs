using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteBook : MonoBehaviour
{
    [SerializeField] private NoteBookSO noteBookSO = null;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private GameObject notebook;

    private TransformDynamic[] transformDynamic;

    private List<Sprite> sprites;

    private struct TransformDynamic
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    private void Awake()
    {
        FillNotebook();
    }

    private void Update()
    {
        //Test

        if (Input.GetKeyDown(KeyCode.I))
        {
            notebook.SetActive(!notebook.activeSelf);
        }

        //Test

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (noteBookSO.pageNumber > 0)
                noteBookSO.pageNumber -= 1;

            FillNotebook();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (noteBookSO.pageNumber < noteBookSO.pageMax)
                noteBookSO.pageNumber += 1;

            FillNotebook();
        }
    }

    private void FillNotebook()
    {
        sprites = noteBookSO.GetPictures;
        transformDynamic = new TransformDynamic[sprites.Count];
        
        if(noteBookSO.pages[noteBookSO.pageNumber].pType == NoteBookSO.pageType.type1)
        {
            SetTransformForPageElements(1);

            spriteRenderers[0].sprite = sprites[0];
            spriteRenderers[1].sprite = sprites[1];
            spriteRenderers[2].sprite = sprites[2];
            spriteRenderers[3].sprite = sprites[3];
            spriteRenderers[4].sprite = sprites[4];
            spriteRenderers[5].sprite = sprites[5];
            spriteRenderers[6].sprite = sprites[6];
        }
        else if(noteBookSO.pages[noteBookSO.pageNumber].pType == NoteBookSO.pageType.type2)
        {
            SetTransformForPageElements(2);

            spriteRenderers[0].sprite = sprites[0];
            spriteRenderers[1].sprite = sprites[1];
            spriteRenderers[2].sprite = sprites[2];
            spriteRenderers[3].sprite = sprites[3];
            spriteRenderers[4].sprite = sprites[4];
            spriteRenderers[5].sprite = sprites[5];
            spriteRenderers[6].sprite = sprites[6];
        }
    }

    private void SetTransformForPageElements(int type)
    {
        if(type == 1)
        {
            transformDynamic[0].position = new Vector3(-7.2f, 0.5f, 0);
            transformDynamic[0].rotation = new Vector3(0, 0, 0);
            transformDynamic[0].scale = new Vector3(-0.95f, -0.95f, 1);

            spriteRenderers[0].transform.localPosition = transformDynamic[0].position;
            spriteRenderers[0].transform.eulerAngles = transformDynamic[0].rotation;
            spriteRenderers[0].transform.localScale = transformDynamic[0].scale;
            //---------------
            transformDynamic[1].position = new Vector3(-7, -0.2f, 0);
            transformDynamic[1].rotation = new Vector3(0, 0, -1);
            transformDynamic[1].scale = new Vector3(0.95f, 0.95f, 1);

            spriteRenderers[1].transform.localPosition = transformDynamic[1].position;
            spriteRenderers[1].transform.eulerAngles = transformDynamic[1].rotation;
            spriteRenderers[1].transform.localScale = transformDynamic[1].scale;
            //---------------
            transformDynamic[2].position = new Vector3(-7f, -0.2f, 0);
            transformDynamic[2].rotation = new Vector3(0, 0, 2);
            transformDynamic[2].scale = new Vector3(-0.95f, -0.95f, 1);

            spriteRenderers[2].transform.localPosition = transformDynamic[2].position;
            spriteRenderers[2].transform.eulerAngles = transformDynamic[2].rotation;
            spriteRenderers[2].transform.localScale = transformDynamic[2].scale;
            //---------------
            transformDynamic[3].position = new Vector3(6.8f, -0.2f, 0);
            transformDynamic[3].rotation = new Vector3(0, 0, 0);
            transformDynamic[3].scale = new Vector3(0.95f, 0.95f, 1);

            spriteRenderers[3].transform.localPosition = transformDynamic[3].position;
            spriteRenderers[3].transform.eulerAngles = transformDynamic[3].rotation;
            spriteRenderers[3].transform.localScale = transformDynamic[3].scale;
            //---------------
            transformDynamic[4].position = new Vector3(6.8f, -0.2f, 0);
            transformDynamic[4].rotation = new Vector3(0, 0, -5);
            transformDynamic[4].scale = new Vector3(-0.95f, -0.95f, 1);

            spriteRenderers[4].transform.localPosition = transformDynamic[4].position;
            spriteRenderers[4].transform.eulerAngles = transformDynamic[4].rotation;
            spriteRenderers[4].transform.localScale = transformDynamic[4].scale;
            //---------------
            transformDynamic[5].position = new Vector3(6.5f, 0.1f, 0);
            transformDynamic[5].rotation = new Vector3(0, 0, -3);
            transformDynamic[5].scale = new Vector3(0.95f, 0.95f, 1);

            spriteRenderers[5].transform.localPosition = transformDynamic[5].position;
            spriteRenderers[5].transform.eulerAngles = transformDynamic[5].rotation;
            spriteRenderers[5].transform.localScale = transformDynamic[5].scale;
            //---------------
            transformDynamic[6].position = new Vector3(6.5f, 0.1f, 0);
            transformDynamic[6].rotation = new Vector3(0, 0, 0);
            transformDynamic[6].scale = new Vector3(0.95f, -0.95f, 1);

            spriteRenderers[6].transform.localPosition = transformDynamic[6].position;
            spriteRenderers[6].transform.eulerAngles = transformDynamic[6].rotation;
            spriteRenderers[6].transform.localScale = transformDynamic[6].scale;
        }
        else if(type == 2)
        {
            transformDynamic[0].position = new Vector3(-8, 3, 0);
            transformDynamic[0].rotation = new Vector3(0, 0, 5);
            transformDynamic[0].scale = new Vector3(0.3f, 0.3f, 1);

            spriteRenderers[0].transform.localPosition = transformDynamic[0].position;
            spriteRenderers[0].transform.eulerAngles = transformDynamic[0].rotation;
            spriteRenderers[0].transform.localScale = transformDynamic[0].scale;
            //---------------
            transformDynamic[1].position = new Vector3(-7, 2.4f, 0);
            transformDynamic[1].rotation = new Vector3(0, 0, -5);
            transformDynamic[1].scale = new Vector3(0.3f, 0.3f, 1);

            spriteRenderers[1].transform.localPosition = transformDynamic[1].position;
            spriteRenderers[1].transform.eulerAngles = transformDynamic[1].rotation;
            spriteRenderers[1].transform.localScale = transformDynamic[1].scale;
            //---------------
            transformDynamic[2].position = new Vector3(-5, 1, 0);
            transformDynamic[2].rotation = new Vector3(0, 0, 10);
            transformDynamic[2].scale = new Vector3(0.3f, 0.3f, 1);

            spriteRenderers[2].transform.localPosition = transformDynamic[2].position;
            spriteRenderers[2].transform.eulerAngles = transformDynamic[2].rotation;
            spriteRenderers[2].transform.localScale = transformDynamic[2].scale;
            //---------------
            transformDynamic[3].position = new Vector3(-6, -4.5f, 0);
            transformDynamic[3].rotation = new Vector3(0, 0, 0);
            transformDynamic[3].scale = new Vector3(0.7f, 0.7f, 1);

            spriteRenderers[3].transform.localPosition = transformDynamic[3].position;
            spriteRenderers[3].transform.eulerAngles = transformDynamic[3].rotation;
            spriteRenderers[3].transform.localScale = transformDynamic[3].scale;
            //---------------
            transformDynamic[4].position = new Vector3(8, 3, 0);
            transformDynamic[4].rotation = new Vector3(0, 0, 0);
            transformDynamic[4].scale = new Vector3(0.3f, 0.3f, 1);

            spriteRenderers[4].transform.localPosition = transformDynamic[4].position;
            spriteRenderers[4].transform.eulerAngles = transformDynamic[4].rotation;
            spriteRenderers[4].transform.localScale = transformDynamic[4].scale;
            //---------------
            transformDynamic[5].position = new Vector3(4.5f, 1, 0);
            transformDynamic[5].rotation = new Vector3(0, 0, 15);
            transformDynamic[5].scale = new Vector3(0.3f, 0.3f, 1);

            spriteRenderers[5].transform.localPosition = transformDynamic[5].position;
            spriteRenderers[5].transform.eulerAngles = transformDynamic[5].rotation;
            spriteRenderers[5].transform.localScale = transformDynamic[5].scale;
            //---------------
            transformDynamic[6].position = new Vector3(6.5f, -4.5f, 0);
            transformDynamic[6].rotation = new Vector3(0, 0, 0);
            transformDynamic[6].scale = new Vector3(0.7f, 0.7f, 1);

            spriteRenderers[6].transform.localPosition = transformDynamic[6].position;
            spriteRenderers[6].transform.eulerAngles = transformDynamic[6].rotation;
            spriteRenderers[6].transform.localScale = transformDynamic[6].scale;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "Note/NoteItem")]
public class NoteBookSO : ScriptableObject
{
    [SerializeField] internal PageSetting[] pages;

    internal int pageNumber = 0;
    internal int pageMax = 2;

    public enum pageType
    {
        none,
        type1,
        type2
    }

    [Serializable]
    public struct PageSetting
    {
        public pageType pType;
        public Sprite[] elements;
    }

    public List<Sprite> GetPictures
    {
        get { return pages[pageNumber].elements.ToList(); }
    }
}

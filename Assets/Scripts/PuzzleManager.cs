using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{

    [SerializeField]
    internal PuzzleElements[] Photos = null;
    [SerializeField]
    internal PuzzleElements[] Places = null;

    internal List<bool> PlantedList = new List<bool>() { false, false, false, false, false };

}

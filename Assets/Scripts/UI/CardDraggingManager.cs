using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDraggingManager : MonoBehaviour
{

    private static CardDraggingManager instance;
    public static CardDraggingManager Instance { get { return instance; } }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("There should be only one CardDraggingManager");
        }
    }
}

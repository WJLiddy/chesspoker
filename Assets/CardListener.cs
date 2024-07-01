using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListener : MonoBehaviour
{
    public int index;
    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        FindObjectOfType<GameManager>().cardClicked(index);
    }
}

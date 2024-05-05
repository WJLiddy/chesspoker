using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessTileListener : MonoBehaviour
{
    public int tileX;
    public int tileY;
    // Start is called before the first frame update
    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        FindObjectOfType<GameManager>().tileClicked(tileX, tileY);
    }
}

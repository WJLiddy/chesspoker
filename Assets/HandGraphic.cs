using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGraphic : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject spriteBase;
    // Start is called before the first frame update
    void Start()
    {

    }

    void makeCard(int spriteID, int pos)
    {
        var v = Instantiate(spriteBase);
        v.transform.SetParent(this.transform);
        v.transform.localPosition = Vector3.right * pos + (Vector3.forward * -pos);
        v.GetComponent<SpriteRenderer>().sprite = sprites[spriteID];
    }

    public void makeHand(List<Deck.Card> cards)
    {
        for(int i = 0 ; i != cards.Count; ++i)
        {
            makeCard(((int)cards[i].Rank-2) + (13*((int)cards[i].Suit)), i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

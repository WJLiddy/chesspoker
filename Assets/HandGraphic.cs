using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGraphic : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject spriteBase;
    public List<GameObject> oldCards;
    // Start is called before the first frame update
    void Awake()
    {
        oldCards = new List<GameObject>();
    }

    void makeCard(int spriteID, int pos, bool raised)
    {
        var v = Instantiate(spriteBase);
        v.transform.SetParent(this.transform);
        v.transform.localPosition = (1.2f * Vector3.right) * pos + (Vector3.forward * -pos) + (raised ? (0.3f * Vector3.up) : Vector3.zero);
        v.GetComponent<SpriteRenderer>().sprite = sprites[spriteID];
        v.GetComponent<CardListener>().index = pos;
        oldCards.Add(v);
    }

    public void renderHand(List<Deck.Card> cards, List<bool> raised, bool show )
    {
        foreach(var v in oldCards)
        {
            Destroy(v.gameObject);
        }
        oldCards.Clear();
        for(int i = 0 ; i != cards.Count; ++i)
        {
            makeCard(!show ? 52 : (((int)cards[i].Rank-2) + (13*((int)cards[i].Suit))), i, raised[i]);
        }
    }

}

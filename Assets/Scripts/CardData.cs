using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardData {

    public CardData() { }

    public CardData(int val, Card.CardSuit suit)
    {
        this.val = val;
        this.suit = suit;
    }
    
    public int val;
    public Card.CardSuit suit;
}

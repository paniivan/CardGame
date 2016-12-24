using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // from 6 to 14
    // J = 11, Q = 12, K = 13, A = 14
    public int cardValue;
    public CardSuit suit;
    public Image cardImage;

    public enum CardSuit { Spades, Clubs, Diamonds, Hearts };
    
    //private Deck deck;

    void Awake()
    {
        this.cardImage = GetComponent<Image>();
       // this.deck = GameObject.Find("Deck").GetComponent<Deck>();
    }

    /**
     * returns true if enemy card (that one which is in Table) can be beaten with this one
     */
    
    public Card Clone()
    {
        return (Card)this.MemberwiseClone();
    }
}

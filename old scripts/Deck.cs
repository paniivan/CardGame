using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Deck : MonoBehaviour {

	//for 36 cards deck
    private int numOfCards = 36;
    private int lowestCardValue = 6;
    //for 52 cards deck
    //int numOfCards = 52;
    //int lowestCardValue = 2;
    private int NUM_OF_SUITS = 4;

    private List<GameObject> cards = new List<GameObject>();
    private Card.CardSuit trumpSuit;

    private GameObject stackCardsImg;
    private GameObject trumpCardImg;
        
    public Sprite back;
    public Sprite[] faces;
    public GameObject cardTemplate;
	
    void Awake()
    {
        this.stackCardsImg = GameObject.Find("StackCardsImg");
        this.trumpCardImg = GameObject.Find("TrumpCardImg");
    }

	void Start () {
        this.InitCards();
        this.ShuffleCards();
        //set trump for current game
        this.InitTrump();
    }

    public Card.CardSuit GetTrumpSuit()
    {
        //TODO: check if u can change that from outside
        //  if so return the copy of suit
        return trumpSuit;
    }

    public GameObject GetTopCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogError("There is no more cards in the deck");
        }

        GameObject card = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);

        if (cards.Count == 1)
        {
            this.stackCardsImg.SetActive(false);
        }

        if (cards.Count == 0)
        {
            this.trumpCardImg.SetActive(false);
        }
        
        return card;
    }

    public bool HasCardsToGive()
    {
        return cards.Count > 0;
    }

    private GameObject GetTrumpCard()
    {
        return cards[0];
    }

    private void InitCards()
    {
        //faces should be in order (1)from 6 to A, (2)from Spades -> Clubs -> Diamonds -> Hearts 
        for (int i = 0; i < numOfCards; i++)
        {
            GameObject cardObj = CreateCard();
            Card cardComp = cardObj.GetComponent<Card>();
            cardObj.transform.SetParent(transform);
            cardComp.cardImage.sprite = faces[i];
            cardComp.cardValue = i / (NUM_OF_SUITS) + lowestCardValue;
            cardComp.suit = (Card.CardSuit)(i % NUM_OF_SUITS);
            cards.Add(cardObj);
        }
    }

    private void ShuffleCards()
    {
        for (int i = 0; i < numOfCards; i++)
        {
            int j = Random.Range(0, numOfCards);
            GameObject temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    /**
     * set trump for current game
     */
    private void InitTrump()
    {
        GameObject trumpCardObj = GetTrumpCard();
        this.trumpCardImg.GetComponent<Image>().sprite =
            trumpCardObj.GetComponent<Image>().sprite;
        this.trumpSuit = trumpCardObj.GetComponent<Card>().suit;
    }
      
    private GameObject CreateCard()
    {
        //TODO: must be better approach
        //mb this objects should be constructed manualy
        GameObject card = GameObject.Instantiate(this.cardTemplate);
        card.name = "Card";
        card.SetActive(true);
        return card;
    }
    
}





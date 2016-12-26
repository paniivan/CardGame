using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Deck : NetworkBehaviour {

	//for 36 cards deck
    private const int numOfCards = 36;
    private const int lowestCardValue = 6;
    //for 52 cards deck
    //int numOfCards = 52;
    //int lowestCardValue = 2;
    private const int numOfSuits = 4;

    private List<CardData> cards = new List<CardData>();
    
    private CardData trumpCardData;

    //server player will be n 0
    private List<Player> players = new List<Player>();

    public GameObject trumpCard;
    public GameObject stackOfCards;


    //[SyncVar/*(hook = "ChangeTrumpImg")*/]
    //public Sprite trumpSprite;
        
    public Sprite back;
    public Sprite[] faces;
    public GameObject cardPrefab;
	
    void Awake()
    {
        //this.trumpSprite = trumpCardImg.GetComponent<Image>.
        //this.stackCardsImg = GameObject.Find("StackCardsImg");
        //this.trumpCardImg = GameObject.Find("TrumpCardImg");
    }

	public override void OnStartServer()
    {
        this.InitCards();
        this.ShuffleCards();
        //set trump for current game
        this.InitTrump();
        this.InitStackOfCards();

        //TODO: send some info to clients
    }

    public Card.CardSuit GetTrumpSuit()
    {
        return this.trumpCardData.suit;
    }

    public CardData GetTopCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogError("There is no more cards in the deck");
        }

        CardData card = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);

        if (cards.Count == 1)
        {
            //TODO
            //this.stackCardsImg.SetActive(false);
        }

        if (cards.Count == 0)
        {
            //TODO
            //this.trumpCardImg.SetActive(false);
        }
        
        return card;
    }

    public bool HasCardsToGive()
    {
        return cards.Count > 0;
    }
    
    private void InitCards()
    {
        //faces should be in order (1)from 6 to A, (2)from Spades -> Clubs -> Diamonds -> Hearts 
        for (int i = 0; i < numOfCards; i++)
        {
            //Sprite sprite = faces[i];
            int val = i / (numOfSuits) + lowestCardValue;
            Card.CardSuit suit = (Card.CardSuit)(i % numOfSuits);

            CardData cd = new CardData(val, suit);

            cards.Add(cd);
        }
    }

    private void ShuffleCards()
    {
        for (int i = 0; i < numOfCards; i++)
        {
            int j = Random.Range(0, numOfCards);
            CardData temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    /**
     * set trump for current game
     */
    private void InitTrump()
    {
        this.SetTrump(cards[0]);
        
        //this.trumpCard = (GameObject)Instantiate(cardPrefab);
        //trumpCard.transform.parent = this.transform;
        //trumpCard.transform.localPosition = new Vector3(-3.999996f, 0);
        //trumpCard.transform.localRotation = new Quaternion(0, 0, 0.7067873f, -0.7074261f);
        //this.trumpCard.GetComponent<Image>().sprite = trumpCardData.sprite;

        //NetworkServer.Spawn(trumpCard);
    }

    private void InitStackOfCards()
    {
        //TODO
    }


    public GameObject CreateCard(CardData cd, Transform parent)
    {
        GameObject card = GameObject.Instantiate(this.cardPrefab);
        card.name = "Card";
        card.SetActive(true);
        card.GetComponent<Card>().cd = cd;
        card.GetComponent<Image>().sprite = this.GetCardSprite(cd);
        card.transform.SetParent(parent);

        return card;
    }
    
    public void SetTrump(CardData cd)
    {
        //Debug.Log("Trump value = " + cd.val + "\nTrump suit" + cd.suit);
        this.trumpCardData = cd;
        this.trumpCard.GetComponent<Image>().sprite = this.GetCardSprite(cd);
    }

    public CardData GetTrumpCardData()
    {
        return this.trumpCardData;
    }

    public Sprite GetCardSprite(CardData cd)
    {
        if (cd.val < 6 || cd.val > 14)
        {
            return this.back;
        }
        return faces[(cd.val - lowestCardValue) * numOfSuits + (int)cd.suit];
    }

    public void RegisterPlayer(Player p)
    {
        Debug.Log("In register player");
        this.players.Add(p);
        //this.GiveCard(players[0]);
        if (players.Count == 2)
        {
            //start game
            //this.PassPlayersToTable();

            for (int i = 0; i < 6; i++)
            {
                foreach (Player player in players)
                {
                    this.GiveCard(player);
                }
            }
        }
    }

    private void GiveCard(Player p)
    {
        CardData cd = this.GetTopCard();
        p.RpcTakeCard(cd);
    }
    
    //private void PassPlayersToTable()
    //{
    //    GameObject.Find("Table").GetComponent<Table>().InitPlayers(this.players);
    //}

}





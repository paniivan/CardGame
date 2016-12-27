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
    private List<int> playerHandSize = new List<int>();

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
            this.RpcDisableStackOfCards();
        }

        if (cards.Count == 0)
        {
            this.RpcDisableTrupmCard();
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
    
    public GameObject CreateCard(CardData cd)
    {
        GameObject card = GameObject.Instantiate(this.cardPrefab);
        card.name = "Card";
        card.SetActive(true);
        card.GetComponent<Card>().cd = cd;
        card.GetComponent<Image>().sprite = this.GetCardSprite(cd);
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
        this.playerHandSize.Add(0);
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
                    this.ChangePlayersHandSize(player, 1);
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

    /**
     * serv only
     */

    //public void SwitchDisablingTableDropzone()
    //{
    //    foreach (Player p in players)
    //    {
    //        p.RpcSwitchDisablingTableDropzone();
    //    }
    //}

    public void SwitchPlayersMode()
    {
        foreach (Player p in players)
        {
            p.RpcSwitchPlayersMode();
        }
    }
    
    /**
     * serv only
     */
    public void RefillHands()
    {
        bool gameEnded = false;

        do
        {
            foreach (Player p in this.players)
            {
                //check for winners
                if (this.GetPlayerHandSize(p) == 0 && !this.HasCards())
                {
                    p.RpcShowEndGameMssg("You win!");
                    gameEnded = true;
                    //TODO: losers message
                }

                //if player need cards and we have them in deck
                if (this.PlayerNeedCard(p) && this.HasCards())
                {
                    p.RpcTakeCard(this.GetTopCard());
                    this.ChangePlayersHandSize(p, 1);
                }
            }
        }
        //while players still need cards and we can give it to em
        while (!this.IsPlayersHandsFull() && this.HasCards());
    }

    public void ChangePlayersHandSize(Player p, int val)
    {
        this.playerHandSize[GetPlayerNum(p)] = this.playerHandSize[GetPlayerNum(p)] + val;
    }

    private int GetPlayerHandSize(Player p)
    {
        return this.playerHandSize[this.GetPlayerNum(p)];
    }

    //[Command]
    //public void CmdChangePlayersHandSize(Player p, int val)
    //{
    //    this.ChangePlayersHandSize(p, val);
    //}

    private int GetPlayerNum(Player p)
    {
        //searching for players number in list
        int playerNumber;
        for (playerNumber = 0; playerNumber < this.players.Count; playerNumber++)
        {
            if (this.players[playerNumber].Equals(p))
            {
                return playerNumber;
            }
        }
        Debug.Log("Cant fing a player with number");
        return -28;
    }

    private bool HasCards()
    {
        if (this.cards.Count > 0)
        { 
            return true;
        }
        return false;
    }

    private bool PlayerNeedCard(Player p)
    {
        int normalHandSize = 6;
        if (normalHandSize - this.playerHandSize[this.GetPlayerNum(p)] > 0)
        {
            return true;
        }
        return false;
    }

    private bool IsPlayersHandsFull()
    {
        bool result = true;

        foreach (Player p in this.players)
        {
            if (this.PlayerNeedCard(p))
            {
                result = false;
            }
        }

        return result;
    }

    [ClientRpc]
    private void RpcDisableTrupmCard()
    {
        this.trumpCard.SetActive(false);
    }

    [ClientRpc]
    private void RpcDisableStackOfCards()
    {
        this.stackOfCards.SetActive(false);
    }
}





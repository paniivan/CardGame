using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Table : NetworkBehaviour
{

    //[SyncVar]
    //private int n;

    //public void SetN(int n)
    //{
    //    this.n = n;
    //}

    //TODO: mb this logic should be stored smhow in other way
    //is player attacks or defends

    //should it be a syncvar?
    [SyncVar]
    public bool offenceMode;

    public GameObject offenceLayer;
    public GameObject defenceLayer;

    private GameObject placeholder;

    private List<CardData> servOffLayer = new List<CardData>();
    private List<CardData> servDefLayer = new List<CardData>();

    //private List<Player> players;

    public Deck deck;

    void Awake()
    {
        //TODO: here should be some checks
        this.offenceMode = true;
        //this.offenceLayer = GameObject.Find("Offence layer");
        //this.defenceLayer = GameObject.Find("Defence layer");
        //this.deck = GameObject.Find("Deck").GetComponent<Deck>();
    }


    /**
     * always on server
     */
    public void ProposeCard(CardData cd, Player player)
    {
        //if table awaits offencive move
        if (offenceMode)
        {
            if (CanBePassed(cd))
            {
                this.AddToTable(cd, player);
                this.SwitchTableMode();
                this.deck.SwitchPlayersMode();
                //this.deck.SwitchDisablingTableDropzone();
                //this.deck.SwitchTableMode();
                Debug.Log("Nice move!");
                //TODO: some signal to players
            }
            else
            {
                // return card to player
                Debug.Log("Bad move!");
                player.RpcTakeCard(cd);
            }
        }
        else
        {
            Debug.Log("In ProposeCard defence mode");
            if (CardCanBeat(cd))
            {
                this.AddToTable(cd, player);
                //on serv
                this.SwitchTableMode();
                //on clients
                this.deck.SwitchPlayersMode();
                //this.deck.SwitchDisablingTableDropzone();
                Debug.Log("Nice move!");
                //TODO: some signal to players
            }
            else
            {
                Debug.Log("Bad move!");
                player.RpcTakeCard(cd);
            }
        }
    }

    private CardData GetNotBeatedCard()
    {
        Debug.Log("in GetNotBeatedCard");
        //if there is not beated card on the table
        if (servOffLayer.Count - servDefLayer.Count > 0)
        {
            return servOffLayer[servOffLayer.Count - 1];
        }
        else
        {
            throw new InvalidOperationException("There is no unbeated card.");
        }
    }

    private bool ContainCardWithValue(int cardValue)
    {
        foreach (CardData cd in servOffLayer)
        {
            if (cd.val == cardValue)
            {
                return true;
            }
        }

        foreach (CardData cd in servDefLayer)
        {
            if (cd.val == cardValue)
            {
                return true;
            }
        }

        return false;
    }

    /**
     * serv only
     */
    public int TakeCards(Player p)
    {
        List<CardData> cardsToTake = new List<CardData>();

        for (int i = 0; i < this.servOffLayer.Count; i++)
        {
            cardsToTake.Add(this.servOffLayer[i]);
        }

        for (int i = 0; i < this.servDefLayer.Count; i++)
        {
            cardsToTake.Add(this.servDefLayer[i]);
        }

        foreach (CardData card in cardsToTake)
        {
            p.RpcTakeCard(card);
        }

        return cardsToTake.Count;
    }

    public bool IsEmpty()
    {
        if (servOffLayer.Count == 0)
        {
            return true;
        }
        return false;
    }

    public bool ClientIsEmpty()
    {
        if (offenceLayer.transform.childCount == 0)
        {
            return true;
        }
        return false;
    }

    private bool CanBePassed(CardData cd)
    {
        return ContainCardWithValue(cd.val) || IsEmpty();
    }

    private bool CardCanBeat(CardData cd)
    {
        Debug.Log("In CardCanBeat");
        CardData cardToBeat = this.GetNotBeatedCard();

        if (cd.suit == cardToBeat.suit && cd.val > cardToBeat.val)
        {
            return true;
        }
        else if (cd.suit == deck.GetTrumpSuit() && cardToBeat.suit != deck.GetTrumpSuit())
        {
            return true;
        }

        return false;
    }

    //private void AddToTable(Dragable dragable)
    //{
    //    Transform newParent;
    //    if (offenceMode)
    //    {
    //        newParent = offenceLayer.transform;
    //    }
    //    else
    //    {
    //        newParent = defenceLayer.transform;
    //    }

    //    dragable.transform.SetParent(newParent);
    //    dragable.enabled = false;
    //}

    private void AddToTable(CardData cd, Player player)
    {
        //server part
        if (this.offenceMode)
        {
            servOffLayer.Add(cd);
        }
        else
        {
            servDefLayer.Add(cd);
        }

        deck.ChangePlayersHandSize(player, -1);

        //client part

        this.RpcAddToTable(cd, this.offenceMode);
        //player.RpcAddCardToTable(cd, this.offenceMode);
    }

    /**
     * recieving offenceMode as parameter in case it will be changed during data sending
     */
    [ClientRpc]
    private void RpcAddToTable(CardData cd, bool offenceMode)
    {
        Transform cardParent;
        if (offenceMode)
        {
            cardParent = this.offenceLayer.transform;
            //adding placeholder to defenceLayer
            this.CreatePlaceholder();
        }
        else
        {
            cardParent = this.defenceLayer.transform;
            this.DeletePlaceholder();
        }

        //adding card to client table
        GameObject card = deck.CreateCard(cd);
        card.transform.SetParent(cardParent);
        card.GetComponent<Dragable>().enabled = false;
    }

    //private void ReturnToHand(Dragable card)
    //{
    //    card.ToHand();
    //}

    //public void InitPlayers(List<Player> players)
    //{
    //    this.players = players;
    //}

    public void SwitchTableMode()
    {
        this.offenceMode = !this.offenceMode;
    }

    public void CreatePlaceholder()
    {
        this.placeholder = new GameObject();
        placeholder.name = "CardPlaceholder";
        placeholder.transform.SetParent(this.defenceLayer.transform);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = 50;
        le.preferredHeight = 70;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        //placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
    }

    public void DeletePlaceholder()
    {
        Destroy(this.placeholder);
    }
    
    /**
     * serv only
     */
    public void Clean()
    {
        servOffLayer.Clear();
        servDefLayer.Clear();
        this.RpcClean();
    }

    [ClientRpc]
    private void RpcClean()
    {
        for (int i = 0; i < offenceLayer.transform.childCount; i++)
        {
            Destroy(offenceLayer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < defenceLayer.transform.childCount; i++)
        {
            Destroy(defenceLayer.transform.GetChild(i).gameObject);
        }
    }

    
}

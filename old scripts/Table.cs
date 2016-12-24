using UnityEngine;
using System;
using System.Collections.Generic;

public class Table : MonoBehaviour
{
    //TODO: mb this logic should be stored smhow in other way
    //is player attacks or defends
    private bool offenceMode;

    private GameObject offenceLayer;
    private GameObject defenceLayer;
    private Deck deck;

    void Awake()
    {
        //TODO: here should be some checks
        this.offenceMode = false;
        this.offenceLayer = GameObject.Find("Offence layer");
        this.defenceLayer = GameObject.Find("Defence layer");
        this.deck = GameObject.Find("Deck").GetComponent<Deck>();
    }

    public void ProposeCard(GameObject cardGameObj)
    {
        Card card = cardGameObj.GetComponent<Card>();
        Dragable dagable = cardGameObj.GetComponent<Dragable>();

        if (offenceMode)
        {
            if (CanBePassed(card))
            {
                AddToTable(dagable);
                //TODO: some ivent
            }
            else
            {
                ReturnToHand(dagable);
            }
            //TODO: jdkfjakfjskfj
        }
        else
        {
            if (CardCanBeat(card))
            {
                AddToTable(dagable);
                //TODO: create/destroy table placeholder
                Debug.Log("Nice move!");
                //TODO: trigger some ivent mb
            }
            else
            {
                Debug.Log("Bad move!");
                ReturnToHand(dagable);
            }
        }
    }

    [RPC]
    public void SetOffenceMode(bool offenceMode)
    {
        if (this.offenceMode != offenceMode)
        {
            //something
            this.offenceMode = offenceMode;
        }
    }

    private Card GetNotBeatedCard()
    {
        //if there is not beated card on the table
        if (offenceLayer.transform.childCount - defenceLayer.transform.childCount == 1)
        { 
            return offenceLayer.transform.GetChild(offenceLayer.transform.childCount - 1).GetComponent<Card>();
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private bool ContainCardWithValue(int cardValue)
    {
        bool result = false;

        for (int i = 0; i < offenceLayer.transform.childCount; i++)
        {
            if (offenceLayer.transform.GetChild(i).GetComponent<Card>().cardValue == cardValue)
                result = true;
        }

        for (int i = 0; i < defenceLayer.transform.childCount; i++)
        {
            if (defenceLayer.transform.GetChild(i).GetComponent<Card>().cardValue == cardValue)
                result = true;
        }

        return result;
    }

    private void Clean()
    {
        for (int i = 0; i < offenceLayer.transform.childCount; i++)
        {
            //mb offenceLayer.transform.GetChild(i).gameObject
            Destroy(offenceLayer.transform.GetChild(i));
            Destroy(defenceLayer.transform.GetChild(i));
        }
    }

    public void TakeCards(Transform newParent)
    {
        for (int i = 0; i < offenceLayer.transform.childCount; i++)
        {
            offenceLayer.transform.GetChild(i).SetParent(newParent);
        }

        for (int i = 0; i < defenceLayer.transform.childCount; i++)
        {
            defenceLayer.transform.GetChild(i).SetParent(newParent);
        }
    }

    private bool IsEmpty()
    {
        if (this.offenceLayer.transform.childCount == 0)
        {
            return true;
        }
        return false;
    }

    private bool CanBePassed(Card card)
    {
        return ContainCardWithValue(card.cardValue) || IsEmpty();
    }

    private bool CardCanBeat(Card card)
    {
        Card cardToBeat = this.GetNotBeatedCard();

        if (card.suit == cardToBeat.suit && card.cardValue > cardToBeat.cardValue)
        {
            return true;
        }
        else if (card.suit == deck.GetTrumpSuit())
        {
            return true;
        }

        return false;
    }

    private void AddToTable(Dragable dragable)
    {
        Transform newParent;
        if (offenceMode)
        {
            newParent = offenceLayer.transform;
        }
        else
        {
            newParent = defenceLayer.transform;
        }

        dragable.transform.SetParent(newParent);
        dragable.enabled = false;
    }

    private void ReturnToHand(Dragable card)
    {
        card.ToHand();
    }
}
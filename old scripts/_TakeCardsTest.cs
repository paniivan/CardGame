using UnityEngine;

public class _TakeCardsTest : MonoBehaviour {

    public GameObject deckObj;
    private Deck deck;
    private int NUM_OF_CARDS = 1;


    void Start()
    {
        deck = deckObj.GetComponent<Deck>();
    }

    void OnEnable()
    {
        EventManager.OnClicked += TakeCards;
    }


    void OnDisable()
    {
        EventManager.OnClicked -= TakeCards;
    }

    public void TakeCards()
    {
        GameObject card;
        for (int i = 0; i < NUM_OF_CARDS; i++)
        {
            card = deck.GetTopCard();
            card.transform.SetParent(transform/*or this.gameObject.transform.parent*/);
        }

    }
}

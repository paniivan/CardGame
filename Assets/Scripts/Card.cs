using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine;

public class Card : NetworkBehaviour {

    public CardData cd;
    public enum CardSuit { Spades, Clubs, Diamonds, Hearts };
}

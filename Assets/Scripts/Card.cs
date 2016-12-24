using UnityEngine.UI;
using UnityEngine.Networking;

public class Card : NetworkBehaviour {

    [SyncVar]
    private Image img;
    [SyncVar]
    private int val = 0;
    //private suit;


    private void ChangeImg()
    {

    }



}

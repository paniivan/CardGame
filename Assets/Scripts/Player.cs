using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private Deck deck;

    private const float startYPosition = 148f;
    private GameObject table;
    private GameObject parent;

    void Awake()
    {
        table = GameObject.Find("Table");
        parent = GameObject.Find("MainParentObj");
        deck = GameObject.Find("Deck").GetComponent<Deck>();
        gameObject.transform.SetParent(parent.transform);
    }

    public override void OnStartLocalPlayer()
    {
        transform.localPosition = new Vector3(0, -startYPosition);

        //asking for trump
        if (!isServer)
        {
            this.CmdAskForTrump();
        }
        
        this.CmdRegisterPlayer();
    }

    public override void OnStartClient()
    {
        transform.localPosition = new Vector3(0, startYPosition);
    }

     /**
     * test method
     */
    void OnGUI()
    {
        //if (GUI.Button(new Rect(400, 10, 50, 50), "Change table number"))
        //{
        //    CmdchangeTable(Random.Range(1, 100));
        //}

        //if (GUI.Button(new Rect(400, 10, 50, 50), "Change table number"))
        //{
        //    //CmdchangeTable(Random.Range(1, 100));
        //    RpcTakeCard(Random.Range(1, 100));
        //}
    }

    [Command]
    private void CmdchangeTable(int n)
    {
        table.GetComponent<Table>().SetN(n);
    }
    
    [Command]
    private void CmdAskForTrump()
    {
        //running on server
        Debug.Log("In CmdAskForTrump");

        CardData trumpCardData = deck.GetTrumpCardData();
        RpcSendTrumpCardData(trumpCardData);

    }

    [ClientRpc]
    private void RpcSendTrumpCardData(CardData cd)
    {
        //running on client
        Debug.Log("In SendTrumpCardData");

        if (isServer)
        {
            return;
        }

        deck.SetTrump(cd);
    }
    
    [Command]
    private void CmdRegisterPlayer()
    {
        deck.RegisterPlayer(this);
    }

    [ClientRpc]
    public void RpcTakeCard(CardData cd)
    {
        if (isLocalPlayer)
        {
            GameObject card = deck.CreateCard(cd, this.transform);
        }
        else
        {
            //terrible crutch
            //if not local player make card flip and show the back
            cd.val = -1;
            GameObject card = deck.CreateCard(cd, this.transform);
        }
    }

}

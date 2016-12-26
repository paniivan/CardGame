using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    private Deck deck;
    private Table table;

    private const float startYPosition = 148f;
    private GameObject parent;
    private HandDropZone hdz;
    private TableDropZone tdz;

    public bool offenceMode;

    void Awake()
    {
        table = GameObject.Find("Table").GetComponent<Table>();
        parent = GameObject.Find("MainParentObj");
        deck = GameObject.Find("Deck").GetComponent<Deck>();
        hdz = this.GetComponent<HandDropZone>();
        tdz = this.GetComponent<TableDropZone>();
        gameObject.transform.SetParent(parent.transform);
    }

    public override void OnStartServer()
    {
       hdz.enabled = false;
    }

    public override void OnStartLocalPlayer()
    {
        transform.localPosition = new Vector3(0, -startYPosition);

        hdz.enabled = true;

        //asking for trump
        if (!isServer)
        {
            this.CmdAskForTrump();
        }
        
        this.CmdRegisterPlayer();

        //lets start with this
        //server(host player) make move first
        if (isServer)
        {
            this.offenceMode = true;
        }
        else
        {
            this.offenceMode = false;
        }
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

    //[Command]
    //private void CmdchangeTable(int n)
    //{
    //    table.GetComponent<Table>().SetN(n);
    //}
    
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

    [Command]
    public void CmdProposeCard(CardData cd)
    {
        table.ProposeCard(cd, this);
    }

    [ClientRpc]
    public void RpcSetOffenceMode(bool b)
    {
        //if changing albablabkjdkfjsdf
        this.offenceMode = b;
    }

    [ClientRpc]
    public void RpcAddCardToTable(CardData cd, bool tableLayer)
    {
        Transform cardParent;
        if (tableLayer)
        {
            cardParent = table.offenceLayer.transform;
            //adding placeholder to defenceLayer
            table.CreatePlaceholder();
        }
        else
        {
            cardParent = table.defenceLayer.transform;
            table.DeletePlaceholder();
        }

        //adding card to client table
        deck.CreateCard(cd, cardParent);
    }
    
    [Command]
    public void CmdDestroyOneCard()
    {
        this.RpcDestroyOneCard();
    }

    [ClientRpc]
    private void RpcDestroyOneCard()
    {
        if (!isLocalPlayer)
        {
            Destroy(this.transform.GetChild(0).gameObject);
        }
    }

}

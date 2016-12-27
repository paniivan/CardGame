using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    private Deck deck;
    private Table table;

    //magic numbers!!
    private const float startYPosition = 148f;
    private GameObject parent;
    private HandDropZone hdz;
    private TableDropZone tdz;

    public bool blockedMode;

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
            this.blockedMode = false;
        }
        else
        {
            this.blockedMode = true;
            table.gameObject.GetComponent<TableDropZone>().enabled = false;
        }
    }

    public override void OnStartClient()
    {
        transform.localPosition = new Vector3(0, startYPosition);
    }
    
    void OnGUI()
    {
        //if u r attacker and u did give at least one card already
        if (isLocalPlayer && !this.blockedMode && table.offenceMode && !table.ClientIsEmpty())
        {
            if (GUI.Button(new Rect(10, 250, 100, 50), "End move"))
            {
                this.CmdCleanTable();

                this.CmdSwitchPlayersMode();
                
                this.CmdRefillHands();
            }
        }

        //if u r defender and there are(is) card(s) on table to take
        if (isLocalPlayer && !this.blockedMode && !table.offenceMode && !table.ClientIsEmpty())
        {
            if (GUI.Button(new Rect(10, 250, 100, 50), "Take cards"))
            {
                this.CmdTakeCards();
                this.CmdCleanTable();

                //this.CmdSwitchDisablingTableDropzone();
                this.CmdSwitchPlayersMode();
                this.CmdSwitchTableMode();

                this.CmdRefillHands();
            }
        }



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
        GameObject card;
        if (isLocalPlayer)
        {
            card = deck.CreateCard(cd);
            card.transform.SetParent(this.transform);
            ////changing players hand size on the server
            //deck.CmdChangePlayersHandSize(this, 1);
        }
        else
        {
            //terrible crutch
            //if not local player make card flip and show the back
            cd.val = -1;
            card = deck.CreateCard(cd);
            card.transform.SetParent(this.transform);
            card.GetComponent<Dragable>().enabled = false;
        }
    }

    [Command]
    public void CmdProposeCard(CardData cd)
    {
        table.ProposeCard(cd, this);
    }

    [ClientRpc]
    public void RpcSwitchPlayersMode()
    {
        if (isLocalPlayer)
        {
            this.blockedMode = !this.blockedMode;
            table.gameObject.GetComponent<TableDropZone>().enabled
                   = !table.gameObject.GetComponent<TableDropZone>().enabled;
        }
    }

    //[ClientRpc]
    //public void RpcSwitchDisablingTableDropzone()
    //{
    //    if (isLocalPlayer)
    //    {
    //        table.gameObject.GetComponent<TableDropZone>().enabled
    //               = !table.gameObject.GetComponent<TableDropZone>().enabled;
    //    }
    //}
    
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

    [ClientRpc]
    public void RpcShowEndGameMssg(string mssg)
    {
        Debug.Log(mssg);
    }

    [Command]
    private void CmdCleanTable()
    {
        this.table.Clean();
    }

    [Command]
    private void CmdSwitchPlayersMode()
    {
        this.deck.SwitchPlayersMode();
    }

    [Command]
    private void CmdSwitchTableMode()
    {
        this.table.SwitchTableMode();
    }

    //[Command]
    //private void CmdSwitchDisablingTableDropzone()
    //{
    //    this.deck.SwitchDisablingTableDropzone();
    //}

    [Command]
    private void CmdRefillHands()
    {
        this.deck.RefillHands();
    }

    [Command]
    private void CmdTakeCards()
    {
        int numOfCards = this.table.TakeCards(this);
        this.deck.ChangePlayersHandSize(this, numOfCards);
    }
}

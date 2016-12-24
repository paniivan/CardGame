using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private const float startYPosition = 154.25f;
    private GameObject table;
    private GameObject parent;

    void Awake()
    {
        table = GameObject.Find("Table");
        parent = GameObject.Find("MainParentObj");
        gameObject.transform.SetParent(parent.transform);
    }

    public override void OnStartLocalPlayer()
    {
        transform.localPosition = new Vector3(0, -startYPosition);
    }

    public override void OnStartClient()
    {
        transform.localPosition = new Vector3(0, startYPosition);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(400, 10, 50, 50), "Change table number"))
        {
            CmdchangeTable(Random.Range(1, 100));
        }
    }

    [Command]
    private void CmdchangeTable(int n)
    {
        table.GetComponent<Table>().SetN(n);
    }
}

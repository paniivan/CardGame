using UnityEngine.Networking;

public class Table : NetworkBehaviour {

    [SyncVar]
    private int n;

    public void SetN(int n)
    {
        this.n = n;
    }
}

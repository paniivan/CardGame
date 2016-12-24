using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool overTable;
    public bool overHand;

    private GameObject placeholder;
    private Transform hand;
    private Table table;

    void Awake()
    {
        overTable = false;
        overHand = true;
        table = GameObject.Find("Table").GetComponent<Table>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log ("OnBeginDrag");
        hand = transform.parent;

        CreatePlaceholder();

        Transform entireCanvas = transform.parent.parent;
        this.transform.SetParent(entireCanvas);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log ("OnDrag");
        this.transform.position = eventData.position;
        
        if (overHand)
        {
            ChangeSinblingIndex();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log ("OnEndDrag");

        if (overTable)
        {
            table.ProposeCard(this.gameObject);
        }
        else
        {
            ToHand();
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(placeholder);
    }

    public void ToHand()
    {
        this.transform.SetParent(hand.transform);
    }
    
    private void CreatePlaceholder()
    {
        placeholder = new GameObject();
        placeholder.name = "CardPlaceholder";
        placeholder.transform.SetParent(hand);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
    }
    
    private void ChangeSinblingIndex()
    {
        //change placeholder's sibling index
        int newSiblingIndex = hand.childCount;

        for (int i = 0; i < hand.childCount; i++)
        {
            if (this.transform.position.x < hand.GetChild(i).position.x)
            {

                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;

                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

}

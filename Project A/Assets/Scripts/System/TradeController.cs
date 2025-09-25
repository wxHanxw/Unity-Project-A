using TMPro;
using UnityEngine;

public class TradeController : MonoBehaviour
{
    public int SellorBuy = 0;
    public int TradeCoin = 0;
    public GameObject ItemInfoPanel;
    public GameObject Canvas;
    public GameObject cantTradeImage;
    public GameObject TradeBackImage;

    public TMP_Text TradeNumText;
    private GameObject Character, InsItem;
    public bool ShowItemInfo = false;
    public bool isTrading = false;

    public bool isEmpty = false;

    //颜色变化
    private float ImageReddeltaTime = 0;
    private Color ImageInitialColor;
    // Start is called before the first frame update
    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
        TradeNumText.text = ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum.ToString();
        cantTradeImage.SetActive(false);
        ImageInitialColor = TradeBackImage.GetComponent<SpriteRenderer>().color;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isEmpty)
        {
            if (ImageReddeltaTime > 0)
            {
                ImageReddeltaTime -= Time.deltaTime;
                if (ImageReddeltaTime <= 0)
                {
                    TradeBackImage.GetComponent<SpriteRenderer>().color = ImageInitialColor;
                }
            }
            /*if (ItemInfoPanel != null && ShowItemInfo && InsItemInfoPanel == null)
            {
                InsItemInfoPanel = Instantiate(ItemInfoPanel, transform.position - new Vector3(10, 0, 0), Canvas.transform.rotation, Canvas.transform);
                InsItemInfoPanel.transform.localPosition = new Vector3(-400, 0, 0);
                InsItemInfoPanel.SetActive(true);
            }
            else if (!ShowItemInfo && InsItemInfoPanel != null)
            {
                Destroy(InsItemInfoPanel);
            }*/

            if (isTrading)
            {
                isTrading = false;
                TradeItem();
            }
        }
    }

    private void TradeItem()
    {
        //买入
        if (TradeCoin < 0 && Character.GetComponent<PlayerController>().TotalCoin >= -TradeCoin)
        {
            //扣钱
            Character.GetComponent<PlayerController>().TotalCoin += TradeCoin;
            Character.GetComponent<PlayerController>().CoinController();
            if (ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum > 1)
            {
                InsItem = Instantiate(ItemInfoPanel.transform.parent, ItemInfoPanel.transform.parent.transform.position, ItemInfoPanel.transform.parent.transform.rotation, ItemInfoPanel.transform.parent.transform.parent).gameObject;
                InsItem.GetComponent<ItemInfo>().ItemNum -= 1;
                ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum = 1;
            }
            else
            {
                TradeNumText.text = "0";
                cantTradeImage.SetActive(true);
                TradeBackImage.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            Character.GetComponent<PlayerController>().packageController.PutintoBag(ItemInfoPanel.transform.parent);
            ItemInfoPanel.transform.parent.gameObject.tag = "BagItem";
            Character.GetComponent<PlayerController>().TakingItemController(ItemInfoPanel.transform.parent.GetComponent<SpriteRenderer>().sprite, ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum);

            if (InsItem != null)
            {
                foreach (Transform child in InsItem.transform)
                {
                    ItemInfoPanel = child.gameObject;
                    TradeNumText.text = ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum.ToString();
                    break;
                }
                InsItem = null;
            }
            else
            {
                ItemInfoPanel = null;
                isEmpty = true;
            }

        }
        else if (TradeCoin < 0)
        {
            TradeBackImage.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.1f, 0.1f, 1);
            ImageReddeltaTime = 0.1f;
        }
    }
}

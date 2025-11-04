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

    private PlayerController playerController;

    //颜色变化
    private float ImageReddeltaTime = 0;
    private Color ImageInitialColor;
    // Start is called before the first frame update
    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
        playerController = Character.GetComponent<PlayerController>();
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
        if (SellorBuy == 0)
        {

            if (TradeCoin < 0 && playerController.TotalCoin >= -TradeCoin)
            {
                //扣钱
                playerController.TotalCoin += TradeCoin;
                playerController.CoinController();
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
                playerController.packageController.PutintoBag(ItemInfoPanel.transform.parent);
                ItemInfoPanel.transform.parent.gameObject.tag = "BagItem";
                playerController.TakingItemController(ItemInfoPanel.transform.parent.GetComponent<SpriteRenderer>().sprite, ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum);

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


        //卖出
        if (SellorBuy == 1)
        {
            for (int i = 0; i < playerController.BagItemID.Length; i++)
            {
                if (playerController.BagItemID[i] == ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemID)
                {
                    if (playerController.BagItemNum[i] >= ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum)
                    {
                        playerController.packageController.BagItemButton[i].GetComponent<PackageItemClick>().Item.GetComponent<ItemInfo>().ItemNum -= ItemInfoPanel.transform.parent.GetComponent<ItemInfo>().ItemNum;
                        playerController.packageController.BagItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                        playerController.TotalCoin += TradeCoin;
                        playerController.CoinController();
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PackageItemClick : MonoBehaviour
{
    public PackageController packageController;
    public GameObject InfoImage;
    public int ItemID;
    public GameObject Item;
    private Button Clickbutton;

    private TMP_Text ItemNumText;

    public int BagButtonID = -1, CaseButtonID = -1;

    public bool isClick = false;
    // Start is called before the first frame update
    void Start()
    {

        Clickbutton = GetComponent<Button>();
        Clickbutton.onClick.AddListener(ClickBagItem);


        //是否储存物体
        if (BagButtonID >= 0)
            UpdateItemNum(BagButtonID);
        else if (CaseButtonID >= 0)
            UpdateItemNum(CaseButtonID);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClickBagItem()
    {
        InfoImage.SetActive(false);
        if (packageController.ItemInfoPanel != null)
            Destroy(packageController.ItemInfoPanel);
        //背包
        if (BagButtonID >= 0)
        {
            for (int i = 0; i < packageController.BagItemButton.Length; i++)
            {
                if (i == BagButtonID)
                {
                    packageController.BagItemIndex = BagButtonID;
                    packageController.EquipItemIndex = -1;
                    packageController.CaseItemIndex = -1;
                    foreach (Transform child in gameObject.transform)
                    {
                        if (child.tag == "BagItem")
                        {
                            InfoImage.transform.position = transform.position + new Vector3(250, 0, 0);
                            isClick = !isClick;
                            InfoImage.SetActive(isClick);
                            packageController.EquipIndex = child.GetComponent<ItemInfo>().EquipType;
                            foreach (Transform childchild in child.transform)
                            {
                                packageController.ItemInfoPanel = Instantiate(childchild, InfoImage.transform.position + new Vector3(0, 0, 0), transform.rotation, InfoImage.transform).gameObject;
                                packageController.ItemInfoPanel.SetActive(true);
                                break;
                            }
                            break;
                        }

                    }
                }
                else
                {
                    packageController.BagItemButton[i].GetComponent<PackageItemClick>().isClick = false;
                }
            }
            for (int i = 0; i < packageController.CaseItemButton.Length; i++)
            {
                packageController.CaseItemButton[i].GetComponent<PackageItemClick>().isClick = false;
            }
        }
        //箱子
        if (CaseButtonID >= 0)
        {
            for (int i = 0; i < packageController.CaseItemButton.Length; i++)
            {
                if (packageController.CaseItemButton[i].gameObject == gameObject)
                {
                    packageController.CaseItemIndex = i;
                    packageController.EquipItemIndex = -1;
                    packageController.BagItemIndex = -1;
                    foreach (Transform child in gameObject.transform)
                    {
                        if (child.tag == "BagItem")
                        {
                            InfoImage.transform.position = transform.position + new Vector3(250, 0, 0);
                            isClick = !isClick;
                            InfoImage.SetActive(isClick);
                            foreach (Transform childchild in child.transform)
                            {
                                packageController.ItemInfoPanel = Instantiate(childchild, InfoImage.transform.position + new Vector3(0, 0, 0), transform.rotation, InfoImage.transform).gameObject;
                                packageController.ItemInfoPanel.SetActive(true);
                                break;
                            }
                            break;
                        }

                    }
                }
                else
                {
                    packageController.CaseItemButton[i].GetComponent<PackageItemClick>().isClick = false;
                }
            }

            for (int i = 0; i < packageController.BagItemButton.Length; i++)
            {
                packageController.BagItemButton[i].GetComponent<PackageItemClick>().isClick = false;
            }
        }

        //已装备
        for (int i = 0; i < packageController.EquipItemButton.Length; i++)
        {
            if (packageController.EquipItemButton[i].gameObject == gameObject)
            {
                packageController.EquipItemIndex = i;
                packageController.BagItemIndex = -1;
                packageController.CaseItemIndex = -1;

                foreach (Transform child in gameObject.transform)
                {
                    if (child.tag == "BagItem")
                    {
                        InfoImage.transform.position = transform.position + new Vector3(250, 0, 0);
                        isClick = !isClick;
                        InfoImage.SetActive(isClick);
                        packageController.EquipIndex = child.GetComponent<ItemInfo>().EquipType;
                        foreach (Transform childchild in child.transform)
                        {
                            packageController.ItemInfoPanel = Instantiate(childchild, InfoImage.transform.position + new Vector3(0, 0, 0), transform.rotation, InfoImage.transform).gameObject;
                            packageController.ItemInfoPanel.SetActive(true);
                        }
                    }
                }
                break;
            }
            else
            {
                packageController.EquipItemButton[i].GetComponent<PackageItemClick>().isClick = false;
            }
        }

    }

    public void UpdateItemNum(int ButtonID)
    {
        bool isEmpty = true;
        if (gameObject.tag == "BagButton")
        {
            if (ItemNumText == null)
                foreach (Transform child in gameObject.transform)
                {
                    if (child.name == "Text (TMP)")
                    {
                        ItemNumText = child.GetComponent<TMP_Text>();
                        ItemNumText.gameObject.SetActive(false);
                    }
                }
            ItemNumText.gameObject.SetActive(false);
            foreach (Transform child in gameObject.transform)
            {
                if (child.tag == "BagItem" && child.GetComponent<ItemInfo>().EquipType == -1 && child.GetComponent<ItemInfo>().ItemNum > 0)
                {
                    Item = child.gameObject;
                    //储存到json
                    if (BagButtonID >= 0)
                    {
                        packageController.playerController.BagItemID[ButtonID] = Item.GetComponent<ItemInfo>().ItemID;
                        packageController.playerController.BagItemNum[ButtonID] = Item.GetComponent<ItemInfo>().ItemNum;
                    }
                    else if (CaseButtonID >= 0)
                    {
                        packageController.playerController.CaseItemID[ButtonID] = Item.GetComponent<ItemInfo>().ItemID;
                        packageController.playerController.CaseItemNum[ButtonID] = Item.GetComponent<ItemInfo>().ItemNum;
                    }
                    ItemID = child.GetComponent<ItemInfo>().ItemID;
                    ItemNumText.text = child.GetComponent<ItemInfo>().ItemNum.ToString();
                    ItemNumText.gameObject.SetActive(true);
                    isEmpty = false;

                }
                else if (child.tag == "BagItem" && child.GetComponent<ItemInfo>().EquipType >= 0)
                {
                    Item = child.gameObject;
                    if (BagButtonID >= 0)
                    {
                        packageController.playerController.BagItemID[ButtonID] = Item.GetComponent<ItemInfo>().ItemID;
                        packageController.playerController.BagItemNum[ButtonID] = Item.GetComponent<ItemInfo>().ItemNum;
                    }
                    else if (CaseButtonID >= 0)
                    {
                        packageController.playerController.CaseItemID[ButtonID] = Item.GetComponent<ItemInfo>().ItemID;
                        packageController.playerController.CaseItemNum[ButtonID] = Item.GetComponent<ItemInfo>().ItemNum;
                    }
                    isEmpty = false;
                }
                else if (child.tag == "BagItem" && child.GetComponent<ItemInfo>().ItemNum <= 0)
                {

                    ItemID = 0;
                    if (BagButtonID >= 0)
                    {
                        packageController.playerController.BagItemID[ButtonID] = 0;
                        packageController.playerController.BagItemNum[ButtonID] = 0;
                    }
                    else if (CaseButtonID >= 0)
                    {
                        packageController.playerController.CaseItemID[ButtonID] = 0;
                        packageController.playerController.CaseItemNum[ButtonID] = 0;
                    }


                    GetComponent<Image>().sprite = packageController.EmptyImage.sprite;
                    Destroy(packageController.ItemInfoPanel);
                    InfoImage.SetActive(false);
                    Destroy(child.gameObject);
                }
            }

            if (isEmpty)
            {
                ItemID = 0;
                if (BagButtonID >= 0)
                {
                    packageController.playerController.BagItemID[ButtonID] = 0;
                    packageController.playerController.BagItemNum[ButtonID] = 0;
                }
                else if (CaseButtonID >= 0)
                {
                    packageController.playerController.CaseItemID[ButtonID] = 0;
                    packageController.playerController.CaseItemNum[ButtonID] = 0;
                }
                GetComponent<Image>().sprite = packageController.EmptyImage.sprite;
            }
        }

    }

}

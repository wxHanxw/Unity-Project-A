using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PackageController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Character;
    public PlayerController playerController;
    private UIController uIController;
    public GameObject InfoImage, PackagePage, CharacterInfoPage;
    public Image EmptyImage;
    public Button EquipButton, toPackageButton, toCharacterInfoButton;
    private GameObject[] BagSpace, CaseSpace;
    public Button[] EquipItemButton;
    public Button[] EquipSkillButton;
    public GameObject[] CharacterEquip;

    public GameObject ItemInfoPanel;
    public GameObject AllBagSpace, AllCaseSpace;

    public Button[] BagItemButton, CaseItemButton;
    public int BagItemIndex, EquipItemIndex, EquipIndex, CaseItemIndex;

    void Start()
    {
        playerController = Character.GetComponent<PlayerController>();
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        //背包
        GameObject[] BagSpaceInitial = GameObject.FindGameObjectsWithTag("BagSpace");
        BagSpace = new GameObject[BagSpaceInitial.Length];
        BagItemButton = new Button[BagSpaceInitial.Length];
        int j = 0;
        foreach (Transform child in AllBagSpace.transform)
        {
            if (child.tag == "BagSpace")
            {
                BagSpace[j] = child.gameObject;
                j = j + 1;
            }
        }
        for (int i = 0; i < BagSpaceInitial.Length; i++)
        {
            //寻找子物体（正在使用的技能）
            foreach (Transform child in BagSpace[i].transform)
            {
                if (child.name == "Button")
                {
                    BagItemButton[i] = child.gameObject.GetComponent<Button>();
                    BagItemButton[i].GetComponent<PackageItemClick>().BagButtonID = i;
                }

                //使用闭包绑定按钮点击事件
            }
        }


        //箱子
        GameObject[] CaseSpaceInitial = GameObject.FindGameObjectsWithTag("CaseSpace");
        CaseSpace = new GameObject[CaseSpaceInitial.Length];
        CaseItemButton = new Button[CaseSpaceInitial.Length];
        j = 0;
        foreach (Transform child in AllCaseSpace.transform)
        {
            if (child.tag == "CaseSpace")
            {
                CaseSpace[j] = child.gameObject;
                j = j + 1;
            }
        }
        for (int i = 0; i < CaseSpaceInitial.Length; i++)
        {
            //寻找子物体
            foreach (Transform child in CaseSpace[i].transform)
            {
                if (child.name == "Button")
                {
                    CaseItemButton[i] = child.gameObject.GetComponent<Button>();
                    CaseItemButton[i].GetComponent<PackageItemClick>().CaseButtonID = i;
                }
                //使用闭包绑定按钮点击事件
            }
        }

        EquipButton.onClick.AddListener(ClickEquipButton);
        toPackageButton.onClick.AddListener(toBag);
        toCharacterInfoButton.onClick.AddListener(toInfo);

        //初始化装备栏
        foreach (Transform child in playerController.EquipmentStore.transform)
        {
            for (int i = 0; i < 4; i++)
            {
                if (child.GetComponent<ItemInfo>().ItemID == playerController.EquipmentID[i])
                {
                    UpdatePackageEquip(Instantiate(child), i);
                    break;
                }
            }
            //初始化背包
            for (int i = 0; i < playerController.BagItemID.Length; i++)
            {
                if (child.GetComponent<ItemInfo>().ItemID == playerController.BagItemID[i])
                {
                    child.GetComponent<ItemInfo>().ItemNum = playerController.BagItemNum[i];
                    Transform childA = Instantiate(child);
                    childA.SetParent(BagItemButton[i].gameObject.transform);
                    BagItemButton[i].gameObject.GetComponent<Image>().sprite = childA.GetComponent<SpriteRenderer>().sprite;
                    BagItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    childA.position = BagItemButton[i].transform.position;
                }
            }
            //初始化箱子
            for (int i = 0; i < playerController.CaseItemID.Length; i++)
            {
                if (child.GetComponent<ItemInfo>().ItemID == playerController.CaseItemID[i])
                {
                    child.GetComponent<ItemInfo>().ItemNum = playerController.CaseItemNum[i];
                    Transform childA = Instantiate(child);
                    childA.SetParent(CaseItemButton[i].gameObject.transform);
                    CaseItemButton[i].gameObject.GetComponent<Image>().sprite = childA.GetComponent<SpriteRenderer>().sprite;
                    CaseItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    childA.position = CaseItemButton[i].transform.position;
                }
            }
        }

        //初始化背包
        foreach (Transform child in playerController.ItemStore.transform)
        {
            for (int i = 0; i < playerController.BagItemID.Length; i++)
            {
                if (child.GetComponent<ItemInfo>().ItemID == playerController.BagItemID[i])
                {
                    child.GetComponent<ItemInfo>().ItemNum = playerController.BagItemNum[i];
                    Transform childA = Instantiate(child);
                    childA.SetParent(BagItemButton[i].gameObject.transform);
                    BagItemButton[i].gameObject.GetComponent<Image>().sprite = childA.GetComponent<SpriteRenderer>().sprite;
                    BagItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    childA.position = BagItemButton[i].transform.position;

                    break;
                }
            }
            for (int i = 0; i < playerController.CaseItemID.Length; i++)
            {
                if (child.GetComponent<ItemInfo>().ItemID == playerController.CaseItemID[i])
                {
                    child.GetComponent<ItemInfo>().ItemNum = playerController.CaseItemNum[i];
                    Transform childA = Instantiate(child);
                    childA.SetParent(CaseItemButton[i].gameObject.transform);
                    CaseItemButton[i].gameObject.GetComponent<Image>().sprite = childA.GetComponent<SpriteRenderer>().sprite;
                    CaseItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    childA.position = CaseItemButton[i].transform.position;

                    break;
                }
            }
        }

    }


    public void RefreshClick()
    {
        if (BagItemIndex >= 0)
        {
            BagItemButton[BagItemIndex].GetComponent<PackageItemClick>().isClick = false;
            BagItemButton[BagItemIndex].GetComponent<PackageItemClick>().InfoImage.SetActive(false);
        }
        else if (EquipIndex >= 0)
        {
            EquipItemButton[EquipIndex].GetComponent<PackageItemClick>().isClick = false;
            EquipItemButton[EquipIndex].GetComponent<PackageItemClick>().InfoImage.SetActive(false);
        }
        else if (CaseItemIndex >= 0)
        {
            CaseItemButton[CaseItemIndex].GetComponent<PackageItemClick>().isClick = false;
            CaseItemButton[CaseItemIndex].GetComponent<PackageItemClick>().InfoImage.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    //更新装备栏装备
    public void UpdatePackageEquip(Transform Item, int equipIndex)
    {
        InfoImage.SetActive(false);

        if (EquipItemButton[equipIndex].GetComponent<Image>().sprite != EmptyImage.sprite)
        {
            foreach (Transform child in EquipItemButton[equipIndex].gameObject.transform)
            {
                if (child.tag == "BagItem")
                {
                    PutintoBag(child);
                }

            }
        }
        Item.SetParent(EquipItemButton[equipIndex].gameObject.transform);
        EquipItemButton[equipIndex].gameObject.GetComponent<Image>().sprite = Item.GetComponent<SpriteRenderer>().sprite;


        //更新至Charater
        playerController.EquipmentID[equipIndex] = Item.GetComponent<ItemInfo>().ItemID;
        foreach (Transform childA in CharacterEquip[equipIndex].transform)
        {
            Destroy(childA.gameObject);
        }
        Vector3 Scale = Item.lossyScale;
        GameObject Equap = Instantiate(Item, CharacterEquip[equipIndex].transform.position + new Vector3(0, 0, 0), CharacterEquip[equipIndex].transform.rotation, CharacterEquip[equipIndex].transform).gameObject;
        Equap.transform.localScale = Scale;

        //if (Equap.GetComponent<ItemInfo>().AttackType == 1)
        // Equap.transform.localEulerAngles -= new Vector3(0, 0, 90);

        StartCoroutine(DelayExecution(playerController.EquipmentInformation));
        playerController.EquipmentInformation();
        //StartCoroutine(UpdateEquip());

    }

    void ClickEquipButton()
    {
        //背包界面
        if (!uIController.isCase)
        {
            //使用物品
            if (BagItemIndex != -1 && EquipIndex < 0)
            {
                foreach (Transform child in BagItemButton[BagItemIndex].transform)
                {
                    if (child.tag == "BagItem" && child.GetComponent<ItemInfo>().canUse)
                    {
                        child.GetComponent<ItemInfo>().ItemFunction();
                        child.GetComponent<ItemInfo>().ItemNum -= 1;
                        BagItemButton[BagItemIndex].GetComponent<PackageItemClick>().UpdateItemNum(BagItemIndex);
                        break;
                    }
                }
            }

            //穿上装备
            if (BagItemIndex != -1 && EquipIndex != -1)
            {
                //寻找子物体
                foreach (Transform child in BagItemButton[BagItemIndex].transform)
                {
                    if (child.tag == "BagItem")
                    {
                        BagItemButton[BagItemIndex].GetComponent<PackageItemClick>().isClick = false;
                        BagItemButton[BagItemIndex].GetComponent<Image>().sprite = EmptyImage.sprite;
                        BagItemButton[BagItemIndex].GetComponent<PackageItemClick>().UpdateItemNum(BagItemIndex);
                        playerController.BagItemID[BagItemIndex] = 0;
                        playerController.BagItemNum[BagItemIndex] = 0;
                        UpdatePackageEquip(child, EquipIndex);

                        BagItemIndex = -1;
                        EquipIndex = -1;
                    }

                }
            }
        }

        //箱子界面
        if (uIController.isCase)
        {
            //放入箱子
            if (BagItemIndex != -1)
            {
                foreach (Transform child in BagItemButton[BagItemIndex].transform)
                {
                    if (child.tag == "BagItem")
                    {
                        PutintoCase(child);
                        child.GetComponent<ItemInfo>().ItemNum -= 1;
                        BagItemButton[BagItemIndex].GetComponent<PackageItemClick>().UpdateItemNum(BagItemIndex);
                        break;
                    }
                }
            }

            //放入背包
            if (CaseItemIndex != -1)
            {
                foreach (Transform child in CaseItemButton[CaseItemIndex].transform)
                {
                    if (child.tag == "BagItem")
                    {
                        PutintoBag(child);
                        child.GetComponent<ItemInfo>().ItemNum -= 1;
                        CaseItemButton[CaseItemIndex].GetComponent<PackageItemClick>().UpdateItemNum(CaseItemIndex);
                        break;
                    }
                }
            }

        }

        //卸下装备
        if (EquipItemIndex != -1 && EquipIndex != -1)
        {
            //寻找子物体
            foreach (Transform child in EquipItemButton[EquipItemIndex].gameObject.transform)
            {

                if (child.tag == "BagItem")
                {
                    InfoImage.SetActive(false);
                    EquipItemButton[EquipItemIndex].GetComponent<PackageItemClick>().isClick = false;
                    PutintoBag(child);
                    EquipItemButton[EquipItemIndex].GetComponent<Image>().sprite = EmptyImage.sprite;

                    playerController.EquipmentID[EquipIndex] = 0;
                    //更新至Charater
                    foreach (Transform childA in CharacterEquip[EquipIndex].transform)
                    {
                        Destroy(childA.gameObject);
                    }
                    StartCoroutine(DelayExecution(playerController.EquipmentInformation));


                    EquipItemIndex = -1;
                    EquipIndex = -1;
                }

            }
        }
    }

    IEnumerator DelayExecution(System.Action action)
    {
        // 等待指定秒数
        yield return new WaitForSeconds(Time.deltaTime);

        // 执行目标方法
        if (action != null)
            action.Invoke();
    }
    public void PutintoBag(Transform child)
    {
        bool isPut = false;
        //已有物品叠加储存
        if (child.GetComponent<ItemInfo>().EquipType < 0)
        {

            for (int i = 0; i < BagSpace.Length; i++)
            {
                if (BagItemButton[i].GetComponent<PackageItemClick>().ItemID == child.GetComponent<ItemInfo>().ItemID)
                {
                    if (uIController.isCase && child.GetComponent<ItemInfo>().ItemNum > 1)
                    {
                        BagItemButton[i].GetComponent<PackageItemClick>().Item.GetComponent<ItemInfo>().ItemNum += 1;
                    }
                    else
                    {
                        BagItemButton[i].GetComponent<PackageItemClick>().Item.GetComponent<ItemInfo>().ItemNum += child.GetComponent<ItemInfo>().ItemNum;
                        Destroy(child.gameObject);
                        InfoImage.SetActive(false);
                    }
                    BagItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    isPut = true;
                    break;
                }
            }
        }

        //未有物品及装备储存
        if (!isPut)
        {
            for (int i = 0; i < BagSpace.Length; i++)
            {
                if (BagItemButton[i].GetComponent<Image>().sprite == EmptyImage.sprite)
                {
                    if (uIController.isCase && child.GetComponent<ItemInfo>().ItemNum > 1)
                    {
                        Debug.Log("Bb");
                        GameObject InsObj = Instantiate(child, BagItemButton[i].gameObject.transform).gameObject;
                        InsObj.GetComponent<ItemInfo>().ItemNum = 1;
                        BagItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    }
                    else
                    {
                        Debug.Log("Cb");
                        child.SetParent(BagItemButton[i].gameObject.transform);
                        BagItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                        if (uIController.isCase)
                            child.GetComponent<ItemInfo>().ItemNum += 1;
                        InfoImage.SetActive(false);
                    }
                    BagItemButton[i].gameObject.GetComponent<Image>().sprite = child.GetComponent<SpriteRenderer>().sprite;
                    child.position = BagItemButton[i].transform.position;
                    break;
                }
            }

        }

    }


    public void PutintoCase(Transform child)
    {
        bool isPut = false;
        //已有物品叠加储存
        if (child.GetComponent<ItemInfo>().EquipType < 0)
        {

            for (int i = 0; i < CaseSpace.Length; i++)
            {
                if (CaseItemButton[i].GetComponent<PackageItemClick>().ItemID == child.GetComponent<ItemInfo>().ItemID)
                {
                    if (child.GetComponent<ItemInfo>().ItemNum > 1)
                    {
                        CaseItemButton[i].GetComponent<PackageItemClick>().Item.GetComponent<ItemInfo>().ItemNum += 1;
                    }
                    else
                    {
                        CaseItemButton[i].GetComponent<PackageItemClick>().Item.GetComponent<ItemInfo>().ItemNum += child.GetComponent<ItemInfo>().ItemNum;
                        Destroy(child.gameObject);
                        InfoImage.SetActive(false);
                    }
                    CaseItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    isPut = true;
                    break;
                }
            }
        }

        //未有物品及装备储存
        if (!isPut)
        {
            for (int i = 0; i < CaseSpace.Length; i++)
            {
                if (CaseItemButton[i].GetComponent<Image>().sprite == EmptyImage.sprite)
                {
                    if (child.GetComponent<ItemInfo>().ItemNum > 1)
                    {
                        GameObject InsObj = Instantiate(child, CaseItemButton[i].gameObject.transform).gameObject;
                        InsObj.GetComponent<ItemInfo>().ItemNum = 1;
                        CaseItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                    }
                    else
                    {
                        child.SetParent(CaseItemButton[i].gameObject.transform);
                        CaseItemButton[i].GetComponent<PackageItemClick>().UpdateItemNum(i);
                        child.GetComponent<ItemInfo>().ItemNum += 1;
                        InfoImage.SetActive(false);
                    }
                    CaseItemButton[i].gameObject.GetComponent<Image>().sprite = child.GetComponent<SpriteRenderer>().sprite;

                    child.position = CaseItemButton[i].transform.position;
                    break;
                }
            }

        }

    }

    public void toBag()
    {
        PackagePage.SetActive(true);
        CharacterInfoPage.SetActive(false);
    }

    public void toInfo()
    {
        PackagePage.SetActive(false);
        CharacterInfoPage.SetActive(true);
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Task : MonoBehaviour
{
    private bool isFinished = false;

    public bool isKillTask = true;
    public int KillAimNum;
    public int FinishKillAimNum;
    public int KillAimID = 1001;


    public bool isCollectTask = false;
    public int CollectAimID = 1001;
    public int CollectNeedNum = 1;
    public int CollectTakeOutNum = 0;
    public int FinishCollectNum = 0;
    private int BagIndex = 0;

    public bool isTakingTask = false;
    public bool UnTakeTask = true;

    //奖励
    public GameObject TaskReward;

    public GameObject RewardItem;
    public int RewardNum = 1;

    public TaskFinish taskFinish;


    private FNPCInfo NPCInfo;
    private PlayerController playerController;
    private TMP_Text AimNumText;
    // Start is called before the first frame update
    void Start()
    {
        NPCInfo = gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<FNPCInfo>();
        //寻找Skill的子物体（正在使用的技能）
        foreach (Transform child in gameObject.transform)
        {
            foreach (Transform childchild in child)
            {
                foreach (Transform childchildchild in childchild)
                {
                    if (childchildchild.name == "AimNumText")
                        AimNumText = childchildchild.gameObject.GetComponent<TMP_Text>();
                }
            }
        }
        playerController = GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerController>();
        KillNumUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnTakeTask && NPCInfo.NoteIndex != 1)
        {
            NPCInfo.NoteIndex = 1;
            foreach (Transform child in NPCInfo.PreTalkBar.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject Icon = Instantiate(NPCInfo.NoteIcon[1], NPCInfo.PreTalkBar.transform.position, NPCInfo.PreTalkBar.transform.rotation, NPCInfo.PreTalkBar.transform);
            Icon.transform.localPosition += new Vector3(0, 0, -0.1f);
            Icon.SetActive(true);
        }
        else if (isTakingTask && NPCInfo.NoteIndex != 2)
        {
            NPCInfo.NoteIndex = 2;
            foreach (Transform child in NPCInfo.PreTalkBar.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject Icon = Instantiate(NPCInfo.NoteIcon[2], NPCInfo.PreTalkBar.transform.position, NPCInfo.PreTalkBar.transform.rotation, NPCInfo.PreTalkBar.transform);
            Icon.transform.localPosition += new Vector3(0, 0, -0.1f);
            Icon.SetActive(true);
        }
        else if (isFinished && NPCInfo.NoteIndex != 3 && NPCInfo.NoteIndex != 0)
        {
            NPCInfo.NoteIndex = 3;
            foreach (Transform child in NPCInfo.PreTalkBar.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject Icon = Instantiate(NPCInfo.NoteIcon[3], NPCInfo.PreTalkBar.transform.position, NPCInfo.PreTalkBar.transform.rotation, NPCInfo.PreTalkBar.transform);
            Icon.transform.localPosition += new Vector3(0, 0, -0.1f);
            Icon.SetActive(true);
        }
    }

    //任务奖励
    public void TaskRewardController()
    {

        if (TaskReward.activeSelf && isCollectTask)
        {
            playerController.packageController.BagItemButton[BagIndex].GetComponent<PackageItemClick>().Item.GetComponent<ItemInfo>().ItemNum -= CollectTakeOutNum;
            playerController.packageController.BagItemButton[BagIndex].GetComponent<PackageItemClick>().UpdateItemNum(BagIndex);
        }

        if (isFinished && NPCInfo.NoteIndex != 0)
        {
            NPCInfo.NoteIndex = 0;
            foreach (Transform child in NPCInfo.PreTalkBar.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject Icon = Instantiate(NPCInfo.NoteIcon[0], NPCInfo.PreTalkBar.transform.position, NPCInfo.PreTalkBar.transform.rotation, NPCInfo.PreTalkBar.transform);
            Icon.transform.localPosition += new Vector3(0, 0, -0.1f);
            Icon.SetActive(true);
        }
        // playerController.TakingItemController(RewardItem.GetComponent<SpriteRenderer>().sprite, RewardNum);
        //金币奖励
        if (RewardItem.name == "CoinCopper")
        {
            playerController.TotalCoin += RewardNum;
            playerController.CoinController();
        }
        else if (RewardItem.name == "CoinSilver")
        {
            playerController.TotalCoin += RewardNum * 100;
            playerController.CoinController();
        }
        else if (RewardItem.name == "CoinGold")
        {
            playerController.TotalCoin += RewardNum * 10000;
            playerController.CoinController();
        }
        else if (TaskReward.activeSelf)
        {
            RewardItem.GetComponent<ItemInfo>().GetItem();
        }

        if (taskFinish != null)
        {
            taskFinish.enabled = true;
        }
        TaskReward.SetActive(false);
        //在任务列表中删除
        playerController.gameObject.GetComponent<TaskController>().TakingTasks.Remove(gameObject.GetComponent<Task>());
        Destroy(gameObject);
    }

    public void KillNumUpdate()
    {
        if (isKillTask)
        {
            if (FinishKillAimNum >= KillAimNum)
            {
                isTakingTask = false;
                if (!isFinished)
                    TaskReward.SetActive(true);
                isFinished = true;

            }
        }

        AimNumText.text = "(" + FinishKillAimNum.ToString() + "/" + KillAimNum.ToString() + ")";
        if (isFinished)
        {
            AimNumText.color = new Color(0, 1, 0);
        }
        else
        {
            AimNumText.color = new Color(1, 0, 0);
        }
    }

    public void CollectNumUpdate()
    {
        if (isCollectTask)
        {
            for (int i = 0; i < playerController.BagItemID.Length; i++)
            {
                if (playerController.BagItemID[i] == CollectAimID)
                {
                    FinishCollectNum = playerController.BagItemNum[i];
                    if (playerController.BagItemNum[i] >= CollectNeedNum)
                    {
                        BagIndex = i;
                        isTakingTask = false;
                        if (!isFinished)
                            TaskReward.SetActive(true);
                        isFinished = true;
                    }
                }
            }

        }
        AimNumText.text = "(" + FinishCollectNum.ToString() + "/" + CollectNeedNum.ToString() + ")";
        if (isFinished)
        {
            AimNumText.color = new Color(0, 1, 0);
        }
        else
        {
            AimNumText.color = new Color(1, 0, 0);
        }
    }
}

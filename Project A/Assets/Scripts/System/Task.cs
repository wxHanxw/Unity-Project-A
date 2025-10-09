using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Task : MonoBehaviour
{
    private bool isFinished = false;
    public int KillAimNum;
    public int FinishKillAimNum;
    public int KillAimID = 1001;
    public bool isTakingTask = false;
    public bool UnTakeTask = true;

    //奖励
    public GameObject TaskReward;

    public GameObject RewardItem;
    public int RewardNum = 1;


    private FNPCInfo NPCInfo;
    private PlayerController PlayerController;
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
        PlayerController = GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerController>();
        NumUpdate();
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

    private void KillTask()
    {
        if (FinishKillAimNum >= KillAimNum)
        {
            isTakingTask = false;
            if (!isFinished)
                TaskReward.SetActive(true);
            isFinished = true;

        }
    }

    //任务奖励
    public void TaskRewardController()
    {
        TaskReward.SetActive(false);
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
        PlayerController.TakingItemController(RewardItem.GetComponent<SpriteRenderer>().sprite, RewardNum);
        //金币奖励
        if (RewardItem.name == "CoinCopper")
        {
            PlayerController.TotalCoin += RewardNum;
            PlayerController.CoinController();
        }
        else if (RewardItem.name == "CoinSilver")
        {
            PlayerController.TotalCoin += RewardNum * 100;
            PlayerController.CoinController();
        }
        else if (RewardItem.name == "CoinGold")
        {
            PlayerController.TotalCoin += RewardNum * 10000;
            PlayerController.CoinController();
        }
        else
        {
            RewardItem.GetComponent<ItemInfo>().GetItem();
        }

        //在任务列表中删除
        PlayerController.gameObject.GetComponent<TaskController>().TakingTasks.Remove(gameObject.GetComponent<Task>());
        Destroy(gameObject);
    }

    public void NumUpdate()
    {
        KillTask();
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
}

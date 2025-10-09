using System.Collections;
using System.Collections.Generic;
using ExternPropertyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class FNPCInfo : MonoBehaviour
{
    [HorizontalLine]
    [Header("Basic Information")]
    public int ID = 1001;
    public bool isGhost = false;
    public float NPCMaxHP = 10;
    public float NPCMaxMP = 10;
    public float NPCDefence = 0;
    public float NPCSpeed = 3;
    public float RegainHP = 1;
    public float RegainMP = 1;

    public bool CanMove = true;

    public bool OnlyTalk = false;

    [HideInInspector]
    public float NPCHP, NPCMP, GetDamage, RealGetDamage, GetHeal;
    private float RegainHPdeltaTime, RegainMPdeltaTime;

    [HorizontalLine]
    [Header("AI Control")]
    public float IdelIntervalTime = 4;
    public float IdelMoveRange = 2;
    public GameObject AttackFollowRange;

    private float IdelIntervaldeltaTime = 0;

    private NavMeshAgent navMeshAgent;

    private Vector3 InitialPosition;

    [HorizontalLine]
    public GameObject Infos;
    public GameObject TakingBar;
    public GameObject PreTalkBar;
    public GameObject TradeBar;

    //任务系统
    private GameObject TaskText;
    public int NoteIndex = 0;
    public Task Task;

    public GameObject[] NoteIcon;

    public GameObject NPCSprite;

    public GameObject NormalAttack;

    public float IntDistance;

    private GameObject Character, AttackAim;

    //灵魂交互
    public GameObject Camera;
    public GameObject HintTexture;
    private Vector3 InitialHintPosition;

    //
    public GameObject NameBar;

    [HideInInspector]
    public bool CanInt = true, isBattle = false, isDead = false, isinTeam = false, CanAttack = true, isTask = false, canReward = false, isTrade = false;
    // Start is called before the first frame update
    void Start()
    {
        NPCHP = NPCMaxHP;
        NPCMP = NPCMaxMP;
        Character = GameObject.FindGameObjectWithTag("Character");
        navMeshAgent = GetComponent<NavMeshAgent>();
        InitialPosition = transform.position;
        AttackFollowRange.GetComponent<ColliderTrigger>().TochedTag = "Enemy";
        foreach (Transform child in Task.gameObject.transform)
        {
            if (child.name == "TaskText")
            {
                TaskText = child.gameObject;
            }

        }
        InitialHintPosition = HintTexture.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Character == null)
        {
            Character = GameObject.FindGameObjectWithTag("Character");
        }
        if (!isDead)
        {
            HPController();

            RandomMove();
            NPCInteractionController();
            NPCGhostInteractionController();

            if (!isGhost)
                AttackFollowCheck();

            if (isinTeam && !isBattle)
                TeamFollow();

            if (TaskText != null)
                NPCTaskController();

            //NPCTradeController();
        }
        else if (NPCSprite.activeSelf)
        {
            NPCSprite.SetActive(false);
            if (Character.GetComponent<PlayerController>().HitAim == gameObject)
            {
                Character.GetComponent<PlayerController>().HitAim = null;
                Character.GetComponent<PlayerController>().isChooseItem = false;
                Character.GetComponent<PlayerController>().Chooser.SetActive(false);
            }
            this.GetComponent<Collider>().enabled = false;
            NormalAttack.SetActive(false);
        }
    }

    //幽灵附体
    private void NPCGhostInteractionController()
    {
        if (HintTexture.activeSelf)
        {
            HintTexture.transform.position = transform.position + InitialHintPosition + 0.1f * new Vector3(0, math.sin(10 * Time.time), 0);
            if (!OnlyTalk && Input.GetKeyDown(KeyCode.F))
            {
                Character.GetComponent<PlayerController>().PlayerHP = Character.GetComponent<PlayerController>().FinalCharacterInfos[0];
                Character.GetComponent<PlayerController>().CharacterSprite.GetComponent<SpriteRenderer>().sprite = NPCSprite.GetComponent<SpriteRenderer>().sprite;
                Character.GetComponent<PlayerController>().CharacterSpriteBack.GetComponent<SpriteRenderer>().sprite = NPCSprite.GetComponent<SpriteRenderer>().sprite;
                Character.GetComponent<PlayerController>().BeAttackedSprite.GetComponent<SpriteRenderer>().sprite = NPCSprite.GetComponent<SpriteRenderer>().sprite;
                Character.GetComponent<PlayerController>().GhostController();
                //附体
                Character.GetComponent<CharacterController>().enabled = false;
                Character.GetComponent<NavMeshAgent>().enabled = false;
                Character.transform.position = transform.position;
                Character.GetComponent<CharacterController>().enabled = true;
                Character.GetComponent<NavMeshAgent>().enabled = true;
                Destroy(gameObject);
            }
        }
        //交互提示
        if (Character.GetComponent<PlayerController>().isGhost == true && !isGhost && (transform.position - Character.transform.position).magnitude < 4)
        {
            if (Camera == null)
                Camera = GameObject.FindGameObjectWithTag("MainCamera");
            else
                HintTexture.transform.eulerAngles = new Vector3(Camera.transform.eulerAngles.x - 17, Camera.transform.eulerAngles.y, HintTexture.transform.eulerAngles.z);

            if (!HintTexture.activeSelf)
            {
                Character.GetComponent<PlayerController>().canGetItem.Add(gameObject);
                HintTexture.SetActive(true);
            }

        }
        else if (HintTexture.activeSelf)
        {
            Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
            HintTexture.SetActive(false);
        }
    }
    private void NPCInteractionController()
    {
        if (((Character.GetComponent<PlayerController>().isGhost == false && !isGhost) || (Character.GetComponent<PlayerController>().isGhost && isGhost)) && (transform.position - Character.transform.position).magnitude < IntDistance)
        {
            CanInt = true;
        }
        else if ((transform.position - Character.transform.position).magnitude > 1.5 * IntDistance)
        {
            CanInt = false;
        }


        if (CanInt)
        {
            if (!isinTeam)
            {
                if (!OnlyTalk)
                    PreTalkBar.SetActive(!TakingBar.activeSelf);
                else
                    PreTalkBar.SetActive(true);
            }

            if (!OnlyTalk && Input.GetKeyDown(KeyCode.F) && (Character.GetComponent<PlayerController>().canGetItem == null || Character.GetComponent<PlayerController>().canGetItem.Count == 0))
            {
                TakingBar.SetActive(!TakingBar.activeSelf);
                if (TakingBar.activeSelf)
                {
                    Character.GetComponent<PlayerController>().HitAim = this.gameObject;
                    Character.GetComponent<PlayerController>().isChooseItem = true;
                    Character.GetComponent<PlayerController>().Chooser.SetActive(true);
                    isTask = false;
                    isTrade = false;
                    NPCTradeController();
                }
                else
                {
                    Character.GetComponent<PlayerController>().HitAim = null;
                    Character.GetComponent<PlayerController>().isChooseItem = false;
                    Character.GetComponent<PlayerController>().Chooser.SetActive(false);
                }
            }

            //名字显示
            if (Character.GetComponent<PlayerController>().VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y <= 0.4f)
            {
                NameBar.SetActive(true);
            }
            else
            {
                NameBar.SetActive(false);
            }
        }
        else
        {
            PreTalkBar.SetActive(false);
            TakingBar.SetActive(false);
            TradeBar.SetActive(false);
            NameBar.SetActive(false);
            if (isTrade)
            {
                isTrade = false;
                NPCTradeController();
            }
            isTask = false;
        }
    }
    private void HPController()
    {
        if (GetDamage != 0)
        {
            RealGetDamage = GetDamage - NPCDefence;
            if (RealGetDamage < 1)
            {
                RealGetDamage = 1;
            }
            NPCHP -= RealGetDamage;
            GetDamage = 0;
        }
        if (NPCHP <= 0)
        {
            NPCHP = 0;
            isDead = true;
        }

        if (GetHeal != 0)
        {
            NPCHP += GetHeal;
            GetHeal = 0;
        }
        if (NPCHP >= NPCMaxHP)
        {
            NPCHP = NPCMaxHP;
        }

        //脱战回血
        if (!isBattle && NPCHP < NPCMaxHP)
        {
            RegainHPdeltaTime += Time.deltaTime;
            if (RegainHPdeltaTime > 1f)
            {
                NPCHP += RegainHP;
                RegainHPdeltaTime = 0;
            }
        }

        //回复魔法值
        RegainMPdeltaTime += Time.deltaTime;
        if (!isBattle && NPCMP < NPCMaxMP)
        {
            if (RegainMPdeltaTime > 1f)
            {
                NPCMP += RegainMP;
                RegainMPdeltaTime = 0;
            }
        }
        else if (NPCHP < NPCMaxHP)
        {
            if (RegainMPdeltaTime > 5f)
            {
                NPCMP += RegainMP;
                RegainMPdeltaTime = 0;
            }
        }

    }

    //随机移动
    private void RandomMove()
    {
        IdelIntervaldeltaTime += Time.deltaTime;
        //未进入战斗，时间间隔
        if (!TakingBar.activeSelf && !isBattle && IdelIntervaldeltaTime > IdelIntervalTime)
        {
            navMeshAgent.speed = NPCSpeed * 0.3f;
            System.Random random = new System.Random();
            IdelIntervaldeltaTime = ((float)random.NextDouble() / 2 - 1) * IdelIntervaldeltaTime / 2;
            float randomR = (float)random.NextDouble() * IdelMoveRange;
            float randomalpha = (float)random.NextDouble() * 2 * math.PI;
            Vector3 MovetoPosition = new Vector3(0, 0, 0);
            if (CanMove)
                MovetoPosition = new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha));

            navMeshAgent.stoppingDistance = 0;
            if (navMeshAgent.enabled)
                navMeshAgent.destination = InitialPosition + MovetoPosition;
        }
    }

    private void TeamFollow()
    {
        navMeshAgent.speed = NPCSpeed;
        navMeshAgent.stoppingDistance = 2;
        navMeshAgent.destination = Character.transform.position;
        InitialPosition = Character.transform.position;

    }

    public void NPCTradeController()
    {
        TradeBar.SetActive(isTrade);
        Character.GetComponent<PlayerController>().TradeBar.SetActive(isTrade);
    }
    private void NPCTaskController()
    {
        TaskText.SetActive(isTask);
        if (isTask)
        {
            PreTalkBar.SetActive(false);
            if (Task.UnTakeTask)
            {
                Task.UnTakeTask = false;
                Task.isTakingTask = true;
                Character.GetComponent<TaskController>().TakingTasks.Add(Task);
            }

        }

    }

    private void AttackFollowCheck()
    {
        if (AttackFollowRange.GetComponent<ColliderTrigger>().isToched && (AttackFollowRange.GetComponent<ColliderTrigger>().isTochedAim.transform.position - Character.transform.position).magnitude < 6)
        {
            isBattle = true;
            navMeshAgent.stoppingDistance = 1.5f;
            AttackAim = AttackFollowRange.GetComponent<ColliderTrigger>().isTochedAim;

            navMeshAgent.speed = NPCSpeed;
            navMeshAgent.destination = AttackAim.transform.position;

            if (AttackAim.GetComponent<EnemyInfo>().isDead)
            {
                AttackFollowRange.GetComponent<ColliderTrigger>().isToched = false;
                AttackFollowRange.GetComponent<ColliderTrigger>().isTochedAim = null;
                AttackAim = AttackFollowRange.GetComponent<ColliderTrigger>().isTochedAim;
            }

        }
        else
        {
            isBattle = false;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using ExternPropertyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class NPCInfo : MonoBehaviour
{
    [HorizontalLine]
    [Header("Basic Information")]
    public float NPCMaxHP = 10;
    public float NPCMaxMP = 10;
    public float NPCDefence = 0;
    public float NPCSpeed = 3;
    public float RegainHP = 1;
    public float RegainMP = 1;

    [HideInInspector]
    public float NPCHP, NPCMP, GetDamage, RealGetDamage;
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
    public GameObject TakingBar;
    public GameObject PreTalkBar;

    public GameObject NPCSprite;

    public GameObject NormalAttack;

    public float IntDistance;

    private GameObject Character, AttackAim;

    [HideInInspector]
    public bool CanInt = true, isBattle = false, isDead = false, isinTeam = false, CanAttack = true;
    // Start is called before the first frame update
    void Start()
    {
        NPCHP = NPCMaxHP;
        NPCMP = NPCMaxMP;
        Character = GameObject.FindGameObjectWithTag("Character");
        navMeshAgent = GetComponent<NavMeshAgent>();
        InitialPosition = transform.position;
        AttackFollowRange.GetComponent<ColliderTrigger>().TochedTag = "Enemy";
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            HPController();
            RandomMove();
            NPCInteractionController();

            AttackFollowCheck();

            if (isinTeam && !isBattle)
                TeamFollow();
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

    private void NPCInteractionController()
    {
        if ((transform.position - Character.transform.position).magnitude < IntDistance)
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
                PreTalkBar.SetActive(!TakingBar.activeSelf);
            if (Input.GetKeyDown(KeyCode.T))
            {
                TakingBar.SetActive(!TakingBar.activeSelf);
                if (TakingBar.activeSelf)
                {
                    Character.GetComponent<PlayerController>().HitAim = this.gameObject;
                    Character.GetComponent<PlayerController>().isChooseItem = true;
                    Character.GetComponent<PlayerController>().Chooser.SetActive(true);
                }
                else
                {
                    Character.GetComponent<PlayerController>().HitAim = null;
                    Character.GetComponent<PlayerController>().isChooseItem = false;
                    Character.GetComponent<PlayerController>().Chooser.SetActive(false);
                }
            }
        }
        else
        {
            PreTalkBar.SetActive(false);
            TakingBar.SetActive(false);
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
            Vector3 MovetoPosition = new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha));

            navMeshAgent.stoppingDistance = 0;
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

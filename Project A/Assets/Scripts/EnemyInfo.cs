using System.Collections;
using System.Collections.Generic;
using ExternPropertyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInfo : MonoBehaviour
{
    //basic info
    [HorizontalLine]
    [Header("Basic Information")]
    public float EnemyMaxHP = 1;
    public float MoveSpeed = 3;
    public float Attack = 1;
    public float AttackRange = 1;
    public float NormalAttackInterval = 1;
    private float NormalAttackIntervaldeltaTime = 0;

    public float EnemyDefence = 1;

    [HideInInspector]
    public float EnemyHP;

    [HideInInspector]
    public float GetDamage = 0;
    public GameObject GetDamageHolder;

    private GameObject[] DamageHolderList;
    private float[] DamageList;

    private GameObject AttackAim;


    private float RealGetDamage = 0;
    private GameObject Canvas;
    private NavMeshAgent navMeshAgent;

    [HorizontalLine]
    [Header("AI Idel")]
    public float IdelHeal = 1;
    public float IdelIntervalTime = 4;
    public float IdelMoveRange = 2;
    public float AttackFollowRange = 3;


    public GameObject EnemySprite;
    public GameObject EnemyBeAttackedSprite;

    private float BeAttackedIntervaldeltaTime = 0;

    private float IdelHealdeltaTime = 0;

    [HorizontalLine]
    [Header("Attack Models")]
    public GameObject NormalAttack;



    private float IdelIntervaldeltaTime = 0, WalkSpeed;

    private Vector3 InitialPosition, MovetoPosition;

    [HideInInspector]
    public bool isFollowing = false, isDead = false;

    private GameObject Character, UICanvas;
    // Start is called before the first frame update
    void Start()
    {
        DamageHolderList = new GameObject[10];
        DamageList = new float[10];
        NormalAttack.GetComponent<NormalAttackTrigger>().Damage = Attack;
        EnemyHP = EnemyMaxHP;
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        Character = GameObject.FindGameObjectWithTag("Character");
        UICanvas = GameObject.FindGameObjectWithTag("Canvas");
        InitialPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        WalkSpeed = navMeshAgent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            HPController();
            RandomMove();
            AttackFollowCheck();
            AttackController();
        }
        else if (EnemySprite.activeSelf)
        {
            EnemySprite.SetActive(false);
            Character.GetComponent<PlayerController>().HitAim = null;
            Character.GetComponent<PlayerController>().isChooseItem = false;
            Character.GetComponent<PlayerController>().Chooser.SetActive(false);
            this.GetComponent<Collider>().enabled = false;
            NormalAttack.SetActive(false);
            Canvas.GetComponent<UIController>().isBattle = false;
        }


    }

    private void AttackController()
    {
        NormalAttackIntervaldeltaTime += Time.deltaTime;
        if (isFollowing && (transform.position - Character.transform.position).magnitude < AttackRange)
        {
            if (NormalAttackIntervaldeltaTime > NormalAttackInterval)
            {
                NormalAttack.SetActive(true);
                NormalAttackIntervaldeltaTime = 0;
            }
        }
        if (NormalAttackIntervaldeltaTime > 5 * Time.deltaTime)
        {
            NormalAttack.SetActive(false);
        }
    }

    private void HPController()
    {
        if (BeAttackedIntervaldeltaTime <= 0.15f)
        {
            BeAttackedIntervaldeltaTime += Time.deltaTime;
            if (BeAttackedIntervaldeltaTime > 0.15f)
            {
                EnemyBeAttackedSprite.SetActive(false);
            }
        }
        if (GetDamage != 0)
        {
            RealGetDamage = GetDamage - EnemyDefence;
            if (RealGetDamage < 1)
            {
                RealGetDamage = 1;
            }
            EnemyHP -= RealGetDamage;
            //记录伤害来源
            for (int i = 0; i < DamageHolderList.Length; i++)
            {
                if (DamageHolderList[i] == GetDamageHolder)
                {
                    DamageList[i] += RealGetDamage;
                    break;
                }
                else if (DamageHolderList[i] == null)
                {
                    DamageHolderList[i] = GetDamageHolder;
                    DamageList[i] += RealGetDamage;
                    break;
                }
            }

            EnemyBeAttackedSprite.SetActive(true);
            BeAttackedIntervaldeltaTime = 0;
            if (Character.GetComponent<PlayerController>().HitAim == null || Character.GetComponent<PlayerController>().HitAim.tag != "Enemy")
            {
                Character.GetComponent<PlayerController>().HitAim = this.gameObject;
                Character.GetComponent<PlayerController>().isChooseItem = true;
                Character.GetComponent<PlayerController>().Chooser.SetActive(true);
            }
            GetDamage = 0;
        }
        if (EnemyHP <= 0)
        {
            EnemyHP = 0;
            isDead = true;
        }
        else if (EnemyHP > EnemyMaxHP)
        {
            EnemyHP = EnemyMaxHP;
        }
    }
    //随机移动
    private void RandomMove()
    {
        IdelIntervaldeltaTime += Time.deltaTime;
        if (!isFollowing && IdelIntervaldeltaTime > IdelIntervalTime)
        {
            navMeshAgent.speed = WalkSpeed;

            System.Random random = new System.Random();
            IdelIntervaldeltaTime = ((float)random.NextDouble() / 2 - 1) * IdelIntervaldeltaTime / 2;
            float randomR = (float)random.NextDouble() * IdelMoveRange;
            float randomalpha = (float)random.NextDouble() * 2 * math.PI;
            MovetoPosition = new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha));

            navMeshAgent.destination = InitialPosition + MovetoPosition;
        }
    }

    private void AttackFollowCheck()
    {
        float MaxDamge = 0;
        for (int i = 0; i < DamageHolderList.Length; i++)
        {
            if (DamageHolderList[i] == null)
            {
                break;
            }
            else if (MaxDamge < DamageList[i])
            {
                MaxDamge = DamageList[i];
                AttackAim = DamageHolderList[i];
            }
        }
        //攻击最高伤害来源
        if (AttackAim != null && (transform.position - InitialPosition).magnitude < 6 * IdelMoveRange)
        {
            isFollowing = true;
            navMeshAgent.speed = MoveSpeed;
            navMeshAgent.destination = AttackAim.transform.position;
        }

        //攻击附近的敌人
        if ((transform.position - InitialPosition).magnitude < 4 * IdelMoveRange && (transform.position - Character.transform.position).magnitude < AttackFollowRange)
        {
            isFollowing = true;
            navMeshAgent.speed = MoveSpeed;
            navMeshAgent.destination = Character.transform.position;
        }
        else if (AttackAim == null)
        {
            isFollowing = false;
        }

        //休战回血
        if (!isFollowing)
        {
            IdelHealdeltaTime += Time.deltaTime;
            if (IdelHealdeltaTime > 0.3f)
            {
                EnemyHP += IdelHeal;
                IdelHealdeltaTime = 0;
            }
        }

        Canvas.GetComponent<UIController>().isBattle = isFollowing;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NormalAttackTrigger : MonoBehaviour
{
    // [HideInInspector]
    public float Damage;
    public int BeatBack = 1;

    public Vector3 InitialPositionShift;
    public bool toEnemy = false;
    public bool toPlayer = false;
    public bool isFarAttack = false;

    [Header("近战模式: (0)挥砍 (1)旋转")]
    public int NearAttackMode = 0;
    public String FarAttackName = "FarAttack";

    [Header("远程模式: (0)加速追踪 (1)平抛 (2)上抛 (3)回旋 (4)直射")]
    public int FarAttackMode = 0;
    public GameObject AttackParticle;
    public bool canRetain = false;
    public float FarAttackRand;

    public GameObject TraceParticle;
    public GameObject ColliderParticle;
    public bool willDisappear = true;

    public float YSpeed = 5;
    private float[] ySpeed = new float[3];

    public float NearAttackDisdeltaTime = 0;
    public bool isCrit;
    public float BulletSpeed = 1;

    [HideInInspector]
    public Vector3 AttackDirection = new Vector3(0, 0, 0);
    private Vector3[] Direction = new Vector3[3], DamageDeriction;
    public GameObject AttackAim;
    private GameObject[] AttackingAim = new GameObject[3], AttackParticleIns = new GameObject[3];
    private float[] BulletdeltaTime = new float[3];

    [HideInInspector]
    public bool isGround;

    public GameObject Holder;
    public GameObject DamageParticle;

    private float AttackIntervaldeltaTime = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        if (isFarAttack && gameObject.name != FarAttackName)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFarAttack && gameObject.name == FarAttackName)
        {
            FarAttackController();
        }
        if (isFarAttack && gameObject.name != FarAttackName && !isGround)
        {
            System.Random random = new System.Random();
            if (TraceParticle != null)
            {
                GameObject Ins = Instantiate(TraceParticle, transform.position, transform.rotation);
                Ins.transform.localScale = TraceParticle.transform.lossyScale * ((float)random.NextDouble() / 1.5f + 0.5f);
                Ins.SetActive(true);
            }

        }


        if (!isFarAttack)
        {
            AttackIntervaldeltaTime += Time.deltaTime;
            if (NearAttackDisdeltaTime >= 0)
            {
                if (NearAttackMode == 1)
                {
                    gameObject.transform.eulerAngles += new Vector3(0, 0, -10 * 360 * Time.deltaTime);
                }
                NearAttackDisdeltaTime -= Time.deltaTime;
                if (NearAttackDisdeltaTime < 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void FarAttackController()
    {
        if (AttackDirection.magnitude > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (AttackParticleIns[i] == null)
                {
                    AttackParticleIns[i] = Instantiate(gameObject, transform.position + InitialPositionShift, transform.rotation);
                    AttackParticleIns[i].transform.localScale = gameObject.transform.lossyScale;
                    AttackParticleIns[i].GetComponent<SpriteRenderer>().enabled = true;
                    AttackParticleIns[i].GetComponent<Collider>().enabled = true;
                    if (AttackParticleIns[i].GetComponent<NormalAttackTrigger>().AttackParticle != null)
                    {
                        AttackParticleIns[i].GetComponent<NormalAttackTrigger>().AttackParticle.SetActive(true);
                    }

                    AttackingAim[i] = AttackAim;
                    ySpeed[i] = YSpeed;
                    Direction[i] = AttackDirection;
                    if (FarAttackMode == 1)
                    {
                        ySpeed[i] = 0;
                    }
                    else if (FarAttackMode == 2)
                    {
                        System.Random random = new System.Random();
                        Direction[i] = AttackingAim[i].transform.position + new Vector3((float)random.NextDouble() * FarAttackRand, 0.2f, (float)random.NextDouble() * FarAttackRand) - AttackParticleIns[i].transform.position;
                    }
                    BulletdeltaTime[i] = 0;
                    AttackDirection = new Vector3(0, 0, 0);
                    break;
                }

            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (AttackParticleIns[i] != null && !AttackParticleIns[i].GetComponent<NormalAttackTrigger>().isGround)
            {
                AttackParticleIns[i].GetComponent<NormalAttackTrigger>().toEnemy = toEnemy;
                AttackParticleIns[i].GetComponent<NormalAttackTrigger>().toPlayer = toPlayer;
                if (FarAttackMode == 0)
                {
                    Direction[i] = (AttackingAim[i].transform.position + new Vector3(0, 0.2f, 0) - AttackParticleIns[i].transform.position).normalized;
                    AttackParticleIns[i].transform.position += math.pow(math.min(BulletdeltaTime[i], 1) + 0.1f, 2f) * Direction[i] * BulletSpeed * Time.deltaTime;
                }
                else if (FarAttackMode == 1)
                {
                    ySpeed[i] -= Time.deltaTime * YSpeed;
                    AttackParticleIns[i].transform.eulerAngles += new Vector3(0, 0, 3 * 360 * Time.deltaTime);
                    AttackParticleIns[i].transform.position += (Direction[i] + new Vector3(0, ySpeed[i], 0)) * BulletSpeed * Time.deltaTime;
                }
                else if (FarAttackMode == 2)
                {
                    ySpeed[i] -= Time.deltaTime * 2 * YSpeed * BulletSpeed;
                    AttackParticleIns[i].transform.position += (Direction[i] + new Vector3(0, ySpeed[i], 0)) * BulletSpeed * Time.deltaTime;
                }
                else if (FarAttackMode == 3)
                {
                    Vector3 NewDirection = Direction[i] - Direction[i].normalized * BulletSpeed / 8 * BulletdeltaTime[i];
                    AttackParticleIns[i].transform.position += NewDirection * BulletSpeed * Time.deltaTime;
                    AttackParticleIns[i].transform.eulerAngles += new Vector3(0, 0, 5 * 360 * Time.deltaTime);
                    if ((AttackParticleIns[i].transform.position - transform.position).magnitude < 0.3f && NewDirection.x * Direction[i].x < 0)
                    {
                        Destroy(AttackParticleIns[i]);
                    }
                }
                else if (FarAttackMode == 4)
                {
                    AttackParticleIns[i].transform.position += Direction[i] * BulletdeltaTime[i] * BulletSpeed * Time.deltaTime;
                }
                BulletdeltaTime[i] += Time.deltaTime;
            }

        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground" && isFarAttack && gameObject.name != FarAttackName)
        {
            if (ColliderParticle != null)
                for (int i = 0; i < 10; i++)
                {
                    GameObject Ins = Instantiate(ColliderParticle, transform.position, transform.rotation);
                    System.Random random = new System.Random();
                    if (i == 0)
                    {
                        //Ins.GetComponent<AudioSource>().enabled = true;
                    }
                    Ins.transform.localScale = ColliderParticle.transform.lossyScale * ((float)random.NextDouble() / 1.5f + 0.5f);
                    Ins.SetActive(true);
                }

            if (!canRetain)
                Destroy(gameObject);
            else
            {
                isGround = true;
                gameObject.transform.eulerAngles = new Vector3(0, 0, -80);
            }
        }
        if (other.tag == "Character" && toPlayer)
        {
            if (gameObject.name != FarAttackName && (AttackIntervaldeltaTime > 0.25f || isFarAttack))
            {
                AttackIntervaldeltaTime = 0;
                other.gameObject.GetComponent<PlayerController>().BeAttackedDirection = (other.transform.position - gameObject.transform.position).normalized * BeatBack / 150 * Time.deltaTime;
                other.gameObject.GetComponent<PlayerController>().GetDamage = Damage;

                System.Random random = new System.Random();
                float randomR = (float)random.NextDouble() * 360;
                GameObject DP = Instantiate(DamageParticle, other.transform.position + new Vector3(0, 0.1f, 0), DamageParticle.transform.rotation);
                DP.transform.eulerAngles = new Vector3(randomR, randomR, randomR);
                DP.transform.localScale = DamageParticle.transform.lossyScale;
                DP.SetActive(true);
                DP.GetComponent<Animator>().enabled = true;
                DP.GetComponent<SpriteRotator>().enabled = true;
                DP.GetComponent<DispersedParticle>().enabled = true;
            }

            if (isFarAttack && gameObject.name != FarAttackName)
            {
                Destroy(gameObject);
            }
        }

        if (other.tag == "NPCFriend" && toPlayer)
        {
            other.gameObject.GetComponent<FNPCInfo>().GetDamage = Damage;
            if (isFarAttack && gameObject.name != FarAttackName)
            {
                Destroy(gameObject);
            }
        }

        /*if (other.tag == "NPCNeutrality")
        {
            other.gameObject.GetComponent<NNPCInfo>().GetDamage = Damage;
            if (isFarAttack)
            {
                Destroy(gameObject);
            }

        }*/

        if ((other.tag == "Enemy" || other.tag == "NPCNeutrality") && toEnemy)
        {
            if (Holder.tag == "Character")
            {
                if (!isFarAttack)
                {
                    //Holder.GetComponent<PlayerController>().PlayerMP += 1;
                    //Holder.GetComponent<PlayerController>().PlayerMP = math.min(Holder.GetComponent<PlayerController>().PlayerMP, Holder.GetComponent<PlayerController>().FinalCharacterInfos[4]);
                    //Holder.GetComponent<PlayerController>().BeAttackedDirection = -(other.transform.position - gameObject.transform.position).normalized * BeatBack * Holder.GetComponent<PlayerController>().FinalCharacterInfos[3] * Time.deltaTime / 800;
                    //Holder.GetComponent<PlayerController>().BeAttackedDirection.y = 0;
                }

            }

            other.gameObject.GetComponent<EnemyInfo>().BeAttackedDeriction = (other.transform.position - gameObject.transform.position).normalized * BeatBack * 2 * Time.deltaTime;
            other.gameObject.GetComponent<EnemyInfo>().BeAttackedDeriction.y = 0;
            if (isFarAttack)
                other.gameObject.GetComponent<EnemyInfo>().GetDamage = (int)(Damage * 0.3f);
            else
                other.gameObject.GetComponent<EnemyInfo>().GetDamage = Damage;
            other.gameObject.GetComponent<EnemyInfo>().isCrit = isCrit;
            other.gameObject.GetComponent<EnemyInfo>().GetDamageHolder = Holder;
            System.Random random = new System.Random();
            float randomR = (float)random.NextDouble() * 360;
            GameObject DP = Instantiate(DamageParticle, other.transform.position + new Vector3(0, 0.4f, 0), DamageParticle.transform.rotation);
            DP.transform.eulerAngles = new Vector3(randomR, randomR, randomR);
            DP.SetActive(true);
            DP.GetComponent<Animator>().enabled = true;
            DP.GetComponent<SpriteRotator>().enabled = true;
            DP.GetComponent<DispersedParticle>().enabled = true;
            if (isCrit)
            {
                DP.transform.localScale *= 1.5f;
            }
            if (isFarAttack && gameObject.name != FarAttackName)
            {
                Destroy(gameObject);
            }
        }
    }

}

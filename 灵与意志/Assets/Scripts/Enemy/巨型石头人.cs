using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor.Rendering;
using UnityEngine;
using Unity.Mathematics;
using TMPro;
using System;

public class 巨型石头人 : MonoBehaviour
{
    public GameObject[] AllNormalFarAttack;

    public GameObject[] SkillBEnemys;
    public Vector3[] RocksInitialPos;
    public GameObject NearAttack, Particle, DeadParticle, DownParticle, TraceParticle;

    private GameObject[] InsDownParticle = new GameObject[2];
    public GameObject BackRing, TreasureChest;
    public GameObject[] Rocks;
    private Vector3 BackRingInitialPos;
    private float SkillAdeltaTime = 0, SkillBdeltaTime = 0;
    private int usingSkill = 1;
    public TMP_Text Text;
    private EnemyInfo enemyInfo;

    private bool isDeadParticle = false;
    private float FinishdeltaTime = 0;

    private bool canTakeNormalAttackTimes = true;

    public AudioSource AttackSFX;
    public AudioSource SkillSFX;



    private float InitialNormalAttackInterval;
    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = gameObject.GetComponent<EnemyInfo>();
        InitialNormalAttackInterval = enemyInfo.NormalAttackInterval;
        BackRingInitialPos = BackRing.transform.position;
        RocksInitialPos = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            RocksInitialPos[i] = Rocks[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyInfo.canAttack && enemyInfo.NormalAttackTimes <= 2)
            NearAttackController();

        if (enemyInfo.NormalAttackTimes > 2 && SkillAdeltaTime == 0 && SkillBdeltaTime == 0)
        {
            SkillSFX.enabled = false;
            System.Random random = new System.Random();
            usingSkill = random.Next(0, 2);
            if (!Rocks[2].activeSelf)
            {
                usingSkill = 0;
            }
        }

        if (usingSkill == 0)
        {
            if (enemyInfo.isAttacking && enemyInfo.NormalAttackTimes > 2 && SkillAdeltaTime < 5)
            {
                SkillSFX.enabled = true;
                enemyInfo.animator.SetBool("isSkill", true);

                if (SkillAdeltaTime < 1)
                {
                    BackRing.transform.position += new Vector3(0, 1, 0) * Time.deltaTime * 1.5f;
                }
                else
                {
                    SkillA();
                }

                SkillAdeltaTime += Time.deltaTime;
                if (SkillAdeltaTime >= 4 && SkillAdeltaTime < 5)
                {
                    BackRing.transform.position -= new Vector3(0, 1, 0) * Time.deltaTime * 1.5f;
                }
                else if (SkillAdeltaTime >= 5)
                {
                    enemyInfo.animator.SetBool("isSkill", false);
                    SkillAdeltaTime = 0;
                    enemyInfo.NormalAttackTimes = 0;
                    enemyInfo.NormalAttackPredeltaTime = 0f;
                    enemyInfo.isAttacking = false;
                }
            }
        }
        else
        {
            if (enemyInfo.isAttacking && enemyInfo.NormalAttackTimes > 2 && SkillBdeltaTime < 4)
            {
                SkillSFX.enabled = true;
                int Index = 0;
                enemyInfo.animator.SetBool("isSkill", true);
                if (SkillBdeltaTime > 0.5 && SkillBdeltaTime < 2)
                {
                    for (int i = 0; i < Rocks.Length; i++)
                    {
                        if (Rocks[i].activeSelf)
                        {
                            Rocks[i].transform.position += new Vector3(0, 1, 0) * Time.deltaTime * 30f * SkillBdeltaTime;
                            Index = i;
                            break;
                        }
                    }
                }
                else if (SkillBdeltaTime >= 2)
                {
                    SkillB();
                }

                SkillBdeltaTime += Time.deltaTime;
                if (SkillBdeltaTime >= 3.5 && SkillBdeltaTime < 4)
                {
                    Rocks[Index].SetActive(false);
                    Rocks[Index].transform.position = RocksInitialPos[Index];
                }
                else if (SkillBdeltaTime >= 4)
                {
                    enemyInfo.animator.SetBool("isSkill", false);
                    SkillBdeltaTime = 0;
                    enemyInfo.NormalAttackTimes = 0;
                    enemyInfo.NormalAttackPredeltaTime = 0f;
                    enemyInfo.isAttacking = false;
                }
            }
        }

        if (enemyInfo.GetDamage != 0)
        {
            enemyInfo.canAttack = true;
        }

        if (enemyInfo.isOutRange)
        {
            enemyInfo.isFollowing = false;
            for (int i = 0; i < Rocks.Length; i++)
            {
                Rocks[i].SetActive(true);
            }

        }
        if (isDeadParticle && FinishdeltaTime < 8)
        {
            FinishdeltaTime += Time.deltaTime;
            if (FinishdeltaTime >= 8)
            {
                Text.gameObject.SetActive(false);
            }
            else if (FinishdeltaTime > 7)
            {
                Text.color -= new Color(0, 0, 0, 1) * Time.deltaTime;
            }
        }
        if (enemyInfo.EnemyHP < enemyInfo.EnemyMaxHP / 2)
        {
            enemyInfo.EnemyHP = (int)enemyInfo.EnemyMaxHP / 2;
            if (!isDeadParticle)
            {
                enemyInfo.canAttack = false;
                isDeadParticle = true;
                Text.color = new Color(1, 1, 1, 1);
                Text.text = "- 完 成 试 炼 -";
                Text.gameObject.SetActive(true);
                TreasureChest.SetActive(true);
                for (int i = 0; i < 20; i++)
                {
                    System.Random random = new System.Random();
                    float randomR = ((float)random.NextDouble() / 1.5f + 0.5f) * 3;
                    float randomalpha = (float)random.NextDouble() * 2 * math.PI;

                    GameObject Ins = Instantiate(DeadParticle, TreasureChest.transform.position + new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha)), transform.rotation);
                    if (i == 0)
                    {
                        Ins.GetComponent<AudioSource>().enabled = true;
                    }
                    Ins.transform.localScale = new Vector3(1f, 1f, 1f) * ((float)random.NextDouble() / 1.5f + 0.5f);
                    Ins.SetActive(true);
                }

                for (int i = 0; i < 50; i++)
                {
                    System.Random random = new System.Random();
                    float randomR = ((float)random.NextDouble() / 1.5f + 0.5f) * 3;
                    float randomalpha = (float)random.NextDouble() * 2 * math.PI;

                    GameObject Ins = Instantiate(DeadParticle, transform.position + new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha)), transform.rotation);
                    if (i == 0)
                    {
                        Ins.GetComponent<AudioSource>().enabled = true;
                    }
                    Ins.transform.localScale = new Vector3(1f, 1f, 1f) * ((float)random.NextDouble() / 1.5f + 0.5f);
                    Ins.SetActive(true);
                }
            }
        }
    }


    public void SkillA()
    {
        enemyInfo.canMove = false;
        enemyInfo.NormalAttackPredeltaTime += Time.deltaTime;
        if (enemyInfo.NormalAttackPredeltaTime > enemyInfo.NormalAttackPre)
        {
            //近战
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, AllNormalFarAttack.Length); // 第二个参数是排他的
            enemyInfo.NormalAttack = AllNormalFarAttack[randomNumber];
            enemyInfo.NormalAttack.SetActive(true);

            //远程
            enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackDirection = (enemyInfo.AttackAim.transform.position - new Vector3(0, 0.5f, 0) - BackRing.transform.position).normalized;
            //enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().InitialPositionShift = new Vector3(0,0,0);
            enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
            enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;
            enemyInfo.NormalAttackPredeltaTime = 0.5f;
        }
    }

    public void SkillB()
    {
        enemyInfo.NormalAttackPredeltaTime += Time.deltaTime;
        for (int i = 0; i < 2; i++)
        {
            if (InsDownParticle[i] == null && SkillBdeltaTime > 2 + i && SkillBdeltaTime < 2.1f + i)
            {
                InsDownParticle[i] = Instantiate(DownParticle, enemyInfo.AttackAim.transform.position + new Vector3(0, 10, 0), transform.rotation);
                InsDownParticle[i].transform.localScale = DownParticle.transform.lossyScale;
                InsDownParticle[i].SetActive(true);
            }

            if (InsDownParticle[i] != null)
            {
                GameObject DP = Instantiate(TraceParticle, InsDownParticle[i].transform.position + new Vector3(0, 1, 0), transform.rotation);
                DP.transform.localScale = TraceParticle.transform.lossyScale;
                DP.SetActive(true);
                DP = Instantiate(TraceParticle, InsDownParticle[i].transform.position, transform.rotation);
                DP.transform.localScale = TraceParticle.transform.lossyScale;
                DP.SetActive(true);

                InsDownParticle[i].transform.position -= new Vector3(0, 1 - i * 0.3f, 0) * Time.deltaTime * 6 * SkillBdeltaTime * SkillBdeltaTime;
                if (InsDownParticle[i].GetComponent<ColliderTrigger>().isToched)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        GameObject Ins = Instantiate(Particle, InsDownParticle[i].transform.position, InsDownParticle[i].transform.rotation);
                        System.Random random = new System.Random();
                        if (j == 0)
                        {
                            Ins.GetComponent<AudioSource>().enabled = true;
                        }
                        Ins.transform.localScale = new Vector3(1f, 1f, 1f) * 0.1f * ((float)random.NextDouble() / 1.5f + 0.5f);
                        Ins.SetActive(true);
                    }
                    System.Random randomB = new System.Random();
                    int IndexIns = randomB.Next(0, 2);
                    GameObject InsE = Instantiate(SkillBEnemys[IndexIns], InsDownParticle[i].transform.position + new Vector3(0, 1 + i * 1.5f, 0), InsDownParticle[i].transform.rotation);
                    InsE.GetComponent<EnemyInfo>().AttackAim = enemyInfo.AttackAim;
                    InsE.SetActive(true);
                    Destroy(InsDownParticle[i]);
                }
            }

        }

    }

    public void NearAttackController()
    {
        if (enemyInfo.isAttacking)
        {
            enemyInfo.NormalAttackPredeltaTime += Time.deltaTime;
            if (enemyInfo.NormalAttackPredeltaTime > 0.4f && enemyInfo.NormalAttackPredeltaTime < 0.5f)
            {
                enemyInfo.animator.SetBool("isAttack", true);
                AttackSFX.enabled = true;
            }


            AttackSFX.enabled = true;
            if (enemyInfo.NormalAttackPredeltaTime > 0.5f)
                enemyInfo.animator.SetBool("isAttack", false);
            if (enemyInfo.NormalAttackPredeltaTime > enemyInfo.NormalAttackPre)
            {
                //近战
                enemyInfo.NormalAttack = NearAttack;
                enemyInfo.NormalAttack.SetActive(true);
                for (int i = 0; i < 20; i++)
                {
                    GameObject Ins = Instantiate(Particle, transform.position, transform.rotation);
                    System.Random random = new System.Random();
                    if (i == 0)
                    {
                        Ins.GetComponent<AudioSource>().enabled = true;
                    }
                    Ins.transform.localScale = new Vector3(1f, 1f, 1f) * 0.06f * ((float)random.NextDouble() / 1.5f + 0.5f);
                    Ins.SetActive(true);
                }
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().NearAttackDisdeltaTime = 0.2f;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;
                enemyInfo.NormalAttackPredeltaTime = 0;
                enemyInfo.NormalAttackIntervaldeltaTime = 0f;
                enemyInfo.isAttacking = false;
                canTakeNormalAttackTimes = true;
                enemyInfo.NormalAttackPredeltaTime = 0;
            }
        }
        else
        {
            enemyInfo.NormalAttackPredeltaTime = 0;
        }

        if (enemyInfo.NormalAttackIntervaldeltaTime > 5 * Time.deltaTime && canTakeNormalAttackTimes)
        {
            canTakeNormalAttackTimes = false;
            AttackSFX.enabled = false;
            enemyInfo.NormalAttackTimes += 1;
            //enemyInfo.NormalAttack.SetActive(false);
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class 小石怪A : MonoBehaviour
{
    public GameObject NearAttack, DeadParticle, TracePartice;

    public bool willDisappear = false;
    private float LiveTime;
    private EnemyInfo enemyInfo;

    private Vector3 DashDirection;

    private bool isDeadParticle = false;

    private float InitialNormalAttackInterval;
    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = gameObject.GetComponent<EnemyInfo>();
        InitialNormalAttackInterval = enemyInfo.NormalAttackInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (willDisappear)
        {
            LiveTime += Time.deltaTime;
            if (LiveTime > 60)
            {
                enemyInfo.EnemyHP = 0;
            }
        }
        if (enemyInfo.isOutRange)
        {
            enemyInfo.canAttack = false;
        }
        if (enemyInfo.canAttack)
            NearAttackController();

        if (enemyInfo.animator.GetBool("isAttacked"))
        {
            enemyInfo.canAttack = true;
        }

        //死亡粒子
        if (enemyInfo.EnemyHP == 0)
        {
            if (!isDeadParticle)
            {
                isDeadParticle = true;

                for (int i = 0; i < 20; i++)
                {
                    System.Random random = new System.Random();
                    float randomR = ((float)random.NextDouble() / 1.5f + 0.5f) * 0.5f;
                    float randomalpha = (float)random.NextDouble() * 2 * math.PI;

                    GameObject Ins = Instantiate(DeadParticle, transform.position + new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha)), transform.rotation);
                    if (i == 0)
                    {
                        Ins.GetComponent<AudioSource>().enabled = true;
                    }
                    Ins.transform.localScale = new Vector3(1f, 1f, 1f) * ((float)random.NextDouble() / 1.5f + 0.5f) / 2;
                    Ins.SetActive(true);
                }
            }
        }
    }

    public void NearAttackController()
    {
        if (enemyInfo.isAttacking && enemyInfo.isFollowing)
        {
            //Debug.Log(enemyInfo.NormalAttackPredeltaTime);
            enemyInfo.NormalAttackPredeltaTime += Time.deltaTime;
            enemyInfo.NormalAttack = NearAttack;
            enemyInfo.NormalAttack.SetActive(true);
            enemyInfo.navMeshAgent.enabled = false;
            enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().NearAttackDisdeltaTime = 0.2f;
            enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
            enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;

            if (enemyInfo.NormalAttackPredeltaTime > 0.4f && enemyInfo.NormalAttackPredeltaTime < 0.5f)
            {
                DashDirection = (enemyInfo.AttackAim.transform.position - transform.position).normalized;
                DashDirection.y = 0;
                enemyInfo.animator.SetBool("isAttack", true);
            }
            if (enemyInfo.NormalAttackPredeltaTime > 0.5f)
                enemyInfo.animator.SetBool("isAttack", false);

            if (enemyInfo.NormalAttackPredeltaTime > enemyInfo.NormalAttackPre && enemyInfo.NormalAttackPredeltaTime < enemyInfo.NormalAttackPre + 0.5f)
            {
                //近战
                GameObject DP = Instantiate(TracePartice, transform.position + new Vector3(0, 0.2f, 0), transform.rotation);
                DP.transform.localScale = TracePartice.transform.lossyScale;
                DP.SetActive(true);
                transform.position += DashDirection * Time.deltaTime * (enemyInfo.NormalAttackPre + 0.5f - enemyInfo.NormalAttackPredeltaTime) * 40;
            }
            else if (enemyInfo.NormalAttackPredeltaTime >= enemyInfo.NormalAttackPre + 0.5f)
            {
                enemyInfo.NormalAttackIntervaldeltaTime = 0f;
                enemyInfo.isAttacking = false;
                enemyInfo.navMeshAgent.enabled = true;
                enemyInfo.NormalAttackPredeltaTime = 0;
            }
        }
        else
        {
            enemyInfo.NormalAttackPredeltaTime = 0;
        }

    }
}

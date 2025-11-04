using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class 强盗鼠鼠 : MonoBehaviour
{
    public GameObject NearAttack, DeadParticle;

    public bool willDisappear = false;
    private float LiveTime;
    private EnemyInfo enemyInfo;

    private int AttackTimes = 0;
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

        NearAttackController();
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
        if (enemyInfo.isAttacking)
        {
            //Debug.Log(enemyInfo.NormalAttackPredeltaTime);
            enemyInfo.NormalAttackPredeltaTime += Time.deltaTime;
            if (enemyInfo.NormalAttackPredeltaTime < 0.4f)
                enemyInfo.animator.SetBool("isAttack", true);
            if (enemyInfo.NormalAttackPredeltaTime > 0.5f)
                enemyInfo.animator.SetBool("isAttack", false);

            if (enemyInfo.NormalAttackPredeltaTime > enemyInfo.NormalAttackPre)
            {
                enemyInfo.NormalAttack = NearAttack;
                // if (AttackTimes == 5)
                // {
                //     NearAttack.GetComponent<NormalAttackTrigger>().NearAttackMode = 1;
                // }
                // else
                // {
                //     NearAttack.GetComponent<NormalAttackTrigger>().NearAttackMode = 0;
                // }
                NearAttack.GetComponent<NormalAttackTrigger>().AttackIntervaldeltaTime = 0.3f;
                System.Random random = new System.Random();
                float randomA = ((float)random.NextDouble() - 0.5f) * 30;
                if (enemyInfo.AttackAim != null)
                {
                    Vector3 Direction = (enemyInfo.AttackAim.transform.position - gameObject.transform.position).normalized;
                    if (Direction.x > 0)
                        NearAttack.transform.eulerAngles = new Vector3(-90 + randomA, -180 + math.acos(Direction.normalized.z) / math.PI * 180, 0);
                    else
                    {
                        NearAttack.transform.eulerAngles = new Vector3(-90 + randomA, 180 - math.acos(Direction.normalized.z) / math.PI * 180, 0);
                    }
                }
                enemyInfo.NormalAttack.SetActive(true);
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().NearAttackDisdeltaTime = 0.1f;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;

                AttackTimes += 1;
                NearAttack.GetComponent<SpriteRenderer>().flipX = !NearAttack.GetComponent<SpriteRenderer>().flipX;
                enemyInfo.NormalAttackPredeltaTime = 0.3f;

                if (AttackTimes > 4)
                {
                    AttackTimes = 0;

                    enemyInfo.NormalAttackIntervaldeltaTime = 0f;
                    enemyInfo.isAttacking = false;
                    enemyInfo.navMeshAgent.enabled = true;
                    enemyInfo.NormalAttackPredeltaTime = 0;
                }
            }
        }
        else
        {
            enemyInfo.NormalAttackPredeltaTime = 0;
        }

    }
}

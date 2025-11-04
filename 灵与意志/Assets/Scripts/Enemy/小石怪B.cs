using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class 小石怪B : MonoBehaviour
{
    public GameObject DeadParticle;

    public bool willDisappear = false;
    private float LiveTime;
    private EnemyInfo enemyInfo;

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
            FarAttackController();

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

    public void FarAttackController()
    {
        if (enemyInfo.isAttacking)
        {
            enemyInfo.canMove = false;
            enemyInfo.NormalAttackPredeltaTime += Time.deltaTime;
            if (enemyInfo.NormalAttackPredeltaTime > enemyInfo.NormalAttackPre)
            {
                enemyInfo.NormalAttack.SetActive(true);

                //远程
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackDirection = (enemyInfo.AttackAim.transform.position + new Vector3(0, -0.1f, 0) - (transform.position + new Vector3(0, 0.1f, 0))).normalized;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().InitialPositionShift = new Vector3(0, 0, 0);
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;
                enemyInfo.NormalAttackPredeltaTime = 0;
                enemyInfo.NormalAttackIntervaldeltaTime = 0;
                enemyInfo.isAttacking = false;
            }
        }
        else
        {
            enemyInfo.NormalAttackPredeltaTime = 0;
            enemyInfo.canMove = true;
        }

    }

}

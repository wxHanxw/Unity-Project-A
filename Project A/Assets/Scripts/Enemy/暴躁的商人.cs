using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class 暴躁的商人 : MonoBehaviour
{
    public GameObject[] AllNormalFarAttack;
    public GameObject NearAttack;
    private EnemyInfo enemyInfo;
    private bool isJumpTimes = false;
    private Vector3 JumpDirection;

    private bool canTakeNormalAttackTimes = true, isAngry = false;
    private float isAngrydeltaTime = 0;

    public GameObject TextBar, AngryImage;

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
        if (enemyInfo.AttackAim != null && (enemyInfo.AttackAim.transform.position - gameObject.transform.position).magnitude < 2 && enemyInfo.NormalAttackTimes > 5 && enemyInfo.isGround)
        {
            NearAttackController();
            enemyInfo.NormalAttackTimes = 0;
        }
        else if (enemyInfo.canAttack)
        {
            FarAttackController();
        }

        if (enemyInfo.AttackAim != null && (enemyInfo.AttackAim.transform.position - gameObject.transform.position).magnitude < 2 && enemyInfo.NormalAttackTimes > 3 && enemyInfo.isGround)
        {
            enemyInfo.NormalAttackTimes = 0;
            isJumpTimes = true;
            enemyInfo.canAttack = false;
            JumpDirection = -(enemyInfo.AttackAim.transform.position - gameObject.transform.position).normalized * 5;
            enemyInfo.JumpFunction(JumpDirection);
        }
        else if (isJumpTimes)
        {
            JumpDirection = new Vector3(0, 0, 0);
            isJumpTimes = false;
            enemyInfo.canAttack = true;
        }

        //愤怒检测
        if (!isAngry && enemyInfo.enabled)
        {
            isAngry = true;
            isAngrydeltaTime = 0;
            TextBar.SetActive(true);
            AngryImage.SetActive(true);
        }

        if (isAngrydeltaTime <= 1)
        {
            enemyInfo.canMove = false;
            enemyInfo.canAttack = false;
            isAngrydeltaTime += Time.deltaTime;
            if (isAngrydeltaTime > 1)
            {
                enemyInfo.canMove = true;
                enemyInfo.canAttack = true;
                TextBar.SetActive(false);
            }
        }

        enemyInfo.NormalAttackInterval = InitialNormalAttackInterval * (0.3f + enemyInfo.EnemyHP / enemyInfo.EnemyMaxHP * 0.7f);
    }


    public void FarAttackController()
    {
        if (enemyInfo.isAttacking)
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
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackDirection = (enemyInfo.AttackAim.transform.position + new Vector3(0, 0.1f, 0) - (transform.position + new Vector3(0, 0.1f, 0))).normalized;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().InitialPositionShift = new Vector3(0, 0.1f, 0);
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;
                enemyInfo.NormalAttackPredeltaTime = 0;
                enemyInfo.NormalAttackIntervaldeltaTime = (float)random.NextDouble() * 0.2f - 0.2f;
                enemyInfo.isAttacking = false;
                canTakeNormalAttackTimes = true;
            }
        }
        else
        {
            enemyInfo.NormalAttackPredeltaTime = 0;
            enemyInfo.canMove = true;
        }

        if (enemyInfo.NormalAttackIntervaldeltaTime > 5 * Time.deltaTime && canTakeNormalAttackTimes)
        {
            canTakeNormalAttackTimes = false;
            enemyInfo.NormalAttackTimes += 1;
            //enemyInfo.NormalAttack.SetActive(false);
        }
    }

    public void NearAttackController()
    {
        //近战
        enemyInfo.NormalAttack = NearAttack;
        enemyInfo.NormalAttack.SetActive(true);

        enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().NearAttackDisdeltaTime = 0.5f;
        enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
        enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;
        enemyInfo.NormalAttackPredeltaTime = 0;
        enemyInfo.NormalAttackIntervaldeltaTime = -0.2f;
        enemyInfo.isAttacking = false;
        enemyInfo.NormalAttackPredeltaTime = 0;

    }
}

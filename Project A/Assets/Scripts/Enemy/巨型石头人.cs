using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor.Rendering;
using UnityEngine;

public class 巨型石头人 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Stone;
    public float StoneInterval = 0.5f;
    private float StoneIntervaldeltaTime = 0;
    public int SkillInterval = 2;
    public float SkillDuration = 1;
    private float SkillDurationdeltaTime = 0;

    public EnemyInfo enemyInfo;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyInfo.NormalAttackTimes >= SkillInterval)
        {
            Debug.Log(enemyInfo.NormalAttackIntervaldeltaTime);
            Skill();
        }
    }

    public void Skill()
    {
        SkillDurationdeltaTime += Time.deltaTime;
        StoneIntervaldeltaTime += Time.deltaTime;
        enemyInfo.NormalAttackIntervaldeltaTime = 0;
        enemyInfo.NormalAttackPredeltaTime = 0;
        enemyInfo.isAttacking = true;
        if (SkillDurationdeltaTime > SkillDuration)
        {
            SkillDurationdeltaTime = 0;
            StoneIntervaldeltaTime = 0;
            enemyInfo.NormalAttackTimes = 0;
        }



        if (StoneIntervaldeltaTime > StoneInterval)
        {
            Stone.GetComponent<NormalAttackTrigger>().AttackDirection = (enemyInfo.AttackAim.transform.position + new Vector3(0, 0.3f, 0) - Stone.transform.position).normalized;
            Stone.GetComponent<NormalAttackTrigger>().InitialPositionShift = new Vector3(0, 0, 0);
            StoneIntervaldeltaTime = 0;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class 史莱姆 : MonoBehaviour
{
    public GameObject[] AllNormalAttack;
    private EnemyInfo enemyInfo;
    private float AttackeddeltaTime = 0;
    private int JumpTimes = 0;

    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = gameObject.GetComponent<EnemyInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyInfo.animator.GetBool("isAttacked"))
        {
            AttackeddeltaTime = 0;
            JumpTimes = 3;
        }
        if (AttackeddeltaTime < 0.1f)
            AttackeddeltaTime += Time.deltaTime;
        else if (enemyInfo.isGround && JumpTimes > 0)
        {
            enemyInfo.JumpFunction(new Vector3(0, 0, 0));
            JumpTimes -= 1;
            AttackeddeltaTime = -0.5f;
        }

    }

}

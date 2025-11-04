using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Unity.Mathematics;

public class 史莱姆 : MonoBehaviour
{
    public GameObject[] AllNormalAttack;
    private EnemyInfo enemyInfo;
    private float AttackeddeltaTime = 0;
    private int JumpTimes = 0;

    private bool isDeadParticle;

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

                    GameObject Ins = Instantiate(enemyInfo.toGroundParticle, transform.position + new Vector3(randomR * math.sin(randomalpha), -0.5f, randomR * math.cos(randomalpha)), transform.rotation);
                    if (i == 0)
                    {
                        //Ins.GetComponent<AudioSource>().enabled = true;
                    }
                    Ins.transform.localScale = new Vector3(1f, 1f, 1f) * 0.5f * ((float)random.NextDouble() / 1.5f + 0.5f) / 2;
                    Ins.SetActive(true);
                }
            }
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class 宝箱怪 : MonoBehaviour
{

    public GameObject NearAttack, HintTexture, Case, CaseEnemy;
    public GameObject Particle;
    private EnemyInfo enemyInfo;
    private float InitialNormalAttackInterval;

    private GameObject Character;
    private GameObject Camera;

    private Vector3 InitialPosition;

    // Start is called before the first frame update
    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
        enemyInfo = gameObject.GetComponent<EnemyInfo>();
        InitialNormalAttackInterval = enemyInfo.NormalAttackInterval;
        InitialPosition = HintTexture.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        NearAttackController();
        Interaction();
    }

    private void Interaction()
    {
        if (HintTexture.activeSelf)
        {
            HintTexture.transform.position = InitialPosition + 0.1f * new Vector3(0, math.sin(10 * Time.time), 0);
        }

        if (Input.GetKeyDown(KeyCode.F) && HintTexture.activeSelf && !Character.GetComponent<PlayerController>().isGhost)
        {
            Case.SetActive(false);
            for (int i = 0; i < 10; i++)
            {
                GameObject Ins = Instantiate(Particle, transform.position + new Vector3(0, 0.25f, 0), transform.rotation);
                System.Random random = new System.Random();
                if (i == 0)
                {
                    Ins.GetComponent<AudioSource>().enabled = true;
                }
                Ins.transform.localScale = new Vector3(1f, 1f, 1f) * ((float)random.NextDouble() / 1.5f + 0.5f);
                Ins.SetActive(true);
            }
            enemyInfo.isAttacking = true;
            gameObject.GetComponent<EnemyInfo>().enabled = true;
            CaseEnemy.GetComponent<MeshRenderer>().enabled = true;
        }

        //交互提示
        if ((transform.position - Character.transform.position).magnitude < 1.5f && Case.activeSelf)
        {
            if (Camera == null)
                Camera = GameObject.FindGameObjectWithTag("MainCamera");
            else
                HintTexture.transform.eulerAngles = new Vector3(Camera.transform.eulerAngles.x - 18, Camera.transform.eulerAngles.y, HintTexture.transform.eulerAngles.z);

            if (!HintTexture.activeSelf)
            {
                Character.GetComponent<PlayerController>().canGetItem.Add(gameObject);
                HintTexture.SetActive(true);
            }

        }
        else if (HintTexture.activeSelf)
        {
            Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
            if (HintTexture.activeSelf)
            {
                HintTexture.SetActive(false);
            }
        }
    }
    public void NearAttackController()
    {
        if (enemyInfo.isAttacking)
        {
            enemyInfo.NormalAttackPredeltaTime += Time.deltaTime;
            enemyInfo.animator.SetBool("isAttack", true);
            if (enemyInfo.NormalAttackPredeltaTime > 0.1f)
                enemyInfo.animator.SetBool("isAttack", false);
            if (enemyInfo.NormalAttackPredeltaTime > enemyInfo.NormalAttackPre)
            {
                //近战
                enemyInfo.NormalAttack = NearAttack;
                Vector3 Rotation = (enemyInfo.AttackAim.transform.position - gameObject.transform.position).normalized;
                if (Rotation.x > 0)
                    enemyInfo.NormalAttack.transform.eulerAngles = new Vector3(0, -180 + math.acos(Rotation.normalized.z) / math.PI * 180, 0);
                else
                {
                    enemyInfo.NormalAttack.transform.eulerAngles = new Vector3(0, 180 - math.acos(Rotation.normalized.z) / math.PI * 180, 0);
                }
                enemyInfo.NormalAttack.SetActive(true);

                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().NearAttackDisdeltaTime = 0.3f;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().AttackAim = enemyInfo.AttackAim;
                enemyInfo.NormalAttack.GetComponent<NormalAttackTrigger>().Damage = enemyInfo.Attack;
                enemyInfo.NormalAttackPredeltaTime = 0;
                enemyInfo.NormalAttackIntervaldeltaTime = 0f;
                enemyInfo.isAttacking = false;
                enemyInfo.NormalAttackPredeltaTime = 0;
            }
        }
        else
        {
            enemyInfo.NormalAttackPredeltaTime = 0;
        }

    }

}


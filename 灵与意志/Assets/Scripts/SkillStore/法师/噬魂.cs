using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.TextCore.Text;

public class 噬魂 : MonoBehaviour
{
    private float Damage;
    private GameObject Character, AimEnemy;

    public GameObject HitGround, DamageParticle;
    public float AttackRange = 3;
    private float AttackdeltaTime = 0;

    private float BeatBack;
    public GameObject ShowRange, AttackCollider;


    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
        Damage = (int)(GetComponent<SkillInfo>().Damage * Character.GetComponent<PlayerController>().FinalCharacterInfos[1]);
        AttackCollider.SetActive(false);
        HitGround.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Character.GetComponent<PlayerController>().HitAim != null && GetComponent<SkillInfo>().isPre && (Character.transform.position - Character.GetComponent<PlayerController>().HitAim.transform.position).magnitude < AttackRange)
        {
            AimEnemy = Character.GetComponent<PlayerController>().HitAim;
            if (AimEnemy.tag == "Enemy" || AimEnemy.tag == "NPCNeutrality")
            {
                gameObject.GetComponent<Collider>().enabled = false;
                if ((-gameObject.transform.position + AimEnemy.transform.position).x > 0)
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, math.acos((-gameObject.transform.position + AimEnemy.transform.position).normalized.z) / math.PI * 180, gameObject.transform.eulerAngles.z);
                else
                {
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, 360 - math.acos((-gameObject.transform.position + AimEnemy.transform.position).normalized.z) / math.PI * 180, gameObject.transform.eulerAngles.z);
                }
                BeatBack = Character.GetComponent<PlayerController>().FinalCharacterInfos[13];
                AttackCollider.SetActive(true);
                AttackdeltaTime = 0;
            }
            else
            {
                AttackdeltaTime = 0;
                GetComponent<SkillInfo>().isPre = false;
                gameObject.SetActive(false);
            }
        }
        else if (GetComponent<SkillInfo>().isPre)
        {
            AttackdeltaTime = 0;
            GetComponent<SkillInfo>().isPre = false;
            gameObject.SetActive(false);
        }
        GetComponent<SkillInfo>().isPre = false;


        if (AttackCollider.activeSelf && AttackdeltaTime < GetComponent<SkillInfo>().Duration)
        {
            AttackdeltaTime += Time.deltaTime;
        }
        else if (AttackCollider.activeSelf)
        {
            AttackCollider.SetActive(false);
            HitGround.SetActive(false);
            gameObject.SetActive(false);
        }

        if (AttackCollider.activeSelf && !HitGround.activeSelf && AttackdeltaTime > 0.05f)
        {
            HitGround.SetActive(true);
        }


        if (AttackdeltaTime > 0.1f && AttackCollider.activeSelf && AttackdeltaTime < GetComponent<SkillInfo>().Duration)
        {
            gameObject.GetComponent<Collider>().enabled = true;
            ShowRange.SetActive(false);
        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "NPCNeutrality")
        {
            other.gameObject.GetComponent<EnemyInfo>().BeAttackedDeriction = (other.transform.position - gameObject.transform.position).normalized * BeatBack * Time.deltaTime * 20;
            other.GetComponent<EnemyInfo>().GetDamage = Damage;
            GameObject DP = Instantiate(DamageParticle, other.transform.position + new Vector3(0, 0.4f, 0), DamageParticle.transform.rotation);
            DP.SetActive(true);
            DP.GetComponent<Animator>().enabled = true;
            DP.GetComponent<SpriteRotator>().enabled = true;
            DP.GetComponent<DispersedParticle>().enabled = true;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class SkillSheep : MonoBehaviour
{
    private float Damage;
    private GameObject Player, AimEnemy, SheepIns;

    private float SheepdeltaTime = 1, MoveIntervaldeltaTime = 0;
    private bool isSheep = false;

    private Vector3 MovetoPosition;
    public GameObject SheepSample;

    void Start()
    {
        Damage = GetComponent<SkillInfo>().Damage;
        Player = GameObject.FindGameObjectWithTag("Character");
        SheepdeltaTime = GetComponent<SkillInfo>().Duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.GetComponent<PlayerController>().HitAim != null && GetComponent<SkillInfo>().isPre && (Player.transform.position - Player.GetComponent<PlayerController>().HitAim.transform.position).magnitude < 10 && !isSheep)
        {
            AimEnemy = Player.GetComponent<PlayerController>().HitAim;
            if (AimEnemy.tag == "Enemy")
            {
                SheepdeltaTime = 0;
                isSheep = true;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (!isSheep)
        {
            gameObject.SetActive(false);
        }

        GetComponent<SkillInfo>().isPre = false;

        if (SheepdeltaTime < GetComponent<SkillInfo>().Duration)
        {
            AimEnemy.GetComponent<EnemyInfo>().EnemySprite.SetActive(false);
            if (SheepdeltaTime == 0)
            {
                SheepIns = Instantiate(SheepSample, AimEnemy.transform.position + new Vector3(0, 0.3f, 0), AimEnemy.transform.rotation);
            }
            SheepdeltaTime += Time.deltaTime;

            MoveIntervaldeltaTime += Time.deltaTime;
            if (MoveIntervaldeltaTime > 0.5f)
            {
                System.Random random = new System.Random();
                MoveIntervaldeltaTime = ((float)random.NextDouble() / 2 - 1) * MoveIntervaldeltaTime / 2;
                float randomR = 1.5f;
                float randomalpha = (float)random.NextDouble() * 2 * math.PI;
                MovetoPosition = new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha));
            }
            SheepIns.transform.position += 1 * MovetoPosition * Time.deltaTime;
            AimEnemy.transform.position = SheepIns.transform.position;
        }
        else
        {
            Destroy(SheepIns);
            AimEnemy.GetComponent<EnemyInfo>().EnemySprite.SetActive(true);
            isSheep = false;
            gameObject.SetActive(false);
        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            other.GetComponent<EnemyInfo>().GetDamage = Damage;
    }
}

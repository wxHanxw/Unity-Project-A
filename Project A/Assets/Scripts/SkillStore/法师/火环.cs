using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 火环 : MonoBehaviour
{
    // Start is called before the first frame update
    private float Damage;
    private float DurationdeltaTime = 0;
    void Start()
    {
        Damage = GetComponent<SkillInfo>().Damage;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SkillInfo>().isPre = false;

        DurationdeltaTime += Time.deltaTime;
        if (DurationdeltaTime > GetComponent<SkillInfo>().Duration)
        {
            gameObject.SetActive(false);
            DurationdeltaTime = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            other.GetComponent<EnemyInfo>().GetDamage = Damage;
    }

}

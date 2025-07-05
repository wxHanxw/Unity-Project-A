using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSample_NearDamage : MonoBehaviour
{
    // Start is called before the first frame update
    private float Damage;
    void Start()
    {
        Damage = this.GetComponent<SkillInfo>().Damage;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            other.GetComponent<EnemyInfo>().GetDamage = Damage;
    }

}

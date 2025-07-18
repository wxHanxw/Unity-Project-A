using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public float Damage = 1;

    public GameObject Holder;

    private float DestroydeltaTime = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (DestroydeltaTime > 0)
        {
            DestroydeltaTime -= Time.deltaTime;
        }
        if (DestroydeltaTime < 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyInfo>().GetDamage = Damage;
            other.gameObject.GetComponent<EnemyInfo>().GetDamageHolder = Holder;
        }

        if (other.tag != "Particle")
        {
            DestroydeltaTime = 0.2f;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public float Damage = 1;

    public GameObject ColliderParticleA, ColliderParticleB, TraceParticle;

    public GameObject Holder;

    private float DestroydeltaTime = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject Ins = Instantiate(TraceParticle, transform.position + new Vector3(0, 1, 0), transform.rotation);
        Ins.SetActive(true);
        Ins = Instantiate(TraceParticle, transform.position, transform.rotation);
        Ins.SetActive(true);

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
        if (other.tag == "Enemy" || other.tag == "NPCNeutrality")
        {
            other.gameObject.GetComponent<EnemyInfo>().GetDamage = Damage;
            other.gameObject.GetComponent<EnemyInfo>().GetDamageHolder = Holder;
        }

        if (other.tag == "Ground" || other.tag == "Enemy" || other.tag == "NPCNeutrality")
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject Ins = Instantiate(ColliderParticleA, transform.position, transform.rotation);
                System.Random random = new System.Random();
                if (j == 0)
                {
                    //Ins.GetComponent<AudioSource>().enabled = true;
                }
                Ins.transform.localScale = new Vector3(1f, 1f, 1f) * 0.06f * ((float)random.NextDouble() / 1.5f + 0.5f);
                Ins.SetActive(true);
            }
            GameObject InsB = Instantiate(ColliderParticleB, transform.position, ColliderParticleB.transform.rotation);
            System.Random randomB = new System.Random();
            InsB.transform.localScale = new Vector3(1f, 1f, 1f) * 0.06f * ((float)randomB.NextDouble() / 1.5f + 0.5f);
            InsB.SetActive(true);
            DestroydeltaTime = 0.2f;
        }

    }
}

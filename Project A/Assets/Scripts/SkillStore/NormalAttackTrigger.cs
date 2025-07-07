using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackTrigger : MonoBehaviour
{
    [HideInInspector]
    public float Damage;

    public bool toEnemy = false;
    public bool toPlayer = false;
    public bool isFarAttack = false;
    public float BulletSpeed = 1, BulletMaxDistance = 20;

    [HideInInspector]
    public Vector3 AttackDirection = new Vector3(0, 0, 0);
    private Vector3 Direction;
    private float BulletDistance;
    private GameObject AttackParticleIns;

    [HideInInspector]
    public bool isTouched;

    public GameObject Holder;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isFarAttack && gameObject.name == "FarAttack")
        {
            FarAttackController();
        }
    }

    private void FarAttackController()
    {
        if (AttackDirection.magnitude > 0)
        {
            AttackParticleIns = Instantiate(gameObject, Holder.transform.position, transform.rotation);
            Direction = AttackDirection;
            BulletDistance = 0;
            AttackDirection = new Vector3(0, 0, 0);
        }

        if (AttackParticleIns != null)
        {
            AttackParticleIns.GetComponent<NormalAttackTrigger>().toEnemy = toEnemy;
            AttackParticleIns.GetComponent<NormalAttackTrigger>().toPlayer = toPlayer;
            AttackParticleIns.transform.position += Direction * BulletSpeed * Time.deltaTime;
            BulletDistance += (Direction * BulletSpeed * Time.deltaTime).magnitude;
        }

        if (BulletDistance > 20 || AttackParticleIns == null)
        {
            if (AttackParticleIns != null)
            {
                Destroy(AttackParticleIns);
                AttackParticleIns = null;
            }
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character" && toPlayer)
        {
            other.gameObject.GetComponent<PlayerController>().GetDamage = Damage;
            if (isFarAttack)
            {
                Destroy(gameObject);
            }
        }

        if (other.tag == "NPCFriend" && toPlayer)
        {
            other.gameObject.GetComponent<NPCInfo>().GetDamage = Damage;
            if (isFarAttack)
            {
                Destroy(gameObject);
            }
        }

        if (other.tag == "Enemy" && toEnemy)
        {
            other.gameObject.GetComponent<EnemyInfo>().GetDamage = Damage;
            if (isFarAttack)
            {
                Destroy(gameObject);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackTrigger : MonoBehaviour
{
    [HideInInspector]
    public float Damage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            other.gameObject.GetComponent<PlayerController>().GetDamage = Damage;
        }
    }

}

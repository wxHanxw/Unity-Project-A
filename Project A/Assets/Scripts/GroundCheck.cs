using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CheckAim;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            CheckAim.GetComponent<PlayerController>().isGround = true;
        }
        else
        {
            CheckAim.GetComponent<PlayerController>().isGround = false;
        }
    }
}

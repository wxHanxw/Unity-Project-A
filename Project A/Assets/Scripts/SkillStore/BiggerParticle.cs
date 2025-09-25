using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggerParticle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localScale *= 1.02f;
    }
}

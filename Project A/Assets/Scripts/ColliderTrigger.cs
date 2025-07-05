using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    public bool isToched;
    public GameObject isTochedAim;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        isToched = true;
        isTochedAim = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        isToched = false;
        isTochedAim = null;
    }
}

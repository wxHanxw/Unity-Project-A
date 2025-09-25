using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerInt : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 InitialPosition;
    private bool isTouch = false;
    void Start()
    {
        InitialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouch && transform.position.y > InitialPosition.y - 0.1f)
        {
            transform.position -= new Vector3(0, 3, 0) * Time.deltaTime;
        }
        else if (!isTouch && transform.position.y < InitialPosition.y)
        {
            transform.position += new Vector3(0, 0.05f, 0) * Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Character")
        {
            isTouch = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Character")
        {
            isTouch = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFigure : MonoBehaviour
{
    private Vector3 InitialPosition;
    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (name != "Figure")
        {
            transform.position += new Vector3(0, 2, 0) * Time.deltaTime;
            if (transform.position.y > InitialPosition.y + 1)
            {
                Destroy(gameObject);
            }
        }

    }
}

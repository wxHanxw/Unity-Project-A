using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name != "Footprint")
        {
            gameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.7f) * Time.deltaTime;
            if (gameObject.GetComponent<SpriteRenderer>().color.a < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}

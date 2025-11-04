using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaCheck : MonoBehaviour
{
    public TMP_Text AreaName;
    public TMP_Text AreaType;
    private float AreaNamedeltaTime = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (AreaNamedeltaTime > 0)
        {
            AreaNamedeltaTime -= Time.deltaTime;
            AreaName.color += new Color(0, 0, 0, 1) * Time.deltaTime;
            AreaType.color += new Color(0, 0, 0, 1) * Time.deltaTime;
        }
        else if (AreaName.color.a > 0.01)
        {
            AreaName.color -= new Color(0, 0, 0, 0.5f) * Time.deltaTime;
            AreaType.color -= new Color(0, 0, 0, 0.5f) * Time.deltaTime;
        }
        else
        {
            AreaName.gameObject.SetActive(false);
            AreaType.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AreaCheck" && !AreaName.gameObject.activeSelf)
        {
            AreaNamedeltaTime = 2;
            AreaName.gameObject.SetActive(true);
            AreaType.gameObject.SetActive(true);
        }
    }
}

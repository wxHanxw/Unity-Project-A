using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Roof, Wall, Wood, Icons;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //碰撞检测
    void OnTriggerStay(Collider other)
    {

        if ((other.tag == "Character" || other.tag == "MainCamera") && !Icons.activeSelf)
        {
            Icons.SetActive(true);
            Roof.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            Wall.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            Wood.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Character" || other.tag == "MainCamera")
        {
            Icons.SetActive(false);
            Roof.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Wall.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Wood.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SpriteRotator : MonoBehaviour
{
    private GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, Camera.transform.eulerAngles.y, 0);
    }
}

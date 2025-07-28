using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        if (Camera == null)
            Camera = GameObject.FindGameObjectWithTag("MainCamera");
        else
            transform.eulerAngles = new Vector3(Camera.transform.eulerAngles.x - 17, Camera.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}

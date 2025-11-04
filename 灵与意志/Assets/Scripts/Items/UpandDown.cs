using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UpandDown : MonoBehaviour
{
    public Vector3 Direction;
    public float Speed;

    private Vector3 InitialPosition;
    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = gameObject.transform.position + Direction * math.sin(Time.time * Speed) * 0.02f;
    }
}

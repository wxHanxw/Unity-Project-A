using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXRandomScale : MonoBehaviour
{
    private float InitialScale, InitialIndensity;
    public float RandomTime = 1, RandomScaleA = 1;
    private float RandomdeltaTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        InitialScale = gameObject.GetComponent<Light>().range;
        InitialIndensity = gameObject.GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        RandomdeltaTime += Time.deltaTime;
        if (RandomdeltaTime > RandomTime)
        {
            System.Random random = new System.Random();
            float randomR = (float)random.NextDouble() * RandomScaleA;
            gameObject.GetComponent<Light>().range = InitialScale + randomR;
            gameObject.GetComponent<Light>().intensity = InitialIndensity + randomR;
            RandomdeltaTime = 0;
        }


    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SkillA : MonoBehaviour
{
    public GameObject PreSkillRange;
    public GameObject StoneSample;

    public GameObject[] StoneIns;

    private float[] RandomStartTime;
    private float[] RandomStartTimedeltaTime;
    public float RandomRange = 1;
    public float StartRange = 2;
    public float FallSpeed = 1;

    private float ReloaddeltaTime = 0;
    private Vector3 StartPosition, FallDirection;
    // Start is called before the first frame update
    void Start()
    {
        StoneIns = new GameObject[10];
        RandomStartTime = new float[10];

        for (int i = 0; i < StoneIns.Length; i++)
        {
            RandomStartTime[i] = i * 0.1f;
        }
        RandomStartTimedeltaTime = new float[10];

    }

    // Update is called once per frame
    void Update()
    {

        ReloaddeltaTime += Time.deltaTime;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            PreSkillRange.transform.position = hit.point + new Vector3(0, 0.1f, 0);
        }
        if (Input.GetMouseButtonDown(0) && PreSkillRange.activeSelf)
        {
            PreSkillRange.SetActive(false);
            System.Random random = new System.Random();
            FallDirection = -new Vector3(5, 10, 0);
            StartPosition = PreSkillRange.transform.position - FallDirection;
            for (int i = 0; i < StoneIns.Length; i++)
            {
                System.Random randomR = new System.Random();
                float Radius = (float)randomR.NextDouble() * StartRange;
                System.Random randomA = new System.Random();
                float Angle = (float)randomA.NextDouble() * 2 * (float)Math.PI;
                StoneIns[i] = Instantiate(StoneSample, StartPosition + new Vector3((float)Radius * math.sin(Angle), 0, (float)Radius * math.cos(Angle)), StoneSample.transform.rotation);
            }
        }

        if (!PreSkillRange.activeSelf)
        {
            for (int i = 0; i < StoneIns.Length; i++)
            {
                RandomStartTimedeltaTime[i] += Time.deltaTime;
                if (RandomStartTimedeltaTime[i] > RandomStartTime[i])
                    if (StoneIns[i] != null)
                    {
                        StoneIns[i].transform.position += FallDirection / FallDirection.magnitude * FallSpeed * Time.deltaTime;
                    }
            }
        }

        if (ReloaddeltaTime >= gameObject.GetComponent<SkillInfo>().Duration)
        {
            PreSkillRange.SetActive(true);
            ReloaddeltaTime = 0;
            RandomStartTimedeltaTime = new float[10];

        }

    }
}

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
    public Vector3 FallDirection = -new Vector3(5, 10, 0);
    public int NumofStone = 1;

    public LayerMask targetLayer;

    private float[] StartTime;
    private float StartTimedeltaTime;
    public float StartRange = 2;
    public float FallSpeed = 1;

    private Vector3 StartPosition;

    private int Index = 0;
    // Start is called before the first frame update
    void Start()
    {
        StoneIns = new GameObject[NumofStone];
        StartTime = new float[NumofStone];

        for (int i = 0; i < StoneIns.Length; i++)
        {
            System.Random random = new System.Random();
            StartTime[i] = i * 0.02f * (1 + (float)random.NextDouble() / 2);
        }
        GetComponent<SkillInfo>().isRefresh = true;

    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            PreSkillRange.transform.position = hit.point + new Vector3(0, 0.1f, 0);
        }

        //刷新技能初值
        if (GetComponent<SkillInfo>().isRefresh)
        {
            PreSkillRange.SetActive(true);
            StartTimedeltaTime = 0;
            GetComponent<SkillInfo>().isRefresh = false;
        }


        GetComponent<SkillInfo>().isPre = PreSkillRange.activeSelf;
        if (Input.GetMouseButtonDown(0) && PreSkillRange.activeSelf)
        {
            PreSkillRange.SetActive(false);
            StartPosition = PreSkillRange.transform.position - FallDirection;
            Index = 0;
        }

        if (!PreSkillRange.activeSelf)
        {
            StartTimedeltaTime += Time.deltaTime;
            for (int i = 0; i < StoneIns.Length; i++)
            {
                if (StartTimedeltaTime > StartTime[i])
                {
                    if (Index == i && StoneIns[Index] == null)
                    {
                        System.Random randomR = new System.Random();
                        float Radius = (float)randomR.NextDouble() * StartRange;
                        System.Random randomA = new System.Random();
                        float Angle = (float)randomA.NextDouble() * 2 * (float)Math.PI;
                        StoneIns[i] = Instantiate(StoneSample, StartPosition + new Vector3((float)Radius * math.sin(Angle), 0, (float)Radius * math.cos(Angle)), StoneSample.transform.rotation);
                        Index += 1;
                    }
                    if (StoneIns[i] != null)
                    {
                        StoneIns[i].transform.position += FallDirection / FallDirection.magnitude * FallSpeed * Time.deltaTime;
                    }
                }
            }
        }

    }

}

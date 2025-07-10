using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SkillC : MonoBehaviour
{
    public GameObject PreSkillRange;
    public GameObject StoneSample;

    public GameObject[] StoneIns;
    public Vector3 FallDirection = new Vector3(0, 0, 0);
    public int NumofStone = 5;

    public LayerMask targetLayer;

    public float StartRange = 1.5f;
    public float FallSpeed = 60f; // 先调大速度
    public float TrackInterval = 0.5f; // 每个石头追踪的时间间隔
    public float HitDistance = 1.5f; // 技能与敌人触碰造成伤害的距离
    public float MaxExpandRange = 20f; // 石头最远能扩展的范围距离
    public float TotalTrackTime = 1.0f; // Inspector可调

    private Vector3 StartPosition;
    //private int Index = 0;

    private Transform nearestEnemy;
    private float[] stoneActivateTime; // 每个石头的激活时间
    private bool[] stoneTracking;      // 每个石头是否已开始追踪
    private float skillStartTime;
    private bool hasGenerated = false;
    private int skillIndex = 0;

    void Start()
    {
        StoneIns = new GameObject[NumofStone];
        stoneActivateTime = new float[NumofStone];
        stoneTracking = new bool[NumofStone];
        GetComponent<SkillInfo>().isRefresh = true;
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            PreSkillRange.transform.position = hit.point + new Vector3(0, 0.1f, 0);
        }

        // 刷新技能初值
        if (GetComponent<SkillInfo>().isRefresh)
        {
            PreSkillRange.SetActive(true);
            GetComponent<SkillInfo>().isRefresh = false;
            hasGenerated = false; // 技能刷新时允许再次生成
            
        }
        GetComponent<SkillInfo>().isPre = PreSkillRange.activeSelf;
        

        // 技能释放时只生成一次石头
        if ( GetComponent<SkillInfo>().isPre  && !hasGenerated)
        {
            if (skillIndex == 0)
            {
                skillIndex++;
            }
            else
            {
            hasGenerated = true;
            PreSkillRange.SetActive(false);
            StartPosition = transform.position; // 以角色为中心
            nearestEnemy = FindNearestEnemy();
            skillStartTime = Time.time;
            Debug.Log($"[{Time.time:F2}] 技能释放，开始生成石头");

            GenerateStones();
            }
        }

        // 控制石头依次追踪敌人
        for (int i = 0; i < StoneIns.Length; i++)
        {
            if (StoneIns[i] != null)
            {
                if (!stoneTracking[i] && Time.time > stoneActivateTime[i])
                {
                    stoneTracking[i] = true;
                    Debug.Log($"石头{i} 开始追踪敌人");
                }

                if (stoneTracking[i])
                {
                    if (nearestEnemy == null)
                    {
                        Debug.Log($"石头{i} 追踪时 nearestEnemy 为空，无法追踪");
                        continue;
                    }
                    Vector3 toEnemy = (nearestEnemy.position - StoneIns[i].transform.position).normalized;
                    StoneIns[i].transform.position += toEnemy * FallSpeed * Time.deltaTime;

                    float dist3D = Vector3.Distance(StoneIns[i].transform.position, nearestEnemy.position);
                    Vector3 stoneXZ = new Vector3(StoneIns[i].transform.position.x, 0, StoneIns[i].transform.position.z);
                    Vector3 enemyXZ = new Vector3(nearestEnemy.position.x, 0, nearestEnemy.position.z);
                    float distXZ = Vector3.Distance(stoneXZ, enemyXZ);
                    Debug.Log($"石头{i} 距离敌人: 3D={dist3D:F2}, XZ={distXZ:F2}");

                    // 命中销毁（采用XZ平面距离）
                    if (distXZ < HitDistance) // 可调节命中判定
                    {
                        Debug.Log($"石头{i} XZ命中敌人，已销毁");
                        Destroy(StoneIns[i]);
                        StoneIns[i] = null;
                    }
                }
                // 超时销毁
                if (StoneIns[i] != null && Time.time - stoneActivateTime[i] > TrackInterval) // Assuming MaxStoneLife is not defined, using TrackInterval for timeout
                {
                    Destroy(StoneIns[i]);
                    StoneIns[i] = null;
                    Debug.Log($"石头{i} 超时未命中，自动销毁");
                }
                // 超出最大攻击距离则销毁
                if (StoneIns[i] != null && Vector3.Distance(transform.position, StoneIns[i].transform.position) > MaxExpandRange)
                {
                    Destroy(StoneIns[i]);
                    StoneIns[i] = null;
                    Debug.Log($"石头{i} 超出最大攻击距离({MaxExpandRange})，自动销毁");
                }
            }
        }
    }

    // 查找最近的“Enemy”对象
    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        Transform nearest = null;
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }
        if (nearest != null)
            Debug.Log($"最近敌人位置: {nearest.position}");
        else
            Debug.Log("未找到敌人");
        return nearest;
    }

    void GenerateStones()
    {
        float trackInterval = TotalTrackTime / NumofStone;
        for (int i = 0; i < NumofStone; i++)
        {
            float angle = (2 * Mathf.PI / NumofStone) * i;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * StartRange;
            Vector3 stonePos = new Vector3(
                StartPosition.x + offset.x,
                StartPosition.y,
                StartPosition.z + offset.z
            );
            StoneIns[i] = Instantiate(StoneSample, stonePos, StoneSample.transform.rotation);
            stoneActivateTime[i] = skillStartTime + i * trackInterval;
            stoneTracking[i] = false;
            Debug.Log($"[{Time.time:F2}] 石头{i} 生成于: {stonePos}");
        }
    }
}
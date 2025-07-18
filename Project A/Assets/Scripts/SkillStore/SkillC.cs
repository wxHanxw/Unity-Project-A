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
    public float HitDistance = 0.5f; // 技能与敌人触碰造成伤害的距离
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
    private bool isSkillBlocked = false; // 新增：技能是否被阻挡

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
            isSkillBlocked = false; // 技能刷新时重置阻挡状态

        }
        GetComponent<SkillInfo>().isPre = PreSkillRange.activeSelf;


        // 技能释放时只生成一次石头
        if (GetComponent<SkillInfo>().isPre && !hasGenerated && !isSkillBlocked)
        {
            if (skillIndex == 0)
            {
                skillIndex++;
            }
            else
            {
                nearestEnemy = FindNearestEnemy();
                float preSkillRangeRadius = PreSkillRange.transform.localScale.x / 2f;
                float distToPlayer = nearestEnemy != null ? Vector3.Distance(transform.position, nearestEnemy.position) : float.MaxValue;

                hasGenerated = true;
                if (PreSkillRange != null) PreSkillRange.SetActive(false);
                StartPosition = transform.position; // 以角色为中心
                skillStartTime = Time.time;
                Debug.Log($"[{Time.time:F2}] 技能释放，开始生成石头");

                // 距离判定，决定是否允许追踪
                bool canTrack = (nearestEnemy != null && distToPlayer <= preSkillRangeRadius);
                GenerateStones();

                for (int i = 0; i < stoneTracking.Length; i++)
                {
                    stoneTracking[i] = canTrack;
                }
                if (!canTrack)
                {
                    Debug.Log("超出施法距离，没有选中目标，但技能依然释放");
                }
            }
        }

        // 技能释放后，才允许石头追踪敌人
        if (hasGenerated)
        {
            TrackStones();
        }
    }

    // 新增：石头追踪敌人逻辑封装为函数
    void TrackStones()
    {
        for (int i = 0; i < StoneIns.Length; i++)
        {
            if (StoneIns[i] != null)
            {
                // 未激活追踪的石头始终跟随玩家移动
                if (!stoneTracking[i])
                {
                    float angle = (2 * Mathf.PI / NumofStone) * i;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * StartRange;
                    Vector3 stonePos = new Vector3(
                        transform.position.x + offset.x,
                        transform.position.y,
                        transform.position.z + offset.z
                    );
                    StoneIns[i].transform.position = stonePos;
                }
                else
                {
                    // 追踪逻辑
                    if (Time.time > stoneActivateTime[i])
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
                        if (distXZ < HitDistance)
                        {
                            Debug.Log($"石头{i} XZ命中敌人，已销毁");
                            EnemyInfo enemyInfo = nearestEnemy.GetComponent<EnemyInfo>();
                            if (enemyInfo != null)
                            {
                                enemyInfo.GetDamage = GetComponent<SkillInfo>().Damage;
                            }
                            Destroy(StoneIns[i]);
                            StoneIns[i] = null;
                        }
                    }
                }
                // 超时销毁
                if (StoneIns[i] != null && Time.time - stoneActivateTime[i] > TrackInterval)
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
        Transform enemiesParent = GameObject.Find("Enemies")?.transform;
        if (enemiesParent == null) return null;

        float minDist = Mathf.Infinity;
        Transform nearest = null;
        foreach (Transform child in enemiesParent)
        {
            float dist = Vector3.Distance(transform.position, child.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = child;
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
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class 魔法飞弹 : MonoBehaviour
{
    public GameObject PreSkillRange;
    public GameObject SkillRange;
    public GameObject StoneSample;
    public GameObject find;

    public GameObject[] StoneIns;
    //public Vector3 FallDirection = new Vector3(0, 0, 0);
    public int NumofStone = 5;

    public LayerMask targetLayer;

    public float StartRange = 1f;
    public float FallSpeed = 60f; // 先调大速度
    public float HitDistance = 0.5f; // 技能与敌人触碰造成伤害的距离
    public float MaxExpandRange = 20f; // 石头最远能扩展的范围距离
    private Vector3 StartPosition;
    private float DurationdeltaTime; // 技能持续时间
    //private int Index = 0;

    private Transform nearestEnemy;
    private float[] stoneActivateTime; // 每个石头的激活时间
    private bool[] stoneTracking;      // 每个石头是否已开始追踪
    private float skillStartTime;
    private bool hasGenerated = false;
    private bool isSkillBlocked = false; // 新增：技能是否被阻挡
    // 旋转速度（弧度/秒）
    private float rotationSpeed = 1f; // 修改为每秒旋转15度
    // 累计旋转角度
    private float currentRotation = 0f;

    void Start()
    {
        StoneIns = new GameObject[NumofStone];
        GetComponent<SkillInfo>().isRefresh = true;
        PreSkillRange.transform.position = transform.position; // 初始化预技能范围的位置
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            PreSkillRange.transform.position = transform.position;
        }

        // 刷新技能初值
        if (GetComponent<SkillInfo>().isRefresh)
        {
            PreSkillRange.SetActive(true);
            SkillRange.SetActive(false);
            GetComponent<SkillInfo>().isRefresh = false;
            hasGenerated = false; // 技能刷新时允许再次生成
            isSkillBlocked = false; // 技能刷新时重置阻挡状态
            nearestEnemy = null; // 初始化最近敌人
            stoneActivateTime = new float[NumofStone];
            stoneTracking = new bool[NumofStone];
            DurationdeltaTime = 0;
            currentRotation = 0f;

        }
        GetComponent<SkillInfo>().isPre = PreSkillRange.activeSelf;

       // 技能释放时只生成一次石头
        if (PreSkillRange.activeSelf)
        {
            //if (skillIndex == 0)
            //{
            //    skillIndex++;
            //}
            //else
            nearestEnemy = FindNearestEnemy();
            //PreSkillRange.transform.position = transform.position;
            StartPosition = PreSkillRange.transform.position; // 以角色为中心
            skillStartTime = Time.time;
            Debug.Log($"[{Time.time:F2}] 技能释放，开始生成石头");
            if (PreSkillRange != null) PreSkillRange.SetActive(false);
            hasGenerated = true;
            GenerateStones();
        }
        if (!PreSkillRange.activeSelf && hasGenerated && !isSkillBlocked)
        {
            SkillRange.SetActive(true);
                SkillRange.transform.position = transform.position;
                Vector3 scale= new Vector3();
                scale.y = PreSkillRange.transform.localScale.x /2f; // 保持Y轴不变
                scale.z =   PreSkillRange.transform.localScale.x / 2f; // 
                scale.x = PreSkillRange.transform.localScale.x / 2f; // 
                SkillRange.transform.localScale = scale;
            DurationdeltaTime = Time.time - skillStartTime;
            if (DurationdeltaTime <= GetComponent<SkillInfo>().Duration)
            {
                TrackStones(); // 传递当前石头到追踪函数
                //DurationdeltaTime += Time.deltaTime;
                
            } else
        {
            // 如果数组为空或null，禁用游戏对象
            gameObject.SetActive(false);
        }
        }
    }

    // 新增：石头追踪敌人逻辑封装为函数
    void TrackStones()
    {
        float trackInterval = GetComponent<SkillInfo>().Duration / NumofStone; // 每个石头追踪的时间间隔
        float preSkillRangeRadius = PreSkillRange.transform.localScale.x / 2f;
        for (int i = 0; i < StoneIns.Length; i++)
        {
            if (StoneIns[i] != null)
            {
                float distToPlayer = nearestEnemy != null ? Vector3.Distance(StoneIns[i].transform.position, nearestEnemy.position) : float.MaxValue;
                bool canTrack = (nearestEnemy != null && distToPlayer <= preSkillRangeRadius && Time.time - stoneActivateTime[i] >0f);
                stoneTracking[i] = canTrack;
                // 追踪逻辑
                if (Time.time - stoneActivateTime[i] < trackInterval-0.01)
                {
                    //Debug.Log($"石头{i} 追踪时间未到，当前已追踪: {Time.time - stoneActivateTime[i]:F2}秒");
                    // 未激活追踪的石头始终跟随玩家移动
                    Debug.Log($"石头{i} 是否激活追踪: {stoneTracking[i]}, 当前已追踪: {Time.time - stoneActivateTime[i]:F2}秒");
                    if (!stoneTracking[i])
                    {
                        //float angle = (2 * Mathf.PI / NumofStone) * i;
                        //Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * StartRange;
                        //Vector3 stonePos = new Vector3(
                        //    transform.position.x + offset.x,
                        //    transform.position.y,
                        //    transform.position.z + offset.z
                        //);
                        //StoneIns[i].transform.position = stonePos;

                        // 更新总旋转角度（每帧调用时累加）
                        currentRotation += rotationSpeed * Time.deltaTime;
                    
                            // 核心修改：在原有角度基础上叠加旋转角度
                            float angle = (2 * Mathf.PI / NumofStone) * i + currentRotation;
                            
                            // 以下是原代码，保持不变
                            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * StartRange;
                            Vector3 stonePos = new Vector3(
                                transform.position.x + offset.x,
                                transform.position.y,
                                transform.position.z + offset.z
                            );
                            StoneIns[i].transform.position = stonePos;
                            //Debug.Log($"石头{i} 未激活追踪，跟随玩家移动，当前总旋转角度: {currentRotation:F2} 弧度,位置: {stonePos}");
                    }else
                    {
                        if (nearestEnemy)
                        {
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
                            }else if (Vector3.Distance(nearestEnemy.position, StoneIns[i].transform.position) > MaxExpandRange)// 超出最大攻击距离则销毁
                            {
                                Destroy(StoneIns[i]);
                                StoneIns[i] = null;
                                Debug.Log($"石头{i} 超出最大攻击距离({MaxExpandRange})，自动销毁");
                            }
                        }else
                        {
                            Debug.Log($"石头{i} 追踪时 nearestEnemy 为空，无法追踪");
                            continue;
                        }
                        
                    }
                }else
                {
                    Destroy(StoneIns[i]);
                    StoneIns[i] = null;
                    Debug.Log($"石头{i} 超时未命中，自动销毁");
                }
            }
        }
    }

    // 查找最近的“Enemy”对象
    Transform FindNearestEnemy()
    {
        //Debug.Log($"找到了角色状态" + stats);
        Transform enemiesParent = GameObject.Find("Enemies")?.transform;
        
        if (enemiesParent == null) 
        {
            Debug.Log("未找到敌人父对象");
            return null; 
            
        }

        float minDist = Mathf.Infinity;
        Transform nearest = null;
        foreach (Transform child in enemiesParent)
        {
            if (find.activeSelf)
            {
                float distance = Vector3.Distance(find.transform.position, child.transform.position);
                if (distance < minDist)
                {
                    minDist = distance;
                    nearest = child;
                }
            }
            else
            {
                float dist = Vector3.Distance(transform.position, child.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = child;
                }
            }
        }
        if (nearest != null)
            {Debug.Log($"最近敌人位置: {nearest.position}");}
        else
            {Debug.Log("未找到敌人");}
        return nearest;
    }

    void GenerateStones()
    {
        float trackInterval = GetComponent<SkillInfo>().Duration / NumofStone; // 每个石头追踪的时间间隔
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
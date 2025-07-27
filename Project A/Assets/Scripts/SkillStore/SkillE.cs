using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SkillE : MonoBehaviour
{
    public float HealAmount = 1f; // 每次回血量，可在Inspector调节
    public float HealInterval = 0.5f; // 回血间隔，可在Inspector调节
    private float healTimer = 0f;
    public float healCastTime = 2f;      // 吟唱时间
    private bool isCastingHeal;  // 是否正在吟唱回血技能
    private float healCastStartTime;     // 回血技能吟唱开始时间
    private Vector3 healCastStartPos;    // 吟唱开始时的位置
    private bool healInterrupted; // 回血技能是否被打断
    public GameObject PreSkillRange;
    public GameObject SkillRange;
    public LayerMask targetLayer;
    private float StartTimedeltaTime = 0f, DurationdeltaTime = 0f;
    private int Index = 0;

    void Start()
    {
        // 初始化技能初值
        GetComponent<SkillInfo>().isRefresh = true;
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            PreSkillRange.transform.position = hit.point;
        }

        // 刷新技能初值
        if (GetComponent<SkillInfo>().isRefresh)
        {
            PreSkillRange.SetActive(true);
            SkillRange.SetActive(false);
            StartTimedeltaTime = 0;
            GetComponent<SkillInfo>().isRefresh = false;
            DurationdeltaTime = 0;
            healInterrupted = false; // 回血技能是否被打断
            isCastingHeal = false;  // 是否正在吟唱回血技能
            healCastTime = 2f;
        }

        GetComponent<SkillInfo>().isPre = PreSkillRange.activeSelf;
        if (PreSkillRange.activeSelf)
        {
            PreSkillRange.SetActive(false);
            Index = 0;

        }

        if (!PreSkillRange.activeSelf)
        {
            if (Index == 0)
            {
                SkillRange.SetActive(true);
                SkillRange.transform.position = transform.position;
                Vector3 scale= new Vector3();
                scale.y = PreSkillRange.transform.localScale.x /2f; // 保持Y轴不变
                scale.z =   PreSkillRange.transform.localScale.x / 2f; // 
                scale.x = PreSkillRange.transform.localScale.x / 2f; // 
                SkillRange.transform.localScale = scale;
                healCastStartPos = transform.position;
            }
            Index++;
            healCastTime -= Time.deltaTime;//吟唱时间
            HandleHealCasting();
            //Debug.Log($"[SkillE] 吟唱时间：{healCastTime}");
            //healCastStartTime += Time.deltaTime;
            if (healCastTime <= 0)
            {
                DurationdeltaTime += Time.deltaTime;
                healTimer += Time.deltaTime;
                if (DurationdeltaTime < GetComponent<SkillInfo>().Duration && !isCastingHeal && !healInterrupted)
                {
                    StartTimedeltaTime += Time.deltaTime;
                    if (healTimer >= HealInterval)
                    {
                        healTimer = 0f;
                        FindAndModifyTaggedObjects();
                        //HandleHealCasting(); // 处理回血技能吟唱
                    }
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
    // 判断目标是否在技能范围内
    bool IsTargetInSkillRange(Transform target)
    {
        if (target == null || PreSkillRange == null)
            return false;

        // 获取技能范围半径（假设PreSkillRange的x缩放值代表直径）
        float skillRadius = PreSkillRange.transform.localScale.x / 2f;

        // 计算目标与角色的距离（忽略Y轴高度差异）
        Vector3 characterPos = transform.position;
        Vector3 targetPos = new Vector3(target.position.x, characterPos.y, target.position.z);
        float distance = Vector3.Distance(characterPos, targetPos);
        //Debug.Log($"[SkillE] 目标与角色的距离：{distance}, 技能范围半径：{skillRadius}");

        // 判断是否在范围内
        return distance <= skillRadius;
    }
    // 持续范围回血
    void FindAndModifyTaggedObjects()
    {
        // 创建列表存储不同标签的对象
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        GameObject[] npcFriends = GameObject.FindGameObjectsWithTag("NPCFriend");
        for (int i = 0; i < npcFriends.Length; i++)
        {
            Debug.Log($"[SkillE] 查找对象{npcFriends[i]}");
            Debug.Log($"[SkillE] NPC距离：{Vector3.Distance(npcFriends[i].transform.position, transform.position)}，技能范围：{PreSkillRange.transform.localScale.x / 2f} ");
            if (!IsTargetInSkillRange(npcFriends[i].transform))
            {
                //Debug.Log($"[SkillE] 移除对象{npcFriends[i]}");
                Array.Clear(npcFriends, i, 1);//移除不在范围内的对象
            }
        }

        // 对Character标签的对象执行属性修改
        foreach (GameObject character in characters)
        {
            // 获取并修改Character的特定属性
            PlayerController stats = character.GetComponent<PlayerController>();
            //Debug.Log($"[SkillE] 查找对象角色：恢复前HP：{stats.PlayerHP}");
            if (stats != null)
            {
                if (stats.PlayerHP < stats.PlayerMaxHP)
                {
                    stats.GetHeal = HealAmount; // 增加生命值
                }
                else
                {
                    stats.GetHeal = 0;
                }
            }
        }

        // 对NPC Friend标签的对象执行不同的属性修改
        foreach (GameObject npc in npcFriends)
        {
            if (npc != null)
            {
                // 获取并修改NPC的特定属性
                NPCInfo behavior = npc.GetComponent<NPCInfo>();
                if (behavior != null)
                {
                    Debug.Log($"[SkillE] 查找对象{npc}：恢复前HP：{behavior.NPCHP}");
                    if (behavior.NPCHP < behavior.NPCMaxHP)
                    {
                        //behavior.SetAggro(false); // 设置为非敌对状态
                        behavior.GetHeal = HealAmount; // 给予治疗
                    }
                    else
                    {
                        behavior.GetHeal = 0;
                    }
                }
            }
        }

        //Debug.Log($"找到 {characters.Length} 个Character对象和 {npcFriends.Length} 个NPC Friend对象");
        return;
    }
    // 新增：处理回血技能吟唱
    void HandleHealCasting()
    {
        // 检测移动（如果移动超过一定距离则打断施法）
        //Debug.Log($"[{Time.time:F2}]  当前位置: {transform.position}, 吟唱开始位置:{healCastStartPos}");
        if (Vector3.Distance(transform.position, healCastStartPos) > 0.1f)
        {
            Debug.Log($"[{Time.time:F2}] 移动打断回血技能吟唱");
            isCastingHeal = false;         // 停止吟唱
            healInterrupted = true;        // 标记为被打断
            return;
        }
        // 吟唱完成
        if (healCastTime <= 0f)
        {
            isCastingHeal = false;
            healInterrupted = false;        // 标记没被打断
            //FindAndModifyTaggedObjects(); // 执行回血操作
        }
    }
}
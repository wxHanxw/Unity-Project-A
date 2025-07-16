using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillD : MonoBehaviour
{
    public GameObject PreSkillRange;
    public GameObject MushroomPrefab; // 蘑菇预制体
    public LayerMask targetLayer;
    public float WanderSpeed = 2f; // 蘑菇游走速度（可调）
    public float ChangeDirInterval = 0.5f; // 换向间隔（可调）

    private Transform nearestEnemy;
    private bool hasCasted = false;
    private int skillIndex = 0;
    private bool isSkillBlocked = false;
    private Vector3 lastMushroomPos; // 保存蘑菇最终位置

    void Start()
    {
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
            hasCasted = false;
            isSkillBlocked = false;
        }
        GetComponent<SkillInfo>().isPre = PreSkillRange.activeSelf;

        // 技能释放时只作用一次
        if (GetComponent<SkillInfo>().isPre && !hasCasted && !isSkillBlocked)
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
                if (distToPlayer <= preSkillRangeRadius)
                {
                    hasCasted = true;
                    if (PreSkillRange != null) PreSkillRange.SetActive(false);
                    Debug.Log($"[{Time.time:F2}] 技能释放，敌人变蘑菇");
                    StartCoroutine(Polymorph(nearestEnemy));
                }
                else
                {
                    // 距离过远，没有目标，但技能依然释放
                    hasCasted = true;
                    if (PreSkillRange != null) PreSkillRange.SetActive(false);
                    Debug.Log("超出施法距离，没有选中目标，但技能依然释放");
                    StartCoroutine(Polymorph(null)); // 传null
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
        return nearest;
    }

    // 变羊协程
    IEnumerator Polymorph(Transform enemy)
    {
        float skillDuration = GetComponent<SkillInfo>().Duration;
        Debug.Log($"[Polymorph] 技能持续时间: {skillDuration}");

        if (enemy == null)
        {
            Debug.Log("[Polymorph] 没有选中目标，技能空释放，仅等待持续时间");
            float timer = 0f;
            while (timer < skillDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Debug.Log("[Polymorph] 空释放技能结束");
            yield break;
        }

        Debug.Log($"[Polymorph] 协程开始，当前时间: {Time.time:F2}");
        Vector3 originalEnemyPos = enemy.position;
        GameObject mushroom = Instantiate(MushroomPrefab, enemy.position, Quaternion.identity);
        Debug.Log($"[Polymorph] 蘑菇已生成，时间: {Time.time:F2}");

        enemy.position = new Vector3(9999f, enemy.position.y, enemy.position.z);
        Debug.Log($"[变羊术] 敌人 {enemy.name} 已移到画布外: {enemy.position}");

        float speed = WanderSpeed;
        float changeDirInterval = ChangeDirInterval;
        Vector3 moveDir = Vector3.zero;
        float changeDirTimer = 0f;
        float timer2 = 0f;
        float angle = Random.Range(0, 2 * Mathf.PI);
        moveDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

        lastMushroomPos = mushroom.transform.position;
        while (timer2 < skillDuration)
        {
            if (enemy == null)
            {
                Debug.LogWarning("[Polymorph] 敌人被销毁，协程终止");
                break;
            }
            if (mushroom == null)
            {
                Debug.LogWarning("[Polymorph] 蘑菇被销毁，协程终止");
                break;
            }

            timer2 += Time.deltaTime;
            changeDirTimer += Time.deltaTime;
            if (changeDirTimer > changeDirInterval)
            {
                angle = Random.Range(0, 2 * Mathf.PI);
                moveDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                changeDirTimer = 0f;
            }
            mushroom.transform.position += moveDir * speed * Time.deltaTime;
            lastMushroomPos = mushroom.transform.position;

            // 每0.5秒输出一次日志
            if (Mathf.Abs(timer2 % 0.5f) < Time.deltaTime)
            {
                Debug.Log($"[Polymorph] timer: {timer2:F2}/{skillDuration}, 当前时间: {Time.time:F2}");
            }

            yield return null;
        }
        Debug.Log($"[Polymorph] while循环结束，timer: {timer2:F2}/{skillDuration}, 当前时间: {Time.time:F2}");

        Vector3 finalPos = lastMushroomPos;
        Debug.Log($"[变羊术] 蘑菇最终位置: {finalPos}");
        Debug.Log($"[{Time.time:F2}] 技能结束，敌人变回");

        if (mushroom != null)
        {
            Destroy(mushroom);
            mushroom = null;
        }
        if (enemy != null)
        {
            enemy.position = finalPos;
            Debug.Log($"[变羊术] 敌人 {enemy.name} 恢复到蘑菇最终位置: {enemy.position}");
        }
        Debug.Log("[Polymorph] 协程结束");
    }
}

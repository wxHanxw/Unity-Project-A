using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ExternPropertyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [HorizontalLine]
    [Header("Basic Operation")]
    public float MoveSpeed;
    public float RotateSpeed;
    public float JumpSpeed;


    [HideInInspector]
    public float GetDamage;
    private float RealGetDamage;

    [HorizontalLine]
    [Header("Basic Information")]
    public float PlayerMaxHP = 10;
    public float PlayerMaxMP = 10;
    public float PlayerAttack = 1;
    public float PlayerAttackRange = 20;
    public float PlayerDefence = 0;
    public float PlayerAttackInterval = 1;
    public float RegainHP = 1;
    public float RegainMP = 1;

    private float RegainHPdeltaTime, RegainMPdeltaTime, PlayerAttackIntervaldeltaTime = 0;

    [HideInInspector]
    public float PlayerHP, PlayerMP;

    //Skill
    [HorizontalLine]
    [Header("Skills")]
    public GameObject[] PlayerSkill;
    public GameObject[] PlayerUsedSkillBar;
    private GameObject[] PlayerUsingSkill;

    private Image[] PlayerUsedSkillBarImage;
    private float[] SkillCD, SkillDuration, SkillMPCost, SkillCDdeltaTime, SkillDurationdeltaTime;
    private bool[] isSkillReady, isSkilling, isSkillPre;

    private bool isSkillingAny = false;

    [HorizontalLine]
    [Header("Others")]
    public GameObject Character;
    public GameObject Chooser;
    public CinemachineVirtualCamera VirtualCamera;

    public GameObject NormalAttack;
    private bool isFarAttack;

    [HideInInspector]
    public GameObject HitAim, HitUIAim;

    [HideInInspector]
    public bool isGround = false, isChooseItem = false, isDead = false, isMouseMove = false, isKeyBoardMove = false, CanMove = true;
    [HideInInspector]
    public bool isBattle;
    private int MoveModel = 0;

    private float ySpeed;

    private CharacterController CharacterController;

    private Vector3 ChooserVelocity;

    //AI
    private NavMeshAgent CharacterAgent;




    // Start is called before the first frame update
    void Start()
    {
        isFarAttack = NormalAttack.GetComponent<NormalAttackTrigger>().isFarAttack;
        NormalAttack.GetComponent<NormalAttackTrigger>().Damage = PlayerAttack;
        NormalAttack.GetComponent<NormalAttackTrigger>().Holder = gameObject;
        PlayerHP = PlayerMaxHP;
        PlayerMP = PlayerMaxMP;
        SkillCD = new float[12];
        SkillDuration = new float[12];
        SkillMPCost = new float[12];

        SkillCDdeltaTime = new float[12];
        SkillDurationdeltaTime = new float[12];
        isSkillReady = new bool[12];
        isSkilling = new bool[12];
        isSkillPre = new bool[12];
        PlayerUsedSkillBarImage = new Image[12];
        PlayerUsingSkill = new GameObject[PlayerSkill.Length];

        CharacterController = Character.GetComponent<CharacterController>();
        CharacterAgent = GetComponent<NavMeshAgent>();


        for (int i = 0; i < PlayerSkill.Length; i++)
        {
            if (PlayerUsedSkillBar[i] != null)
                PlayerUsedSkillBarImage[i] = PlayerUsedSkillBar[i].GetComponent<Image>();

            isSkillReady[i] = true;

            //寻找Skill的子物体（正在使用的技能）
            foreach (Transform child in PlayerSkill[i].transform)
            {
                if (child.tag == "Skill")
                    PlayerUsingSkill[i] = child.gameObject;
            }

            SkillCD[i] = PlayerUsingSkill[i].GetComponent<SkillInfo>().CoolDown;
            SkillCDdeltaTime[i] = SkillCD[i] - 0.001f;
            SkillDuration[i] = PlayerUsingSkill[i].GetComponent<SkillInfo>().Duration;
            SkillMPCost[i] = PlayerUsingSkill[i].GetComponent<SkillInfo>().MPCost;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerHP > 0)
        {
            if (CanMove)
            {
                Move();
                MouseInteraction();
            }


            SkillController();


            HPController();
            AttackModelController();
        }
    }

    private void AttackModelController()
    {
        PlayerAttackIntervaldeltaTime += Time.deltaTime;
        if (isFarAttack && HitAim != null && (HitAim.transform.position - transform.position).magnitude < PlayerAttackRange && isChooseItem && HitAim.tag == "Enemy" && PlayerAttackIntervaldeltaTime > PlayerAttackInterval)
        {
            PlayerAttackIntervaldeltaTime = 0;
            NormalAttack.GetComponent<NormalAttackTrigger>().AttackDirection = (HitAim.transform.position - transform.position) / (HitAim.transform.position - transform.position).magnitude;
        }
    }
    private void HPController()
    {
        if (GetDamage != 0)
        {
            RealGetDamage = GetDamage - PlayerDefence;
            if (RealGetDamage < 1)
            {
                RealGetDamage = 1;
            }
            PlayerHP -= RealGetDamage;
            GetDamage = 0;
        }
        if (PlayerHP <= 0)
        {
            PlayerHP = 0;
            isDead = true;
        }

        //脱战回血
        if (!isBattle && PlayerHP < PlayerMaxHP)
        {
            RegainHPdeltaTime += Time.deltaTime;
            if (RegainHPdeltaTime > 1f)
            {
                PlayerHP += RegainHP;
                RegainHPdeltaTime = 0;
            }
        }

        //回复魔法值
        RegainMPdeltaTime += Time.deltaTime;
        if (!isBattle && PlayerMP < PlayerMaxMP)
        {
            if (RegainMPdeltaTime > 1f)
            {
                PlayerMP += RegainMP;
                RegainMPdeltaTime = 0;
            }
        }
        else if (PlayerMP < PlayerMaxMP)
        {
            if (RegainMPdeltaTime > 5f)
            {
                //PlayerMP += RegainMP;
                RegainMPdeltaTime = 0;
            }
        }

    }
    private void Move()
    {
        float MoveDirectionx = Input.GetAxis("Horizontal");
        float MoveDirectiony = Input.GetAxis("Vertical");
        if (MoveDirectionx != 0 || MoveDirectiony != 0)
        {
            if (CharacterAgent.isOnNavMesh)
                CharacterAgent.isStopped = true;
            CharacterController.enabled = true;
            isKeyBoardMove = true;
            MoveModel = 0;
        }
        else
        {
            isKeyBoardMove = false;
        }


        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            ySpeed = JumpSpeed;
        }
        if (!isGround)
            ySpeed -= 0.4f * Time.deltaTime;
        else if (ySpeed < 0)
        {
            ySpeed = 0;
        }


        Vector3 NormVelocity = new Vector3(math.sin(transform.eulerAngles.y / 180 * math.PI), 0, math.cos(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectiony + new Vector3(math.cos(transform.eulerAngles.y / 180 * math.PI), 0, -math.sin(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectionx;
        if (NormVelocity.magnitude > 1)
            NormVelocity = NormVelocity / NormVelocity.magnitude;
        ChooserVelocity = MoveSpeed * Time.deltaTime * NormVelocity;

        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.eulerAngles += new Vector3(0, RotateSpeed * Time.deltaTime * 2, 0);
        }

        if (Input.GetKey(KeyCode.E))
        {
            this.transform.eulerAngles -= new Vector3(0, RotateSpeed * Time.deltaTime * 2, 0);
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newSize = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += -scroll * 2f;
            VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Clamp(newSize, 10, 15);
        }

        if (CharacterController.enabled)
        {
            CharacterController.Move(ChooserVelocity + new Vector3(0, ySpeed, 0));
            if (isKeyBoardMove && isGround)
                CharacterAgent.Warp(transform.position);
        }

    }

    //鼠标交互锁定目标，UI显示目标状态
    private void MouseInteraction()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "ChatUI" && hit.collider.name == "Team")
            {
                hit.collider.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                HitUIAim = hit.collider.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    HitUIAim.transform.parent.gameObject.SetActive(false);
                    HitUIAim.transform.parent.gameObject.transform.parent.GetComponent<NPCInfo>().isinTeam = !HitUIAim.transform.parent.gameObject.transform.parent.GetComponent<NPCInfo>().isinTeam;
                    HitUIAim = null;
                }
            }
            else if (HitUIAim != null)
            {
                HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                HitUIAim = null;
            }
        }
        //右键移动
        if (Input.GetMouseButton(1) && !isKeyBoardMove)
        {
            isMouseMove = true;
            CharacterAgent.stoppingDistance = 0;
            CharacterAgent.isStopped = false;
            CharacterController.enabled = false;
            if (Input.GetMouseButtonDown(1) && MoveModel == 0)
            {
                CharacterAgent.Warp(transform.position);
            }
            MoveModel = 1;
            CharacterAgent.destination = hit.point;

        }
        else
        {
            isMouseMove = false;
        }

        isSkillingAny = false;
        for (int i = 0; i < isSkilling.Length; i++)
        {
            if (isSkilling[i])
            {
                isSkillingAny = true;
                break;
            }

        }

        if (!isChooseItem)
        {
            //预选则
            if (hit.collider != null && (hit.collider.tag == "IntItem" || hit.collider.tag == "Enemy" || hit.collider.tag == "NPCFriend"))
            {
                Chooser.SetActive(true);
                HitAim = hit.collider.gameObject;
                //确定选择
                if (Input.GetMouseButtonDown(0))
                {
                    isChooseItem = true;
                }
            }
            else if (isChooseItem == false)
            {
                HitAim = null;
                Chooser.SetActive(false);
            }
        }
        //移动到目标(按下鼠标且没有释放技能)
        else if (Input.GetMouseButtonDown(0) && isSkillingAny)
        {
            if (hit.collider.gameObject != HitAim)
            {
                if (hit.collider.tag == "IntItem" || hit.collider.tag == "Enemy" || hit.collider.tag == "NPCFriend")
                {
                    Chooser.SetActive(true);
                    HitAim = hit.collider.gameObject;
                }
                else
                {
                    isChooseItem = false;
                }
            }
            else
            {
                if (HitAim.tag == "Enemy")
                {
                    CharacterAgent.stoppingDistance = PlayerAttackRange;
                }
                else
                {
                    CharacterAgent.stoppingDistance = 1.5f;
                }
                CharacterAgent.isStopped = false;
                CharacterController.enabled = false;
                CharacterAgent.Warp(transform.position);
                CharacterAgent.destination = HitAim.transform.position;
                MoveModel = 1;
            }

        }
        if (HitAim != null)
            Chooser.transform.position = HitAim.transform.position;
    }

    private void SkillController()
    {
        //Skill 1

        for (int SkillIndex = 0; SkillIndex < PlayerUsingSkill.Length; SkillIndex++)
        {
            if (PlayerMP > SkillMPCost[SkillIndex]
                && ((Input.GetKeyDown(KeyCode.Alpha1) && SkillIndex == 0)
                   || (Input.GetKeyDown(KeyCode.Alpha2) && SkillIndex == 1)
                   || (Input.GetKeyDown(KeyCode.Alpha3) && SkillIndex == 2)
                   )
                && isSkillReady[SkillIndex])
            {
                if (!isSkilling[SkillIndex])
                {
                    //寻找UingSkill的子物体PreSkill
                    foreach (Transform child in PlayerUsingSkill[SkillIndex].transform)
                    {
                        if (child.tag == "PreSkill")
                            child.gameObject.SetActive(false);
                    }
                    PlayerUsingSkill[SkillIndex].GetComponent<SkillInfo>().isRefresh = true;
                    PlayerUsingSkill[SkillIndex].SetActive(true);
                    PlayerUsingSkill[SkillIndex].GetComponent<SkillInfo>().isPre = true;
                    isSkilling[SkillIndex] = true;
                    isSkillPre[SkillIndex] = true;
                    for (int i = 0; i < PlayerUsingSkill.Length; i++)
                    {
                        if (i != SkillIndex && isSkillPre[i])
                        {
                            PlayerUsingSkill[i].SetActive(false);
                        }
                    }
                }
                else if (isSkilling[SkillIndex])
                {
                    PlayerUsingSkill[SkillIndex].SetActive(false);
                    isSkilling[SkillIndex] = false;
                }
            }
            else if (!isSkilling[SkillIndex])
            {
                PlayerUsingSkill[SkillIndex].SetActive(false);
            }

            //结束预备后消耗魔法，计时器归零
            if (isSkilling[SkillIndex] && isSkillPre[SkillIndex] && PlayerUsingSkill[SkillIndex].GetComponent<SkillInfo>().isPre == false)
            {
                PlayerMP -= SkillMPCost[SkillIndex];
                SkillCDdeltaTime[SkillIndex] = 0;
                SkillDurationdeltaTime[SkillIndex] = 0;
                isSkillReady[SkillIndex] = false;
                isSkillPre[SkillIndex] = false;
            }

            //关闭技能
            if (!PlayerUsingSkill[SkillIndex].GetComponent<SkillInfo>().isPre && SkillDurationdeltaTime[SkillIndex] < SkillDuration[SkillIndex])
            {
                SkillDurationdeltaTime[SkillIndex] += Time.deltaTime;

                if (PlayerUsingSkill[SkillIndex].activeSelf && SkillDurationdeltaTime[SkillIndex] > SkillDuration[SkillIndex])
                {
                    PlayerUsingSkill[SkillIndex].SetActive(false);
                    SkillCDdeltaTime[SkillIndex] = 0;
                    isSkilling[SkillIndex] = false;
                }
            }

            //开始冷却
            if (SkillCDdeltaTime[SkillIndex] < SkillCD[SkillIndex])
            {
                SkillCDdeltaTime[SkillIndex] += Time.deltaTime;
                if (!PlayerUsingSkill[SkillIndex].activeSelf)
                {
                    if (SkillCDdeltaTime[SkillIndex] >= SkillCD[SkillIndex])
                    {
                        SkillCDdeltaTime[SkillIndex] = SkillCD[SkillIndex];
                        isSkillReady[SkillIndex] = true;
                    }
                    else
                    {
                        isSkillReady[SkillIndex] = false;
                    }
                    PlayerUsedSkillBarImage[SkillIndex].fillAmount = (SkillCD[SkillIndex] - SkillCDdeltaTime[SkillIndex]) / SkillCD[SkillIndex];
                }

            }

        }

    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGround = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGround = false;
        }
    }



}
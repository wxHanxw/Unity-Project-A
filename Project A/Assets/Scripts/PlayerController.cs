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

    public float PlayerDefence = 0;

    [HideInInspector]
    public float PlayerHP, PlayerMP;

    //Skill
    [HorizontalLine]
    [Header("Skills")]
    public GameObject[] PlayerSkill;
    public GameObject[] PlayerUsedSkillBar;
    private GameObject[] PlayerUsingSkill;

    private Image[] PlayerUsedSkillBarImage;
    private float[] SkillCD, SkillDuration, SkilldeltaTime;
    private bool[] isSkillReady;

    [HorizontalLine]
    [Header("Others")]
    public GameObject Character;
    public GameObject Chooser;
    public CinemachineVirtualCamera VirtualCamera;

    [HideInInspector]
    public GameObject HitAim;

    [HideInInspector]
    public bool isGround = false, isChooseItem = false, isDead = false, isMouseMove = false, isKeyBoardMove = false, CanMove = true;
    private int MoveModel = 0;

    private float ySpeed;

    private CharacterController CharacterController;

    private Vector3 ChooserVelocity;

    //AI
    private NavMeshAgent CharacterAgent;




    // Start is called before the first frame update
    void Start()
    {
        PlayerHP = PlayerMaxHP;
        PlayerMP = PlayerMaxMP;
        SkillCD = new float[12];
        SkillDuration = new float[12];
        SkilldeltaTime = new float[12];
        isSkillReady = new bool[12];
        PlayerUsedSkillBarImage = new Image[12];
        PlayerUsingSkill = new GameObject[PlayerSkill.Length];

        CharacterController = Character.GetComponent<CharacterController>();
        CharacterAgent = GetComponent<NavMeshAgent>();


        for (int i = 0; i < PlayerSkill.Length; i++)
        {
            if (PlayerUsedSkillBar[i] != null)
                PlayerUsedSkillBarImage[i] = PlayerUsedSkillBar[i].GetComponent<Image>();
            SkilldeltaTime[i] = SkillCD[i] - 0.0001f;

            //寻找Skill的子物体（正在使用的技能）
            foreach (Transform child in PlayerSkill[i].transform)
            {
                if (child.tag == "Skill")
                    PlayerUsingSkill[i] = child.gameObject;
            }

            SkillCD[i] = PlayerUsingSkill[i].GetComponent<SkillInfo>().CollDown;
            SkillDuration[0] = PlayerUsingSkill[i].GetComponent<SkillInfo>().Duration;
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
    }
    private void Move()
    {
        float MoveDirectionx = Input.GetAxis("Horizontal");
        float MoveDirectiony = Input.GetAxis("Vertical");
        if (MoveDirectionx != 0 || MoveDirectiony != 0)
        {
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
            this.transform.eulerAngles += new Vector3(0, RotateSpeed, 0);
        }

        if (Input.GetKey(KeyCode.E))
        {
            this.transform.eulerAngles -= new Vector3(0, RotateSpeed, 0);
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newSize = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += -scroll * 2f;
            VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Clamp(newSize, 10, 15);
        }

        if (CharacterController.enabled)
            CharacterController.Move(ChooserVelocity + new Vector3(0, ySpeed, 0));

    }

    //鼠标交互锁定目标，UI显示目标状态
    private void MouseInteraction()
    {
        //右键移动
        if (Input.GetMouseButton(1) && !isKeyBoardMove)
        {
            isMouseMove = true;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitmove;
            if (Physics.Raycast(ray, out hitmove))
            {
                CharacterAgent.stoppingDistance = 0;
                CharacterAgent.isStopped = false;
                CharacterController.enabled = false;
                if (Input.GetMouseButtonDown(1) && MoveModel == 0)
                {
                    CharacterAgent.Warp(transform.position);
                }
                MoveModel = 1;
                CharacterAgent.destination = hitmove.point;
            }

        }
        else
        {
            isMouseMove = false;
        }

        if (!isChooseItem)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "IntItem" || hit.collider.tag == "Enemy" || hit.collider.tag == "NPCFriend")
                {
                    Chooser.SetActive(true);
                    HitAim = hit.collider.gameObject;

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
        }
        else if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
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
                    CharacterAgent.stoppingDistance = 1.5f;
                    CharacterAgent.isStopped = false;
                    CharacterController.enabled = false;
                    CharacterAgent.Warp(transform.position);
                    CharacterAgent.destination = HitAim.transform.position;
                    MoveModel = 1;
                }


            }
        }
        if (HitAim != null)
            Chooser.transform.position = HitAim.transform.position;
    }

    private void SkillController()
    {
        //Skill 1

        if (Input.GetKeyDown(KeyCode.Alpha1) && isSkillReady[0])
        {
            PlayerSkill[0].SetActive(true);
            SkilldeltaTime[0] = 0;
        }
        if (SkilldeltaTime[0] < SkillCD[0])
        {
            SkilldeltaTime[0] += Time.deltaTime;
            if (PlayerSkill[0].activeSelf && SkilldeltaTime[0] > SkillDuration[0])
            {
                PlayerSkill[0].SetActive(false);
                SkilldeltaTime[0] = 0;
            }

            if (!PlayerSkill[0].activeSelf)
            {
                if (SkilldeltaTime[0] >= SkillCD[0])
                {
                    SkilldeltaTime[0] = SkillCD[0];
                    isSkillReady[0] = true;
                }
                else
                {
                    isSkillReady[0] = false;
                }
                PlayerUsedSkillBarImage[0].fillAmount = (SkillCD[0] - SkilldeltaTime[0]) / SkillCD[0];
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
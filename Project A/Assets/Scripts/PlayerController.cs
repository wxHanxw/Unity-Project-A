using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ExternPropertyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using Spine.Unity;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


//序列化储存数据
[System.Serializable]
public class PlayerInfo
{
    public float GameTime = 0;
    public string NewestDate;
    public int[] CharacterInfos = new int[14];
    public int[] FinalCharacterInfos = new int[14];
    public bool isSit;
    public int[] EquipmentID = new int[4];
    public int[] BagItemID = new int[16];
    public int[] BagItemNum = new int[16];

    public int[] CaseItemID = new int[16];
    public int[] CaseItemNum = new int[16];

    public int[,] PlayerUsedSkillPosition = new int[6, 10];
    public int CharacterSpriteID = 101;
    public Vector3 EuipPosition;
    public Vector3 PlayerPosition;

}

public class PlayerInfoList
{
    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();
}


//总控程序
public class PlayerController : MonoBehaviour
{
    //数据实例化
    public PlayerInfoList list = new PlayerInfoList();

    [HorizontalLine]
    [Header("Basic Operation")]
    public float PlayerMoveSpeed;
    public float RotateSpeed;
    public float JumpSpeed;
    private float JumpdeltaTime = 0;
    private bool CanLongJump = true;
    private int Weight = 5;
    private float CameraY = 1;

    public bool UseChooser = true;

    [HideInInspector]
    public float GetDamage, GetHeal;
    private float RealGetDamage;

    //脚印
    public GameObject Footprint, toGroundParticle;
    private float FootprintdeltaTime = 0;
    private bool FootLeft = true;

    public AudioSource JumpSFX;

    [HorizontalLine]
    [Header("Information UI")]
    public TMP_Text[] CharacterInfoUI;

    [HorizontalLine]
    [Header("Basic Information")]
    [Header("0:生命 1:攻击 2:防御 3:速度 4:能量 5:攻击速度 6:攻击范围")]
    [Header("7:能量恢复 8:伤害加成 9:伤害减免 10:暴击率 11:暴击伤害 12:盔甲")]
    [Header("13:击退")]
    //角色初始数据
    //0：生命 1：攻击 2：防御 3：速度 4：能量 5：攻击速度 6：攻击范围 7：能量恢复 8：伤害加成 9：伤害减免 10：暴击率 11：暴击伤害 12：盔甲 13:击退
    public int[] CharacterInfos = new int[14];
    public int[] EquipCharacterInfos = new int[14];
    public int[] FinalCharacterInfos = new int[14];


    private float RegainMPdeltaTime, PlayerAttackIntervaldeltaTime = 0;

    [HideInInspector]
    public float PlayerHP, PlayerMP;

    //Coins
    [HorizontalLine]
    [Header("Coins")]
    public int TotalCoin;
    public int CopperCoin;
    public int SilverCoin;
    public int GoldCoin;
    public int Diamond;

    //小型交易
    public TMP_Text[] TradeCoin;
    public GameObject TradeBar;

    public GameObject TakingItem;
    public TMP_Text ItemNum;
    public TMP_Text DamageFigure;

    private float TakingItemdeltaTime = 0;

    private Vector3 ItemLocalPosition;

    public List<GameObject> canGetItem;

    //Skill
    [HorizontalLine]
    [Header("Skills")]

    [Header("装备职业: (0)无职业 (1)战士 (2)骑士 (3)法师 (4)牧师 (5)游侠")]
    public int PlayerProfession = 0;

    public GameObject[] SkillStoreProfession;
    //index1:职业 6 index2:ID index3:装备格子0/1
    public int[,] PlayerUsedSkillPosition = new int[6, 10];
    public GameObject[] PlayerSkill;
    public GameObject[] PlayerUsedSkillBar;
    private GameObject[] PlayerUsingSkill;

    private Image[] PlayerUsedSkillCD, PlayerUsedSkillUI;
    private float[] SkillCD, SkillDuration, SkillMPCost, SkillCDdeltaTime, SkillDurationdeltaTime;
    private bool[] isSkillReady, isSkilling, isSkillPre;



    //Equipment
    [HorizontalLine]
    [Header("Equipments")]
    public GameObject PlayerEquipmentAll;
    public GameObject[] PlayerEquipment;

    private Vector3 EuipPosition;


    [HorizontalLine]
    [Header("Others")]
    public GameObject Character;
    public GameObject Chooser;

    public CinemachineVirtualCamera VirtualCamera;

    public PackageController packageController;
    public GameObject[] NormalAttack;
    private int AttackModel = 0;

    //被攻击显示
    private float BeAttackedIntervaldeltaTime = 0;
    public Vector3 BeAttackedDirection;

    public GameObject CharacterSprite;
    private int EquipArmorID = 0;
    public Animator CharacterSpriteAnimator, ArmorAnimator, CapAnimator;
    public GameObject CharacterSpriteBack;

    public GameObject CharacterSitSprite;
    private GameObject LeftFoot, RightFoot, ThinkBubble;
    public float SitdeltaTime = 0;

    private int isFront = 0;
    public GameObject MainSprite;
    public GameObject BeAttackedSprite;

    public GameObject CharacterWeapon;
    public GameObject CavasUI;

    [HideInInspector]
    public GameObject HitAim, HitUIAim;

    public GameObject HitParticle;

    [HideInInspector]
    public bool isGround = false, isWater = false, isChooseItem = false, isGhost = false, isSit = false, isMouseMove = false, isKeyBoardMove = false, CanMove = true, xzCanMove = true;
    [HideInInspector]
    public bool isBattle = false;

    // private int MoveModel = 0;

    [HideInInspector]
    public float ySpeed;

    [HideInInspector]
    public CharacterController CharacterController;

    [HideInInspector]
    public Vector3 ChooserVelocity;

    //灵魂控制
    public GameObject GhostSprite;
    public GameObject GhostSpriteBack;

    //AI
    private NavMeshAgent CharacterAgent;

    //记录场景位置
    private GameObject PlayerPositioninScene;

    private Volume GlobalVolume;
    private ColorAdjustments colorAdjustments;

    public AllNPCController allNPCController;

    //装备储存加载
    public int[] EquipmentID = new int[4];
    public int[] BagItemID = new int[16];
    public int[] BagItemNum = new int[16];

    public int[] CaseItemID = new int[16];
    public int[] CaseItemNum = new int[16];
    public GameObject EquipmentStore;
    public GameObject ItemStore;

    //储存信息
    PlayerInfo CharacterA;
    public int ArchiveID = 0;
    public int CharacterSpriteID = 201;
    private float GameTime = 0;

    private Light Gamelight;

    private float StartdeltaTime = 1f;
    //确认加载
    // private bool isLoad = false;
    public Texture2D CursorIcon;
    // Start is called before the first frame update
    void Start()
    {
        //text
        PlayerUsedSkillPosition[1, 0] = 1;
        PlayerUsedSkillPosition[1, 1] = 2;
        PlayerUsedSkillPosition[3, 0] = 1;
        //--------
        //数据加载
        LoadArchive();
        LoadData(ArchiveID);
        //找脚
        foreach (Transform child in CharacterSitSprite.transform)
        {
            if (child.name == "LeftFoot")
            {
                LeftFoot = child.gameObject;
            }
            else if (child.name == "RightFoot")
            {
                RightFoot = child.gameObject;
            }
            else if (child.name == "ThinkBubble")
            {
                ThinkBubble = child.gameObject;
            }
        }

        Cursor.SetCursor(CursorIcon, new Vector2(0, 0), CursorMode.ForceSoftware);
        foreach (Transform child in CharacterSprite.transform)
        {
            BeAttackedSprite = child.gameObject;
            break;
        }

        foreach (Transform child in PlayerEquipment[0].transform)
        {
            AttackModel = child.gameObject.GetComponent<ItemInfo>().AttackType;
            break;
        }

        SkillCD = new float[12];
        SkillDuration = new float[12];
        SkillMPCost = new float[12];

        SkillCDdeltaTime = new float[12];
        SkillDurationdeltaTime = new float[12];
        isSkillReady = new bool[12];
        isSkilling = new bool[12];
        isSkillPre = new bool[12];
        PlayerUsedSkillCD = new Image[12];
        PlayerUsedSkillUI = new Image[12];
        PlayerUsingSkill = new GameObject[PlayerSkill.Length];

        CharacterController = Character.GetComponent<CharacterController>();
        CharacterAgent = GetComponent<NavMeshAgent>();

        for (int i = 0; i < PlayerSkill.Length; i++)
        {
            if (PlayerUsedSkillBar[i] != null)
                foreach (Transform child in PlayerUsedSkillBar[i].transform)
                {
                    if (child.name == "SkillCD")
                        PlayerUsedSkillCD[i] = child.GetComponent<Image>();
                    else if (child.name == "SkillUI")
                        PlayerUsedSkillUI[i] = child.GetComponent<Image>();
                }
        }

        //参数初始化
        LoadEquip();
        //EquipmentInformation();

        PlayerHP = FinalCharacterInfos[0];
        PlayerMP = FinalCharacterInfos[4];
        /*for (int i = 0; i < PlayerSkill.Length; i++)
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
            }*/

        CharacterSpriteAnimator = CharacterSprite.GetComponent<Animator>();
        ArmorAnimator = PlayerEquipment[3].GetComponent<Animator>();
        CapAnimator = PlayerEquipment[2].GetComponent<Animator>();

        ItemLocalPosition = TakingItem.transform.localPosition;
        CoinController();
    }

    // Update is called once per frame
    void Update()
    {
        //光标隐藏
        /*if (Input.GetKey(KeyCode.LeftCommand) || CavasUI.GetComponent<UIController>().PackagePanel.activeSelf)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }*/

        //选择框可见性
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UseChooser = !UseChooser;
        }

        //进入游戏禁止移动时间
        if (StartdeltaTime > 0)
        {
            xzCanMove = false;
            StartdeltaTime -= Time.deltaTime;
            if (StartdeltaTime <= 0)
            {
                xzCanMove = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //string jsonPath = Application.streamingAssetsPath + "/PlayerInfo.json";
            //string jsonPath = Application.persistentDataPath + "/PlayerInfo.json";
            //File.Delete(jsonPath);
        }

        if (GlobalVolume == null)
        {
            GlobalVolume = GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>();
            GlobalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
            Gamelight = GlobalVolume.transform.parent.GetComponent<Light>();
        }

        TimeController();
        //玩家进入场景位置
        /*if (PlayerPositioninScene == null)
        {
            PlayerPositioninScene = GameObject.FindGameObjectWithTag("PlayerPositioninScene");
            GetComponent<CharacterController>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = PlayerPositioninScene.transform.position;
            GetComponent<CharacterController>().enabled = true;
            GetComponent<NavMeshAgent>().enabled = true;
        }*/

        if (CanMove)
        {
            Move();
            //激活相机后开始鼠标交互
            if (Camera.main != null)
                MouseInteraction();
        }

        HPController();
        if (PlayerHP > 0)
        {
            SkillController();
            SitController();
            AttackModelController();
            TakingItemControllerSelf();
        }


        if (colorAdjustments.saturation.value > -100 && isGhost)
        {
            colorAdjustments.saturation.Override(Mathf.Clamp(Mathf.Lerp(a: colorAdjustments.saturation.value, b: -100, t: 3 * Time.deltaTime), -100f, 100f));
        }
        else if (colorAdjustments.saturation.value < 0 && !isGhost)
        {
            colorAdjustments.saturation.Override(Mathf.Clamp(Mathf.Lerp(a: colorAdjustments.saturation.value, b: 0, t: 3 * Time.deltaTime), -100f, 100f));
        }

    }

    public void TimeController()
    {
        //10分钟一天
        GameTime += Time.deltaTime / (Gamelight.intensity + 0.5f);

        Gamelight.intensity = 4.01f - (1 + math.sin(-math.PI / 2 + GameTime / 10)) * 2f;
        Gamelight.colorTemperature = 6000 + (1 + math.sin(-math.PI / 2 + GameTime / 10)) * 7000f;

    }

    //装备储存加载
    public void LoadEquip()
    {
        foreach (Transform child in EquipmentStore.transform)
        {
            for (int i = 0; i < 4; i++)
            {
                if (child.GetComponent<ItemInfo>().ItemID == EquipmentID[i])
                {

                    Vector3 Scale = child.lossyScale;
                    GameObject Equap = Instantiate(child, PlayerEquipment[i].transform.position + new Vector3(0, 0, 0), PlayerEquipment[i].transform.rotation, PlayerEquipment[i].transform).gameObject;
                    Equap.transform.localScale = Scale;
                    EquipmentInformation();
                    break;
                }
            }
        }
    }

    //坐下与存档
    public void SitController()
    {
        if (isSit)
        {
            if (isFront != 1)
            {
                //骨骼读取
                SkeletonMecanim CharacterskeletonMecanim = CharacterSprite.GetComponent<SkeletonMecanim>();
                SkeletonMecanim ArmorskeletonMecanim = PlayerEquipment[3].GetComponent<SkeletonMecanim>();
                isFront = 1;
                PlayerEquipment[2].transform.localEulerAngles = new Vector3(0, 0, 0);
                //皮肤切换以及重加载
                CharacterskeletonMecanim.skeleton.SetSkin("1");
                CharacterskeletonMecanim.skeleton.SetSlotsToSetupPose();

                CharacterWeapon.transform.localPosition = new Vector3(0, 0, 0.07f);
                CharacterWeapon.transform.localEulerAngles = new Vector3(CharacterWeapon.transform.localEulerAngles.x, 0, -90);
            }

            EuipPosition = new Vector3(0, -0.02f, 0f);

            PlayerEquipmentAll.transform.localPosition = EuipPosition;
            CharacterSpriteAnimator.SetBool("isSit", true);
            ArmorAnimator.SetBool("isSit", true);
            if (SitdeltaTime > 0)
            {
                SitdeltaTime -= Time.deltaTime;
                if (SitdeltaTime <= 0.5 && SitdeltaTime > 0)
                {
                    ThinkBubble.transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime / SitdeltaTime * 3;
                    ThinkBubble.transform.localPosition -= new Vector3(0, 1, 0) * Time.deltaTime / SitdeltaTime * 5;
                    if (ThinkBubble.transform.localScale.x < 0)
                        ThinkBubble.transform.localScale = new Vector3(0, 0, 0);
                    if (ThinkBubble.transform.localPosition.y < 30)
                    {
                        ThinkBubble.transform.localPosition = new Vector3(0, 30, 0);
                    }
                }
                else if (SitdeltaTime > 0.5)
                {
                    if (ThinkBubble.transform.localScale.x < 5)
                    {
                        ThinkBubble.transform.localScale = new Vector3(1, 1, 1) * 5;
                    }
                    else if (ThinkBubble.transform.localScale.x < 11)
                    {
                        ThinkBubble.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime / SitdeltaTime * 15;
                    }
                    if (ThinkBubble.transform.localPosition.y < 40)
                    {
                        ThinkBubble.transform.localPosition += new Vector3(0, 1, 0) * Time.deltaTime / SitdeltaTime * 20;
                    }

                }
                else if (SitdeltaTime <= 0)
                {
                    xzCanMove = true;
                }
            }
            LeftFoot.transform.localEulerAngles = new Vector3(0, 0, math.sin(Time.time * 2) - 0.5f) * 20;
            RightFoot.transform.localEulerAngles = new Vector3(0, 0, -math.sin(Time.time * 2) + 0.5f) * 20;
            CharacterSitSprite.SetActive(true);
            //CharacterSprite.SetActive(false);

        }
        else if (CharacterSpriteAnimator.GetBool("isSit"))
        {
            EuipPosition = new Vector3(0, 0, 0);
            PlayerEquipmentAll.transform.localPosition = EuipPosition;
            CharacterSpriteAnimator.SetBool("isSit", false);
            ArmorAnimator.SetBool("isSit", false);
            CharacterSitSprite.SetActive(false);
            //CharacterSprite.SetActive(true);
        }
    }
    //灵魂系统
    public void GhostController()
    {
        if (isGhost)
        {
            CharacterSprite.SetActive(false);
            GhostSprite.SetActive(true);
            allNPCController.NPCGhostUpdate();
            BeAttackedDirection = new Vector3(0, 0, 0);

            for (int i = 0; i < PlayerEquipment.Length; i++)
            {
                PlayerEquipment[i].SetActive(false);
            }

        }
        if (PlayerHP > 0)
        {
            BeAttackedSprite.SetActive(false);
            CharacterSprite.SetActive(true);
            GhostSprite.SetActive(false);
            isGhost = false;
            //关闭NPC魂
            allNPCController.NPCGhostUpdate();
            for (int i = 0; i < PlayerEquipment.Length; i++)
            {
                PlayerEquipment[i].SetActive(true);
            }
        }

    }

    private void TakingItemControllerSelf()
    {
        if (TakingItemdeltaTime < 0.5f)
        {
            TakingItemdeltaTime += Time.deltaTime;
            TakingItem.transform.localPosition += new Vector3(0, (0.5f - TakingItemdeltaTime) * 4 * Time.deltaTime, 0);
        }
        else
        {
            TakingItem.SetActive(false);
        }
    }
    public void TakingItemController(Sprite ItemSprite, int Num)
    {
        TakingItemdeltaTime = 0;
        TakingItem.transform.localPosition = ItemLocalPosition;
        TakingItem.SetActive(true);
        TakingItem.GetComponent<SpriteRenderer>().sprite = ItemSprite;
        ItemNum.text = "+" + Num.ToString();
    }

    //金币更新
    public void CoinController()
    {
        GoldCoin = TotalCoin / 10000;
        SilverCoin = TotalCoin % 10000 / 100;
        CopperCoin = TotalCoin % 100;

        TradeCoin[0].text = GoldCoin.ToString();
        TradeCoin[1].text = SilverCoin.ToString();
        TradeCoin[2].text = CopperCoin.ToString();
    }

    //技能信息更新
    public void SkillInformation()
    {
        for (int i = 0; i < 6; i++)
        {
            PlayerUsedSkillUI[i].sprite = null;
            PlayerUsedSkillCD[i].fillAmount = 1;
            packageController.EquipSkillButton[i].GetComponent<Image>().sprite = null;
            foreach (Transform child in PlayerSkill[i].transform)
            {
                Destroy(child.gameObject);
            }
            if (PlayerProfession != 0)
            {
                foreach (Transform child in SkillStoreProfession[PlayerProfession - 1].transform)
                {
                    if (PlayerUsedSkillPosition[PlayerProfession, child.GetComponent<SkillInfo>().SkillID] == i + 1)
                    {
                        PlayerUsedSkillUI[i].sprite = child.GetComponent<SpriteRenderer>().sprite;
                        packageController.EquipSkillButton[i].GetComponent<Image>().sprite = child.GetComponent<SpriteRenderer>().sprite;
                        Instantiate(child, transform.position, child.rotation, PlayerSkill[i].transform);
                    }
                }
            }
        }

        for (int i = 0; i < PlayerSkill.Length; i++)
        {
            isSkillReady[i] = true;

            //寻找Skill的子物体（正在使用的技能）
            foreach (Transform child in PlayerSkill[i].transform)
            {
                if (child.tag == "Skill")
                    PlayerUsingSkill[i] = child.gameObject;
            }
            if (PlayerUsingSkill[i] != null)
            {
                SkillCD[i] = PlayerUsingSkill[i].GetComponent<SkillInfo>().CoolDown;
                SkillCDdeltaTime[i] = SkillCD[i] - 0.001f;
                SkillDuration[i] = PlayerUsingSkill[i].GetComponent<SkillInfo>().Duration;
                SkillMPCost[i] = PlayerUsingSkill[i].GetComponent<SkillInfo>().MPCost;

            }

        }

    }
    //装备信息更新
    public void EquipmentInformation()
    {

        EquipCharacterInfos = new int[14];
        AttackModel = 0;
        ArmorAnimator.gameObject.GetComponent<MeshRenderer>().enabled = false;
        int LastPlayerProfession = PlayerProfession;
        PlayerProfession = 0;
        for (int i = 0; i < PlayerEquipment.Length; i++)
        {
            //寻找Equipment的子物体
            foreach (Transform child in PlayerEquipment[i].transform)
            {
                if (child.tag == "BagItem")
                {
                    ItemInfo itemInfo = child.gameObject.GetComponent<ItemInfo>();
                    if (i == 0)
                    {
                        AttackModel = itemInfo.AttackType;
                        EquipCharacterInfos[6] = (int)itemInfo.AttackRange;
                        EquipCharacterInfos[5] = (int)itemInfo.AttackSpeed;
                    }

                    EquipCharacterInfos[1] += (int)itemInfo.Attack;
                    EquipCharacterInfos[2] += (int)itemInfo.Defence;
                    EquipCharacterInfos[0] += (int)itemInfo.MaxHP;
                    EquipCharacterInfos[7] += (int)itemInfo.MaxMP;
                    EquipCharacterInfos[3] += (int)itemInfo.Speed;
                    EquipCharacterInfos[13] += itemInfo.BeatBack;

                    //更新动画贴图
                    if (i == 3)
                    {
                        SkeletonMecanim ArmorskeletonMecanim = PlayerEquipment[3].GetComponent<SkeletonMecanim>();
                        EquipArmorID = itemInfo.ItemID;
                        //皮肤切换以及重加载
                        if (isFront == 1)
                            ArmorskeletonMecanim.skeleton.SetSkin(EquipArmorID.ToString() + "A");
                        else
                            ArmorskeletonMecanim.skeleton.SetSkin(EquipArmorID.ToString() + "B");

                        ArmorskeletonMecanim.skeleton.SetSlotsToSetupPose();
                        ArmorAnimator.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    }

                    if (i == 0)
                    {
                        PlayerProfession = itemInfo.EquipProfession;
                    }

                    break;
                }
            }
        }


        for (int i = 0; i < CharacterInfos.Length; i++)
        {
            if (i == 5 && EquipCharacterInfos[i] != 0)
            {
                FinalCharacterInfos[i] = EquipCharacterInfos[i];
            }
            else
                FinalCharacterInfos[i] = CharacterInfos[i] + EquipCharacterInfos[i];
        }

        //伤害更新
        for (int i = 0; i < 2; i++)
        {
            NormalAttack[i].GetComponent<NormalAttackTrigger>().Damage = FinalCharacterInfos[1];
            NormalAttack[i].GetComponent<NormalAttackTrigger>().BeatBack = FinalCharacterInfos[13];
            NormalAttack[i].GetComponent<NormalAttackTrigger>().Holder = gameObject;
        }

        //面板展示
        for (int i = 0; i < CharacterInfoUI.Length; i++)
        {
            CharacterInfoUI[i].text = FinalCharacterInfos[i].ToString();
        }

        //技能更新
        if (LastPlayerProfession != PlayerProfession)
            SkillInformation();

    }


    private void AttackModelController()
    {
        PlayerAttackIntervaldeltaTime += Time.deltaTime;

        if (PlayerAttackIntervaldeltaTime > 0.1f)
        {
            CharacterSpriteAnimator.SetBool("isAttack", false);
            ArmorAnimator.SetBool("isAttack", false);
        }
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.J)) && AttackModel == 1 && PlayerMP > 5 && HitAim != null && (HitAim.transform.position - transform.position).magnitude < (float)FinalCharacterInfos[6] / 4 && isChooseItem && (HitAim.tag == "Enemy" || HitAim.tag == "NPCNeutrality") && PlayerAttackIntervaldeltaTime > (float)FinalCharacterInfos[5] / 10)
        {
            PlayerMP -= 3;
            PlayerAttackIntervaldeltaTime = 0;
            CharacterSpriteAnimator.SetBool("isAttack", true);
            ArmorAnimator.SetBool("isAttack", true);
            System.Random random = new System.Random();
            PlayerEquipment[0].transform.localEulerAngles = new Vector3(0, 0, 45 + isFront * 180);
            PlayerEquipment[0].transform.localPosition = new Vector3(isFront * 0.3f, 0.3f, -isFront * 0.05f);
            NormalAttack[1].GetComponent<NormalAttackTrigger>().AttackAim = HitAim;
            NormalAttack[1].GetComponent<NormalAttackTrigger>().AttackDirection = (HitAim.transform.position + new Vector3(0, 0.3f, 0) - (transform.position + new Vector3(-isFront * 0.3f, 0.3f, -isFront * 0.05f) + new Vector3(0, 0.3f, 0))).normalized;
            NormalAttack[1].GetComponent<NormalAttackTrigger>().InitialPositionShift = new Vector3(-isFront * 0.3f, 0.3f, -isFront * 0.05f) + new Vector3(0, 0.3f, 0);
            //暴击计算
            if ((float)random.NextDouble() <= FinalCharacterInfos[10] / 100)
            {
                NormalAttack[1].GetComponent<NormalAttackTrigger>().Damage = FinalCharacterInfos[1] * (1 + FinalCharacterInfos[11] / 100);
                NormalAttack[1].GetComponent<NormalAttackTrigger>().isCrit = true;
            }
            else
            {
                NormalAttack[1].GetComponent<NormalAttackTrigger>().Damage = FinalCharacterInfos[1];
                NormalAttack[1].GetComponent<NormalAttackTrigger>().isCrit = false;
            }
        }
        else if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.J)) && AttackModel == 0 && PlayerAttackIntervaldeltaTime > (float)FinalCharacterInfos[5] / 10)
        {
            PlayerAttackIntervaldeltaTime = 0;
            CharacterSpriteAnimator.SetBool("isAttack", true);
            ArmorAnimator.SetBool("isAttack", true);
            PlayerEquipment[0].SetActive(false);
            NormalAttack[0].GetComponent<SpriteRenderer>().flipX = !NormalAttack[0].GetComponent<SpriteRenderer>().flipX;

            System.Random random = new System.Random();
            if ((float)random.NextDouble() <= FinalCharacterInfos[10] / 100)
            {
                NormalAttack[0].GetComponent<NormalAttackTrigger>().Damage = FinalCharacterInfos[1] * (1 + FinalCharacterInfos[11] / 100);
                NormalAttack[0].GetComponent<NormalAttackTrigger>().isCrit = true;
            }
            else
            {
                NormalAttack[0].GetComponent<NormalAttackTrigger>().Damage = FinalCharacterInfos[1];
                NormalAttack[0].GetComponent<NormalAttackTrigger>().isCrit = false;
            }

            Vector3 HitPos = new Vector3(0, 0, 0);

            //攻击方向

            if (HitAim != null)
            {
                HitPos = HitAim.transform.position;
            }
            else
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    HitPos = hit.point;
                }
            }

            if ((-gameObject.transform.position + HitPos).x > 0)
                NormalAttack[0].transform.eulerAngles = new Vector3(-90, -180 + math.acos((-gameObject.transform.position + HitPos).normalized.z) / math.PI * 180, 0);
            else
            {
                NormalAttack[0].transform.eulerAngles = new Vector3(-90, 180 - math.acos((-gameObject.transform.position + HitPos).normalized.z) / math.PI * 180, 0);
            }

            NormalAttack[0].transform.localScale = new Vector3(1, 1, 1) * (float)FinalCharacterInfos[6] / 8;
            NormalAttack[0].GetComponent<NormalAttackTrigger>().NearAttackDisdeltaTime = 0.2f;
            NormalAttack[0].SetActive(true);
        }
        PlayerEquipment[0].SetActive(!NormalAttack[0].activeSelf);
        if (isGhost)
        {
            PlayerEquipment[0].SetActive(false);
        }
        if (PlayerEquipment[0].transform.localPosition.y != 0 && PlayerAttackIntervaldeltaTime > 0.2f)
        {
            // PlayerEquipment[0].transform.localEulerAngles = new Vector3(0, 0, -90);
            PlayerEquipment[0].transform.localPosition = new Vector3(PlayerEquipment[0].transform.localPosition.x, 0, PlayerEquipment[0].transform.localPosition.z);
        }
    }
    private void HPController()
    {
        if (BeAttackedDirection.magnitude > 0.01)
        {
            BeAttackedDirection *= 0.95f;
        }
        else
        {
            BeAttackedDirection = new Vector3(0, 0, 0);
        }

        if (BeAttackedIntervaldeltaTime <= 0.15f)
        {
            CharacterSpriteAnimator.SetBool("isAttacked", true);
            ArmorAnimator.SetBool("isAttacked", true);

            BeAttackedIntervaldeltaTime += Time.deltaTime;

            //VirtualCamera.m_Lens.Dutch = (float)(math.sin(Time.time * 100) * BeAttackedIntervaldeltaTime * 2);
            if (BeAttackedIntervaldeltaTime > 0.15f)
            {
                CharacterSpriteAnimator.SetBool("isAttacked", false);
                ArmorAnimator.SetBool("isAttacked", false);
                //VirtualCamera.m_Lens.Dutch = 0;
                xzCanMove = true;
                CavasUI.GetComponent<UIController>().vignette.rounded.value = false;
                CharacterSprite.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f);
                // BeAttackedSprite.SetActive(false);
            }

        }
        if (GetDamage != 0 && BeAttackedIntervaldeltaTime > 0.15f)
        {
            RealGetDamage = (int)(GetDamage - FinalCharacterInfos[2]);
            if (RealGetDamage < 1)
            {
                RealGetDamage = 1;
            }

            //受伤视觉效果
            CavasUI.GetComponent<UIController>().vignette.rounded.value = true;
            CharacterSprite.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.6f, 0.6f);
            //BeAttackedSprite.SetActive(true);
            BeAttackedIntervaldeltaTime = 0;
            xzCanMove = false;

            TMP_Text DamageFigureIns = Instantiate(DamageFigure, transform.position + new Vector3(0, 0.5f, 0), transform.rotation, DamageFigure.transform.parent);
            DamageFigureIns.text = RealGetDamage.ToString();
            DamageFigureIns.gameObject.transform.localScale = DamageFigureIns.gameObject.transform.localScale * (1 + 0.1f * math.log(RealGetDamage));
            DamageFigureIns.color = new Color(1, 0, 0);//red
            DamageFigureIns.gameObject.SetActive(true);
            PlayerHP -= RealGetDamage;
            GetDamage = 0;
        }
        if (PlayerHP <= 0)
        {
            PlayerHP = 0;
            xzCanMove = true;
            BeAttackedDirection = new Vector3(0, 0, 0);
            CavasUI.GetComponent<UIController>().vignette.rounded.value = false;
            CharacterSprite.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f);
            CharacterSpriteAnimator.SetBool("isAttacked", false);
            isGhost = true;
            GhostController();
        }

        if (GetHeal != 0)
        {
            PlayerHP += GetHeal;
            TMP_Text DamageFigureIns = Instantiate(DamageFigure, transform.position + new Vector3(0, 0.5f, 0), transform.rotation, DamageFigure.transform.parent);
            DamageFigureIns.text = GetHeal.ToString();
            DamageFigureIns.gameObject.transform.localScale = DamageFigureIns.gameObject.transform.localScale * (1 + 0.1f * math.log(GetHeal));
            DamageFigureIns.color = new Color(0.1f, 0.9f, 0);//green
            DamageFigureIns.gameObject.SetActive(true);
            GetHeal = 0;
        }
        if (PlayerHP >= FinalCharacterInfos[0])
        {
            PlayerHP = FinalCharacterInfos[0];
        }

        /*
                //脱战回血
                if (!isBattle && PlayerHP < PlayerMaxHP)
                {
                    RegainHPdeltaTime += Time.deltaTime;
                    if (RegainHPdeltaTime > 1f)
                    {
                        PlayerHP += PlayerRegainHP;
                        RegainHPdeltaTime = 0;
                    }
                }*/
        //回复魔法值
        RegainMPdeltaTime += Time.deltaTime;
        if (!isBattle && PlayerMP < FinalCharacterInfos[4])
        {
            if (RegainMPdeltaTime > 1f)
            {
                PlayerMP += FinalCharacterInfos[7];
                RegainMPdeltaTime = 0;
            }
        }
        else if (isKeyBoardMove == false && PlayerMP < FinalCharacterInfos[4])
        {
            if (RegainMPdeltaTime > 0.2f)
            {
                PlayerMP += FinalCharacterInfos[7];
                RegainMPdeltaTime = 0;
            }
        }

    }
    private void Move()
    {
        float MoveDirectionx = 0;
        float MoveDirectiony = 0;
        if (xzCanMove)
        {
            MoveDirectionx = Input.GetAxis("Horizontal");
            MoveDirectiony = Input.GetAxis("Vertical");
        }
        if (MoveDirectionx != 0 || MoveDirectiony != 0)
        {
            isSit = false;
            CharacterSpriteAnimator.SetBool("isMove", true);
            ArmorAnimator.SetBool("isMove", true);
            //脚印
            if (isGround && !isGhost && (math.abs(MoveDirectionx) > 0.1 || math.abs(MoveDirectiony) > 0.1))
            {
                //声音 
                gameObject.GetComponent<AudioSource>().enabled = true;
                FootprintdeltaTime += Time.deltaTime;
            }
            else
            {
                gameObject.GetComponent<AudioSource>().enabled = false;
            }

            if (CharacterAgent.isOnNavMesh)
                CharacterAgent.isStopped = true;
            CharacterController.enabled = true;
            isKeyBoardMove = true;

            //骨骼读取
            SkeletonMecanim CharacterskeletonMecanim = CharacterSprite.GetComponent<SkeletonMecanim>();
            SkeletonMecanim ArmorskeletonMecanim = PlayerEquipment[3].GetComponent<SkeletonMecanim>();
            //SkeletonMecanim CapskeletonMecanim = PlayerEquipment[2].GetComponent<SkeletonMecanim>();
            //左右翻转
            if (MoveDirectionx < 0)
            {
                CharacterskeletonMecanim.skeleton.ScaleX = -1;
                CharacterskeletonMecanim.skeleton.SetToSetupPose(); // 应用姿势刷新

                ArmorskeletonMecanim.skeleton.ScaleX = -1;
                ArmorskeletonMecanim.skeleton.SetToSetupPose(); // 应用姿势刷新
            }
            else if (MoveDirectionx > 0)
            {
                CharacterskeletonMecanim.skeleton.ScaleX = 1;
                CharacterskeletonMecanim.skeleton.SetToSetupPose(); // 应用姿势刷新

                ArmorskeletonMecanim.skeleton.ScaleX = 1;
                ArmorskeletonMecanim.skeleton.SetToSetupPose(); // 应用姿势刷新
            }
            //朝向
            if (MoveDirectiony < 0 && PlayerAttackIntervaldeltaTime > 0.2f && (isFront != 1 || CharacterWeapon.transform.localPosition.z < 0))
            {
                isFront = 1;
                PlayerEquipment[2].transform.localEulerAngles = new Vector3(0, 0, 0);
                //皮肤切换以及重加载
                CharacterskeletonMecanim.skeleton.SetSkin("1");
                if (ArmorAnimator.gameObject.GetComponent<MeshRenderer>().enabled)
                    ArmorskeletonMecanim.skeleton.SetSkin(EquipArmorID.ToString() + "A");
                CharacterskeletonMecanim.skeleton.SetSlotsToSetupPose();
                ArmorskeletonMecanim.skeleton.SetSlotsToSetupPose();

                CharacterWeapon.transform.localPosition = new Vector3(0, 0, 0.07f);
                CharacterWeapon.transform.localEulerAngles = new Vector3(CharacterWeapon.transform.localEulerAngles.x, 0, -90);

                //CharacterSprite.GetComponent<SpriteRenderer>().enabled = true;
                GhostSprite.GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (MoveDirectiony > 0 && PlayerAttackIntervaldeltaTime > 0.2f && (isFront != -1 || CharacterWeapon.transform.localPosition.z > 0))
            {
                isFront = -1;
                PlayerEquipment[2].transform.localEulerAngles = new Vector3(0, 180, 0);
                //皮肤切换以及重加载
                CharacterskeletonMecanim.skeleton.SetSkin("2");
                if (ArmorAnimator.gameObject.GetComponent<MeshRenderer>().enabled)
                    ArmorskeletonMecanim.skeleton.SetSkin(EquipArmorID.ToString() + "B");
                CharacterskeletonMecanim.skeleton.SetSlotsToSetupPose();
                ArmorskeletonMecanim.skeleton.SetSlotsToSetupPose();

                CharacterWeapon.transform.localPosition = new Vector3(-0.03f, 0, -0.07f);
                CharacterWeapon.transform.localEulerAngles = new Vector3(CharacterWeapon.transform.localEulerAngles.x, 180, -90);
                //CharacterSprite.GetComponent<SpriteRenderer>().enabled = false;
                GhostSprite.GetComponent<SpriteRenderer>().enabled = false;
            }
            PlayerEquipmentAll.transform.localPosition = EuipPosition + new Vector3(0, math.sin(Time.time * 20) / 80, 0);
        }
        else
        {
            gameObject.GetComponent<AudioSource>().enabled = false;
            CharacterSpriteAnimator.SetBool("isMove", false);
            ArmorAnimator.SetBool("isMove", false);
            PlayerEquipmentAll.transform.localPosition = EuipPosition;
            isKeyBoardMove = false;
        }

        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            isSit = false;
            CanLongJump = true;
            ySpeed = JumpSpeed;
            CharacterSpriteAnimator.SetBool("isJump", true);
            ArmorAnimator.SetBool("isJump", true);
            JumpSFX.enabled = false;
            JumpSFX.enabled = true;
        }


        if (!Input.GetKey(KeyCode.Space))
        {
            CanLongJump = false;
        }
        //长按跳跃
        if (CanLongJump && JumpdeltaTime < 0.3f)
        {
            JumpdeltaTime += Time.deltaTime;
            ySpeed = JumpSpeed / (1 + JumpdeltaTime);
            CharacterSpriteAnimator.SetBool("isJump", true);
            ArmorAnimator.SetBool("isJump", true);
        }



        if (!isGround)
            ySpeed -= 40f * Time.deltaTime;
        else if (ySpeed < 0)
        {
            if (ySpeed < -JumpSpeed)
                for (int i = 0; i < 10; i++)
                {
                    GameObject Ins = Instantiate(toGroundParticle, transform.position - new Vector3(0, 0.25f, 0), transform.rotation);
                    System.Random random = new System.Random();
                    if (i == 0 && StartdeltaTime <= 0)
                    {
                        Ins.GetComponent<AudioSource>().enabled = true;
                        Ins.GetComponent<AudioSource>().volume *= 0.006f * math.abs(ySpeed) * math.abs(ySpeed);
                    }
                    Ins.transform.localScale = new Vector3(1f, 1f, 1f) * 0.01f * math.sqrt(math.abs(ySpeed)) * ((float)random.NextDouble() / 1.5f + 0.5f);
                    Ins.SetActive(true);
                }

            if (ySpeed < -4 * JumpSpeed)
            {
                GetDamage = (int)-ySpeed / JumpSpeed;
            }
            ySpeed = 0;
            JumpdeltaTime = 0;
            CharacterSpriteAnimator.SetBool("isJump", false);
            ArmorAnimator.SetBool("isJump", false);
        }


        Vector3 NormVelocity = new Vector3(math.sin(transform.eulerAngles.y / 180 * math.PI), 0, math.cos(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectiony + new Vector3(math.cos(transform.eulerAngles.y / 180 * math.PI), 0, -math.sin(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectionx;
        if (NormVelocity.magnitude > 1)
        {
            NormVelocity = NormVelocity.normalized;
        }
        ChooserVelocity = PlayerMoveSpeed * Time.deltaTime * NormVelocity;

        //角色朝向
        if (isKeyBoardMove)
        {
            isSit = false;
            //CharacterSprite.GetComponent<SpriteRenderer>().enabled
            /*if (CharacterSprite.GetComponent<SkeletonMecanim>().initialSkinName == "1")
            {
                //CharacterWeapon.transform.localPosition = new Vector3(CharacterWeapon.transform.localPosition.x, CharacterWeapon.transform.localPosition.y, 0.12f);
            }
            else
            {
                // CharacterWeapon.transform.localPosition = new Vector3(CharacterWeapon.transform.localPosition.x, CharacterWeapon.transform.localPosition.y, -0.12f);
            }
            //CharacterSpriteBack.SetActive(!CharacterSprite.GetComponent<SpriteRenderer>().enabled);*/
            GhostSpriteBack.SetActive(!GhostSprite.GetComponent<SpriteRenderer>().enabled);
        }

        //脚印
        if (FootprintdeltaTime > 0.1f)
        {
            FootprintdeltaTime = 0;
            FootLeft = !FootLeft;
            if (FootLeft)
            {
                Vector3 LeftPrint = (new Vector3(math.sin(transform.eulerAngles.y / 180 * math.PI + 90), 0, math.cos(transform.eulerAngles.y / 180 * math.PI + 90)) * MoveDirectiony + new Vector3(math.cos(transform.eulerAngles.y / 180 * math.PI + 90), 0, -math.sin(transform.eulerAngles.y / 180 * math.PI + 90)) * MoveDirectionx).normalized;
                Instantiate(Footprint, transform.position + new Vector3(0, -0.25f, 0) + LeftPrint * 0.13f, Footprint.transform.rotation);
            }
            else
            {
                Vector3 RightPrint = (new Vector3(math.sin(transform.eulerAngles.y / 180 * math.PI - 90), 0, math.cos(transform.eulerAngles.y / 180 * math.PI - 90)) * MoveDirectiony + new Vector3(math.cos(transform.eulerAngles.y / 180 * math.PI - 90), 0, -math.sin(transform.eulerAngles.y / 180 * math.PI - 90)) * MoveDirectionx).normalized;
                Instantiate(Footprint, transform.position + new Vector3(0, -0.25f, 0) + RightPrint * 0.1f, Footprint.transform.rotation);
            }
        }
        //鼠标控制视角旋转
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            this.transform.eulerAngles += new Vector3(0, RotateSpeed * Time.deltaTime * mouseX * 15, 0);
            float mouseY = Input.GetAxis("Mouse Y");
            VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y / CameraY;
            CameraY += RotateSpeed * Time.deltaTime * mouseY * 0.2f;
            CameraY = Mathf.Clamp(CameraY, 0.2f, 1);
            VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y * CameraY;
            VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_ScreenY -= RotateSpeed * Time.deltaTime * mouseY * 0.025f;
            VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = Mathf.Clamp(VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_ScreenY, 0.5f, 0.6f);
        }

        //视角旋转
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
            float newSizey = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y / CameraY;
            newSizey += -scroll * 1.5f;
            float newSizez = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z += scroll * 1.5f;
            float newComposer = VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y += -scroll * 0.3f;
            VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Clamp(newSizey, 3, 9) * CameraY;
            VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = Mathf.Clamp(newSizez, -12, -6);
            VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = Mathf.Clamp(newComposer, 0, 1.2f);

        }

        if (CharacterController.enabled)
        {
            CharacterController.Move(ChooserVelocity + 1000 * BeAttackedDirection / Weight + new Vector3(0, ySpeed * Time.deltaTime, 0));

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
            //NPC交互
            if (hit.collider.tag == "ChatUI" && hit.collider.name == "Team")
            {
                if (HitUIAim != null && HitUIAim.name != "TaskReward")
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                hit.collider.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                HitUIAim = hit.collider.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    HitUIAim.transform.parent.gameObject.SetActive(false);
                    HitUIAim.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<FNPCInfo>().isinTeam = !HitUIAim.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<FNPCInfo>().isinTeam;
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    HitUIAim = null;
                }
            }
            else if (hit.collider.tag == "ChatUI" && hit.collider.name == "Task")
            {
                if (HitUIAim != null && HitUIAim.name != "TaskReward")
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                hit.collider.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                HitUIAim = hit.collider.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    HitUIAim.transform.parent.gameObject.SetActive(false);
                    HitUIAim.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<FNPCInfo>().isTask = true;
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    HitUIAim = null;
                }
            }
            else if (hit.collider.tag == "ChatUI" && hit.collider.name == "TaskReward")
            {
                if (HitUIAim != null && HitUIAim.name != "TaskReward")
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                HitUIAim = hit.collider.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    HitUIAim.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<FNPCInfo>().isTask = false;
                    HitUIAim.transform.parent.gameObject.transform.parent.GetComponent<Task>().TaskRewardController();
                    HitUIAim = null;
                }
            }
            else if (hit.collider.tag == "ChatUI" && hit.collider.name == "Trade")//NPC交易
            {
                if (HitUIAim != null && HitUIAim.name != "TaskReward")
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                hit.collider.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                HitUIAim = hit.collider.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    HitUIAim.transform.parent.gameObject.SetActive(false);
                    HitUIAim.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<FNPCInfo>().isTrade = true;
                    HitUIAim.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<FNPCInfo>().NPCTradeController();
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    HitUIAim = null;
                }
            }
            /*else if (hit.collider.tag == "ChatUI" && (hit.collider.name == "Sell" || hit.collider.name == "Buy"))//物品信息显示
            {
                hit.collider.gameObject.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
                HitUIAim = hit.collider.gameObject;
                HitUIAim.transform.parent.gameObject.transform.parent.GetComponent<TradeController>().ShowItemInfo = true;
            }*/
            else if (hit.collider.tag == "ChatUI" && hit.collider.name == "TradeButton")//达成交易
            {
                if (HitUIAim != null && HitUIAim.name != "TaskReward")
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                hit.collider.gameObject.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
                HitUIAim = hit.collider.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    HitUIAim.GetComponent<TradeController>().isTrading = true;
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    HitUIAim = null;
                    isSit = false;
                }
            }
            else if (HitUIAim != null)
            {
                //if (HitUIAim.name == "Sell" || HitUIAim.name == "Buy")
                //    HitUIAim.transform.parent.gameObject.transform.parent.GetComponent<TradeController>().ShowItemInfo = false;
                if (HitUIAim.name != "TaskReward")
                    HitUIAim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                HitUIAim = null;
            }
        }
        //右键移动
        /*if (Input.GetMouseButton(1) && !isKeyBoardMove && Input.GetKey(KeyCode.C))
        {
            isSit = false;
            isMouseMove = true;
            isGround = true;
            CharacterAgent.stoppingDistance = 0;
            CharacterAgent.isStopped = false;
            CharacterController.enabled = false;
            if (Input.GetMouseButtonDown(1) && MoveModel == 0)
            {
                CharacterAgent.Warp(transform.position);
            }
            MoveModel = 1;
            CharacterAgent.destination = hit.point;

            if (Input.GetMouseButtonDown(1))
                Instantiate(HitParticle, hit.point + new Vector3(0, 0.001f, 0), HitParticle.transform.rotation);
        }
        else
        {
            isMouseMove = false;
        }*/

        if (!isChooseItem)
        {
            //预选则
            if (hit.collider != null && (hit.collider.tag == "IntItem" || hit.collider.tag == "Enemy" || hit.collider.tag == "NPCNeutrality" || hit.collider.tag == "NPCFriend"))
            {
                if (UseChooser)
                    Chooser.SetActive(true);
                HitAim = hit.collider.gameObject;
                //确定选择
                if (Input.GetMouseButtonDown(0))
                {
                    isChooseItem = true;
                }
            }
            else if (isChooseItem == false && !isBattle)
            {
                HitAim = null;
                Chooser.SetActive(false);
            }
        }
        //移动到目标(按下鼠标且没有释放技能)
        else if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider == null || hit.collider.gameObject != HitAim)
            {
                if (hit.collider != null && (hit.collider.tag == "IntItem" || hit.collider.tag == "Enemy" || hit.collider.tag == "NPCFriend"))
                {
                    if (UseChooser)
                        Chooser.SetActive(true);
                    HitAim = hit.collider.gameObject;
                }
                else if (!isBattle)
                {
                    isChooseItem = false;
                }
            }
            //自动追踪
            /*else
            {
                if (HitAim.tag == "Enemy" || HitAim.tag == "NPCNeutrality")
                {
                    CharacterAgent.stoppingDistance = FinalCharacterInfos[6];
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
            }*/
        }
        if (HitAim != null)
            Chooser.transform.position = HitAim.transform.position;
    }

    private void SkillController()
    {
        //Skill 1

        for (int SkillIndex = 0; SkillIndex < PlayerUsingSkill.Length; SkillIndex++)
        {
            if (PlayerUsingSkill[SkillIndex] != null)
            {
                if (PlayerMP >= SkillMPCost[SkillIndex]
                    && ((Input.GetKeyDown(KeyCode.Alpha1) && SkillIndex == 0)
                       || (Input.GetKeyDown(KeyCode.Alpha2) && SkillIndex == 1)
                       || (Input.GetKeyDown(KeyCode.Alpha3) && SkillIndex == 2)
                       || (Input.GetKeyDown(KeyCode.Alpha4) && SkillIndex == 3)
                       || (Input.GetKeyDown(KeyCode.Alpha5) && SkillIndex == 4)
                       || (Input.GetKeyDown(KeyCode.Alpha6) && SkillIndex == 5)
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
                        CavasUI.GetComponent<UIController>().isSkillUIMove[SkillIndex] = true;
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
                if (PlayerUsingSkill[SkillIndex].activeSelf)
                {
                    SkillCDdeltaTime[SkillIndex] = 0;
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
                            isSkilling[SkillIndex] = false;
                        }
                        else
                        {
                            isSkillReady[SkillIndex] = false;
                        }
                        PlayerUsedSkillCD[SkillIndex].fillAmount = (SkillCD[SkillIndex] - SkillCDdeltaTime[SkillIndex]) / SkillCD[SkillIndex];
                    }

                }

            }
        }
    }

    public void GenerateData(int ArchiveID)
    {
        CharacterA = new PlayerInfo();
        list.playerInfos.Add(CharacterA);
        //生命
        list.playerInfos[ArchiveID].CharacterInfos[0] = 10;
        //攻击
        list.playerInfos[ArchiveID].CharacterInfos[1] = 1;
        //速度
        list.playerInfos[ArchiveID].CharacterInfos[3] = 5;
        //能量
        list.playerInfos[ArchiveID].CharacterInfos[4] = 20;
        //攻速
        list.playerInfos[ArchiveID].CharacterInfos[5] = 5;
        //攻击范围
        list.playerInfos[ArchiveID].CharacterInfos[6] = 5;
        //能量恢复
        list.playerInfos[ArchiveID].CharacterInfos[7] = 1;
        //暴击率
        list.playerInfos[ArchiveID].CharacterInfos[10] = 10;
        //暴击伤害
        list.playerInfos[ArchiveID].CharacterInfos[11] = 100;
        //击退
        list.playerInfos[ArchiveID].CharacterInfos[13] = 5;
        for (int i = 0; i < CharacterInfos.Length; i++)
        {
            list.playerInfos[ArchiveID].FinalCharacterInfos[i] = list.playerInfos[ArchiveID].CharacterInfos[i];
        }

    }

    //数据保存
    public void SaveData()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            CharacterA.PlayerPosition = gameObject.transform.position;

        for (int i = 0; i < CharacterInfos.Length; i++)
        {
            CharacterA.FinalCharacterInfos[i] = FinalCharacterInfos[i];
            CharacterA.CharacterInfos[i] = CharacterInfos[i];
        }
        CharacterA.GameTime = GameTime;
        CharacterA.EquipmentID = EquipmentID;
        CharacterA.BagItemID = BagItemID;
        CharacterA.BagItemNum = BagItemNum;
        CharacterA.CaseItemID = CaseItemID;
        CharacterA.CaseItemNum = CaseItemNum;
        CharacterA.isSit = isSit;
        CharacterA.EuipPosition = EuipPosition;
        CharacterA.CharacterSpriteID = CharacterSpriteID;
        CharacterA.NewestDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();

        string json = JsonUtility.ToJson(list, true);
        string filepath = Application.streamingAssetsPath + "/PlayerInfo.json";

        using (StreamWriter sw = new StreamWriter(filepath))
        {
            sw.WriteLine(json);
            sw.Close();
            sw.Dispose();
        }
    }

    //数据加载
    public void LoadData(int ArchiveID)
    {
        string json;
        string filepath = Application.streamingAssetsPath + "/PlayerInfo.json";
        // Debug.Log(filepath);
        if (File.Exists(filepath))
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                json = sr.ReadToEnd();
                sr.Close();
            }

            list = JsonUtility.FromJson<PlayerInfoList>(json);
            if (list.playerInfos.Count < ArchiveID + 1)
            {
                GenerateData(ArchiveID);
            }
            else
            {
                CharacterA = list.playerInfos[ArchiveID];
                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Character.GetComponent<CharacterController>().enabled = false;
                    Character.GetComponent<NavMeshAgent>().enabled = false;
                    gameObject.transform.position = CharacterA.PlayerPosition + new Vector3(0, 1, 0);
                    Character.GetComponent<CharacterController>().enabled = true;
                    Character.GetComponent<NavMeshAgent>().enabled = true;
                }

                for (int i = 0; i < CharacterInfos.Length; i++)
                {
                    CharacterInfos[i] = CharacterA.CharacterInfos[i];
                }
                GameTime = CharacterA.GameTime;
                EquipmentID = CharacterA.EquipmentID;
                BagItemID = CharacterA.BagItemID;
                BagItemNum = CharacterA.BagItemNum;
                CaseItemID = CharacterA.CaseItemID;
                CaseItemNum = CharacterA.CaseItemNum;
                isSit = CharacterA.isSit;
                EuipPosition = CharacterA.EuipPosition;
                PlayerEquipmentAll.transform.localPosition = EuipPosition;

            }

        }
        else
        {
            GenerateData(ArchiveID);
        }

    }

    void LoadArchive()
    {
        string json;
        string filepath = Application.streamingAssetsPath + "/ArchiveInfo.json";

        if (File.Exists(filepath))
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                json = sr.ReadToEnd();
                sr.Close();
            }

            //ArchiveID = JsonUtility.FromJson<ArchiveInfoList>(json).ArchiveInfos[0].ArchiveID;

        }
    }

    //碰撞检测
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
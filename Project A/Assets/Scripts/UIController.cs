using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using ExternPropertyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.AI;
using TMPro;
using Unity.Mathematics;


public class UIController : MonoBehaviour
{
    //Player UI
    [HorizontalLine]
    [Header("Player UI")]
    public Image PlayerHPImage;
    public Image PlayerHPImageUI;
    public Image PlayerMPImage;
    public Image PlayerArmorImage;

    [HorizontalLine]
    [Header("Map")]
    public Camera MiniMapCamera;
    public Camera MapCamera;
    public GameObject MapCavas;

    public RenderTexture MapTexture;

    private Vector3 InitialMapCameraRotation, lastMousePosition;

    [HorizontalLine]
    [Header("Coins")]
    public TMP_Text CNum;
    public TMP_Text SNum;
    public TMP_Text GNum;

    public Button ClosePackageButton;

    public GameObject CharacterInfoCamera;

    public GameObject PausePanel, PackagePanel, InfoPanel, AimInfoPanel, DeadPanel, DeadPanelButton, Player;

    private Volume GlobalVolume;

    public Vignette vignette;
    public Image AimImage, AimHPImage;


    public GameObject PlayerOptionInfo;
    public GameObject[] SkillUI;

    public bool[] isSkillUIMove;

    private float PlayerOptionInfoVelocity = 0;

    private float[] SkillUIVelocity;
    private Vector3 PlayerOptionInfoInitialPosition;
    private Vector3 AimInfoPanelInitialPosition;

    private Vector3[] SkillUIInitialPosition;

    private float SkillBarExistTime = 0;

    public bool isPause = false;

    public List<GameObject> isBattleFrom;
    public Image SecneTrans;

    private float SecneTransdeltaTime = 0;

    //箱子控制
    public GameObject CasePanel;
    public GameObject LeftPanel;
    public bool isCase = false;

    //性能释放
    [HorizontalLine]
    [Header("Performance release")]
    public float TickTime = 0.2f;
    private float TickdeltaTime = 0;

    private float SetPackagedeltaTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        PackagePanel.SetActive(true);
        PlayerOptionInfoInitialPosition = PlayerOptionInfo.transform.position;
        AimInfoPanelInitialPosition = AimInfoPanel.transform.position;
        SkillUIInitialPosition = new Vector3[SkillUI.Length];
        SkillUIVelocity = new float[SkillUI.Length];

        isSkillUIMove = new bool[SkillUI.Length];

        for (int i = 0; i < SkillUI.Length; i++)
        {
            SkillUIInitialPosition[i] = SkillUI[i].transform.position;
            SkillUIVelocity[i] = 0;
        }
        //需要更新
        GlobalVolume = GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>();
        GlobalVolume.profile.TryGet<Vignette>(out vignette);
        ClosePackageButton.onClick.AddListener(BagController);

        SecneTransdeltaTime = 0.5f;
        SecneTrans.color = new Color(1, 1, 1, 1);
        SecneTrans.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (SecneTransdeltaTime > 0)
        {
            SecneTransdeltaTime -= Time.deltaTime;
        }
        else if (SecneTransdeltaTime != -1)
        {
            if (SecneTrans.enabled && SecneTrans.color.a > 0.01f)
            {
                SecneTrans.color -= new Color(0, 0, 0, Time.deltaTime * 0.6f / (SecneTrans.color.a + 0.1f));
                if (SecneTrans.color.a < 0.01f)
                {
                    SecneTransdeltaTime = -1;
                    SecneTrans.enabled = false;
                }

            }
        }

        if (SetPackagedeltaTime >= 0 && SetPackagedeltaTime <= 0.5f)
            SetPackagedeltaTime += Time.deltaTime;
        else if (SetPackagedeltaTime > 0.5f)
        {
            SetPackagedeltaTime = -1;
            PackagePanel.SetActive(false);
        }

        if (vignette == null)
        {
            GlobalVolume = GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>();
            GlobalVolume.profile.TryGet<Vignette>(out vignette);
        }
        TickdeltaTime += Time.deltaTime;
        if (TickdeltaTime > TickTime)
        {
            TickdeltaTime = 0;
            SkillBarController();
            AimBarController();
            PlayerInfoBarController();
            MapController();

            if (Input.GetKeyDown(KeyCode.B) && !PausePanel.activeSelf)
                BagController();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseController();
            }
            //DeadPanelController();

            //进入战斗屏幕变红
            if (isBattleFrom.Count != 0)
            {
                SkillBarExistTime = 0;

                if (vignette.color.value.r < 1)
                {
                    vignette.color.value += new Color(Time.deltaTime, 0, 0);
                    vignette.intensity.value += Time.deltaTime * 0.1f;
                }
            }
            else
            {
                if (SkillBarExistTime < 5)
                    SkillBarExistTime += Time.deltaTime;
                if (vignette.color.value.r > 0)
                {
                    vignette.color.value -= new Color(Time.deltaTime, 0, 0);
                    vignette.intensity.value -= Time.deltaTime * 0.1f;
                }

            }
            Player.GetComponent<PlayerController>().isBattle = isBattleFrom.Count != 0;
        }

    }

    /*private void DeadPanelController()
    {
        if (Player.GetComponent<PlayerController>().PlayerHP <= 0)
        {
            DeaddeltaTime += Time.deltaTime;
            MapCavas.SetActive(false);
            InfoPanel.SetActive(false);
            PackagePanel.SetActive(false);

            if (DeaddeltaTime > 2 && !DeadPanel.activeSelf)
            {
                DeadPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                DeadPanel.SetActive(true);
            }
            if (DeaddeltaTime > 3)
            {
                DeadPanelButton.SetActive(true);
            }
        }
        else if (DeadPanel.activeSelf)
        {
            DeaddeltaTime = 0;
            DeadPanel.SetActive(false);
            DeadPanelButton.SetActive(false);
            PackagePanel.SetActive(false);
            MapCavas.SetActive(false);
            InfoPanel.SetActive(true);
        }

        if (DeadPanel.activeSelf)
        {
            if (DeadPanel.GetComponent<Image>().color.a < 0.8f)
                DeadPanel.GetComponent<Image>().color += new Color(0, 0, 0, 1) * Time.deltaTime;
        }
    }*/

    public void BagController()
    {

        CNum.text = Player.GetComponent<PlayerController>().CopperCoin.ToString();
        SNum.text = Player.GetComponent<PlayerController>().SilverCoin.ToString();
        GNum.text = Player.GetComponent<PlayerController>().GoldCoin.ToString();

        //箱子控制
        CasePanel.SetActive(isCase);
        LeftPanel.SetActive(!isCase);

        if (PackagePanel.activeSelf)
            isCase = false;

        PackagePanel.SetActive(!PackagePanel.activeSelf);
        CharacterInfoCamera.SetActive(PackagePanel.activeSelf);
        InfoPanel.SetActive(!PackagePanel.activeSelf);

        Player.GetComponent<PlayerController>().packageController.RefreshClick();
    }
    public void PauseController()
    {

        isPause = !isPause;
        PausePanel.SetActive(isPause);
        PackagePanel.SetActive(false);
        MapCavas.SetActive(false);
        InfoPanel.SetActive(!isPause);
    }
    private void MapController()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MapCavas.SetActive(!MapCavas.activeSelf);
            MapCamera.enabled = MapCavas.activeSelf;
            InfoPanel.SetActive(!MapCavas.activeSelf);
            Player.GetComponent<PlayerController>().CanMove = !MapCavas.activeSelf;

            if (MapCavas.activeSelf)
            {
                InitialMapCameraRotation = MapCamera.transform.eulerAngles;
                MapCamera.transform.eulerAngles = new Vector3(90, 0, 0);
            }
            else
            {
                MapCamera.transform.position = new Vector3(Player.transform.position.x, MapCamera.transform.position.y, Player.transform.position.z);
                MapCamera.transform.eulerAngles = InitialMapCameraRotation;
            }

        }

        //传送

        if (MapCavas.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = MapCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Teleportation Point")
                    {
                        Player.GetComponent<CharacterController>().enabled = false;
                        Player.GetComponent<NavMeshAgent>().enabled = false;
                        Player.transform.position = hit.collider.transform.position;
                        MapCamera.transform.position = new Vector3(Player.transform.position.x, MapCamera.transform.position.y, Player.transform.position.z);
                        Player.GetComponent<CharacterController>().enabled = true;
                        Player.GetComponent<NavMeshAgent>().enabled = true;
                    }

                }
            }


            // 右键拖拽移动
            if (Input.GetMouseButtonDown(1))
            {
                lastMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(1))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                MapCamera.transform.position += new Vector3(-delta.x * 0.2f, 0, -delta.y * 0.2f);
                lastMousePosition = Input.mousePosition;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float newSize = MapCamera.orthographicSize - scroll * 5f;
                MapCamera.orthographicSize = Mathf.Clamp(newSize, 70, 200);
            }
        }
    }
    private void PlayerInfoBarController()
    {
        PlayerArmorImage.fillAmount = Mathf.Lerp(a: PlayerArmorImage.fillAmount, b: Player.GetComponent<PlayerController>().FinalCharacterInfos[12] / Player.GetComponent<PlayerController>().FinalCharacterInfos[0], t: 3 * Time.deltaTime);
        PlayerHPImage.fillAmount = Mathf.Lerp(a: PlayerHPImage.fillAmount, b: Player.GetComponent<PlayerController>().PlayerHP / Player.GetComponent<PlayerController>().FinalCharacterInfos[0], t: 3 * Time.deltaTime);
        PlayerHPImageUI.fillAmount = Mathf.Lerp(a: PlayerHPImageUI.fillAmount, b: Player.GetComponent<PlayerController>().PlayerHP / Player.GetComponent<PlayerController>().FinalCharacterInfos[0], t: 3 * Time.deltaTime);
        PlayerMPImage.fillAmount = Player.GetComponent<PlayerController>().PlayerMP / Player.GetComponent<PlayerController>().FinalCharacterInfos[4];
    }
    private void AimBarController()
    {
        if (Player.GetComponent<PlayerController>().HitAim != null)
        {
            if (Player.GetComponent<PlayerController>().HitAim.tag == "NPCFriend")
            {
                AimHPImage.enabled = true;
                AimHPImage.color = new Color(80f / 255, 150f / 255, 80f / 255, 0.8f);
                AimHPImage.fillAmount = Mathf.Lerp(a: AimHPImage.fillAmount, b: Player.GetComponent<PlayerController>().HitAim.GetComponent<FNPCInfo>().NPCHP / Player.GetComponent<PlayerController>().HitAim.GetComponent<FNPCInfo>().NPCMaxHP, t: 3 * Time.deltaTime);

            }
            else if (Player.GetComponent<PlayerController>().HitAim.tag == "IntItem")
            {
                AimHPImage.enabled = true;
                AimHPImage.color = new Color(200f / 255, 200f / 255, 200f / 255, 0.8f);
            }
            else if (Player.GetComponent<PlayerController>().HitAim.tag == "Enemy")
            {
                AimHPImage.enabled = true;
                AimHPImage.color = new Color(170f / 255, 50f / 255, 50f / 255, 0.8f);
                if (Player.GetComponent<PlayerController>().HitAim != null)
                    AimHPImage.fillAmount = Mathf.Lerp(a: AimHPImage.fillAmount, b: Player.GetComponent<PlayerController>().HitAim.GetComponent<EnemyInfo>().EnemyHP / Player.GetComponent<PlayerController>().HitAim.GetComponent<EnemyInfo>().EnemyMaxHP, t: 3 * Time.deltaTime);
            }
            else if (Player.GetComponent<PlayerController>().HitAim.tag == "NPCNeutrality")
            {
                AimHPImage.enabled = true;
                AimHPImage.color = new Color(230f / 255, 210f / 255, 60f / 255, 0.8f);
                if (Player.GetComponent<PlayerController>().HitAim != null)
                    AimHPImage.fillAmount = Mathf.Lerp(a: AimHPImage.fillAmount, b: Player.GetComponent<PlayerController>().HitAim.GetComponent<EnemyInfo>().EnemyHP / Player.GetComponent<PlayerController>().HitAim.GetComponent<EnemyInfo>().EnemyMaxHP, t: 3 * Time.deltaTime);
            }
            AimImage.enabled = true;
            AimImage.sprite = Player.GetComponent<PlayerController>().HitAim.GetComponent<SpriteRenderer>().sprite;

            AimInfoPanel.transform.position += 10 * (AimInfoPanelInitialPosition - AimInfoPanel.transform.position) * Time.deltaTime;
        }
        else if (AimInfoPanel.transform.position.y < AimInfoPanelInitialPosition.y + 250)
        {
            AimInfoPanel.transform.position += new Vector3(0, (0.1f + 15 * math.abs(AimInfoPanel.transform.position.y - AimInfoPanelInitialPosition.y)) * Time.deltaTime, 0);
            AimImage.enabled = false;
            AimHPImage.enabled = false;
        }

    }
    private void SkillBarController()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (PlayerOptionInfoInitialPosition.y - PlayerOptionInfo.transform.position.y > 250)
            {
                PlayerOptionInfo.transform.position = new Vector3(PlayerOptionInfo.transform.position.x, PlayerOptionInfoInitialPosition.y - 250, PlayerOptionInfo.transform.position.z);
                PlayerOptionInfoVelocity = 20;
            }
            SkillBarExistTime = 0;

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SkillBarExistTime = 10;
            PlayerOptionInfoVelocity = 500;
            for (int i = 0; i < SkillUI.Length; i++)
                SkillUIVelocity[i] = 600;
        }

        if (SkillBarExistTime < 3 && (PlayerOptionInfoVelocity > 3 || math.abs(PlayerOptionInfoInitialPosition.y - PlayerOptionInfo.transform.position.y) > 1.5))
        {
            PlayerOptionInfoVelocity += 100 * (-10 + (PlayerOptionInfoInitialPosition.y - PlayerOptionInfo.transform.position.y)) * Time.deltaTime;
            PlayerOptionInfoVelocity *= 0.975f;
            PlayerOptionInfo.transform.position += new Vector3(0, PlayerOptionInfoVelocity * Time.deltaTime, 0);
        }
        else if (math.abs(PlayerOptionInfoInitialPosition.y - PlayerOptionInfo.transform.position.y) <= 1.5 && SkillBarExistTime < 3 && PlayerOptionInfoVelocity <= 3)
        {
            PlayerOptionInfo.transform.position += (PlayerOptionInfoInitialPosition - PlayerOptionInfo.transform.position) / 4;
        }
        else if (SkillBarExistTime == 10 && PlayerOptionInfo.transform.position.y > PlayerOptionInfoInitialPosition.y - 250)
        {
            PlayerOptionInfoVelocity -= 5000 * Time.deltaTime;
            PlayerOptionInfo.transform.position += new Vector3(0, Time.deltaTime * PlayerOptionInfoVelocity, 0);
        }

        for (int i = 0; i < SkillUI.Length; i++)
        {
            if (isSkillUIMove[i])
            {
                isSkillUIMove[i] = false;
                //SkillUI[i].transform.position = new Vector3(SkillUI[i].transform.position.x, SkillUIInitialPosition[i].y - 250, SkillUI[i].transform.position.z);
                SkillUIVelocity[i] = 500;
            }

            if (SkillBarExistTime < 3 && (SkillUIVelocity[i] > 3 || math.abs(SkillUIInitialPosition[i].y - SkillUI[i].transform.position.y) > 1.5))
            {
                SkillUIVelocity[i] += 100 * (-10 + (SkillUIInitialPosition[i].y - SkillUI[i].transform.position.y)) * Time.deltaTime;
                SkillUIVelocity[i] *= 0.975f;
                SkillUI[i].transform.position += new Vector3(0, SkillUIVelocity[i] * Time.deltaTime, 0);
            }
            else if (SkillBarExistTime < 3 && math.abs(SkillUIInitialPosition[i].y - SkillUI[i].transform.position.y) <= 1.5 && SkillUIVelocity[i] <= 3)
            {
                SkillUI[i].transform.position += (SkillUIInitialPosition[i] - SkillUI[i].transform.position) / 4;
            }
            else if (SkillBarExistTime == 10 && SkillUI[i].transform.position.y > SkillUIInitialPosition[i].y - 250)
            {
                SkillUIVelocity[i] -= 5000 * Time.deltaTime;
                SkillUI[i].transform.position += new Vector3(0, Time.deltaTime * SkillUIVelocity[i], 0);
            }
        }
    }

}

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


public class UIController : MonoBehaviour
{
    //Player UI
    [HorizontalLine]
    [Header("Player UI")]
    public Image PlayerHPImage;
    public Image PlayerMPImage;

    [HorizontalLine]
    [Header("Map")]
    public Camera MiniMapCamera;
    public Camera MapCamera;
    public GameObject MapCavas;

    public RenderTexture MapTexture;

    private Vector3 InitialMapCameraRotation, lastMousePosition;



    public GameObject PausePanel, PackagePanel, InfoPanel, AimInfoPanel, Player;

    public Volume GlobalVolume;

    private Vignette vignette;
    public Image AimImage, AimImageBackGround, AimHPBarLong, AimHPImageLong, AimHPBarShort, AimHPImageShort;

    public Image SkillBar, AimBar;
    public GameObject SkillBarRoller, SkillBarLum, AimBarRoller;

    private Animator SkillBarLumAnimator;

    public float SkillBarRollerValue, AimBarRollerValue;
    private float SkillBarExistTime = 0;
    private int SkillBarLumState = 0;
    public bool isPause = false, isBattle = false;

    // Start is called before the first frame update
    void Start()
    {
        SkillBarLumAnimator = SkillBarLum.GetComponent<Animator>();
        GlobalVolume.profile.TryGet<Vignette>(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        SkillBarController();
        AimBarController();
        PlayerInfoBarController();
        MapController();

        if (isBattle)
        {
            SkillBarExistTime = 0;

            if (vignette.color.value.r < 1)
            {
                vignette.color.value += new Color(Time.deltaTime, 0, 0);
            }
        }
        else
        {
            if (SkillBarExistTime < 5)
                SkillBarExistTime += Time.deltaTime;
            if (vignette.color.value.r > 0)
            {
                vignette.color.value -= new Color(Time.deltaTime, 0, 0);
            }

        }

        if (Input.GetKeyDown(KeyCode.B) && !PausePanel.activeSelf)
        {
            PackagePanel.SetActive(!PackagePanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
            PausePanel.SetActive(isPause);
            PackagePanel.SetActive(false);
            MapCavas.SetActive(false);
            InfoPanel.SetActive(!isPause);
        }



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
                MapCamera.transform.position += new Vector3(-delta.x * 0.1f, 0, -delta.y * 0.1f);
                lastMousePosition = Input.mousePosition;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float newSize = MapCamera.orthographicSize - scroll * 5f;
                MapCamera.orthographicSize = Mathf.Clamp(newSize, 70, 130);
            }
        }
    }
    private void PlayerInfoBarController()
    {
        PlayerHPImage.fillAmount = Mathf.Lerp(a: PlayerHPImage.fillAmount, b: Player.GetComponent<PlayerController>().PlayerHP / Player.GetComponent<PlayerController>().PlayerMaxHP, t: 3 * Time.deltaTime);
        PlayerMPImage.fillAmount = Mathf.Lerp(a: PlayerMPImage.fillAmount, b: Player.GetComponent<PlayerController>().PlayerMP / Player.GetComponent<PlayerController>().PlayerMaxMP, t: 3 * Time.deltaTime);
    }
    private void AimBarController()
    {
        if (Player.GetComponent<PlayerController>().HitAim != null)
        {
            if (Player.GetComponent<PlayerController>().HitAim.tag == "NPCFriend")
            {
                AimHPImageShort.enabled = true;
                AimHPBarShort.enabled = true;
                AimHPImageLong.enabled = false;
                AimHPBarLong.enabled = false;
                AimImageBackGround.enabled = true;
                AimImageBackGround.color = new Color(80f / 255, 150f / 255, 80f / 255, 0.8f);
                if (AimBarRollerValue <= 0.5f)
                {
                    AimBarRollerValue += 3 * Time.deltaTime;
                }
                else if (AimBarRollerValue > 0.5f + 3.5 * Time.deltaTime)
                {
                    AimBarRollerValue -= 3 * Time.deltaTime;
                }
            }
            else if (Player.GetComponent<PlayerController>().HitAim.tag == "IntItem")
            {
                AimHPImageShort.enabled = false;
                AimHPBarShort.enabled = false;
                AimHPImageLong.enabled = false;
                AimHPBarLong.enabled = false;
                if (AimBarRollerValue <= 0.1f)
                {
                    AimBarRollerValue += 2 * Time.deltaTime;
                }
                else if (AimBarRollerValue > 0.1f + 2.5 * Time.deltaTime)
                {
                    AimBarRollerValue -= 2 * Time.deltaTime;
                }
            }
            else if (Player.GetComponent<PlayerController>().HitAim.tag == "Enemy")
            {
                AimHPImageShort.enabled = false;
                AimHPBarShort.enabled = false;
                AimHPImageLong.enabled = true;
                AimHPBarLong.enabled = true;
                AimImageBackGround.enabled = true;
                AimImageBackGround.color = new Color(170f / 255, 50f / 255, 50f / 255, 0.8f);
                AimHPImageLong.fillAmount = Mathf.Lerp(a: AimHPImageLong.fillAmount, b: Player.GetComponent<PlayerController>().HitAim.GetComponent<EnemyInfo>().EnemyHP / Player.GetComponent<PlayerController>().HitAim.GetComponent<EnemyInfo>().EnemyMaxHP, t: 3 * Time.deltaTime);
                if (AimBarRollerValue <= 1)
                {
                    AimBarRollerValue += 4 * Time.deltaTime;
                }
            }
            AimImage.enabled = true;
            AimImage.sprite = Player.GetComponent<PlayerController>().HitAim.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            AimBarRollerValue -= 6 * Time.deltaTime;
            AimHPImageShort.enabled = false;
            AimHPBarShort.enabled = false;
            AimHPImageLong.enabled = false;
            AimHPBarLong.enabled = false;
            AimImage.enabled = false;
            AimImageBackGround.enabled = false;
        }

        if (AimBarRollerValue <= 0.03)
        {
            AimBarRollerValue = 0.03f;
        }
        else if (AimBarRollerValue >= 1)
        {
            AimBarRollerValue = 1f;
        }

        AimBar.fillAmount = AimBarRollerValue;
        AimBar.gameObject.GetComponent<Slider>().value = AimBarRollerValue;
    }
    private void SkillBarController()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SkillBarExistTime = 0;
        }
        if (SkillBarExistTime < 1)
        {
            SkillBarLumState = 1;
            SkillBarRollerValue += 4 * Time.deltaTime;
        }
        else if (SkillBarExistTime >= 5)
        {
            SkillBarLumState = 3;
            SkillBarRollerValue -= 2 * Time.deltaTime;
        }

        if (SkillBarRollerValue <= 0.062)
        {
            SkillBarLumState = 0;
            SkillBarRollerValue = 0.062f;
        }
        else if (SkillBarRollerValue >= 1)
        {
            SkillBarLumState = 2;
            SkillBarRollerValue = 1;
            SkillBarLum.GetComponent<Image>().sprite = SkillBarLum.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            SkillBarRoller.GetComponent<Image>().sprite = SkillBarRoller.GetComponent<SpriteRenderer>().sprite;
            SkillBarLum.GetComponent<Image>().sprite = SkillBarLum.GetComponent<SpriteRenderer>().sprite;
        }
        SkillBarLumAnimator.SetInteger("State", SkillBarLumState);
        SkillBar.fillAmount = SkillBarRollerValue / 1;
        SkillBar.gameObject.GetComponent<Slider>().value = SkillBarRollerValue / 1;
    }
}

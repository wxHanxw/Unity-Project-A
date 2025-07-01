using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public GameObject PausePanel, PackagePanel, InfoPanel, AimInfoPanel, Player;

    public Volume GlobalVolume;

    private Vignette vignette;
    public Image AimImage, AimBackImage;


    public bool isPause = false, isBattle = false;
    // Start is called before the first frame update
    void Start()
    {
        GlobalVolume.profile.TryGet<Vignette>(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        if (isBattle)
        {

            if (vignette.color.value.g > 0)
            {
                vignette.color.value -= new Color(0, Time.deltaTime, Time.deltaTime);
            }
        }
        else
        {
            if (vignette.color.value.g < 1)
            {
                vignette.color.value += new Color(0, Time.deltaTime, Time.deltaTime);
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
            InfoPanel.SetActive(!isPause);
        }

        if (Player.GetComponent<PlayerController>().HitAim != null)
        {
            AimInfoPanel.SetActive(true);
            if (Player.GetComponent<PlayerController>().HitAim.tag == "NPCFriend")
            {
                AimBackImage.color = new Color(150f / 255, 180f / 255, 100f / 255);
            }
            else if (Player.GetComponent<PlayerController>().HitAim.tag == "IntItem")
            {
                AimBackImage.color = new Color(0.9f, 0.9f, 0.9f);
            }
            else if (Player.GetComponent<PlayerController>().HitAim.tag == "Enemy")
            {
                AimBackImage.color = new Color(200f / 255, 80f / 255, 80f / 255);
            }

            AimImage.sprite = Player.GetComponent<PlayerController>().HitAim.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            AimInfoPanel.SetActive(false);
        }

    }
}

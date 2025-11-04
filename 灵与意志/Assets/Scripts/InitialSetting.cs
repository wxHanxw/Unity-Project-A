using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class InitialSetting : MonoBehaviour
{
    private int screenWidth, screenHeight;
    public GameObject AimInfoUI, PlayerInfoUI, AreaInfoUI, PlayerPackageUI, StartPanel, WhiteBox, SecneTrans;
    public GameObject CharacterCamera;
    public GameObject CharacterCanvas;

    public GameObject Character;
    public GameObject CharacterSprite;

    // Start is called before the first frame update

    void Start()
    {
        //屏幕适配
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Vector3 scale;
        if (Screen.width > 2560)
        {
            scale = new Vector3(1, 1, 1) * math.sqrt((float)Screen.width / 2560);
            PlayerPackageUI.transform.localScale = new Vector3(1, 1, 1) * math.pow(scale.x, 1.9f);
            SecneTrans.transform.localScale = new Vector3(1, 1, 1) * math.pow(scale.x, 1.9f);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                StartPanel.transform.localScale = new Vector3(1, 1, 1) * math.pow(scale.x, 1.9f);
                WhiteBox.transform.localScale = new Vector3(WhiteBox.transform.localScale.x * Screen.width / Screen.height / 1.6f, WhiteBox.transform.localScale.y, WhiteBox.transform.localScale.z);
            }
        }
        else
        {
            scale = new Vector3(1, 1, 1) * Screen.width / 2560;
            PlayerPackageUI.transform.localScale = scale;
            SecneTrans.transform.localScale = new Vector3(1, 1, 1) * math.pow(scale.x, 1.9f);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                StartPanel.transform.localScale = scale;
                WhiteBox.transform.localScale = new Vector3(WhiteBox.transform.localScale.x * Screen.width / Screen.height / 1.6f, WhiteBox.transform.localScale.y, WhiteBox.transform.localScale.z);
            }

        }

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            AimInfoUI.transform.localScale = scale;
            PlayerInfoUI.transform.localScale = scale;
            AreaInfoUI.transform.localScale = scale;
        }



        //单例模式
        //if (SceneManager.GetActiveScene().buildIndex != 0)
        //{
        //   if (DontDestroy != null)
        //   {
        //       Destroy(DontDestroy.gameObject);
        //   }
        //   DontDestroy = this;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //补充更新
        //InitialSet();
    }

    //进入场景初始化
    public void InitialSet()
    {
        if (!CharacterCamera.activeSelf && SceneManager.GetActiveScene().buildIndex != 0)
        {
            CharacterCamera.SetActive(true);
            CharacterCanvas.SetActive(true);
            CharacterSprite.GetComponent<SpriteRotator>().enabled = true;
            Character.GetComponent<PlayerController>().enabled = true;
            Character.GetComponent<PlayerController>().isGround = false;
        }
        else if (CharacterCamera.activeSelf && SceneManager.GetActiveScene().buildIndex == 0)
        {
            CharacterCamera.SetActive(false);
            CharacterCanvas.SetActive(false);
            CharacterCanvas.GetComponent<UIController>().PauseController();
            CharacterSprite.transform.eulerAngles = new Vector3(0, 0, 0);
            CharacterSprite.GetComponent<SpriteRotator>().enabled = false;
            Character.GetComponent<PlayerController>().enabled = false;

            //GameObject StartSceneController = GameObject.FindGameObjectWithTag("StartSceneController");
            // Character.transform.position = StartSceneController.GetComponent<StartSceneController>().Characters[CharacterIndex].GetComponent<InitialSetting>().Character.transform.position;
            //Destroy(StartSceneController.GetComponent<StartSceneController>().Character);
            //StartSceneController.GetComponent<StartSceneController>().Character = this.gameObject;
            gameObject.SetActive(false);
        }
    }
}

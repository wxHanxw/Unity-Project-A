using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class InitialSetting : MonoBehaviour
{
    public GameObject CharacterCamera;
    public GameObject CharacterCanvas;

    public GameObject Character;
    public GameObject CharacterSprite;

    private static InitialSetting DontDestroy;

    public int CharacterIndex = 0;
    // Start is called before the first frame update

    void Start()
    {
        //单例模式
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (DontDestroy != null)
            {
                Destroy(DontDestroy.gameObject);
            }
            DontDestroy = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //补充更新
        InitialSet();
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

            GameObject StartSceneController = GameObject.FindGameObjectWithTag("StartSceneController");
            Character.transform.position = StartSceneController.GetComponent<StartSceneController>().Characters[CharacterIndex].GetComponent<InitialSetting>().Character.transform.position;
            Destroy(StartSceneController.GetComponent<StartSceneController>().Characters[CharacterIndex]);
            StartSceneController.GetComponent<StartSceneController>().Characters[CharacterIndex] = this.gameObject;
            gameObject.SetActive(false);
        }
    }
}

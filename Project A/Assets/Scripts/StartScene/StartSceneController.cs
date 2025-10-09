using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

//序列化储存数据
[System.Serializable]
public class ArchiveInfo
{
    public int ArchiveID;
    public bool[] isNewArchive = new bool[5];
    public int[] CharacterBaiscinfo = new int[3];

}

public class ArchiveInfoList
{
    public List<ArchiveInfo> ArchiveInfos = new List<ArchiveInfo>();
}

public class StartSceneController : MonoBehaviour
{
    public ArchiveInfoList list = new ArchiveInfoList();
    public PlayerInfoList listPlayer = new PlayerInfoList();

    public TMP_Text AttackText, HPText, DefenceText, SpeedText;

    public TMP_Text[] DateTimeText;
    public PlayerController playerController;
    public int ArchiveID = 0, ArchiveNum = 0;
    public bool isNew = false;
    public ArchiveInfo Archive;
    public Button NextSceneButton, StartButton, QuitButton, BacktoStartButton;


    public GameObject Camera;
    private Vector3 IntitialCameraRoation;
    public Image Character, WeaponA, Cap, Armor;
    public GameObject CharacterSprites, EquipmentSprites;
    public GameObject StartPanel, ChoosePanel, RighCharacterPage;
    public Image BlackImage;

    public GameObject[] ArchiveButton = new GameObject[5];

    //视觉效果
    public Material skyboxMaterial;
    public Texture2D CursorIcon;
    public UniversalRenderPipelineAsset urpAsset;

    private float targetRotation = 0;
    private bool isChangeScene = false;

    //过场漫画
    public GameObject[] Story;
    // Start is called before the first frame update
    void Start()
    {
        BlackImage.color = new Color(1, 1, 1, 1);

        LoadData();
        for (int i = 0; i < ArchiveNum; i++)
        {
            LoadData();
            ArchiveID += 1;
        }
        //场景性能初始化
        DepthOfField depthOfField;
        GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>().profile.TryGet<DepthOfField>(out depthOfField);
        depthOfField.active = false;
        urpAsset.renderScale = 1.5f;
        Cursor.visible = true;
        int ExposureID = Shader.PropertyToID("_Exposure");
        skyboxMaterial.SetFloat(ExposureID, 1.5f);


        NextSceneButton.onClick.AddListener(NextScene);
        StartButton.onClick.AddListener(StartGame);
        QuitButton.onClick.AddListener(QuitGame);
        BacktoStartButton.onClick.AddListener(BacktoStart);

        IntitialCameraRoation = Camera.transform.eulerAngles;
        Cursor.SetCursor(CursorIcon, new Vector2(0, 0), CursorMode.ForceSoftware);

        UpateArchiveButton();
    }

    // Update is called once per frame
    void Update()
    {
        //存档管理
        if (Input.GetKeyDown(KeyCode.P))
        {
            string jsonPathPlayer = Application.persistentDataPath + "/PlayerInfo.json";
            string jsonPath = Application.persistentDataPath + "/ArchiveInfo.json";
            File.Delete(jsonPathPlayer);
            File.Delete(jsonPath);
        }
        //天空盒旋转
        targetRotation += Time.deltaTime * 2;
        if (targetRotation >= 360)
            targetRotation = 0;
        int rotationID = Shader.PropertyToID("_Rotation");
        //skyboxMaterial.SetFloat(rotationID, targetRotation);

        //相机摇晃
        Vector3 mousePos = new Vector3(-(Input.mousePosition.y / Screen.height - 0.5f), Input.mousePosition.x / Screen.width - 0.5f, 0);
        Camera.transform.eulerAngles = IntitialCameraRoation + 2 * mousePos;



        //场景退出
        if (isChangeScene)
        {
            Cursor.visible = false;
            BlackImage.enabled = true;
            BlackImage.color += new Color(0, 0, 0, Time.deltaTime * 2);
            if (BlackImage.color.a > 0.99f)
            {
                BlackImage.color = new Color(1, 1, 1, 1);
                isChangeScene = false;
                SaveData();
                //场景性能初始化
                DepthOfField depthOfField;
                GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>().profile.TryGet<DepthOfField>(out depthOfField);
                depthOfField.active = false;
                urpAsset.renderScale = 1.1f;
                SceneManager.LoadScene(1);
            }

        }
        //场景进入
        else if (BlackImage.color.a > 0.01f)
        {
            BlackImage.color -= new Color(0, 0, 0, Time.deltaTime * 0.6f / (BlackImage.color.a + 0.1f));
            if (BlackImage.color.a <= 0.01f)
            {
                BlackImage.enabled = false;
            }
        }

        if (Story[0].activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F)))
        {
            for (int i = 1; i < 7; i++)
            {
                if (!Story[i].activeSelf)
                {
                    Story[i].SetActive(true);
                    if (i == 5)
                    {
                        for (int j = 1; j < 5; j++)
                            Story[j].GetComponent<Image>().enabled = false;
                    }
                    break;
                }
                if (i == 6)
                {
                    isChangeScene = true;
                }
            }
        }
    }

    public void UpateArchiveButton()
    {
        for (int i = 0; i < ArchiveNum + 1; i++)
        {
            if (i == ArchiveNum)
            {
                ArchiveButton[i].GetComponent<ArchiveButtonScript>().DeleteArchiveButton.gameObject.SetActive(false);
            }
            else
            {
                ArchiveButton[i].GetComponent<ArchiveButtonScript>().DeleteArchiveButton.gameObject.SetActive(true);
            }
            ArchiveButton[i].SetActive(true);
        }
        for (int i = ArchiveNum + 1; i < 5; i++)
        {
            ArchiveButton[i].SetActive(false);
        }
    }
    public void NextScene()
    {

        //DontDestroyOnLoad(Characters[CharacterIndex]);
        if (isNew)
        {
            Story[0].SetActive(true);
            Story[1].SetActive(true);
        }
        else
            isChangeScene = true;
    }

    public void StartGame()
    {
        StartPanel.SetActive(false);
        ChoosePanel.SetActive(true);
    }

    public void BacktoStart()
    {
        StartPanel.SetActive(true);
        ChoosePanel.SetActive(false);
        RighCharacterPage.SetActive(false);
    }

    public void QuitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }


    void GenerateData()
    {
        Archive = new ArchiveInfo();

        if (list.ArchiveInfos.Count == 0)
            list.ArchiveInfos.Add(Archive);
    }

    void SaveData()
    {
        list.ArchiveInfos[0].ArchiveID = ArchiveID;
        string json = JsonUtility.ToJson(list, true);
        //string filepath = Application.streamingAssetsPath + "/ArchiveInfo.json";
        string filepath = Application.persistentDataPath + "/ArchiveInfo.json";

        using (StreamWriter sw = new StreamWriter(filepath))
        {
            sw.WriteLine(json);
            sw.Close();
            sw.Dispose();
        }

        //playerController.SaveData();
    }

    public void LoadData()
    {
        string json;
        // string filepath = Application.streamingAssetsPath + "/ArchiveInfo.json";
        string filepath = Application.persistentDataPath + "/ArchiveInfo.json";


        string jsonPlayer;
        //string filepathPlayer = Application.streamingAssetsPath + "/PlayerInfo.json";
        string filepathPlayer = Application.persistentDataPath + "/PlayerInfo.json";

        if (File.Exists(filepathPlayer))
        {
            using (StreamReader sr = new StreamReader(filepathPlayer))
            {
                jsonPlayer = sr.ReadToEnd();
                sr.Close();
            }
            if (JsonUtility.FromJson<PlayerInfoList>(jsonPlayer).playerInfos.Count < ArchiveID + 1)
            {
                //playerController.GenerateData(ArchiveID);
                isNew = true;
                using (StreamReader sr = new StreamReader(filepathPlayer))
                {
                    jsonPlayer = sr.ReadToEnd();
                    sr.Close();
                }
                listPlayer = JsonUtility.FromJson<PlayerInfoList>(jsonPlayer);
                if (ArchiveNum > 0)
                {
                    AttackText.text = "1";
                    HPText.text = "10";
                    DefenceText.text = "0";
                    SpeedText.text = "5";
                }
                foreach (Transform child in CharacterSprites.transform)
                {
                    if (child.gameObject.GetComponent<SpriteInfo>().SpriteID == 101)
                    {
                        Character.sprite = child.gameObject.GetComponent<Image>().sprite;
                    }
                }
                WeaponA.enabled = false;
                Armor.enabled = false;
                Cap.enabled = false;
            }
        }
        else
        {
            isNew = true;
            AttackText.text = "1";
            HPText.text = "10";
            DefenceText.text = "0";
            SpeedText.text = "5";

            foreach (Transform child in CharacterSprites.transform)
            {
                if (child.gameObject.GetComponent<SpriteInfo>().SpriteID == 101)
                {
                    Character.sprite = child.gameObject.GetComponent<Image>().sprite;
                }
            }
            WeaponA.enabled = false;
            Armor.enabled = false;
            Cap.enabled = false;
        }

        if (File.Exists(filepath))
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                json = sr.ReadToEnd();
                sr.Close();
            }

            list = JsonUtility.FromJson<ArchiveInfoList>(json);
            Archive = list.ArchiveInfos[0];

            using (StreamReader sr = new StreamReader(filepathPlayer))
            {
                jsonPlayer = sr.ReadToEnd();
                sr.Close();
            }
            listPlayer = JsonUtility.FromJson<PlayerInfoList>(jsonPlayer);
            ArchiveNum = listPlayer.playerInfos.Count;
            if (ArchiveNum > 0 && ArchiveNum > ArchiveID)
            {
                AttackText.text = listPlayer.playerInfos[ArchiveID].FinalCharacterInfos[1].ToString();
                HPText.text = listPlayer.playerInfos[ArchiveID].FinalCharacterInfos[0].ToString();
                DefenceText.text = listPlayer.playerInfos[ArchiveID].FinalCharacterInfos[2].ToString();
                SpeedText.text = listPlayer.playerInfos[ArchiveID].FinalCharacterInfos[3].ToString();

                DateTimeText[ArchiveID].text = listPlayer.playerInfos[ArchiveID].NewestDate;
                foreach (Transform child in CharacterSprites.transform)
                {
                    if (child.gameObject.GetComponent<SpriteInfo>().SpriteID == listPlayer.playerInfos[ArchiveID].CharacterSpriteID)
                    {
                        Character.sprite = child.gameObject.GetComponent<Image>().sprite;
                    }
                }

                WeaponA.enabled = false;
                Armor.enabled = false;
                Cap.enabled = false;
                foreach (Transform child in EquipmentSprites.transform)
                {
                    if (child.gameObject.GetComponent<ItemInfo>().ItemID == listPlayer.playerInfos[ArchiveID].EquipmentID[0])
                    {
                        WeaponA.sprite = child.gameObject.GetComponent<SpriteRenderer>().sprite;
                        WeaponA.enabled = true;
                    }
                    if (child.gameObject.GetComponent<ItemInfo>().ItemID == listPlayer.playerInfos[ArchiveID].EquipmentID[3])
                    {
                        Armor.sprite = child.gameObject.GetComponent<SpriteRenderer>().sprite;
                        Armor.enabled = true;
                    }
                    if (child.gameObject.GetComponent<ItemInfo>().ItemID == listPlayer.playerInfos[ArchiveID].EquipmentID[2])
                    {
                        Cap.sprite = child.gameObject.GetComponent<SpriteRenderer>().sprite;
                        Cap.enabled = true;
                    }
                }

            }

            //Debug.Log(ArchiveNum);
        }
        else
        {
            GenerateData();
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

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
    public ArchiveInfo Archive;
    public Button NextSceneButton, StartButton, QuitButton, BacktoStartButton;

    public Image Character, WeaponA, Cap, Armor;
    public GameObject CharacterSprites, EquipmentSprites;
    public GameObject StartPanel, ChoosePanel, RighCharacterPage;
    public Image BlackImage;

    public GameObject[] ArchiveButton = new GameObject[5];

    private bool isChangeScene = false;
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        for (int i = 0; i < ArchiveNum; i++)
        {
            LoadData();
            ArchiveID += 1;
        }



        NextSceneButton.onClick.AddListener(NextScene);
        StartButton.onClick.AddListener(StartGame);
        QuitButton.onClick.AddListener(QuitGame);
        BacktoStartButton.onClick.AddListener(BacktoStart);
        BlackImage.color = new Color(0, 0, 0, 0);

        UpateArchiveButton();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChangeScene)
        {
            BlackImage.color += new Color(0, 0, 0, Time.deltaTime * 2);
            if (BlackImage.color.a > 0.99f)
            {
                isChangeScene = false;
                SaveData();
                SceneManager.LoadScene(1);
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
        string filepath = Application.streamingAssetsPath + "/ArchiveInfo.json";

        using (StreamWriter sw = new StreamWriter(filepath))
        {
            sw.WriteLine(json);
            sw.Close();
            sw.Dispose();
        }
    }

    public void LoadData()
    {
        string json;
        string filepath = Application.streamingAssetsPath + "/ArchiveInfo.json";

        string jsonPlayer;
        string filepathPlayer = Application.streamingAssetsPath + "/PlayerInfo.json";

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

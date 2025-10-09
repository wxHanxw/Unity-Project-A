using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ArchiveButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button ArchiveButton, DeleteArchiveButton;
    public StartSceneController startSceneController;
    public int ButtonID = 0;

    public PlayerInfoList list = new PlayerInfoList();
    void Start()
    {
        ArchiveButton = gameObject.GetComponent<Button>();
        startSceneController.ArchiveButton[ButtonID] = gameObject;
        ArchiveButton.onClick.AddListener(ArchiveButtonController);
        DeleteArchiveButton.onClick.AddListener(DeleteArchiveButtonController);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ArchiveButtonController()
    {
        startSceneController.RighCharacterPage.SetActive(true);
        startSceneController.ArchiveID = ButtonID;
        startSceneController.LoadData();
    }

    public void DeleteArchiveButtonController()
    {
        startSceneController.isNew = false;
        string jsonPlayer;
        string filepathPlayer = Application.streamingAssetsPath + "/PlayerInfo.json";

        using (StreamReader sr = new StreamReader(filepathPlayer))
        {
            jsonPlayer = sr.ReadToEnd();
            sr.Close();
        }
        list = JsonUtility.FromJson<PlayerInfoList>(jsonPlayer);
        list.playerInfos.Remove(list.playerInfos[ButtonID]);
        Debug.Log(list.playerInfos.Count);

        jsonPlayer = JsonUtility.ToJson(list, true);
        using (StreamWriter sw = new StreamWriter(filepathPlayer))
        {
            sw.WriteLine(jsonPlayer);
            sw.Close();
            sw.Dispose();
        }

        startSceneController.ArchiveNum -= 1;
        startSceneController.ArchiveID = 0;
        for (int i = -1; i < startSceneController.ArchiveNum; i++)
        {
            startSceneController.LoadData();
            startSceneController.ArchiveID += 1;
        }
        startSceneController.DateTimeText[startSceneController.ArchiveNum].text = "新 的 开 始";
        startSceneController.UpateArchiveButton();
        startSceneController.RighCharacterPage.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.IO;


[System.Serializable]
public class SystemInfo
{
    public float MasterVolume = 0.6f;
    public float BGMVolume = 0.6f;
    public float SFXVolume = 0.6f;

}
public class SystemInfoList
{
    public List<SystemInfo> SystemInfos = new List<SystemInfo>();
}
public class SystemController : MonoBehaviour
{
    public SystemInfoList list = new SystemInfoList();
    // Start is called before the first frame update

    public Button QuitButton, MenuButton, AudioButton;
    public Button[] BacktoMainPanelButton;

    public GameObject MainPanel, AudioPanel;

    public AudioMixerGroup MainGroup, BGMGroup, SFXGroup;
    public Slider MainAudioSlider, BGMSlider, SFXSlider;
    //记录场景位置
    private GameObject PlayerPositioninScene;
    private InitialSetting InitialSetting;

    SystemInfo systemInfo;
    void Start()
    {
        LoadData();
        if (SceneManager.GetActiveScene().buildIndex != 0)
            PlayerPositioninScene = GameObject.FindGameObjectWithTag("PlayerPositioninScene");
        QuitButton.onClick.AddListener(QuitGame);
        MenuButton.onClick.AddListener(BacktoMenu);
        AudioButton.onClick.AddListener(AudioButtonController);
        InitialSetting = gameObject.transform.parent.gameObject.GetComponent<InitialSetting>();
        for (int i = 0; i < BacktoMainPanelButton.Length; i++)
        {
            BacktoMainPanelButton[i].onClick.AddListener(BacktoMainPanel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPositioninScene == null && SceneManager.GetActiveScene().buildIndex != 0)
            PlayerPositioninScene = GameObject.FindGameObjectWithTag("PlayerPositioninScene");

        AudioController();
    }

    private void AudioController()
    {
        // 将0-100范围转换为-80到0分贝（Mixer的常用范围）
        float db = 100 * Mathf.Log10(0.1f + MainAudioSlider.value * 0.9f) + 20;
        MainGroup.audioMixer.SetFloat("Master", db);

        db = 100 * Mathf.Log10(0.1f + BGMSlider.value * 0.9f) + 20;
        BGMGroup.audioMixer.SetFloat("BGM", db);

        db = 100 * Mathf.Log10(0.1f + SFXSlider.value * 0.9f) + 20;
        SFXGroup.audioMixer.SetFloat("SFX", db);
    }
    void BacktoMainPanel()
    {
        MainPanel.SetActive(true);
        AudioPanel.SetActive(false);
        SaveData();
    }
    void AudioButtonController()
    {
        MainPanel.SetActive(false);
        AudioPanel.SetActive(true);
    }
    void QuitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void BacktoMenu()
    {
        //DontDestroyOnLoad(gameObject.transform.parent);
        PlayerPositioninScene.transform.position = InitialSetting.Character.transform.position;
        SceneManager.LoadScene(0);
    }


    void GenerateData()
    {
        systemInfo = new SystemInfo();
        list.SystemInfos.Add(systemInfo);
    }

    void SaveData()
    {
        systemInfo.MasterVolume = MainAudioSlider.value;
        systemInfo.BGMVolume = BGMSlider.value;
        systemInfo.SFXVolume = SFXSlider.value;

        string json = JsonUtility.ToJson(list, true);
        string filepath = Application.persistentDataPath + "/SystemInfo.json";

        using (StreamWriter sw = new StreamWriter(filepath))
        {
            sw.WriteLine(json);
            sw.Close();
            sw.Dispose();
        }
    }

    void LoadData()
    {
        string json;
        string filepath = Application.persistentDataPath + "/SystemInfo.json";

        if (File.Exists(filepath))
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                json = sr.ReadToEnd();
                sr.Close();
            }

            list = JsonUtility.FromJson<SystemInfoList>(json);
            systemInfo = list.SystemInfos[0];
            MainAudioSlider.value = systemInfo.MasterVolume;
            BGMSlider.value = systemInfo.BGMVolume;
            SFXSlider.value = systemInfo.SFXVolume;
        }
        else
        {
            GenerateData();
        }

    }
}

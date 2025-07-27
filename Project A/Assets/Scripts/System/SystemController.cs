using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SystemController : MonoBehaviour
{
    // Start is called before the first frame update
    public Button QuitButton, MenuButton;

    //记录场景位置
    private GameObject PlayerPositioninScene;
    private InitialSetting InitialSetting;
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            PlayerPositioninScene = GameObject.FindGameObjectWithTag("PlayerPositioninScene");
        QuitButton.onClick.AddListener(QuitGame);
        MenuButton.onClick.AddListener(BacktoMenu);
        InitialSetting = gameObject.transform.parent.gameObject.GetComponent<InitialSetting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPositioninScene == null && SceneManager.GetActiveScene().buildIndex != 0)
            PlayerPositioninScene = GameObject.FindGameObjectWithTag("PlayerPositioninScene");
    }

    void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void BacktoMenu()
    {
        //DontDestroyOnLoad(gameObject.transform.parent);
        PlayerPositioninScene.transform.position = InitialSetting.Character.transform.position;
        SceneManager.LoadScene(0);
    }
}

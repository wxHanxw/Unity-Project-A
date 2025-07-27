using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    public Button LButton, RButton, NextSceneButton, StartButton, QuitButton, BacktoStartButton;
    private int CharacterIndex = 0;
    public GameObject[] Characters;
    public GameObject StartPanel, ChoosePanel;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if (i == CharacterIndex)
                Characters[i].SetActive(true);
            else
                Characters[i].SetActive(false);
        }

        LButton.onClick.AddListener(LeftOne);
        RButton.onClick.AddListener(RightOne);
        NextSceneButton.onClick.AddListener(NextScene);
        StartButton.onClick.AddListener(StartGame);
        QuitButton.onClick.AddListener(QuitGame);
        BacktoStartButton.onClick.AddListener(BacktoStart);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LeftOne()
    {
        if (Characters[CharacterIndex] != null)
        {
            Characters[CharacterIndex].SetActive(false);
        }
        CharacterIndex -= 1;
        CharacterIndex = (CharacterIndex + Characters.Length) % Characters.Length;
        if (Characters[CharacterIndex] != null)
        {
            Characters[CharacterIndex].SetActive(true);
        }
    }

    public void RightOne()
    {
        if (Characters[CharacterIndex] != null)
        {
            Characters[CharacterIndex].SetActive(false);
        }
        CharacterIndex += 1;
        CharacterIndex = (CharacterIndex + Characters.Length) % Characters.Length;
        if (Characters[CharacterIndex] != null)
        {
            Characters[CharacterIndex].SetActive(true);
        }
    }

    public void NextScene()
    {

        DontDestroyOnLoad(Characters[CharacterIndex]);
        SceneManager.LoadScene(1);
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
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}

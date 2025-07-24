using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    public Button LButton, RButton;
    private int CharacterIndex = 0;
    public GameObject[] Characters;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TaskFinish : MonoBehaviour
{
    // Start is called before the first frame update
    public Image BackIm;
    public TMP_Text Text;

    private int CharacterNum = -2;

    private float CharacterdeltaTime = 0;
    void Start()
    {
        BackIm.gameObject.SetActive(true);
        Text.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (CharacterNum < 18)
        {
            if (BackIm.color.a < 0.999)
            {
                BackIm.color += new Color(0, 0, 0, 1f) * Time.deltaTime;
            }
            else
            {
                CharacterdeltaTime += Time.deltaTime;
                if (CharacterdeltaTime > 0.2f)
                {
                    CharacterdeltaTime = 0;
                    if (CharacterNum == 0)
                    {
                        Text.text = "新";
                    }
                    else if (CharacterNum == 1)
                    {
                        Text.text = "新的";
                    }
                    else if (CharacterNum == 2)
                    {
                        Text.text = "新的冒";
                    }
                    else if (CharacterNum == 3)
                    {
                        Text.text = "新的冒险";
                    }
                    else if (CharacterNum == 4)
                    {
                        Text.text = "新的冒险即";
                    }
                    else if (CharacterNum == 5)
                    {
                        Text.text = "新的冒险即将";
                    }
                    else if (CharacterNum == 6)
                    {
                        Text.text = "新的冒险即将启";
                    }
                    else if (CharacterNum == 7)
                    {
                        Text.text = "新的冒险即将启程";
                    }
                    else if (CharacterNum == 8)
                    {
                        Text.text = "新的冒险即将启程。";
                    }
                    else if (CharacterNum == 9)
                    {
                        Text.text = "新的冒险即将启程。。";
                    }
                    else if (CharacterNum == 10)
                    {
                        Text.text = "新的冒险即将启程。。。";
                    }

                    CharacterNum += 1;
                }

            }
        }
        else
        {
            if (BackIm.color.a > 0.01)
            {
                BackIm.color -= new Color(0, 0, 0, 0.5f) * Time.deltaTime;
                Text.color -= new Color(0, 0, 0, 0.5f) * Time.deltaTime;
            }
            else
            {
                BackIm.gameObject.SetActive(false);
                Text.gameObject.SetActive(false);
                enabled = false;
            }
        }
    }
}

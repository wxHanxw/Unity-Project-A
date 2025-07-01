using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject TakingBar, PreTalkBar;

    public float IntDistance;

    private GameObject Character;
    private bool CanInt;
    // Start is called before the first frame update
    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - Character.transform.position).magnitude < IntDistance)
        {
            CanInt = true;
        }
        else if ((transform.position - Character.transform.position).magnitude > 1.5 * IntDistance)
        {
            CanInt = false;
        }

        if (CanInt)
        {
            PreTalkBar.SetActive(!TakingBar.activeSelf);
            if (Input.GetKeyDown(KeyCode.T))
            {
                TakingBar.SetActive(!TakingBar.activeSelf);
                if (TakingBar.activeSelf)
                {
                    Character.GetComponent<PlayerController>().HitAim = this.gameObject;
                    Character.GetComponent<PlayerController>().isChooseItem = true;
                    Character.GetComponent<PlayerController>().Chooser.SetActive(true);
                }
                else
                {
                    Character.GetComponent<PlayerController>().HitAim = null;
                    Character.GetComponent<PlayerController>().isChooseItem = false;
                    Character.GetComponent<PlayerController>().Chooser.SetActive(false);
                }
            }
        }
        else
        {
            PreTalkBar.SetActive(false);
            TakingBar.SetActive(false);
        }
    }

}

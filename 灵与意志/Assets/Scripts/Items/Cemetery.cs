using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Cemetery : MonoBehaviour
{
    public GameObject Camera;
    public GameObject HintTexture;
    private Vector3 InitialPosition;
    private GameObject Character;
    // Start is called before the first frame update
    void Start()
    {
        HintTexture.SetActive(false);
        InitialPosition = HintTexture.transform.position;
        Character = GameObject.FindGameObjectWithTag("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if (HintTexture.activeSelf)
        {
            HintTexture.transform.position = InitialPosition + 0.1f * new Vector3(0, math.sin(10 * Time.time), 0);
            if (Input.GetKeyDown(KeyCode.F))
            {
                Character.GetComponent<PlayerController>().PlayerHP = Character.GetComponent<PlayerController>().FinalCharacterInfos[0];
                Character.GetComponent<PlayerController>().GhostController();
            }
        }
        //交互提示
        if (Character.GetComponent<PlayerController>().isGhost == true && (transform.position - Character.transform.position).magnitude < 3)
        {
            if (Camera == null)
                Camera = GameObject.FindGameObjectWithTag("MainCamera");
            else
                HintTexture.transform.eulerAngles = new Vector3(Camera.transform.eulerAngles.x - 17, Camera.transform.eulerAngles.y, HintTexture.transform.eulerAngles.z);

            if (!HintTexture.activeSelf)
            {
                Character.GetComponent<PlayerController>().canGetItem.Add(gameObject);
                HintTexture.SetActive(true);
            }

        }
        else if (HintTexture.activeSelf)
        {
            Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
            HintTexture.SetActive(false);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.AI;
public class ChairandDataSave : MonoBehaviour
{
    public Vector3 FixChairPosition;
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
        }

        if (Input.GetKey(KeyCode.F) && HintTexture.activeSelf)
        {
            HintTexture.SetActive(false);
            Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
            Character.GetComponent<CharacterController>().enabled = false;
            Character.transform.position = gameObject.transform.position + FixChairPosition;
            Character.GetComponent<CharacterController>().enabled = true;
            PlayerController playerController = Character.GetComponent<PlayerController>();
            playerController.xzCanMove = false;
            playerController.SitdeltaTime = 2;
            playerController.isSit = true;
            playerController.CharacterWeapon.transform.localPosition = new Vector3(playerController.CharacterWeapon.transform.localPosition.x, playerController.CharacterWeapon.transform.localPosition.y, 0.12f);
            playerController.SaveData();

        }


        //交互提示
        if ((transform.position - Character.transform.position).magnitude < 1.5f && !Character.GetComponent<PlayerController>().isSit && !Character.GetComponent<PlayerController>().isGhost)
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

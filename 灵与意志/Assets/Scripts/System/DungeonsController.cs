using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class DungeonsController : MonoBehaviour
{
    private GameObject Camera;
    public bool NeedRotate = true;
    public GameObject HintTexture;
    private Vector3 InitialPosition;
    private GameObject Character;
    private UIController uIController;
    public int DungeonSceneID = 2;

    // Start is called before the first frame update
    void Start()
    {
        HintTexture.SetActive(false);
        InitialPosition = HintTexture.transform.position;
        Character = GameObject.FindGameObjectWithTag("Character");
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HintTexture.activeSelf)
        {
            HintTexture.transform.position = InitialPosition + 0.1f * new Vector3(0, math.sin(10 * Time.time), 0);
        }

        if (Input.GetKeyDown(KeyCode.F) && HintTexture.activeSelf && Character.GetComponent<PlayerController>().canGetItem.Count == 1 && Character.GetComponent<PlayerController>().canGetItem[0] == gameObject)
        {
            if (SceneManager.GetActiveScene().buildIndex == DungeonSceneID)
            {
                Debug.Log("2");
                LeaveDungeons();
            }
            else
            {
                Debug.Log("1");
                GotoDungeons();
            }
        }

        //交互提示
        if ((transform.position - Character.transform.position).magnitude < 4)
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
            if (HintTexture.activeSelf)
            {
                uIController.PackagePanel.SetActive(false);
                uIController.isCase = false;
                HintTexture.SetActive(false);
            }
        }
    }

    private void GotoDungeons()
    {
        Character.GetComponent<PlayerController>().SaveData();
        Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
        //DontDestroyOnLoad(Character.transform.parent);
        SceneManager.LoadScene(DungeonSceneID);
    }

    private void LeaveDungeons()
    {
        Character.GetComponent<PlayerController>().SaveData();
        Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
        //DontDestroyOnLoad(Character.transform.parent);
        SceneManager.LoadScene(1);
    }
}

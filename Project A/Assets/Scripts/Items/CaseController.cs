using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CaseController : MonoBehaviour
{
    public GameObject Camera;
    public bool NeedRotate = true;
    public GameObject HintTexture;
    private Vector3 InitialPosition;
    private GameObject Character;
    private UIController uIController;

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
            uIController.isCase = true;
            uIController.BagController();
        }

        //交互提示
        if ((transform.position - Character.transform.position).magnitude < 1.5f)
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
}

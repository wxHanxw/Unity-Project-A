using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public GameObject Camera;
    public bool NeedRotate = true;
    public GameObject HintTexture;
    public GameObject Text;
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
            if (NeedRotate)
            {
                if (Camera == null)
                    Camera = GameObject.FindGameObjectWithTag("MainCamera");
                else
                    Text.transform.eulerAngles = new Vector3(Camera.transform.eulerAngles.x - 17, Camera.transform.eulerAngles.y, Text.transform.eulerAngles.z);

            }
            HintTexture.GetComponent<SpriteRenderer>().enabled = false;
            Text.SetActive(true);
        }
        else
        {
            HintTexture.GetComponent<SpriteRenderer>().enabled = true;
            Text.SetActive(false);
        }

        //交互提示
        if ((transform.position - Character.transform.position).magnitude < 3)
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

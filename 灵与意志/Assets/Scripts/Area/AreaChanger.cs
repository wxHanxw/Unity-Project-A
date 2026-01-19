using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AreaChanger : MonoBehaviour
{
    private Image SceneTrans;
    private bool isChangeArea = false, isOnThisTrigger = false;
    private UIController uIController;
    public GameObject ThisArea, TanstoArea;

    private GameObject Character;

    private Vector3 InitialPos;
    private float BlackdeltaTime = 0, ColoraTanser = 0;
    // Start is called before the first frame update
    void Start()
    {
        InitialPos = gameObject.transform.position;
        SceneTrans = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>().SecneTrans;
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        Character = GameObject.FindGameObjectWithTag("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>().isChangeArea && isOnThisTrigger)
        {

            SceneTrans.enabled = true;
            SceneTrans.color += new Color(0, 0, 0, Time.deltaTime * 2f / (ColoraTanser + 0.5f));
            ColoraTanser += Time.deltaTime;

            if (SceneTrans.enabled && SceneTrans.color.a > 0.99f)
            {

                BlackdeltaTime += Time.deltaTime;
                if (BlackdeltaTime > 0.1f)
                {
                    ColoraTanser = 0;
                    gameObject.transform.position = InitialPos;
                    BlackdeltaTime = 0;
                    GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>().isChangeArea = false;
                    TanstoArea.SetActive(true);
                    ThisArea.SetActive(false);
                    isOnThisTrigger = false;
                }

            }

        }
        else if (uIController.SecneTransdeltaTime == -1 && SceneTrans.enabled && SceneTrans.color.a > 0.01f && (Character.transform.position - transform.position).magnitude < 5)
        {

            if (SceneTrans.color.a > 1f)
            {
                SceneTrans.color = new Color(0, 0, 0, 1);
            }
            SceneTrans.color -= new Color(0, 0, 0, Time.deltaTime * 2f / (ColoraTanser + 0.5f));
            ColoraTanser += Time.deltaTime;
            if (SceneTrans.color.a < 0.01f)
            {
                ColoraTanser = 0;
                SceneTrans.enabled = false;
            }

        }
    }

    //碰撞检测
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>().isChangeArea = true;
            isOnThisTrigger = true;
        }
    }
}

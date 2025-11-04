using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaChanger : MonoBehaviour
{
    private Image SceneTrans;
    private bool isChangeArea = false;
    private UIController uIController;
    public GameObject ThisArea, TanstoArea;

    private Vector3 InitialPos;
    private float BlackdeltaTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        InitialPos = gameObject.transform.position;
        SceneTrans = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>().SecneTrans;
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChangeArea)
        {
            SceneTrans.enabled = true;
            SceneTrans.color += new Color(0, 0, 0, Time.deltaTime * 2f / (SceneTrans.color.a + 0.1f));
            if (SceneTrans.enabled && SceneTrans.color.a > 0.99f)
            {
                BlackdeltaTime += Time.deltaTime;
                if (BlackdeltaTime > 0.1f)
                {
                    gameObject.transform.position = InitialPos;
                    BlackdeltaTime = 0;
                    isChangeArea = false;
                    TanstoArea.SetActive(true);
                    ThisArea.SetActive(false);
                }

            }

        }
        else
        {
            if (uIController.SecneTransdeltaTime == -1 && SceneTrans.enabled && SceneTrans.color.a > 0.01f)
            {
                SceneTrans.color -= new Color(0, 0, 0, Time.deltaTime * 2f / (SceneTrans.color.a + 0.1f));
                if (SceneTrans.color.a < 0.01f)
                {
                    SceneTrans.enabled = false;
                }

            }
        }
    }

    //碰撞检测
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            isChangeArea = true;
        }
    }

}

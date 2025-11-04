using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TreasureChest : MonoBehaviour
{
    public GameObject Camera;
    public GameObject HintTexture, TreasureUI, FX;

    public Animator animator;

    public AudioSource AudioOpen, AudioClose;
    private Vector3 InitialPosition;
    private GameObject Character;
    private UIController uIController;

    private float ItemsOutdeltaTime = 0;

    public GameObject[] TreasureItems;

    private bool canInt = true;
    private float OpenIntervaldeltaTime = 0, CloseIntervaldeltaTime = 0;

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


        //拾取物品
        if (Input.GetKeyDown(KeyCode.F) && TreasureUI.activeSelf)
        {
            animator.SetBool("isOpen", false);
            AudioClose.enabled = false;
            AudioClose.enabled = true;
            HintTexture.SetActive(false);
            OpenIntervaldeltaTime = 0;
            CloseIntervaldeltaTime = 0;
            canInt = false;
            FX.SetActive(false);
        }


        if (Input.GetKeyDown(KeyCode.F) && HintTexture.activeSelf && !Character.GetComponent<PlayerController>().isGhost)
        {
            if (!animator.GetBool("isOpen"))
            {
                OpenIntervaldeltaTime = 0;
                animator.SetBool("isOpen", true);
                AudioOpen.enabled = false;
                AudioOpen.enabled = true;
                HintTexture.SetActive(false);
            }
            else
            {
                OpenIntervaldeltaTime = 0.2999f;
            }


        }

        if (OpenIntervaldeltaTime < 0.3f && animator.GetBool("isOpen"))
        {
            OpenIntervaldeltaTime += Time.deltaTime;

        }
        else if (OpenIntervaldeltaTime >= 0.3f && OpenIntervaldeltaTime < 0.6f)
        {
            OpenIntervaldeltaTime += Time.deltaTime;
            ItemsOut();
            TreasureUI.SetActive(true);
        }

        //交互提示
        if (canInt && (transform.position - Character.transform.position).magnitude < 1.5f && !TreasureUI.activeSelf)
        {
            if (Camera == null)
                Camera = GameObject.FindGameObjectWithTag("MainCamera");
            else
                HintTexture.transform.eulerAngles = new Vector3(Camera.transform.eulerAngles.x - 17, Camera.transform.eulerAngles.y, HintTexture.transform.eulerAngles.z);


            if (!HintTexture.activeSelf && !animator.GetBool("isOpen"))
            {
                HintTexture.SetActive(true);
            }

        }
        else if (HintTexture.activeSelf)
        {
            OpenIntervaldeltaTime = 0;
            HintTexture.SetActive(false);
        }
        else if (animator.GetBool("isOpen") && TreasureUI.activeSelf && (transform.position - Character.transform.position).magnitude > 2f)
        {
            animator.SetBool("isOpen", false);
            AudioClose.enabled = false;
            AudioClose.enabled = true;
            HintTexture.SetActive(false);
            OpenIntervaldeltaTime = 0;
            CloseIntervaldeltaTime = 0;
        }

        if (CloseIntervaldeltaTime < 0.1)
        {
            CloseIntervaldeltaTime += Time.deltaTime;
            ItemsIn();
        }
        else if (!animator.GetBool("isOpen") && TreasureUI.activeSelf)
        {
            TreasureUI.SetActive(false);
            for (int i = 0; i < TreasureItems.Length; i++)
            {
                TreasureItems[i].transform.localPosition = new Vector3(0, 0, -0.2f);
                TreasureItems[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

    }

    private void ItemsOut()
    {
        ItemsOutdeltaTime += Time.deltaTime;
        for (int i = 0; i < TreasureItems.Length; i++)
        {
            TreasureItems[i].transform.localPosition += new Vector3((i - ((float)TreasureItems.Length - 1) / 2) / 1.2f, 1 - math.pow(i - ((float)TreasureItems.Length - 1) / 2, 2) / 5, 0) * 0.4f * Time.deltaTime / (OpenIntervaldeltaTime / 30 - 0.009f) / 2;
            TreasureItems[i].transform.eulerAngles += new Vector3(0, 1, -1) * (i - ((float)TreasureItems.Length - 1) / 2) * 0.4f * Time.deltaTime / (OpenIntervaldeltaTime / 30 - 0.009f) / 2;
        }
    }

    private void ItemsIn()
    {
        ItemsOutdeltaTime += Time.deltaTime;
        if (TreasureItems[(TreasureItems.Length - 1) / 2].transform.localPosition.y > 0)
            for (int i = 0; i < TreasureItems.Length; i++)
            {
                TreasureItems[i].transform.localPosition -= new Vector3((i - ((float)TreasureItems.Length - 1) / 2) / 1.2f, 1 - math.pow(i - ((float)TreasureItems.Length - 1) / 2, 2) / 5, 0) * Time.deltaTime / (CloseIntervaldeltaTime / 10 + 0.005f);
                TreasureItems[i].transform.eulerAngles -= new Vector3(0, 1, -1) * (i - ((float)TreasureItems.Length - 1) / 2) * Time.deltaTime / (CloseIntervaldeltaTime / 10 + 0.005f);
            }
    }
}

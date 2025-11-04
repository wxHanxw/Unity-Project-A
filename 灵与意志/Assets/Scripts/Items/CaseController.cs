using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CaseController : MonoBehaviour
{
    public GameObject Camera;
    public bool NeedRotate = true;
    public GameObject HintTexture;

    public Animator animator;

    public AudioSource AudioOpen, AudioClose;
    private Vector3 InitialPosition;
    private GameObject Character;
    private UIController uIController;

    private float OpenIntervaldeltaTime = 0;

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

        //&& Character.GetComponent<PlayerController>().canGetItem.Count == 1 && Character.GetComponent<PlayerController>().canGetItem[0] == gameObject
        if (Input.GetKeyDown(KeyCode.F) && HintTexture.activeSelf && !Character.GetComponent<PlayerController>().isGhost)
        {
            if (!animator.GetBool("isOpen"))
            {
                OpenIntervaldeltaTime = 0;
                animator.SetBool("isOpen", true);
                AudioOpen.enabled = false;
                AudioOpen.enabled = true;
            }
            else
            {
                OpenIntervaldeltaTime = 0.5999f;
            }


        }

        if (OpenIntervaldeltaTime < 0.6f && animator.GetBool("isOpen"))
        {
            OpenIntervaldeltaTime += Time.deltaTime;
            if (OpenIntervaldeltaTime >= 0.6f)
            {
                uIController.isCase = true;
                uIController.BagController();
            }

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
                if (animator.GetBool("isOpen"))
                {
                    uIController.PackagePanel.SetActive(false);
                    animator.SetBool("isOpen", false);
                    AudioClose.enabled = false;
                    AudioClose.enabled = true;
                    OpenIntervaldeltaTime = 0;
                }

                uIController.isCase = false;
                HintTexture.SetActive(false);
            }
        }
    }
}

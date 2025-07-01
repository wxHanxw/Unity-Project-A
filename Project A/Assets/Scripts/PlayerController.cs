using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public GameObject Character, Chooser;

    public GameObject HitAim;
    public float MoveSpeed, RotateSpeed, JumpSpeed;

    private float ySpeed;

    public bool isGround = false, isChooseItem = false;
    private CharacterController CharacterController;

    private Vector3 ChooserVelocity;

    //AI
    private NavMeshAgent CharacterAgent;


    // Start is called before the first frame update
    void Start()
    {
        CharacterController = Character.GetComponent<CharacterController>();
        CharacterAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        MouseInteraction();
    }

    private void Move()
    {
        float MoveDirectionx = Input.GetAxis("Horizontal");
        float MoveDirectiony = Input.GetAxis("Vertical");
        if (MoveDirectionx != 0 || MoveDirectiony != 0)
        {
            CharacterAgent.isStopped = true;
            CharacterController.enabled = true;
        }


        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            ySpeed = JumpSpeed;
        }
        if (!isGround)
            ySpeed -= 0.2f * Time.deltaTime;
        else if (ySpeed < 0)
        {
            ySpeed = 0;
        }


        Vector3 NormVelocity = new Vector3(math.sin(transform.eulerAngles.y / 180 * math.PI), 0, math.cos(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectiony + new Vector3(math.cos(transform.eulerAngles.y / 180 * math.PI), 0, -math.sin(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectionx;
        if (NormVelocity.magnitude > 1)
            NormVelocity = NormVelocity / NormVelocity.magnitude;
        ChooserVelocity = MoveSpeed * Time.deltaTime * NormVelocity;

        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.eulerAngles += new Vector3(0, RotateSpeed, 0);
        }

        if (Input.GetKey(KeyCode.E))
        {
            this.transform.eulerAngles -= new Vector3(0, RotateSpeed, 0);
        }
        if (CharacterController.enabled)
            CharacterController.Move(ChooserVelocity + new Vector3(0, ySpeed, 0));

    }
    private void MouseInteraction()
    {
        if (!isChooseItem)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "IntItem" || hit.collider.tag == "Enemy" || hit.collider.tag == "NPCFriend")
                {
                    Chooser.SetActive(true);
                    HitAim = hit.collider.gameObject;

                    if (Input.GetMouseButtonDown(0))
                    {
                        isChooseItem = true;
                    }
                }
                else if (isChooseItem == false)
                {
                    HitAim = null;
                    Chooser.SetActive(false);
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject != HitAim)
                {
                    if (hit.collider.tag == "IntItem" || hit.collider.tag == "Enemy" || hit.collider.tag == "NPCFriend")
                    {
                        Chooser.SetActive(true);
                        HitAim = hit.collider.gameObject;
                    }
                    else
                    {
                        isChooseItem = false;
                    }
                }
                else
                {
                    CharacterAgent.isStopped = false;
                    CharacterController.enabled = false;
                    CharacterAgent.Warp(transform.position);
                    CharacterAgent.destination = HitAim.transform.position;

                }


            }
        }
        if (HitAim != null)
            Chooser.transform.position = HitAim.transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGround = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGround = false;
        }
    }



}
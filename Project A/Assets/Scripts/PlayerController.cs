using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Character, Chooser;
    public float MoveSpeed, RotateSpeed, JumpSpeed;

    private float ySpeed;

    public bool isGround = false;
    private CharacterController CharacterController;

    private Vector3 ChooserVelocity;


    // Start is called before the first frame update
    void Start()
    {
        CharacterController = Character.GetComponent<CharacterController>();
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
        CharacterController.Move(ChooserVelocity + new Vector3(0, ySpeed, 0));

    }
    private void MouseInteraction()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "IntItem")
            {
                Chooser.SetActive(true);
                Chooser.transform.position = hit.collider.transform.position;
            }
            else
            {
                Chooser.SetActive(false);
            }
        }
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
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Character;
    public float MoveSpeed, RotateSpeed, JumpSpeed;

    private float ySpeed;
    private Rigidbody CharacterRigibody;

    private Vector3 ChooserVelocity;


    // Start is called before the first frame update
    void Start()
    {
        CharacterRigibody = Character.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float MoveDirectionx = Input.GetAxis("Horizontal");
        float MoveDirectiony = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ySpeed = JumpSpeed;
        }
        if (ySpeed > 0)
            ySpeed -= 0.2f * Time.deltaTime;


        Vector3 NormVelocity = new Vector3(math.sin(transform.eulerAngles.y / 180 * math.PI), 0, math.cos(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectiony + new Vector3(math.cos(transform.eulerAngles.y / 180 * math.PI), 0, -math.sin(transform.eulerAngles.y / 180 * math.PI)) * MoveDirectionx;
        if (NormVelocity.magnitude > 1)
            NormVelocity = NormVelocity / NormVelocity.magnitude;
        ChooserVelocity = MoveSpeed * Time.deltaTime * NormVelocity;

        if (Input.GetKey(KeyCode.Q))
        {
            CharacterRigibody.MoveRotation(Quaternion.Euler(transform.eulerAngles - new Vector3(0, RotateSpeed, 0)));
        }

        if (Input.GetKey(KeyCode.E))
        {
            CharacterRigibody.MoveRotation(Quaternion.Euler(transform.eulerAngles + new Vector3(0, RotateSpeed, 0)));
        }
        CharacterRigibody.MovePosition(transform.position + ChooserVelocity + new Vector3(0, ySpeed, 0));

    }
}
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject Character;
    private CinemachineVirtualCamera VirtualCamera;

    private bool inMaze = false, canChangeCamera = true;
    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
        VirtualCamera = Character.GetComponent<PlayerController>().VirtualCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (inMaze)
        {
            if (VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y < 24.99)
            {
                VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Lerp(a: VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y, b: 20, t: 3 * Time.deltaTime);
                VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = Mathf.Lerp(a: VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, b: -5, t: 3 * Time.deltaTime);
                Character.GetComponent<PlayerController>().CameraY = 1;
                Character.GetComponent<PlayerController>().canMoveCamera = false;
                canChangeCamera = true;
            }
        }
        else if (canChangeCamera)
        {
            if (VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y > 9.01)
            {
                VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Lerp(a: VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y, b: 9, t: 3 * Time.deltaTime);
                VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = Mathf.Lerp(a: VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, b: -12, t: 3 * Time.deltaTime);
                Character.GetComponent<PlayerController>().canMoveCamera = true;
            }
            else
            {
                canChangeCamera = false;
            }
        }

    }

    //碰撞检测
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Character")
        {
            inMaze = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Character")
        {
            inMaze = false;
        }
    }
}

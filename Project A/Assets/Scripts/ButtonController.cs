using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class ButtonController : MonoBehaviour
{
    public Button ReviveButton;
    private GameObject Player;
    private GameObject[] Cemetery;
    private float MinCemeteryDistance = 10000;
    private GameObject NearestCemetery;
    // Start is called before the first frame update
    void Start()
    {
        Cemetery = GameObject.FindGameObjectsWithTag("Cemetery");
        Player = GameObject.FindGameObjectWithTag("Character");
        ReviveButton.onClick.AddListener(ReviveButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReviveButtonClick()
    {
        for (int i = 0; i < Cemetery.Length; i++)
        {
            if ((Cemetery[i].transform.position - Player.transform.position).magnitude < MinCemeteryDistance)
            {
                MinCemeteryDistance = (Cemetery[i].transform.position - Player.transform.position).magnitude;
                NearestCemetery = Cemetery[i];
            }
        }

        Player.GetComponent<CharacterController>().enabled = false;
        Player.GetComponent<NavMeshAgent>().enabled = false;
        Player.transform.position = NearestCemetery.transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        Player.GetComponent<NavMeshAgent>().enabled = true;
        Player.GetComponent<PlayerController>().PlayerHP = Player.GetComponent<PlayerController>().PlayerMaxHP;

    }
}

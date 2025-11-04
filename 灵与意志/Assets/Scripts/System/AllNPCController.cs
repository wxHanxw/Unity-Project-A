using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllNPCController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject AllEnemy;
    public GameObject AllNPC;

    private PlayerController playerController;

    private List<GameObject> AllNPCList;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerController>();

        playerController.allNPCController = this;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void NPCGhostUpdate()
    {
        foreach (Transform child in AllNPC.transform)
        {
            if (child.GetComponent<FNPCInfo>().isGhost)
            {
                child.gameObject.SetActive(playerController.isGhost);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int NumberofCoin = 1;

    public GameObject ItemSprite;

    private GameObject Player;
    private float ySpeed = 0;
    private bool isGround = false, isTake = false;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGround)
        {
            transform.eulerAngles += new Vector3((0 - transform.eulerAngles.x) / 3, 0, 0);
            transform.position += new Vector3(0, ySpeed * Time.deltaTime, 0);
            if (ySpeed > -5)
                ySpeed -= 20 * Time.deltaTime;
            transform.eulerAngles += new Vector3(0, 500 * Time.deltaTime, 0);
        }
        else
        {
            transform.eulerAngles += new Vector3((90 - transform.eulerAngles.x) / 3, 0, 0);
        }

        if (Input.GetKey(KeyCode.F) && ySpeed < -2 && isTake)
        {
            Destroy(gameObject);
            if (gameObject.name == "CoinCopper" || gameObject.name == "CoinCopper(Clone)")
            {
                Player.GetComponent<PlayerController>().TotalCoin += NumberofCoin;
                Player.GetComponent<PlayerController>().CoinController();
            }
            else if (gameObject.name == "CoinSilver" || gameObject.name == "CoinSilver(Clone)")
            {
                Player.GetComponent<PlayerController>().TotalCoin += NumberofCoin * 100;
                Player.GetComponent<PlayerController>().CoinController();
            }
            else if (gameObject.name == "CoinGold" || gameObject.name == "CoinGold(Clone)")
            {
                Player.GetComponent<PlayerController>().TotalCoin += NumberofCoin * 10000;
                Player.GetComponent<PlayerController>().CoinController();
            }
            Player.GetComponent<PlayerController>().TakingItemController(ItemSprite.GetComponent<SpriteRenderer>().sprite, NumberofCoin);

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character" && other.GetComponent<PlayerController>().isGhost == false)
        {
            isTake = true;
            isGround = false;
            ySpeed = 5;
        }
        if (other.tag == "Ground" && ySpeed < 0)
        {
            isTake = false;
            isGround = true;
        }
    }

}

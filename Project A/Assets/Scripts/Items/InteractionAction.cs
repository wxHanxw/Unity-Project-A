using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionAction : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject IntParticle;
    public Vector3 PosShift;
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            if ((gameObject.transform.position - other.transform.position).x * math.cos(other.transform.eulerAngles.y / 180 * math.PI) + (gameObject.transform.position - other.transform.position).z * math.sin(other.transform.eulerAngles.y / 180 * math.PI) > 0)
                gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, -15);
            else
                gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 15);

            for (int i = 0; i < 10; i++)
            {
                GameObject Ins = Instantiate(IntParticle, transform.position + PosShift, transform.rotation);
                System.Random random = new System.Random();
                Ins.transform.localScale = new Vector3(1f, 1f, 1f) * 0.1f * ((float)random.NextDouble() / 1.5f + 0.5f);
                Ins.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Character")
        {
            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 0);
        }
    }
}

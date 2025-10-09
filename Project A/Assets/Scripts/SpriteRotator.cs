using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteRotator : MonoBehaviour
{
    private GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (Camera != null)
        {
            transform.eulerAngles = new Vector3(math.max(Camera.transform.eulerAngles.x - 10, 0.01f), Camera.transform.eulerAngles.y, transform.eulerAngles.z);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera == null)
        {
            Camera = GameObject.FindGameObjectWithTag("MainCamera");

        }
        else
        {
            transform.eulerAngles = new Vector3((1 - math.cos(Camera.transform.eulerAngles.x / 180 * math.PI)) * 70, Camera.transform.eulerAngles.y, transform.eulerAngles.z);
        }

    }
}

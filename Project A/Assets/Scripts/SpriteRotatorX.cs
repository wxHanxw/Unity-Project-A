using UnityEngine;

public class SpriteRotatorX : MonoBehaviour
{
    private GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera == null)
            Camera = GameObject.FindGameObjectWithTag("MainCamera");
        else
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}

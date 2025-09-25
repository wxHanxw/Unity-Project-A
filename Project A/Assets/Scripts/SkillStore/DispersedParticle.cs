using UnityEngine;
using Unity.Mathematics;

public class DispersedParticle : MonoBehaviour
{
    public float DispersedSpeed = 0.5f;
    public float ScaleVariateSpeed = 1;
    public float Gravity = 1;
    public bool needRandom = false;
    private Vector3 ParticleDirection;
    public float ParticleSpeed = 0;
    public float ParticleRange = 0;

    public string NotDisdroyName = "DispersedParticle";
    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
        if (needRandom)
        {
            DispersedSpeed = DispersedSpeed * ((float)random.NextDouble() * 0.5f + 0.75f);
            ScaleVariateSpeed = ScaleVariateSpeed * ((float)random.NextDouble() * 0.02f + 0.99f);
        }
        float randomSpeed = (float)random.NextDouble() / 1.5f + 0.5f;
        float randomR = ((float)random.NextDouble() / 1.5f + 0.5f) * ParticleRange;
        float randomalpha = (float)random.NextDouble() * 2 * math.PI;
        ParticleDirection = new Vector3(randomR * math.sin(randomalpha), ParticleSpeed * randomSpeed, randomR * math.cos(randomalpha));
    }

    // Update is called once per frame
    void Update()
    {
        if (name != NotDisdroyName)
        {
            GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 1) * Time.deltaTime * DispersedSpeed;
            transform.localScale = transform.localScale * ScaleVariateSpeed;
            if (ParticleSpeed > 0)
            {
                ParticleDirection.y -= Time.deltaTime * 10 * Gravity;
                transform.position += ParticleDirection * Time.deltaTime;
            }
            if (GetComponent<SpriteRenderer>().color.a < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}

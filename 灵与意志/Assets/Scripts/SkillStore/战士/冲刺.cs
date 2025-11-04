using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class 冲刺 : MonoBehaviour
{
    private float Damage;
    public GameObject PreSkillRange;

    public GameObject DustParticle, DamageParticle;

    public LayerMask targetLayer;

    private float DurationdeltaTime = 0;

    private Vector2 DashVector;
    private Vector3 DashVelocity;

    private GameObject Character;

    private float BeatBack;

    // Start is called before the first frame update
    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
        Damage = (int)(GetComponent<SkillInfo>().Damage * Character.GetComponent<PlayerController>().FinalCharacterInfos[1]);
        GetComponent<SkillInfo>().isRefresh = true;

    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            if (PreSkillRange.activeSelf)
            {
                if ((-gameObject.transform.position + hit.point).x > 0)
                    PreSkillRange.transform.eulerAngles = new Vector3(90, math.acos((-gameObject.transform.position + hit.point).normalized.z) / math.PI * 180, 0);
                else
                {
                    PreSkillRange.transform.eulerAngles = new Vector3(90, 360 - math.acos((-gameObject.transform.position + hit.point).normalized.z) / math.PI * 180, 0);
                }
                DashVector = new Vector2(hit.point.x - gameObject.transform.position.x, hit.point.z - gameObject.transform.position.z).normalized;
            }
        }

        //刷新技能初值
        if (GetComponent<SkillInfo>().isRefresh)
        {
            if ((-gameObject.transform.position + hit.point).x > 0)
                PreSkillRange.transform.eulerAngles = new Vector3(90, math.acos((-gameObject.transform.position + hit.point).normalized.z) / math.PI * 180, 0);
            else
            {
                PreSkillRange.transform.eulerAngles = new Vector3(90, 360 - math.acos((-gameObject.transform.position + hit.point).normalized.z) / math.PI * 180, 0);
            }
            PreSkillRange.SetActive(true);
            GetComponent<SkillInfo>().isRefresh = false;
            DurationdeltaTime = 0;
            gameObject.GetComponent<Collider>().enabled = false;
        }


        GetComponent<SkillInfo>().isPre = PreSkillRange.activeSelf;
        if (Input.GetMouseButtonDown(0) && PreSkillRange.activeSelf)
        {
            PreSkillRange.SetActive(false);
            Character.GetComponent<PlayerController>().xzCanMove = false;
            DashVelocity = 12 * new Vector3(DashVector.x, 0, DashVector.y) * Character.GetComponent<PlayerController>().FinalCharacterInfos[3];
            BeatBack = Character.GetComponent<PlayerController>().FinalCharacterInfos[13];
            gameObject.GetComponent<Collider>().enabled = true;
            foreach (Transform child in Character.GetComponent<PlayerController>().PlayerEquipment[0].transform)
            {
                DurationdeltaTime = 0;
                child.gameObject.SetActive(false);
            }
        }

        if (!PreSkillRange.activeSelf)
        {
            DurationdeltaTime += Time.deltaTime;
            if (DurationdeltaTime > GetComponent<SkillInfo>().Duration)
            {

                foreach (Transform child in Character.GetComponent<PlayerController>().PlayerEquipment[0].transform)
                {
                    child.gameObject.SetActive(true);
                }
                if (Character.GetComponent<PlayerController>().CharacterController.enabled)
                    Character.GetComponent<PlayerController>().ChooserVelocity = new Vector3(0, 0, 0);
                Character.GetComponent<PlayerController>().MainSprite.SetActive(true);
                gameObject.SetActive(false);
            }
            else
            {
                float VariVelocity = -5 / math.pow(GetComponent<SkillInfo>().Duration, 2) * DurationdeltaTime * (DurationdeltaTime - GetComponent<SkillInfo>().Duration);

                GameObject InsDust = Instantiate(DustParticle, Character.transform.position - new Vector3(0, 0.2f, 0), DustParticle.transform.rotation);
                InsDust.transform.localScale = 1.2f * DustParticle.transform.lossyScale * (15 / math.pow(GetComponent<SkillInfo>().Duration, 2) * DurationdeltaTime * math.pow(-DurationdeltaTime + GetComponent<SkillInfo>().Duration, 1.5f));
                InsDust.transform.eulerAngles = PreSkillRange.transform.eulerAngles;
                InsDust.SetActive(true);
                Character.GetComponent<PlayerController>().xzCanMove = true;


                Character.GetComponent<PlayerController>().MainSprite.SetActive(false);
                Character.GetComponent<PlayerController>().GetDamage = 0;
                Character.GetComponent<PlayerController>().PlayerEquipment[0].SetActive(true);
                if (!Character.GetComponent<PlayerController>().isGhost)
                    Character.GetComponent<PlayerController>().CharacterController.enabled = true;
                Character.GetComponent<PlayerController>().CharacterController.Move(DashVelocity * Time.deltaTime * VariVelocity);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "NPCNeutrality")
        {
            other.gameObject.GetComponent<EnemyInfo>().BeAttackedDeriction = (other.transform.position - gameObject.transform.position).normalized * BeatBack * Time.deltaTime * 5;
            other.GetComponent<EnemyInfo>().GetDamage = Damage;
            System.Random random = new System.Random();
            float randomR = (float)random.NextDouble() * 360;
            GameObject DP = Instantiate(DamageParticle, other.transform.position + new Vector3(0, 0.4f, 0), DamageParticle.transform.rotation);
            DP.transform.eulerAngles = new Vector3(randomR, randomR, randomR);
            DP.SetActive(true);
            DP.GetComponent<Animator>().enabled = true;
            DP.GetComponent<SpriteRotator>().enabled = true;
            DP.GetComponent<DispersedParticle>().enabled = true;
        }

    }
}

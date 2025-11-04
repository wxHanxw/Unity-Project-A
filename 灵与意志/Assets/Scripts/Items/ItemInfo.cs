using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExternPropertyAttributes;
public class ItemInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public int ItemID = 0;
    public int ItemNum = 1;

    public bool canUse = true;

    public float ItemSize = 5;
    public float ChooserHight = 0;

    //装备信息
    [HorizontalLine]
    [Header("装备类型: (0)主武器/双手武器 (1)副武器 (2)盔甲 (3)头盔")]
    public int EquipType = 0;
    [Header("装备职业: (0)无职业 (1)战士 (2)骑士 (3)法师 (4)牧师 (5)游侠")]
    public int EquipProfession = 0;

    [Header("攻击类型: (0)近战 (1)远程")]
    public int AttackType = 0;
    public int AttackRange = 1;
    public int AttackSpeed = 10;

    public int BeatBack = 1;

    [HorizontalLine]
    [Header("数值")]
    public float Attack = 0;
    public float Defence = 0;
    public float MaxHP = 0;
    public float MaxMP = 0;
    public float Speed = 0;

    //价格
    public int SellPrice = 10;
    public int BuyPrice = 20;

    //交互性
    public bool canGet = true;
    public bool canForceGet = false;

    public bool canDrop = false;
    public bool isGround = true;
    public Vector3 Velocity;
    public GameObject ForceGetHolder;

    //功能性
    public int HealHP = 0;

    private GameObject Character, InsChooser;

    void Start()
    {
        Character = GameObject.FindGameObjectWithTag("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if (canDrop)
            ItemMove();
        if (Character == null)
        {
            Character = GameObject.FindGameObjectWithTag("Character");
        }
        if (Character.GetComponent<PlayerController>().isGhost == false && canGet && (transform.position - Character.transform.position).magnitude < ItemSize / 3)
        {
            if (InsChooser == null)
            {
                InsChooser = Instantiate(Character.GetComponent<PlayerController>().Chooser, gameObject.transform.position + new Vector3(0, ChooserHight, 0), Character.GetComponent<PlayerController>().Chooser.transform.rotation, transform).gameObject;
                InsChooser.transform.localScale = new Vector3(ItemSize, ItemSize, ItemSize);
                Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
                Character.GetComponent<PlayerController>().canGetItem.Add(gameObject);
                InsChooser.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                GetItem();
            }
        }
        else
        {
            Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
            Destroy(InsChooser);
        }

        if (Character.GetComponent<PlayerController>().isGhost == false && canForceGet && (transform.position - Character.transform.position).magnitude < ItemSize / 3)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Get");
                PriceofForceGet();
                gameObject.tag = "BagItem";
                Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
                Character.GetComponent<PlayerController>().packageController.PutintoBag(gameObject.transform);
                canGet = false;
                canForceGet = false;
                Character.GetComponent<PlayerController>().TakingItemController(GetComponent<SpriteRenderer>().sprite, ItemNum);
            }
        }
    }

    public void GetItem()
    {
        gameObject.tag = "BagItem";
        Character.GetComponent<PlayerController>().canGetItem.Remove(gameObject);
        Character.GetComponent<PlayerController>().packageController.PutintoBag(gameObject.transform);
        canGet = false;
        canDrop = false;
        Character.GetComponent<PlayerController>().TakingItemController(GetComponent<SpriteRenderer>().sprite, ItemNum);

        Character.GetComponent<TaskController>().TaskUpdate();
    }
    private void ItemMove()
    {
        if (!isGround)
            gameObject.transform.position += Velocity * Time.deltaTime;

        if (isGround && Velocity.y < 0)
            Velocity = new Vector3(0, 0, 0);
        else
        {
            Velocity -= new Vector3(0, 1, 0) * Time.deltaTime * 5f;
        }

    }
    //物品功能
    public void ItemFunction()
    {
        if (canUse)
            if (HealHP > 0)
                Heal();
    }

    private void Heal()
    {
        if (Character == null)
        {
            Character = GameObject.FindGameObjectWithTag("Character");
        }
        Character.GetComponent<PlayerController>().GetHeal = HealHP;
    }

    private void PriceofForceGet()
    {
        if (ForceGetHolder != null)
        {
            ForceGetHolder.GetComponent<EnemyInfo>().enabled = true;
            ForceGetHolder.GetComponent<FNPCInfo>().Infos.SetActive(false);
            ForceGetHolder.GetComponent<FNPCInfo>().enabled = false;
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGround = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGround = false;
        }
    }

}

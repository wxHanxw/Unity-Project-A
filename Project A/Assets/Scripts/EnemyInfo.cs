using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInfo : MonoBehaviour
{
    private GameObject Canvas;
    private NavMeshAgent navMeshAgent;
    public float FollowSpeed, IdelIntervalTime, IdelMoveRange, AttackFollowRange, AttackRange;

    private float IdelIntervaldeltaTime = 0, WalkSpeed;

    private Vector3 InitialPosition, MovetoPosition;

    private bool isFollowing = false;

    private GameObject Character;
    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        Character = GameObject.FindGameObjectWithTag("Character");
        InitialPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        WalkSpeed = navMeshAgent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        RandomMove();
        AttackFollowCheck();


    }
    //随机移动
    private void RandomMove()
    {
        IdelIntervaldeltaTime += Time.deltaTime;
        if (!isFollowing && IdelIntervaldeltaTime > IdelIntervalTime)
        {
            navMeshAgent.speed = WalkSpeed;
            IdelIntervaldeltaTime = 0;
            System.Random random = new System.Random();
            float randomR = (float)random.NextDouble() * IdelMoveRange;
            float randomalpha = (float)random.NextDouble() * 2 * math.PI;
            MovetoPosition = new Vector3(randomR * math.sin(randomalpha), 0, randomR * math.cos(randomalpha));

            navMeshAgent.destination = InitialPosition + MovetoPosition;
        }
    }

    private void AttackFollowCheck()
    {
        if ((transform.position - InitialPosition).magnitude < 2 * IdelMoveRange && (transform.position - Character.transform.position).magnitude < AttackFollowRange)
        {
            isFollowing = true;
            navMeshAgent.speed = FollowSpeed;
            navMeshAgent.destination = Character.transform.position;
        }
        else
        {
            isFollowing = false;
        }

        Canvas.GetComponent<UIController>().isBattle = isFollowing;
    }
}

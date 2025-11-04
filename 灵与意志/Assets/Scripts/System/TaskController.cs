using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskController : MonoBehaviour
{
    public List<Task> TakingTasks = new List<Task>();
    public int AimID;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TaskUpdate()
    {
        foreach (Task TakingTask in TakingTasks)
        {
            if (AimID == TakingTask.KillAimID)
            {
                TakingTask.FinishKillAimNum += 1;
                TakingTask.KillNumUpdate();
            }

            if (TakingTask.isCollectTask)
            {
                TakingTask.CollectNumUpdate();
            }
        }
    }

}

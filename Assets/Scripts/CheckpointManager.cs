using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject Player;
    private Stack<GameObject> CheckpointStack = new Stack<GameObject>();

    public void Push(GameObject point)
    {
        Player.GetComponent<FPSController>().SetRespawnPoint(point.GetComponent<Checkpoint>().GetCoordinates());
        CheckpointStack.Push(point);
        point.SetActive(false);
    }

    public void Pop()
    {
        if (CheckpointStack.Count == 0) return;
        var point = CheckpointStack.Pop();
        point.SetActive(true);

        if (CheckpointStack.Count != 0)
        {
            var lastElem = CheckpointStack.Peek();
            Player.GetComponent<FPSController>().SetRespawnPoint(lastElem.GetComponent<Checkpoint>().GetCoordinates());    
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
}
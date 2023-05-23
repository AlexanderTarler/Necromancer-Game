using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MinionController : MonoBehaviour
{
    public MinionMovement[] minions;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f;

            foreach (MinionMovement minion in minions)
            {
                minion.SetTargetPosition(targetPosition);
                minion.IsReturning = false;
                minion.GetComponent<MinionAggro>().enabled = false;
                minion.GetComponent<MinionCombat>().enabled = false;
                minion.enabled = true;
            }
        }
    }
}

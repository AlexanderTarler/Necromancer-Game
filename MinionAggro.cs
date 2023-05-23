using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAggro : MonoBehaviour
{
    public MinionMovement movement;
    public MinionCombat combat;

    [SerializeField]
    private float chargeRange = 10.0f; // The range at which the Minion starts charging
    [SerializeField]
    private float attackRange = 2.0f; // The range at which the Minion can attack

    private bool isCharging = false;
    private bool isAttackCooldown = false;
    public float attackCooldownDuration = 1.0f;
    private float attackCooldownTimer = 0.0f;

    public Transform[] enemyTargets; // Array of enemy targets
    public Transform playerTarget; // Reference to the player target
    private Transform currentEnemy;
    private int currentEnemyIndex = 0;

    private void Update()
    {
        if (!isCharging)
        {
            float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);

            if (isAttackCooldown)
            {
                // Check if there are still enemies in range
                if (enemyTargets.Length == 0)
                {
                    isAttackCooldown = false;
                    movement.IsReturning = true;
                }
            }
            else if (currentEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(currentEnemy.transform.position, transform.position);
                if (distanceToEnemy <= chargeRange)
                {
                    if (distanceToEnemy < attackRange)
                    {
                        AttackEnemy();
                    }
                    else
                    {
                        StartCharge(currentEnemy);
                    }
                }
            }
            else if (currentEnemyIndex < enemyTargets.Length)
            {
                currentEnemy = enemyTargets[currentEnemyIndex];
                StartCharge(currentEnemy);
                currentEnemyIndex++;
            }
            else if (distanceToPlayer <= chargeRange)
            {
                StartCharge(playerTarget);
            }

            UpdateCooldownTimer();
        }
    }

    public void SetTargetEnemies(Transform[] enemyTransforms)
    {
        enemyTargets = enemyTransforms;
    }

    private void StartCharge(Transform target)
    {
        Debug.Log("Charging!");
        isCharging = true;
        movement.IsInRange = true;
        movement.SetTargetPosition(target.position); // Set the target position
    }

    public void AttackEnemy()
    {
        Debug.Log("Attacking!");

        if (currentEnemy != null)
        {
            combat.AttackEnemy();
        }

        isAttackCooldown = true;
        movement.IsInRange = false;

        // Start the attack cooldown timer
        attackCooldownTimer = attackCooldownDuration;

        // Check if there are nearby enemies
        if (enemyTargets.Length > 0 && currentEnemyIndex < enemyTargets.Length)
        {
            currentEnemy = enemyTargets[currentEnemyIndex];
            StartCharge(currentEnemy);
            currentEnemyIndex++;
        }
        else
        {
            currentEnemy = null;
            currentEnemyIndex = 0;
        }
    }

    private void UpdateCooldownTimer()
    {
        if (isAttackCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0.0f)
            {
                isAttackCooldown = false;
            }
        }
    }
}

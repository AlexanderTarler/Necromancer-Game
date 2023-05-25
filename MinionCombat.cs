using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionCombat : MonoBehaviour
{
    public int damage;
    public Flash flash;
    public MinionMovement movement;
    public Transform player;

    private Enemy enemy;

    private List<Enemy> enemiesInRange = new List<Enemy>(); // List of enemies in range

    public bool isAttacking = false;

    private bool isAttackCooldown = false;
    public float attackCooldownDuration = 1.0f;
    private float attackCooldownTimer = 0.0f;

    private void Update()
    {
        UpdateEnemiesInRange(enemiesInRange);
        GetNearestEnemy();

        enemy = GetNearestEnemy();

        if(enemy != null)
        {
            AttackEnemy();
        } else {
            GetNearestEnemy();
        }

        UpdateCooldownTimer();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }

            flash.FlashEffect();
            AttackEnemy();
        }
    }

    public void AttackEnemy()
    {
        Enemy nearestEnemy = GetNearestEnemy();
        if (nearestEnemy != null && nearestEnemy.currentHealth > 0 && !isAttackCooldown)
        {   
            Debug.Log("Attacking!");
            isAttacking = true;
            movement.SetTargetPosition(nearestEnemy.gameObject.transform.position);
            nearestEnemy.TakeDamage(damage);

            attackCooldownTimer = attackCooldownDuration;
            isAttackCooldown = true;
        }

        if (nearestEnemy == null || nearestEnemy.currentHealth <= 0f)
        {   
            isAttacking = false;
            ClearEnemyFromList(nearestEnemy);
            movement.SetTargetPosition(player.position); // might remove

        }
    }

    private Enemy GetNearestEnemy()
    {
        float minDistance = float.MaxValue;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    public void UpdateEnemiesInRange(List<Enemy> updatedEnemies)
    {
        enemiesInRange = updatedEnemies;
    }

    private void ClearEnemyFromList(Enemy killedEnemy)
    {
        if (killedEnemy != null)
        {
            enemiesInRange.Remove(killedEnemy);
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

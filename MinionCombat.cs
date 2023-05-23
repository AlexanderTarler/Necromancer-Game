using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionCombat : MonoBehaviour
{
    public int damage;
    public Flash flash;
    public MinionMovement movement;

    private List<Enemy> enemiesInRange = new List<Enemy>(); // List of enemies in range

    private void Update()
    {
        UpdateEnemiesInRange(enemiesInRange);
        GetNearestEnemy();
    }

    public void OnCollisionEnter2D(Collision2D collision)
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
        if (nearestEnemy != null && nearestEnemy.currentHealth > 0)
        {
            movement.SetTargetPosition(nearestEnemy.gameObject.transform.position);
            nearestEnemy.TakeDamage(damage);
        }

        if (nearestEnemy == null || nearestEnemy.currentHealth < 0.1f)
        {
            ClearEnemyFromList(nearestEnemy);
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
}

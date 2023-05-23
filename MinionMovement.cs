using UnityEngine;
using Pathfinding;

public class MinionMovement : MonoBehaviour
{
    public Transform playerTarget;
    public Transform minionGFX;

    public float followRange = 5f;
    public float speed = 100f;
    public float nextWaypointDistance = 3f;

    public float obstacleAvoidanceTime = 1.5f;
    public float obstacleAvoidanceForce = 200f;

    private bool isAvoidingObstacle = false;
    private float obstacleAvoidanceTimer = 0f;
    private Vector2 avoidanceDirection;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;

    public bool IsInRange { get; set; }
    public bool IsReturning { get; set; }

    public MinionAggro aggro;

    public bool isCharging = false;


    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone() && IsPlayerInRange() && reachedEndOfPath)
        {
            seeker.StartPath(rb.position, playerTarget.position, OnPathComplete);
            reachedEndOfPath = false; // Reset the flag
        }
    }

    public void OnPathComplete(Path p)
{
    if (!p.error)
    {
        path = p;
        currentWaypoint = 0;
        reachedEndOfPath = false;
        isCharging = false; // Reset isCharging when the path is complete
    }
}


    private void Update()
    {

        Debug.Log(isCharging);
        if (!IsPlayerInRange() || isCharging)
    {
        rb.velocity = Vector2.zero;
        return;
    }

        if (path == null || reachedEndOfPath)
        {
            // No path or reached end of path, stop moving
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (isAvoidingObstacle)
        {
            rb.AddForce(avoidanceDirection * obstacleAvoidanceForce * Time.deltaTime);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
            if (currentWaypoint >= path.vectorPath.Count)
            {
                // Reached the end of the path
                reachedEndOfPath = true;
                rb.velocity = Vector2.zero;
                return;
            }
        }

        rb.velocity = force;

        if (force.x >= 0.01f)
        {
            minionGFX.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            minionGFX.localScale = new Vector3(1f, 1f, 1f);
        }

        if (isAvoidingObstacle)
        {
            obstacleAvoidanceTimer -= Time.deltaTime;

            if (obstacleAvoidanceTimer <= 0f)
            {
                isAvoidingObstacle = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isAvoidingObstacle)
        {
            avoidanceDirection = (rb.position - collision.contacts[0].point).normalized;
            isAvoidingObstacle = true;
            obstacleAvoidanceTimer = obstacleAvoidanceTime;
            rb.velocity = -rb.velocity;
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, playerTarget.position) <= followRange;
    }

    public void SetTargetPosition(Vector3 targetPosition)
{
    isCharging = true; // Set isCharging to true when setting a new target position
    seeker.StartPath(rb.position, targetPosition, OnPathComplete);
}
}

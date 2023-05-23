using UnityEngine;
using Pathfinding;

public class Follower : MonoBehaviour
{
    public Transform playerTarget;
    public float followDistance = 1.5f;
    public float speed = 5f;
    public float destinationDelay = 3f;

    private Seeker seeker;
    private Path currentPath;
    private int currentWaypointIndex;
    private bool isFollowing = true;
    private bool isMoving = false;
    private bool hasReachedDestination = false;
    private float destinationTimer = 0f;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
    }

    private void Update()
    {
        if (isFollowing)
        {
            FollowPlayer();
        }
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = 0f;


                isFollowing = false;
                isMoving = true;
                hasReachedDestination = false;
                destinationTimer = 0f;

                FindPath(transform.position, targetPosition);
            }

        if (isMoving)
        {
            if (!hasReachedDestination)
            {
                MoveToDestination();
            }
            else
            {
                destinationTimer += Time.deltaTime;
                if (destinationTimer >= destinationDelay)
                {
                    ResumeFollowing();
                }
            }
        }
        
    }

    private void FollowPlayer()
{
    if (playerTarget != null)
    {
        Vector3 targetPosition = playerTarget.position - (transform.position - playerTarget.position).normalized * followDistance;
        MoveToTarget(targetPosition);
    }
}


    private void MoveToDestination()
    {
        if (currentPath != null)
        {
            if (currentWaypointIndex < currentPath.vectorPath.Count)
            {
                Vector3 targetPosition = currentPath.vectorPath[currentWaypointIndex];
                MoveToTarget(targetPosition);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= currentPath.vectorPath.Count)
                    {
                        hasReachedDestination = true;
                    }
                }
            }
        }
    }

    private void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 velocity = direction * speed * Time.deltaTime;
        transform.position += velocity;
    }

    private void ResumeFollowing()
    {
        isFollowing = true;
        isMoving = false;
    }

    private void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        seeker.StartPath(startPosition, targetPosition, OnPathComplete);
    }

    private void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            currentPath = path;
            currentWaypointIndex = 0;
        }
    }
}


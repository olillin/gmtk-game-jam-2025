using UnityEngine;
using System.Collections.Generic;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool loop = true;

    private int currentPatrolPointIndex = 0;

    void Update()
    {
        if (patrolPoints.Count == 0)
        {
            return;
        }

        Transform target = patrolPoints[currentPatrolPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentPatrolPointIndex++;
            if (currentPatrolPointIndex >= patrolPoints.Count)
            {
                currentPatrolPointIndex = loop ? 0 : patrolPoints.Count - 1;
            }
        }
    }
}
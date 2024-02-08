using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAEnemy : MonoBehaviour
{
    // Start is called before the first frame update
   

    // Update is called once per frame
    enum State
    {
        Patrolling, 
        Chasing
    }
    
    State currentState;

    NavMeshAgent enemyAgent;
    Transform playersTransform;

    [SerializeField] Transform patrolAreaCenter;

    [SerializeField] Vector2 patrolAreaSize;

    [SerializeField] float visionRange = 15;

    [SerializeField] float visionAngle = 90;

    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        playersTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

     void Start()
    {
        currentState = State.Patrolling;
    }

    void Update()
    {
        switch(currentState)
        {
            case State.Patrolling:
                Patrol();
            break;
            case State.Chasing:
                Chase();
            break;
        }
    }
    
    void Patrol()
    {
        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }

        if(enemyAgent.remainingDistance < 0.5f)
        {
            SetRandomPoint();
        }
    }

    void Chase()
    {
        enemyAgent.destination = playersTransform.position;

        if(OnRange() == false)
        {
            currentState = State.Patrolling;
        }
    }

    void SetRandomPoint()
    {
        float randomX = Random.Range(-patrolAreaSize.x / 2, patrolAreaSize.x /2);
        float randomZ = Random.Range(-patrolAreaSize.y / 2, patrolAreaSize.y /2);
        Vector3 randomPoint = new Vector3(randomX, 0f, randomZ) + patrolAreaCenter.position;

        enemyAgent.destination = randomPoint;
    }

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position, playersTransform.position) <= visionRange)
        {
            return true;
        }

        return false;*/

        Vector3 directionToPlayer = playersTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolAreaCenter.position, new Vector3(patrolAreaSize.x, 0, patrolAreaSize.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Vector3 fovLine1 = Quaternion.AngleAxis(visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
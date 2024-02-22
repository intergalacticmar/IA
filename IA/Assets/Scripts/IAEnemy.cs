/*using System.Collections;
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
        Chasing,
        Waiting,
        Attacking
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

        return false;

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
}*/
using UnityEngine;
using System.Collections.Generic;

public class PatrolAI : MonoBehaviour
{
    public List<Transform> patrolPoints;
    private int currentPatrolIndex = 0;

    public float moveSpeed = 5f;
    public float waitTime = 5f; 
    public float chaseRange = 10f;
    public float attackRange = 2f;

    private enum AIState { Patrolling, Waiting, Chasing, Attacking };
    private AIState currentState = AIState.Patrolling;
    private float waitTimer = 0f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Waiting:
                Wait();
                break;
            case AIState.Chasing:
                Chase();
                break;
            case AIState.Attacking:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < chaseRange)
        {
            currentState = AIState.Chasing;
            return;
        }

        if (patrolPoints.Count > 0)
        {
            Transform currentPatrolPoint = patrolPoints[currentPatrolIndex];
            transform.position = Vector3.MoveTowards(transform.position, currentPatrolPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 0.1f)
            {
                currentState = AIState.Waiting;
                waitTimer = waitTime;
            }
        }
    }

    void Wait()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Count)
            {
                currentPatrolIndex = 0;
            }
            currentState = AIState.Patrolling;
        }
    }

    void Chase()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime); 

        if (Vector3.Distance(transform.position, player.position) > chaseRange)
        {
            currentState = AIState.Patrolling; 
        }
        else if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = AIState.Attacking; 
        }
    }

    void Attack()
    {
        Debug.Log("Ataque");

        currentState = AIState.Chasing;
    }
}


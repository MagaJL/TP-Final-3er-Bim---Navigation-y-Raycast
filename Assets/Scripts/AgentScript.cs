using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] float velocity;

    [Header("Patrullaje")]
    [SerializeField] Transform[] patrolPoints;
    public float pointReachedThreshold = 0.5f;
    private int currentPoint = 0;

    [Header("Detección")]
    public float detectionRange = 10f;   // hasta dónde ve
    public float detectionAngle = 45f;   // ángulo de visión
    private Transform player;

    private bool isChasing = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentPoint = 0;
            agent.destination = patrolPoints[currentPoint].position;
        }
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            // Cambia a persecución
            isChasing = true;
            agent.destination = player.position;
        }
        else
        {
            // Si no lo ve, vuelve a patrullar
            if (isChasing)
            {
                isChasing = false;
                GoToNextPoint();
            }

            if (!isChasing && !agent.pathPending && agent.remainingDistance <= pointReachedThreshold)
            {
                GoToNextPoint();
            }
        }

        // Actualiza animación
        velocity = agent.velocity.magnitude;
        anim.SetFloat("Speed", velocity);
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPoint = (currentPoint + 1) % patrolPoints.Length;
        agent.destination = patrolPoints[currentPoint].position;
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (angle < detectionAngle * 0.5f)
            {
                RaycastHit hit;

                // Más alto para que salga desde "la cabeza"
                Vector3 origin = transform.position + Vector3.up * 1.5f;

                // Ignorar la capa del enemigo
                int mask = ~LayerMask.GetMask("NPC");

                if (Physics.Raycast(origin, dirToPlayer, out hit, detectionRange, mask))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 rightDir = Quaternion.Euler(0, detectionAngle * 0.5f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDir * detectionRange);
        Gizmos.DrawRay(transform.position, leftDir * detectionRange);
    }
}


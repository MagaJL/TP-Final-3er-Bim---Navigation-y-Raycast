using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] float velocity;

    [Header("Patrullaje")]
    [Tooltip("Si está vacío, el script seguirá usando targetTR (si lo asignás).")]
    [SerializeField] Transform targetTR;
    public Transform[] patrolPoints;     // <-- asignar en inspector
    public float pointReachedThreshold = 0.5f;
    private int currentPoint = 0;

    [Header("Detección")]
    public float detectionRange = 10f;   // hasta dónde ve
    public float detectionAngle = 45f;   // ángulo de visión
    private Transform player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentPoint = 0;
            targetTR = patrolPoints[currentPoint];
            agent.destination = targetTR.position;
        }
        else if (targetTR != null)
        {
            agent.destination = targetTR.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            // Si llegamos al punto, avanzamos al siguiente (cíclico)
            if (!agent.pathPending && agent.remainingDistance <= pointReachedThreshold)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                targetTR = patrolPoints[currentPoint];
                agent.destination = targetTR.position;
            }
        }
        else if (targetTR != null)
        {
            // comportamiento original: seguir a targetTR
            agent.destination = targetTR.position;
        }
        
        velocity = agent.velocity.magnitude;
        anim.SetFloat("Speed",velocity);

        if (CanSeePlayer())
        {
            Debug.Log("¡Jugador detectado!");
        }
    }

    bool CanSeePlayer()
    {
        // Dirección desde NPC hacia el jugador
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        // Ángulo entre la mirada del NPC y el jugador
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        // Si el jugador está dentro del ángulo y del rango
        if (angle < detectionAngle && Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            RaycastHit hit;
            // El raycast sale desde un poco más arriba para evitar chocar con el suelo
            if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true; // lo ve
                }
            }
        }
        return false; // no lo ve
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 rightDir = Quaternion.Euler(0, detectionAngle, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -detectionAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDir * detectionRange);
        Gizmos.DrawRay(transform.position, leftDir * detectionRange);
    }

}

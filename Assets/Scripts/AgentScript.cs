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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChase : MonoBehaviour
{
    public enum AIState { PATROL = 0, CHASE = 1, ATTACK = 2 };
    private NavMeshAgent ThisAgent;
    private Transform Player = null;
    public AIState CurrentState
    {
        get { return _CurrentState; }
        set
        {
            StopAllCoroutines();
            _CurrentState = value;
            switch (CurrentState)
            {
                case AIState.PATROL:
                    StartCoroutine(StatePatrol());
                    break;

                case AIState.CHASE:
                    StartCoroutine(StateChase());
                    break;

                case AIState.ATTACK:
                    StartCoroutine(StateAttack());
                    break;
            }
        }
    }
    [SerializeField]

    private AIState _CurrentState = AIState.PATROL;
    private void Awake()
    {
        ThisAgent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        CurrentState = AIState.PATROL;
    }

    public void ChangeState(AIState NewState)
    {
        CurrentState = NewState;
    }
    public IEnumerator StateChase()
    {
        float AttackDistance = 2f;

        while (CurrentState == AIState.CHASE)
        {
            if(Vector3.Distance(transform.position, Player.transform.position) < AttackDistance)
            {
                CurrentState = AIState.ATTACK;
                yield break;
            }
            ThisAgent.SetDestination(Player.transform.position);
            yield return null ;
        }
    }

    public IEnumerator StateAttack()
    {
        float AttackDistance = 2f;
        while (CurrentState == AIState.ATTACK)
        {
            if (Vector3.Distance(transform.position, Player.transform.position) > AttackDistance)
            {
                CurrentState = AIState.CHASE;
                yield break;
            }
            print("Attack!");
            ThisAgent.SetDestination(Player.transform.position);
            yield return null;
        }
    }
    void Update()
    {
        ThisAgent.SetDestination(Player.position);
    }

    public IEnumerator StatePatrol()
    {
        GameObject [] Waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        GameObject CurrentWaypoint = Waypoints[Random.Range(0, Waypoints.Length)];
        float TargetDistance = 2f;

        while (CurrentState == AIState.PATROL)
        {
            ThisAgent.SetDestination(CurrentWaypoint.transform.position);
            if (Vector3.Distance(transform.position, CurrentWaypoint.transform.position) < TargetDistance)
            {
                CurrentWaypoint = Waypoints[Random.Range(0, Waypoints.Length)];
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        CurrentState = AIState.CHASE;
    }
}

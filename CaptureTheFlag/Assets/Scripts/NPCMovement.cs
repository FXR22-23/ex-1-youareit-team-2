using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    private float _speed = 0.5f;
    [SerializeField] private float _rotationSpeed = 0.5f;
    [SerializeField] private float _minDistance = 1.5f;
    [SerializeField] float detectionRadius = 5f; // from where will the AI go faster
    [SerializeField] float speedMultiplier = 2;

    private NavMeshAgent _navMeshAgent;
    Transform player;

    public enum MovementType { costume, navmesh };

    public MovementType movementType = MovementType.costume;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _speed = _navMeshAgent.speed;
        player = PlayerMovement.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius) {
            _navMeshAgent.speed = _speed * speedMultiplier;
        }
        else {
            _navMeshAgent.speed = _speed;
        }
        switch (movementType)
        {
            case MovementType.costume:
                CostumeMovement();
                break;
            case MovementType.navmesh:
                NavMeshMovement();
                break;
        }
    }


    private void CostumeMovement()
    {
        var goal = player.position;
        Vector3 realGoal = new Vector3(goal.x, transform.position.y, goal.z);
        Vector3 direction = realGoal - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed);
        Debug.DrawRay(transform.position, direction, Color.green); // To show where AI facing

        if (direction.magnitude >= _minDistance)
        {
            Vector3 pushVector = direction.normalized * _speed;
            transform.Translate(pushVector, Space.World);
        }
        else
        {
            // Animator set enum to "close" 
        }
    }
    private void NavMeshMovement()
    {
        _navMeshAgent.SetDestination(GameManager.Instance.Player.transform.position);
    }
}
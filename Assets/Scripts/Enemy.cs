using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    private enum EnemyState
    {
        idle,
        attacking
    }

    [SerializeField]
    private EnemyState dummyAIState;

    public float minDistance;
    public float maxDistance;

    private GameObject playerReference;

    private Camera mainCamera;
    private Plane[] cFrustum;
    private Collider thisCollider;

    private NavMeshAgent thisAgent;

    private Outline thisOutline;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        thisCollider = this.GetComponent<Collider>();
        thisOutline = this.GetComponent<Outline>();
        thisAgent = this.GetComponent<NavMeshAgent>();
        playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        amIInVision();
        manageEnemyMovement();
    }

    void amIInVision()
    {
        var tempBounds = thisCollider.bounds;
        cFrustum = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        if(GeometryUtility.TestPlanesAABB(cFrustum, tempBounds))
        {
            dummyAIState = EnemyState.attacking;
            thisOutline.enabled = true;
        }
        else
        {
            dummyAIState = EnemyState.idle;
            thisOutline.enabled = false;
        }
    }

    void manageEnemyMovement()
    {
        transform.LookAt(playerReference.transform.position);

        if(Vector3.Distance(transform.position, playerReference.transform.position) > maxDistance)
        {
            thisAgent.SetDestination(playerReference.transform.position);
        }
        else if(Vector3.Distance(transform.position, playerReference.transform.position) < minDistance)
        {
            Vector3 playerDir = transform.position - playerReference.transform.position;
            Vector3 newDestination = transform.position + playerDir;

            thisAgent.SetDestination(newDestination);
        }
        else
        {
            thisAgent.SetDestination(transform.position);
        }
    }
}

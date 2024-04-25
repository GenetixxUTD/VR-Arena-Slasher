using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private Animator modelAnimator;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        thisCollider = this.GetComponent<Collider>();
        thisOutline = this.GetComponent<Outline>();
        thisAgent = this.GetComponent<NavMeshAgent>();
        modelAnimator = this.GetComponent<Animator>();
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
        transform.LookAt(new Vector3(playerReference.transform.position.x, transform.position.y, playerReference.transform.position.z));

        if(Vector3.Distance(transform.position, playerReference.transform.position) > maxDistance)
        {
            thisAgent.SetDestination(playerReference.transform.position);

            modelAnimator.SetBool("forwardswalk", true);
            modelAnimator.SetBool("idle", false);
            modelAnimator.SetBool("backwardswalk", false);
        }
        else if(Vector3.Distance(transform.position, playerReference.transform.position) < minDistance)
        {
            Vector3 playerDir = transform.position - playerReference.transform.position;
            Vector3 newDestination = transform.position + playerDir;

            thisAgent.SetDestination(newDestination);

            modelAnimator.SetBool("forwardswalk", false);
            modelAnimator.SetBool("idle", false);
            modelAnimator.SetBool("backwardswalk", true);
        }
        else
        {
            thisAgent.SetDestination(transform.position);

            modelAnimator.SetBool("forwardswalk", false);
            modelAnimator.SetBool("idle", true);
            modelAnimator.SetBool("backwardswalk", false);
        }
    }
}

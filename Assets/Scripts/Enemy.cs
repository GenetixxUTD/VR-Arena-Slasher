using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    
    private enum EnemyState
    {
        idle,
        attacking,
        stunned
    }

    [SerializeField]
    private EnemyState dummyAIState;

    private EnemyState oldState;

    public float minDistance;
    public float maxDistance;

    private GameObject playerReference;

    private Camera mainCamera;
    private Plane[] cFrustum;
    private Collider thisCollider;

    private NavMeshAgent thisAgent;

    private Outline thisOutline;

    private Animator modelAnimator;

    private int attackingQuadrant; //10 for not attacking

    private Coroutine attackReference;

    private int health;
    public Slider healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        health = 5;
        mainCamera = Camera.main;
        thisCollider = this.GetComponent<Collider>();
        thisOutline = this.GetComponent<Outline>();
        thisAgent = this.GetComponent<NavMeshAgent>();
        modelAnimator = this.GetComponent<Animator>();
        playerReference = GameObject.FindGameObjectWithTag("Player");
        attackingQuadrant = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if(dummyAIState == EnemyState.stunned)
        {
            healthSlider.gameObject.SetActive(true);
        }
        else
        {
            healthSlider.gameObject.SetActive(false);
        }
        healthSlider.value = health;
        if((dummyAIState == EnemyState.stunned || dummyAIState == EnemyState.idle) && attackingQuadrant != 10)
        {
            playerReference.GetComponent<PlayerCombat>().leaveQuadrant(attackingQuadrant);
            StopCoroutine(attackReference);
            playerReference.GetComponent<PlayerCombat>().warningLights[attackingQuadrant].gameObject.SetActive(false);
            attackingQuadrant = 10;
            StartCoroutine("takeABreather");
        }
        if (dummyAIState != EnemyState.stunned)
        {
            amIInVision();
        }
        if (dummyAIState == EnemyState.attacking && attackingQuadrant == 10)
        {
            prepareAttack();
        }
        if(dummyAIState != EnemyState.stunned)
        {
            manageEnemyMovement();
        }

        if(health == 0)
        {
            onDeath();
        }
        
    }

    void amIInVision()
    {
        var tempBounds = thisCollider.bounds;
        cFrustum = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        if(GeometryUtility.TestPlanesAABB(cFrustum, tempBounds))
        {
            if(Vector3.Distance(this.gameObject.transform.position, playerReference.gameObject.transform.position) <= 5f)
            {
                dummyAIState = EnemyState.attacking;
            }
            else
            {
                dummyAIState = EnemyState.idle;
            }
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

    public void prepareAttack()
    {
        int randomint = Random.Range(0, 4);
        if(playerReference.GetComponent<PlayerCombat>().checkQuadrantOccupancy(randomint))
        {
            StartCoroutine("takeABreather");
        }
        else
        {
            playerReference.GetComponent<PlayerCombat>().occupyQuadrant(randomint);
            attackingQuadrant = randomint;
            playerReference.GetComponent<PlayerCombat>().warningLights[randomint].gameObject.SetActive(true);
            attackReference = StartCoroutine("attackRoutine");
        }
    }

    public IEnumerator attackRoutine()
    {
        yield return new WaitForSeconds(5);

        modelAnimator.SetTrigger("attackcall");
        playerReference.GetComponent<PlayerCombat>().warningLights[attackingQuadrant].gameObject.SetActive(false);
        if (playerReference.GetComponent<PlayerCombat>().getSwordQuadrant() != attackingQuadrant)
        {
            playerReference.GetComponent<PlayerCombat>().modifyHealth(-1);
            playerReference.GetComponent<PlayerCombat>().leaveQuadrant(attackingQuadrant);
            
            attackingQuadrant = 10;
            
            StartCoroutine("takeABreather");
        }
        else
        {
            modelAnimator.SetTrigger("stun call");
            dummyAIState = EnemyState.stunned;
            attackingQuadrant = 10;
            StartCoroutine("takeABreather");
        }
    }

    public IEnumerator takeABreather()
    {
        modelAnimator.SetBool("forwardswalk", false);
        modelAnimator.SetBool("idle", true);
        modelAnimator.SetBool("backwardswalk", false);
        thisAgent.SetDestination(transform.position);
        dummyAIState = EnemyState.stunned;
        yield return new WaitForSeconds(Random.Range(3, 6));
        dummyAIState = EnemyState.idle;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "sword" && dummyAIState == EnemyState.stunned)
        {
            health -= 1;
        }
    }

    private void onDeath()
    {
        playerReference.GetComponent<PlayerCombat>().score += 1;
        Destroy(this.gameObject);
    }
}

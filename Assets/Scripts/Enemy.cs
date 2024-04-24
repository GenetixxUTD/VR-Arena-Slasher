using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private enum EnemyState
    {
        idle,
        attacking
    }

    private EnemyState dummyAIState;

    private Camera mainCamera;
    private MeshRenderer thisRenderer;
    private Plane[] cFrustum;
    private Collider thisCollider;

    private Outline thisOutline;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        thisRenderer = this.GetComponent<MeshRenderer>();
        thisCollider = this.GetComponent<Collider>();
        thisOutline = this.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        amIInVision();
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicPortal : MonoBehaviour
{
    public GameObject skeletonPrefab;

    public Transform spawnLocation;

    private bool spawning;

    private void Start()
    {
        spawning = false;
    }

    private void Update()
    {
        if(!spawning)
        {
            StartCoroutine("spawnSkeleton");
            spawning = true;
        }
    }

    private IEnumerator spawnSkeleton()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));

        Instantiate(skeletonPrefab, spawnLocation);
        spawning = false;
    }
}

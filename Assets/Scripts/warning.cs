using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class warning : MonoBehaviour
{
    public TMP_Text warningText;

    private Coroutine warningCoroutine;

    private bool warningCoroutineRunning;

    private void Start()
    {
        warningText = GetComponent<TMP_Text>();
        warningCoroutineRunning = false;
    }
    private void Update()
    {
        if (!warningCoroutineRunning)
        {
            warningText.color = Color.yellow;
            
        }
        warningCoroutine = StartCoroutine("colorRed");
        warningCoroutineRunning = true;
    }

    private void OnDisable()
    {
        StopCoroutine(warningCoroutine);
        warningCoroutineRunning = false;
    }

    private IEnumerator colorRed()
    {
        yield return new WaitForSeconds(3);

        warningText.color = Color.red;
    }
}

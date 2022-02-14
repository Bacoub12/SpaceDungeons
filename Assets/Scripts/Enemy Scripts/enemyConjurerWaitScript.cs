using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyConjurerWaitScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(4f);
        GetComponent<enemyConjurerScript>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

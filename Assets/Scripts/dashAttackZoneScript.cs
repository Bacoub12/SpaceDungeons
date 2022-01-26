using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashAttackZoneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(decay());
    }

    IEnumerator decay()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

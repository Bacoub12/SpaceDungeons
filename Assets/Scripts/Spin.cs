using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    bool rotationComplete;

    // Start is called before the first frame update
    void Start()
    {
        rotationComplete = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationComplete == true)
        {
            StartCoroutine(FullRotation());
        }
    }

    IEnumerator FullRotation()
    {
        rotationComplete = false;

        /*
         * 
        for (int i = 0; i < 360; i++)
        {
            yield return new WaitForSeconds(0.1f);
            //turn 1 degré
            gameObject.transform.Rotate(new Vector3(1f, 0f, 0f));
        }
         */
        for (int i = 0; i < 720; i++)
        {
            yield return new WaitForSeconds(0.025f);
            //turn
            gameObject.transform.Rotate(new Vector3(0.5f, 0f, 0f));
        }

        rotationComplete = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moneyDropScript : MonoBehaviour
{
    public GameObject cash;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name == "EnemyBossCorpse" || gameObject.name == "EnemyBossCorpse(Clone)")
            dropCash(Random.Range(40, 51)); //40 à 50
        else
            dropCash(Random.Range(3, 6)); //3 à 5
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void dropCash(int nbrDrops)
    {
        Vector3 upShift = new Vector3(0f, 0.5f, 0f); //0f, 0.5f, 0f

        for (int i = 0; i < nbrDrops; i++)
        {
            float shiftSize = 1f;
            Vector3 launchVector = new Vector3(Random.Range(-shiftSize, shiftSize), Random.Range(0f, shiftSize), Random.Range(-shiftSize, shiftSize));

            GameObject dropObject = Instantiate(cash, transform.position + upShift, transform.rotation);

            Rigidbody rb = dropObject.GetComponent<Rigidbody>();

            rb.AddForce(launchVector * Random.Range(150f, 250f));
            rb.AddTorque(transform.right * 50f);
            rb.AddTorque(transform.up * 50f);
        }
    }
}

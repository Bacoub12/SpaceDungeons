using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpiderNestScript : MonoBehaviour
{
    public GameObject waitParticles;
    public GameObject spawnParticles;
    public GameObject spider;

    Transform target;
    float radius = 10f;
    bool spawning;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        spawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < radius && spawning == false)
        {
            StartCoroutine(spawnCrab());
        }
    }

    IEnumerator spawnCrab()
    {
        spawning = true;

        GameObject digging = Instantiate(waitParticles, transform.position, Quaternion.identity);
        digging.transform.Rotate(-90f, 0f, 0f, Space.Self);
        yield return new WaitForSeconds(1.5f);
        digging.GetComponent<ParticleSystem>().Stop();

        GameObject dust = Instantiate(spawnParticles, transform.position, Quaternion.identity);
        dust.transform.Rotate(-90f, 0f, 0f, Space.Self);
        Instantiate(spider, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

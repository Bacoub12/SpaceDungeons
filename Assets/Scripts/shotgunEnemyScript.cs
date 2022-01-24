using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class shotgunEnemyScript : MonoBehaviour
{
    public GameObject bullet;
    public Transform shootPoint;

    float lookRadius = 20f;
    Transform target;
    NavMeshAgent agent;
    bool canShoot;
    float cooldownLength = 3f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            Vector3 direction = ((target.position + new Vector3(1.5f, 3f, 0f)) - shootPoint.position).normalized;

            if (distance <= agent.stoppingDistance)
            {
                agent.SetDestination(transform.position);
                FaceTarget(direction);

                if (canShoot)
                {
                    AttackTarget(direction);
                    StartCoroutine(shotCooldown());
                }
            }
        }
    }

    private void FaceTarget(Vector3 _direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void AttackTarget(Vector3 _direction)
    {
        //NOT SHOTGUN-LIKE YET
        for (int i = 0; i < 20; i++)
        {
            float randomX = Random.Range(0f, 20f);
            float randomY = Random.Range(0f, 20f);
            float randomZ = Random.Range(0f, 20f);
            Rigidbody rb = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.transform.forward = _direction;
            rb.transform.Rotate(randomX, randomY, randomZ);
            rb.AddForce(rb.transform.forward * 500f);
        }
    }

    IEnumerator shotCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownLength);
        canShoot = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        /*
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        */
    }
}

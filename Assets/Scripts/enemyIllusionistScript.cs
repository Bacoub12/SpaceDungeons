using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyIllusionistScript : MonoBehaviour
{
    public GameObject bullet;
    public Transform shootPoint;
    public GameObject illusion;

    float lookRadius = 20f;
    Transform target;
    NavMeshAgent agent;
    bool canShoot;
    float shotCooldownLength = 1f;
    bool canIllus;
    float illusionCooldownLength = 2f;
    float illusionRadius = 5f;
    int maxIllusionsPerIllusionist = 4;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        canShoot = true;
        canIllus = true;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            Vector3 to = target.position + new Vector3(0f, 1f, 0f);
            Vector3 from = shootPoint.position;
            Vector3 direction = (to - from).normalized;
            Debug.DrawLine(from, to);

            if (distance <= agent.stoppingDistance)
            {
                agent.SetDestination(transform.position);
                FaceTarget(direction);

                if (canShoot)
                {
                    AttackTarget(direction);
                    StartCoroutine(shotCooldown());
                }

                if (canIllus)
                {
                    GameObject[] illusionists = GameObject.FindGameObjectsWithTag("Illusionist");
                    GameObject[] illusions = GameObject.FindGameObjectsWithTag("Illusion");
                    int totalMaxIllusions = illusionists.Length * maxIllusionsPerIllusionist;

                    if (illusions.Length < totalMaxIllusions)
                    {
                        CreateIllusion();
                        StartCoroutine(illusCooldown());
                    }
                }
            }
        }
    }

    private void FaceTarget(Vector3 _direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        //transform.LookAt(target);
    }

    private void AttackTarget(Vector3 _direction)
    {
        Rigidbody rb = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.transform.forward = _direction;
        rb.AddForce(rb.transform.forward * 750f);
    }

    private void CreateIllusion()
    {
        //coin flip as to whether the original changes place or not
        int teleport = Random.Range(0, 2); //0 ou 1

        if (teleport == 0) //no tp
        {
            Vector3 randomPos = new Vector3(
                transform.position.x + Random.Range(-illusionRadius, illusionRadius),
                transform.position.y,
                transform.position.z + Random.Range(-illusionRadius, illusionRadius));
            Instantiate(illusion, randomPos, Quaternion.identity);
        } else if (teleport == 1) //tp
        {
            Vector3 oldPos = transform.position;
            Vector3 randomPos = new Vector3(
                transform.position.x + Random.Range(-illusionRadius, illusionRadius),
                transform.position.y,
                transform.position.z + Random.Range(-illusionRadius, illusionRadius));
            transform.position = randomPos;
            Instantiate(illusion, oldPos, Quaternion.identity);
        }
    }

    IEnumerator shotCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shotCooldownLength);
        canShoot = true;
    }

    IEnumerator illusCooldown()
    {
        canIllus = false;
        yield return new WaitForSeconds(illusionCooldownLength);
        canIllus = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        /*
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        */
    }
}
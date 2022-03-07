using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyIllusionistScript : MonoBehaviour
{
    public GameObject bullet;
    public GameObject explosion;
    public GameObject gunshotDust;
    public Transform shootPoint;
    public Transform eye;
    public GameObject illusion;
    public GameObject illusionEffect;
    public AudioSource audioShot;

    float lookRadius = 20f;
    Transform target;
    NavMeshAgent agent;
    bool canShoot;
    float shotCooldownLength = 1f;
    bool canIllus;
    float illusionCooldownLength = 2f;
    float illusionRadius = 5f;
    int maxIllusionsPerIllusionist = 4;
    bool dead;
    float health = 120f;
    bool enemyInSight, alerted;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        canShoot = true;
        canIllus = true;
        dead = false;
        enemyInSight = false;
        alerted = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        Vector3 eyePos = eye.position;

        Vector3 vectorToEnemy = (eyePos - target.position) * -1 + new Vector3(0f, 1f, 0f);

        int gunLayerIndex = LayerMask.NameToLayer("Gun");
        int gunLayerMask = (1 << gunLayerIndex);
        gunLayerMask = ~gunLayerMask;

        RaycastHit hit; //= new RaycastHit()
        if (Physics.Raycast(eyePos, vectorToEnemy, out hit, lookRadius, gunLayerMask))
        {
            /*
            Debug.Log("Tag:" + hit.collider.gameObject.tag);
            Debug.Log("Name: " + hit.collider.gameObject.name);
            */
            if (hit.collider.gameObject.tag == "Player") // && Vector3.Angle(eye.forward, vectorToEnemy) <= fieldOfView / 2
            {
                enemyInSight = true;
            }
            else
            {
                enemyInSight = false;
            }
        }
        else
        {
            enemyInSight = false;
        }

        if (distance <= lookRadius && (enemyInSight || alerted))
        {
            agent.SetDestination(target.position);

            alerted = true;

            Vector3 to = target.position + new Vector3(0f, 1f, 0f);
            Vector3 from = shootPoint.position;
            Vector3 direction = (to - from).normalized;
            Debug.DrawLine(from, to);

            if (distance <= agent.stoppingDistance)
            {
                agent.SetDestination(transform.position);
                FaceTarget(direction);

                if (canShoot && Vector3.Angle(eye.forward, vectorToEnemy) <= 20f)
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
        audioShot.Play();
        Rigidbody rb = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        Instantiate(gunshotDust, shootPoint.position, transform.rotation);
        rb.transform.forward = _direction;
        rb.AddForce(rb.transform.forward * 750f);
    }

    private void CreateIllusion()
    {
        //coin flip as to whether the original changes place or not
        int teleport = Random.Range(0, 2); //0 ou 1

        Vector3 downShift = new Vector3(0f, -1f, 0f);

        if (teleport == 0) //no tp
        {
            Vector3 randomPos = new Vector3(
                transform.position.x + Random.Range(-illusionRadius, illusionRadius),
                transform.position.y,
                transform.position.z + Random.Range(-illusionRadius, illusionRadius));
            Instantiate(illusion, randomPos, transform.rotation);
            Instantiate(illusionEffect, randomPos + downShift, Quaternion.identity);
        } else if (teleport == 1) //tp
        {
            Vector3 oldPos = transform.position;
            Vector3 randomPos = new Vector3(
                transform.position.x + Random.Range(-illusionRadius, illusionRadius),
                transform.position.y,
                transform.position.z + Random.Range(-illusionRadius, illusionRadius));
            transform.position = randomPos;
            Instantiate(illusion, oldPos, transform.rotation);
            Instantiate(illusionEffect, oldPos + downShift, Quaternion.identity);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet(Clone)")
        {
            TakeDamage(24); //5 shots to kill
            if (gameObject.GetComponent<NavMeshAgent>().enabled == true)
            {
                agent.SetDestination(target.position);

                alerted = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            dead = true;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;

            Instantiate(explosion, transform.position, transform.rotation);

            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addToScore(20);

            //Invoke(nameof(DestroyThis), 3f);
            DestroyThis();
        }
    }

    public bool checkIfDead()
    {
        return dead;
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
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

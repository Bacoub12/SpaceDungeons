using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text.RegularExpressions;

public class enemyShotgunScript : MonoBehaviour
{
    public GameObject bullet;
    public GameObject explosion;
    public GameObject gunshotDust;
    public Transform shootPoint;
    public Transform eye;
    public AudioSource audioShot;
    [SerializeField] float health;

    float lookRadius = 20f;
    Transform target;
    NavMeshAgent agent;
    bool canShoot;
    float cooldownLength = 3f;
    bool dead;
    bool enemyInSight, alerted;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        canShoot = true;
        dead = false;
        enemyInSight = false;
        alerted = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
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
        for (int i = 0; i < 40; i++)
        {
            float randomX = Random.Range(-20f, 20f);
            float randomY = Random.Range(-20f, 20f);
            float randomZ = Random.Range(-20f, 20f);
            Rigidbody rb = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.transform.forward = _direction;
            rb.transform.Rotate(randomX, randomY, randomZ);
            rb.AddForce(rb.transform.forward * 500f);
        }
        Instantiate(gunshotDust, shootPoint.position, transform.rotation);
    }

    IEnumerator shotCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownLength);
        canShoot = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet(Clone)")
        {
            TakeDamage(other.gameObject.GetComponent<BulletScript>().getTrueDamage());
            if (gameObject.GetComponent<NavMeshAgent>().enabled == true)
            {
                agent.SetDestination(target.position);
                callForAid(target.position);

                alerted = true;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            dead = true;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;

            Instantiate(explosion, transform.position, transform.rotation);

            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addToScore(15);

            //Invoke(nameof(DestroyThis), 3f);
            DestroyThis();
        }
    }

    public void callForAid(Vector3 playerPos)
    {
        string enemyGameObjectRegex = "^Enemy(?!Bullet|SpawnManager)";
        foreach (GameObject GOinScene in FindObjectsOfType<GameObject>())
        {
            if (Regex.IsMatch(GOinScene.name, enemyGameObjectRegex)
                && Vector3.Distance(gameObject.transform.position, GOinScene.transform.position) <= 10f
                && GOinScene != gameObject)
            {
                switch (GOinScene.name)
                {
                    case "EnemyDash":
                    case "EnemyDash(Clone)":
                        GOinScene.GetComponent<Animator>().SetBool("Walk Forward", true);
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyIllusionist":
                    case "EnemyIllusionist(Clone)":
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyMelee":
                    case "EnemyMelee(Clone)":
                        GOinScene.GetComponent<Animation>().Play("Run");
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyRifle":
                    case "EnemyRifle(Clone)":
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyShotgun":
                    case "EnemyShotgun(Clone)":
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemySpider":
                    case "EnemySpider(Clone)":
                        GOinScene.GetComponent<Animator>().SetBool("running", true);
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                }
            }
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

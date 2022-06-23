using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text.RegularExpressions;
using StarterAssets;

public class enemyRifleScript : MonoBehaviour
{
    public GameObject bullet;
    public GameObject explosion;
    public GameObject gunshotDust;
    public Transform shootPoint;
    public Transform eye;
    public AudioSource audioShot;
    public string guyType;
    [SerializeField] float health;

    float lookRadius = 20f;
    Transform target;
    NavMeshAgent agent;
    bool canShoot;
    float cooldownLength;
    bool dead;
    bool enemyInSight, alerted, calling;
    float bulletSpeed = 1000f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        canShoot = true;
        dead = false;
        enemyInSight = false;
        alerted = false;
        calling = false;
        cooldownLength = Random.Range(1f, 2f);
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
            //Debug.DrawLine(from, to);

            if (distance <= agent.stoppingDistance)
            {
                agent.SetDestination(transform.position);
                FaceTarget(direction);

                if (canShoot && Vector3.Angle(eye.forward, vectorToEnemy) <= 35f)
                {
                    //check player position, player velocity, extrapolate to shoot where you will be
                    //https://www.reddit.com/r/Unity3D/comments/do5ymo/how_to_predict_the_future_position_of_a_moving/

                    FirstPersonController FPScomp = target.gameObject.GetComponent<FirstPersonController>();
                    Vector3 playerMotion = FPScomp.motion;
                    Vector3 predictedPlayerPos = predictedPosition(to, from, playerMotion * 1000f, bulletSpeed);
                    Vector3 realDirection = (predictedPlayerPos - from).normalized;

                    /*
                    if (FPScomp.Grounded)
                        realDirection = new Vector3(realDirection.x, 0f, realDirection.z);
                    */

                    AttackTarget(realDirection);
                    StartCoroutine(shotCooldown());
                }
            }
        }
    }

    private Vector3 predictedPosition(Vector3 targetPosition, Vector3 shooterPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 displacement = targetPosition - shooterPosition;
        float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;
        //if the target is stopping or if it is impossible for the projectile to catch up with the target (Sine Formula)
        if (targetVelocity.magnitude == 0 || targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
        {
            Debug.Log("Position prediction is not feasible.");
            return targetPosition;
        }
        //also Sine Formula
        float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
        return targetPosition + targetVelocity * displacement.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
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
        rb.AddForce(rb.transform.forward * bulletSpeed);
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
                StartCoroutine(callForAid(target.position));
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            dead = true;

            Instantiate(explosion, transform.position, transform.rotation);

            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addToScore(10);

            //Invoke(nameof(DestroyThis), 3f);
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            DestroyThis();
        }
    }

    IEnumerator callForAid(Vector3 playerPos)
    {
        if (calling == false)
        {
            calling = true;
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

            yield return new WaitForSeconds(0.5f);
            calling = false;
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

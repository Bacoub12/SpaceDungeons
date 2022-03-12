using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyBossScript : MonoBehaviour
{
    public GameObject dashAttackZone;
    public GameObject slamAttackZone;
    public GameObject bullet;
    public GameObject spawnedEnemy;
    public GameObject dashDustParticles;
    public GameObject dashEnergyParticles;
    public GameObject corpse;
    public Transform shootPoint;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform eye;
    public Animator anim;
    public AudioSource audioShoot;
    public AudioSource audioSlam;
    public AudioSource audioWalk;
    [SerializeField] float health; //1000? 2000?

    float lookRadius = 30f;
    Transform target;
    NavMeshAgent agent;
    bool canAttack, doingAttack;
    float cooldownLength = 4f;
    float dashDistance = 15f;
    float slamDistance = 5f;
    bool dead;
    bool enemyInSight, alerted;
    GameObject existingAttackVisual;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;
        doingAttack = false;
        dead = false;
        enemyInSight = false;
        alerted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (doingAttack == false && dead == false)
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
                alerted = true;

                agent.SetDestination(target.position);
                anim.SetBool("Walk Forward", true);

                if (!audioWalk.isPlaying)
                    audioWalk.Play();
                audioWalk.mute = false;

                Vector3 to = target.position + new Vector3(0f, 1f, 0f);
                Vector3 from = transform.position;
                Vector3 direction = (to - from).normalized;
                Debug.DrawLine(from, to);

                if (distance <= agent.stoppingDistance)
                {
                    agent.SetDestination(transform.position);
                    anim.SetBool("Walk Forward", false);
                    audioWalk.mute = true;
                    FaceTarget(direction);

                    if (canAttack)
                    {
                        int attackChoice;

                        int amntSpawns = GameObject.FindGameObjectsWithTag("BossSpawns").Length;
                        if (amntSpawns < 4)
                            attackChoice = Random.Range(1, 4); //1 à 3
                        else
                            attackChoice = Random.Range(1, 3); //1 à 2

                        switch (attackChoice)
                        {
                            case 1:
                                if (Vector3.Angle(eye.forward, vectorToEnemy) <= 20f)
                                {
                                    StartCoroutine(dash());
                                    StartCoroutine(strikeCooldown());
                                }
                                break;
                            case 2:
                                if (Vector3.Angle(eye.forward, vectorToEnemy) <= 20f)
                                {
                                    StartCoroutine(shoot(transform.forward));
                                    StartCoroutine(strikeCooldown());
                                }
                                break;
                            case 3:
                                StartCoroutine(slam());
                                StartCoroutine(strikeCooldown());
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            if (audioWalk.isPlaying)
                audioWalk.mute = true;
        }
    }

    private void FaceTarget(Vector3 _direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        //transform.LookAt(target);
    }

    IEnumerator dash()
    {
        doingAttack = true;
        canAttack = false;

        //spawn prospective attack zone
        Vector3 centerShift = transform.forward * (dashDistance / 2);
        existingAttackVisual = Instantiate(dashAttackZone, transform.position + centerShift, transform.rotation);

        //wait
        anim.SetBool("Defend", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("Defend", false);
        anim.SetTrigger("Stab Attack");
        yield return new WaitForSeconds(0.5f);

        //attack, teleport to end of strike, deal damage to player eventually

        //check if player is intersecting zone collider
        Vector3 playerPos = target.position;
        if (existingAttackVisual.GetComponent<Collider>().bounds.Contains(playerPos))
        {
            //deal damage
            Debug.Log("hit");
        }

        Destroy(existingAttackVisual);
        GameObject dust = Instantiate(dashDustParticles, transform.position, Quaternion.identity);
        dust.transform.Rotate(-90f, 0f, 0f, Space.Self);
        GameObject trail = Instantiate(dashEnergyParticles, transform.position + centerShift, transform.rotation);
        trail.transform.Rotate(0f, 180f, 0f, Space.Self);

        transform.position = transform.position + (transform.forward * dashDistance);

        //wait
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(1.5f);

        //resume walking around
        doingAttack = false;
    }

    IEnumerator slam()
    {
        doingAttack = true;
        canAttack = false;

        //spawn prospective attack zone
        Vector3 centerShift = transform.forward * (slamDistance / 2);
        existingAttackVisual = Instantiate(slamAttackZone, transform.position + centerShift, transform.rotation);

        //wait
        anim.SetBool("Defend", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Defend", false);
        anim.SetTrigger("Smash Attack");
        yield return new WaitForSeconds(0.6f);
        audioSlam.Play();
        yield return new WaitForSeconds(0.2f);

        //attack, deal damage to player eventually

        //check if player is intersecting zone collider
        Vector3 playerPos = target.position;
        if (existingAttackVisual.GetComponent<Collider>().bounds.Contains(playerPos))
        {
            //deal damage
            Debug.Log("hit");
        }
        Destroy(existingAttackVisual);

        //spawn enemies
        GameObject dust1 = Instantiate(dashDustParticles, spawnPoint1.position, Quaternion.identity);
        dust1.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
        GameObject spawn1 = Instantiate(spawnedEnemy, spawnPoint1.position, transform.rotation);
        spawn1.tag = "BossSpawns";
        GameObject dust2 = Instantiate(dashDustParticles, spawnPoint2.position, Quaternion.identity);
        dust2.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
        GameObject spawn2 = Instantiate(spawnedEnemy, spawnPoint2.position, transform.rotation);
        spawn2.tag = "BossSpawns";

        //wait
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(1f);

        //resume walking around
        doingAttack = false;
    }

    IEnumerator shoot(Vector3 _direction)
    {
        doingAttack = true;
        canAttack = false;

        //wait
        anim.SetBool("Defend", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Defend", false);
        anim.SetTrigger("Cast Spell");
        audioShoot.Play();
        yield return new WaitForSeconds(0.5f);

        //[DO ATTACK]
        for (int i = 0; i < 80; i++)
        {
            float randomX = Random.Range(-40f, 40f);
            float randomY = Random.Range(-40f, 40f);
            float randomZ = Random.Range(-40f, 40f);
            Rigidbody rb = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.transform.forward = _direction;
            rb.transform.Rotate(randomX, randomY, randomZ);
            rb.AddForce(rb.transform.forward * 750f);
        }

        //wait
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(1f);

        //resume walking around
        doingAttack = false;
    }

    IEnumerator strikeCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldownLength);
        canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet(Clone)")
        {
            TakeDamage(other.gameObject.GetComponent<BulletScript>().getTrueDamage());
            if (gameObject.GetComponent<NavMeshAgent>().enabled == true)
            {
                agent.SetDestination(target.position);

                alerted = true;

                if (!audioWalk.isPlaying)
                    audioWalk.Play();
                audioWalk.mute = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            dead = true;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;

            audioWalk.mute = true;

            if (existingAttackVisual != null)
            {
                Destroy(existingAttackVisual);
            }

            Instantiate(corpse, transform.position, transform.rotation);

            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addToScore(100);

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
}

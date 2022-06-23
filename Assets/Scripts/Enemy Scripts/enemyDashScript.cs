using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text.RegularExpressions;

public class enemyDashScript : MonoBehaviour
{
    public GameObject dashAttackZone;
    public GameObject dashDustParticles;
    public GameObject dashEnergyParticles;
    public Animator anim;
    public GameObject corpse;
    public AudioSource audioWalk;
    public Transform eye;
    [SerializeField] float health;

    float lookRadius = 30f;
    Transform target;
    NavMeshAgent agent;
    bool canAttack, doingAttack;
    float cooldownLength = 5f;
    float attackDistance = 15f;
    bool dead;
    GameObject existingAttackVisual;
    bool enemyInSight, alerted, calling;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;
        doingAttack = false;
        dead = false;
        enemyInSight = false;
        alerted = false;
        calling = false;
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
                string tag = hit.collider.gameObject.tag;
                /*
                Debug.Log("Tag:" + tag);
                Debug.Log("Name: " + hit.collider.gameObject.name);
                */
                if (tag == "Player")
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

                NavMeshPath path = new NavMeshPath();
                bool pathFound = NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
                if (pathFound)
                    agent.SetDestination(target.position);
                else //calculate path to closest valid navmesh
                {
                    NavMeshHit hitMesh;
                    if (NavMesh.SamplePosition(target.position, out hitMesh, 5f, NavMesh.AllAreas))
                        agent.SetDestination(hitMesh.position);
                }

                anim.SetBool("Walk Forward", true);

                if (!audioWalk.isPlaying)
                    audioWalk.Play();
                audioWalk.mute = false;

                Vector3 to = target.position;
                Vector3 from = transform.position;
                Vector3 direction = (to - from).normalized;
                Debug.DrawLine(from, to);

                if (distance <= agent.stoppingDistance
                    && direction.y > -3f && direction.y < 3f)
                {
                    agent.SetDestination(transform.position);
                    anim.SetBool("Walk Forward", false);
                    audioWalk.mute = true;
                    FaceTarget(direction);

                    bool possibleEndPoint = false;
                    Vector3 endPosition = new Vector3(0f, 0f, 0f);
                    NavMeshHit hitMesh;
                    if (NavMesh.SamplePosition(transform.position + (direction * attackDistance), out hitMesh, 5f, NavMesh.AllAreas))
                    {
                        possibleEndPoint = true;
                        endPosition = hitMesh.position;
                    }

                    bool targetIsGrounded = target.gameObject.GetComponent<StarterAssets.FirstPersonController>().Grounded;

                    if (canAttack && Vector3.Angle(eye.forward, vectorToEnemy) <= 30f && possibleEndPoint && targetIsGrounded)
                    {
                        //CHECK IF THERES ROOM TO ATTACK, if not, either abort or (more complicated) do a shorter dash, until the obstacle
                        StartCoroutine(strike(direction, endPosition));
                        StartCoroutine(strikeCooldown());
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

    IEnumerator strike(Vector3 _direction, Vector3 endPosition)
    {
        doingAttack = true;
        canAttack = false;

        //spawn prospective attack zone
        Vector3 centerShift = _direction * (attackDistance / 2);
        existingAttackVisual = Instantiate(dashAttackZone, transform.position + centerShift, transform.rotation);
        existingAttackVisual.transform.LookAt(endPosition);

        float rotX = existingAttackVisual.transform.localRotation.x;
        if (rotX >= -0.02f && rotX <= 0.02f)
        {
            Vector3 zoneRotation = existingAttackVisual.transform.rotation.eulerAngles;
            existingAttackVisual.transform.rotation = Quaternion.Euler(0f, zoneRotation.y, zoneRotation.z);
        }
        else if (rotX > 0f && _direction.y > 0f)
        {
            Vector3 zoneRotation = existingAttackVisual.transform.rotation.eulerAngles;
            existingAttackVisual.transform.rotation = Quaternion.Euler(-(zoneRotation.x), zoneRotation.y, zoneRotation.z);
        }

        //wait
        anim.SetBool("Defend", true);
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("Defend", false);
        anim.SetTrigger("Stab Attack");
        yield return new WaitForSeconds(0.5f);

        //attack, teleport to end of strike (deal damage to player eventually)

        //check if player is intersecting zone collider
        Vector3 playerPos = target.position;
        if (existingAttackVisual != null)
            if (existingAttackVisual.GetComponent<Collider>().bounds.Contains(playerPos))
            {
                target.gameObject.GetComponent<PlayerScript>().Damage(15);
            }
        
        Destroy(existingAttackVisual);
        GameObject dust = Instantiate(dashDustParticles, transform.position, Quaternion.identity);
        dust.transform.Rotate(-90f, 0f, 0f, Space.Self);
        GameObject trail = Instantiate(dashEnergyParticles, transform.position + centerShift, transform.rotation);
        trail.transform.Rotate(0f, 180f, 0f, Space.Self);

        //transform.position = transform.position + (transform.forward * attackDistance);
        transform.position = endPosition;

        //wait
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(2f);

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
                StartCoroutine(callForAid(target.position));
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

        if (health <= 0 && !dead)
        {
            dead = true;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;

            audioWalk.mute = true;

            if (doingAttack)
            {
                StopCoroutine("strike");
                anim.SetBool("Defend", false);
            }

            if (existingAttackVisual != null)
            {
                Destroy(existingAttackVisual);
            }

            Instantiate(corpse, transform.position, transform.rotation);

            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addToScore(20);

            //Invoke(nameof(DestroyThis), 3f);
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
}

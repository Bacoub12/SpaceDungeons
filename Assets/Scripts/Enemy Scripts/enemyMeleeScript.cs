using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyMeleeScript : MonoBehaviour
{
    public Animation anim;
    public Collider attackBox;
    public string type;
    public GameObject corpse;
    public AudioSource audioAttack;
    public AudioSource audioWalk;
    public Transform eye;
    [SerializeField] float health;

    Transform target;
    NavMeshAgent agent;
    float lookRadius = 30f;
    bool canAttack, attacking, dead;
    float cooldownLength = 2.5f;
    float attackRange = 3f;
    float fieldOfView = 90f;
    bool enemyInSight, alerted;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;
        attacking = false;
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
        if (Physics.Raycast(eyePos, vectorToEnemy, out hit, lookRadius, gunLayerMask, QueryTriggerInteraction.Ignore))
        {
            string tag = hit.collider.gameObject.tag;
            /*
            Debug.Log("Tag:" + tag);
            Debug.Log("Name: " + hit.collider.gameObject.name);
            */
            if (tag == "Player") // && Vector3.Angle(eye.forward, vectorToEnemy) <= fieldOfView / 2
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

        //Debug.Log("enemy in sight: " + enemyInSight + ", attacking: " + attacking + ", distance: " + distance);
        if (distance <= lookRadius && attacking == false && (enemyInSight || alerted))
        {
            agent.SetDestination(target.position);

            alerted = true;

            Vector3 to = target.position + new Vector3(0f, 1f, 0f);
            Vector3 from = transform.position;
            Vector3 direction = (to - from).normalized;
            Debug.DrawLine(from, to);

            if (distance <= agent.stoppingDistance)
            {
                agent.SetDestination(transform.position);
                anim.Play("Idle");
                FaceTarget(direction);

                if (!audioWalk.isPlaying)
                    audioWalk.Play();
                audioWalk.mute = true;

                if (canAttack && Vector3.Distance(target.position, transform.position) < attackRange)
                {
                    StartCoroutine(AttackTarget(direction));
                    StartCoroutine(strikeCooldown());
                }
            }
            else
            {
                anim.Play("Run");

                if (!audioWalk.isPlaying)
                    audioWalk.Play();
                audioWalk.mute = false;
            }
        }
    }

    private void FaceTarget(Vector3 _direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        //transform.LookAt(target);
    }

    IEnumerator AttackTarget(Vector3 _direction)
    {
        attacking = true;

        agent.SetDestination(transform.position);

        anim.Play("Attack1");
        audioAttack.Play();

        yield return new WaitForSeconds(0.6f);

        //deal damage to player if player is still in range
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= agent.stoppingDistance && attackBox.bounds.Contains(target.position))
        {
            target.gameObject.GetComponent<PlayerScript>().Damage(15);
        }

        yield return new WaitForSeconds(1f);

        anim.Play("Idle");

        attacking = false;
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

            Instantiate(corpse, transform.position, transform.rotation);

            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addToScore(10);

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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        /*
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        */
    }
}

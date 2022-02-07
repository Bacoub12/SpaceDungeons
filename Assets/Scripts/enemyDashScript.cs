using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyDashScript : MonoBehaviour
{
    public GameObject dashAttackZone;
    public GameObject dashDustParticles;
    public GameObject dashEnergyParticles;
    public Animator anim;

    float lookRadius = 30f;
    Transform target;
    NavMeshAgent agent;
    bool canAttack, doingAttack;
    float cooldownLength = 5f;
    float attackDistance = 15f;
    bool dead;
    float health = 120f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;
        doingAttack = false;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (doingAttack == false)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius)
            {
                agent.SetDestination(target.position);
                anim.SetBool("Walk Forward", true);

                Vector3 to = target.position + new Vector3(0f, 1f, 0f);
                Vector3 from = transform.position;
                Vector3 direction = (to - from).normalized;
                Debug.DrawLine(from, to);

                if (distance <= agent.stoppingDistance)
                {
                    agent.SetDestination(transform.position);
                    anim.SetBool("Walk Forward", false);
                    FaceTarget(direction);

                    if (canAttack)
                    {
                        //CHECK IF THERES ROOM TO ATTACK, if not, either abort or (more complicated) do a shorter dash, until the obstacle
                        StartCoroutine(strike(direction));
                        StartCoroutine(strikeCooldown());
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

    IEnumerator strike(Vector3 _direction)
    {
        doingAttack = true;
        canAttack = false;

        //spawn prospective attack zone
        Vector3 centerShift = transform.forward * (attackDistance / 2);
        GameObject zone = Instantiate(dashAttackZone, transform.position + centerShift, transform.rotation);

        //wait
        anim.SetBool("Defend", true);
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("Defend", false);
        anim.SetTrigger("Stab Attack");
        yield return new WaitForSeconds(0.5f);

        //attack, teleport to end of strike (deal damage to player eventually)

        //check if player is intersecting zone collider
        Vector3 playerPos = target.position;
        if (zone.GetComponent<Collider>().bounds.Contains(playerPos))
        {
            //deal damage
            Debug.Log("hit");
        }
        
        Destroy(zone);
        GameObject dust = Instantiate(dashDustParticles, transform.position, Quaternion.identity);
        dust.transform.Rotate(-90f, 0f, 0f, Space.Self);
        GameObject trail = Instantiate(dashEnergyParticles, transform.position + centerShift, transform.rotation);
        trail.transform.Rotate(0f, 180f, 0f, Space.Self);

        transform.position = transform.position + (transform.forward * attackDistance);

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
            TakeDamage(24); //5 shots to kill
            if (gameObject.GetComponent<NavMeshAgent>().enabled == true)
            {
                agent.SetDestination(target.position);
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

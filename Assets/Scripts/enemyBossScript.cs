using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyBossScript : MonoBehaviour
{
    public GameObject dashAttackZone;
    public GameObject slamAttackZone;
    public GameObject bullet;
    public Animator anim;

    float lookRadius = 30f;
    Transform target;
    NavMeshAgent agent;
    bool canAttack, doingAttack;
    float cooldownLength = 5f;
    float dashDistance = 15f;
    float slamDistance = 5f;
    bool dead;
    float health = 2000f;

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
                        int attackChoice = Random.Range(1, 4); //1 à 3
                        switch (attackChoice)
                        {
                            case 1:
                                StartCoroutine(dash());
                                break;
                            case 2:
                                StartCoroutine(slam());
                                break;
                            case 3:
                                StartCoroutine(shoot());
                                break;
                        }

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

    IEnumerator dash()
    {
        doingAttack = true;
        canAttack = false;

        //spawn prospective attack zone
        Vector3 centerShift = transform.forward * (dashDistance / 2);
        GameObject zone = Instantiate(dashAttackZone, transform.position + centerShift, transform.rotation);

        //wait
        anim.SetBool("Defend", true);
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("Defend", false);
        anim.SetTrigger("Stab Attack");
        yield return new WaitForSeconds(0.5f);

        //attack, teleport to end of strike, deal damage to player eventually
        Destroy(zone);

        transform.position = transform.position + (transform.forward * dashDistance);
        Debug.Log("dash");

        //wait
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(2f);

        //resume walking around
        doingAttack = false;
    }

    IEnumerator slam()
    {
        doingAttack = true;
        canAttack = false;

        //spawn prospective attack zone
        Vector3 centerShift = transform.forward * (slamDistance / 2);
        GameObject zone = Instantiate(slamAttackZone, transform.position + centerShift, transform.rotation);

        //wait
        anim.SetBool("Defend", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("Defend", false);
        anim.SetTrigger("Smash Attack");
        yield return new WaitForSeconds(0.5f);

        //attack, deal damage to player eventually
        Destroy(zone);
        Debug.Log("slam");

        //wait
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(1f);

        //resume walking around
        doingAttack = false;
    }

    IEnumerator shoot()
    {
        doingAttack = true;
        canAttack = false;

        //[DO ATTACK]
        Debug.Log("shoot");

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
            TakeDamage(10); //5 shots to kill
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

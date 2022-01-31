using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyConjurerScript : MonoBehaviour
{
    public GameObject summonedEnemy;
    public GameObject summonZone;
    public Animator anim;

    NavMeshAgent agent;
    float walkRadius = 7f;
    float minimumWalkDistance = 3f;
    bool canSummon;
    int maxSpawnsPerSummoner = 5;
    float health = 120f;
    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        canSummon = true;
        dead = false;
        setNewDest();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 1f && canSummon)
        {
            GameObject[] summoners = GameObject.FindGameObjectsWithTag("Summoner");
            GameObject[] summoned = GameObject.FindGameObjectsWithTag("Summoned");
            int totalMaxSummons = summoners.Length * maxSpawnsPerSummoner;

            if (summoned.Length < totalMaxSummons)
            {
                StartCoroutine(summon());
            }
            else
            {
                StartCoroutine(onHold());
            }
        }
    }

    private void setNewDest()
    {
        Vector3 newDest = new Vector3(
            transform.position.x + Random.Range(-walkRadius, walkRadius), 
            transform.position.y, 
            transform.position.z + Random.Range(-walkRadius, walkRadius));

        if (Vector3.Distance(transform.position, newDest) > minimumWalkDistance)
        {
            agent.SetDestination(newDest);
            anim.SetBool("Walk_Anim", true);
        }
    }

    IEnumerator summon()
    {
        canSummon = false;
        //when this starts, conjurer has reached destination
        agent.SetDestination(transform.position);
        anim.SetBool("Walk_Anim", false);

        //starts conjuring, takes 2 seconds, put an effect where the summoning takes place
        Vector3 summonLocation = transform.position + (transform.forward * 2f);
        GameObject zone = Instantiate(summonZone, summonLocation, Quaternion.identity);
        yield return new WaitForSeconds(2f);

        //conjures
        Destroy(zone);
        Instantiate(summonedEnemy, summonLocation, transform.rotation);

        yield return new WaitForSeconds(1f);

        setNewDest();
        canSummon = true;
    }

    IEnumerator onHold()
    {
        anim.SetBool("Walk_Anim", false);
        yield return new WaitForSeconds(1f);
        setNewDest();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet(Clone)")
        {
            TakeDamage(24); //5 shots to kill
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

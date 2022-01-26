using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemySwordDashScript : MonoBehaviour
{
    public GameObject dashAttackZone;

    float lookRadius = 30f;
    Transform target;
    NavMeshAgent agent;
    bool canAttack, doingAttack;
    float cooldownLength = 5f;
    float attackDistance = 15f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;
        doingAttack = false;
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

                Vector3 to = target.position + new Vector3(0f, 1f, 0f);
                Vector3 from = transform.position;
                Vector3 direction = (to - from).normalized;
                Debug.DrawLine(from, to);

                if (distance <= agent.stoppingDistance)
                {
                    agent.SetDestination(transform.position);
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
        Vector3 downShift = new Vector3(0f, -1f, 0f);
        Instantiate(dashAttackZone, transform.position + centerShift + downShift, transform.rotation);

        //wait
        yield return new WaitForSeconds(2f);

        //attack, teleport to end of strike (deal damage to player eventually)
        transform.position = transform.position + (transform.forward * attackDistance);
        Debug.Log("attack");

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
}

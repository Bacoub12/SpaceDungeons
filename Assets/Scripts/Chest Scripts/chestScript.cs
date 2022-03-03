using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public GameObject drop;

    Animation anim;
    bool opened;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        opened = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void open()
    {
        if (opened == false)
        {
            anim.Play("Crate_Open");
            StartCoroutine(dropLoot());
        }
        opened = true;
    }

    public bool isOpened()
    {
        return opened;
    }

    IEnumerator dropLoot()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 upShift = new Vector3(0f, 0.5f, 0f); //0f, 0.5f, 0f
        Vector3 launchVector = transform.forward + transform.up;
        Rigidbody rb = Instantiate(drop, transform.position + upShift, transform.rotation).GetComponent<Rigidbody>();
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), drop.GetComponent<Collider>());
        rb.AddForce(launchVector * 150f);
        rb.AddTorque(transform.right * 50f);
    }
}

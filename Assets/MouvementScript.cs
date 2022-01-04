using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouvementScript : MonoBehaviour
{
    [SerializeField] Vector3 moveVal;
    [SerializeField] float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position += moveSpeed * Time.deltaTime * moveVal;
    }

    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>().normalized;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

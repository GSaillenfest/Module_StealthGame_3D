using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float forwardSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float cameraFollowSpeed;

    Rigidbody playerRb;
    float horizontalInput;
    float verticalInput;
    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        direction = (horizontalInput * rotationSpeed * Camera.main.transform.right) + (verticalInput * forwardSpeed * Camera.main.transform.forward);
        direction.y = 0;
    }

    private void FixedUpdate()
    {
        Move();
        RotateTowardsCamera();
    }

    private void Move()
    {
        direction.y = playerRb.velocity.y;
        playerRb.velocity = direction;
    }

    private void RotateTowardsCamera()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        // TODO : try to pass a upward argument to LookRoration methods without declaring cameraForward vector3
        Quaternion lookRotation = Quaternion.LookRotation(cameraForward);
        Quaternion rotation = Quaternion.RotateTowards(playerRb.rotation, lookRotation, cameraFollowSpeed * Time.fixedDeltaTime);
        playerRb.MoveRotation(rotation);
    }
}

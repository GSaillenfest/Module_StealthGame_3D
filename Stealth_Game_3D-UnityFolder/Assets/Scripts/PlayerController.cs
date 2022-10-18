using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float cameraFollowSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float raycastOffset;

    Rigidbody playerRb;
    float horizontalInput;
    float verticalInput;
    bool canJump;
    bool doJump;
    bool isSneaky;
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

        canJump = CheckGround();

        if (canJump && Input.GetButtonDown("Jump")) doJump = true;

        direction = (horizontalInput * Camera.main.transform.right) + (verticalInput * Camera.main.transform.forward).normalized;
        direction.y = 0;

        if (Input.GetButtonDown("Stealth")) isSneaky = !isSneaky;
    }

    private bool CheckGround()
    {
        bool ground = false;
        Ray ray;
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                ray = new Ray(transform.position + new Vector3(0, 0.9f, 0), Vector3.down);
            }
            else
            {
                ray = new Ray(transform.localPosition + Vector3.up + (Quaternion.Euler(0, i * 90f, 0) * Vector3.right * raycastOffset), Vector3.down);
                Debug.DrawRay(transform.localPosition + Vector3.up + (Quaternion.Euler(0, i * 90f, 0) * Vector3.right * raycastOffset), Vector3.down, Color.blue);
            }

            Physics.Raycast(ray, out RaycastHit hit, 1.1f);
            if (hit.transform != null)
            {
                if (hit.transform.CompareTag("Ground")) ground = true; return ground;
            }
        }

        return ground;
    }

    private void FixedUpdate()
    {
        if (direction.magnitude != 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion rotation = Quaternion.RotateTowards(playerRb.rotation, lookRotation, 500 * Time.fixedDeltaTime);
            playerRb.MoveRotation(rotation);
        }


        Move();

        //RotateTowardsCamera();

        if (doJump)
        {
            Jump();
        }

        if (isSneaky)
        {
            transform.localScale = Vector3.one * 0.4f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void Move()
    {
        playerRb.velocity = new Vector3(0, playerRb.velocity.y, 0) + (transform.forward * direction.magnitude) * speed;
    }

    private void Jump()
    {
        playerRb.AddForce(jumpForce * Vector3.up, ForceMode.VelocityChange);
        doJump = false;
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

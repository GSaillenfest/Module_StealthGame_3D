using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float baseSpeed;
    [SerializeField] float sneakySpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float cameraFollowSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float raycastOffset;
    [SerializeField] Animator animator;
    [SerializeField] Transform focusPoint;
    public Cinemachine.CinemachineFreeLook fl;

    Rigidbody playerRb;
    float maxDistance = 1.1f;
    float horizontalInput;
    float verticalInput;
    bool canJump;
    bool doJump;
    bool isSneaky;
    Vector3 direction;
    float signedAngle;
    bool isOnEdge;
    bool isLanding;
    bool isFalling;
    bool sprint;
    float speed;
    public bool test;
    bool isAiming;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        fl.m_YAxisRecentering = new AxisState.Recentering();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        AnimatorSetters();

        canJump = CheckGround();

        if (playerRb.velocity.y > 0) isLanding = false;
        if (canJump && Input.GetButtonDown("Jump")) doJump = true;

        direction = (horizontalInput * Camera.main.transform.right) + (verticalInput * Camera.main.transform.forward);
        direction.y = 0;

        signedAngle = Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up);

        if (signedAngle < -150 || signedAngle > 150)
        {
            test = true;
            fl.m_YAxisRecentering = new AxisState.Recentering(false, 1f, 2f);
        }
        else test = false;
        SetSpeed();

        if (isAiming)
        {
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1);
        }
        else
        {

            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
        }
    }

    private void SetSpeed()
    {
        if (sprint) speed = sprintSpeed;
        else if (isSneaky) speed = sneakySpeed;
        else if (!canJump) speed = baseSpeed * 0.5f;
        else speed = baseSpeed;
    }

    private void GetInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        
        verticalInput = Input.GetAxis("Vertical");
        
        if (Input.GetButtonDown("Stealth")) isSneaky = !isSneaky;
        
        sprint = Input.GetButton("Sprint");
        
        if (Input.GetButtonDown("Aim"))
        {
            isAiming = !isAiming;
            if (isAiming)
            {
                fl.LookAt = ChoseTarget();
            }
            else
            {
                fl.LookAt = focusPoint;
            }
        }

    }

    private void AnimatorSetters()
    {
        animator.SetFloat("HorizontalInput", horizontalInput);
        animator.SetFloat("VerticalInput", verticalInput);
        animator.SetFloat("DirectionMagnitude", direction.magnitude);
        animator.SetBool("isOnEdge", isOnEdge);
        animator.SetBool("Sprint", sprint);
        animator.SetBool("isLanding", isLanding);
        animator.SetBool("isSneaky", isSneaky);
    }

    private bool CheckGround()
    {
        int rayCastCount = 0;
        bool ground = false;
        Ray ray;
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                ray = new Ray(transform.position + Vector3.up, Vector3.down);
            }
            else
            {
                ray = new Ray(transform.localPosition + Vector3.up + (Quaternion.Euler(0, i * 90f, 0) * Vector3.right * raycastOffset), Vector3.down);
                //Debug.DrawRay(transform.localPosition + Vector3.up + (Quaternion.Euler(0, i * 90f, 0) * Vector3.right * raycastOffset), Vector3.down, Color.blue);
            }

            if (playerRb.velocity.y < 0) maxDistance = 2f;
            else maxDistance = 1.2f;

            Physics.Raycast(ray, out RaycastHit hit, maxDistance);

            if (hit.transform != null)
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    rayCastCount++;
                }
            }
        }
        Debug.Log(rayCastCount);
        if (rayCastCount > 0)
        {
            if (playerRb.velocity.y < 0) isLanding = true;
            ground = true;

            if (rayCastCount == 3)
            {
                isOnEdge = true;
            }
            else isOnEdge = false;
        }
        return ground;
    }

    private void FixedUpdate()
    {

        if (isAiming)
        {
            RotateTowardsCamera();
            MoveAiming();
        }
        else
        {
            Move();
            RotateTowardsInputDirection();
        }


        if (doJump)
        {
            Jump();
        }
    }

    private void RotateTowardsInputDirection()
    {
        if (direction.magnitude != 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion rotation = Quaternion.RotateTowards(playerRb.rotation, lookRotation, 500 * Time.fixedDeltaTime);
            playerRb.MoveRotation(rotation);

        }
    }

    private void Move()
    {
        playerRb.velocity = new Vector3(0, playerRb.velocity.y, 0) + (transform.forward * Mathf.Clamp01(direction.magnitude)) * speed;
    }

    private void MoveAiming()
    {
        Vector3 moveDirection = Camera.main.transform.forward * verticalInput + Camera.main.transform.right * horizontalInput;
        moveDirection.y = playerRb.velocity.y;
        Vector3.Normalize(moveDirection);
        playerRb.velocity = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed);
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
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

    private Transform ChoseTarget()
    {
        
        return Physics.OverlapSphere(transform.position, 100f, 8)[0].transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    // Referncing other componenets
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer tr;


    // Main variables
    public float speed;
    public float jumpForce;
    public float checkRadius;
    public float jumpTime;

    // Dashing
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = -5f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    // Coyote Time
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    // Jump buffer
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    // Flipping
    private bool isFacingLeft = true;

    // Horizontal inputs
    private float moveInput;
    private bool isGrounded;
    private float jumpTimeCounter;
    private bool isJumping;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // So we dont run and jump while dashing
        if (isDashing)
        {
            return;
        }

        // Makes our Character move

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    void Update()
    {
        // So we dont run and jump while dashing
        if (isDashing) 
        {
            return;
        }

        // Determines our horizontal input
        moveInput = Input.GetAxisRaw("Horizontal");

        // Checks to flip character
        Flip();

        // Checks if our character is grounded by using the Physics2D.OverlapCircle
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        // We can jump after we've fallen off a ledge
        if (isGrounded == true)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            // Makes our character jump
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;

            jumpBufferCounter = 0f;
        }
        // If we let go of jump earlier, we will fall down, but if we keep holding jump we will have a longer jump 
        if (isJumping == true)
        {
            if (Input.GetButtonUp("Jump"))
            {
                isJumping = false;
            }
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
                coyoteTimeCounter = 0f;
            }
            else
            {
                isJumping = false;
            }



        }

        // Makes us run the IEnum???
        if (Input.GetButtonDown("Fire3") && canDash)
        {
            StartCoroutine(Dash());
        }
        

    }
    // I don't know how or why this works but apparently it does
    private IEnumerator Dash()
    {
        isJumping = false; 
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    // Flips character
    private void Flip()
    {
        if (isFacingLeft && moveInput > 0f || !isFacingLeft && moveInput < 0f)
        {
            isFacingLeft = !isFacingLeft;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale; 
        }
    }

}
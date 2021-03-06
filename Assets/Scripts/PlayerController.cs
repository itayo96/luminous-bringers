﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody2D rigidBody;
    public Animator animator;

    // Public Movement Members
    public float runSpeed = 12f;
    public float jumpForce = 250f;
    [Range(0, 0.3f)] public float movementSmoothing = 0.05f;

    // Private Movement Members
    private float horizontalMove = 0f;
    private bool isPreAirborn = false;
    private float lastDistance = 0f;
    public static bool isInputEnabled = true;

    // Public Ground Members
    public LayerMask whatIsGround;
    public LayerMask whatIsDangerZone;
    public Transform groundChecker;

    // Private Ground Members
    const float groundOverlapRadius = 0.2f;
    protected bool isFacingRight = true;
    private Vector3 velocity = Vector3.zero;

    // Public Stats Members
    public int maxHealthCount = 3;
    public Texture2D healthTexture;
    public string sceneToLoadOnDeath = "";
    public int health = 3;

    // Public Animation Timers
    public float jumpAnimTime = 0.25f;
    public float leftClickAnimTime = 0.7f;
    public float rightClickAnimTime = 0.7f;
    public float hurtAnimTime = 0.25f;
    public float deathAnimTime = 2f;

    // Animator Flags
    private bool isJumping = false;
    private bool isBeingHurt = false;
    private bool isLeftClick = false;
    private bool isRightClick = false;

    // --------
    // Starters
    // --------
    protected virtual void Start()
    {
        health = maxHealthCount;
        isInputEnabled = true;
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update()
    {
        // Don't check for input if disabled
        if (!isInputEnabled)
        {
            horizontalMove = 0;

            animator.SetFloat("Speed", 0);

            return;
        }

        // Get horizontal button for movement direction
        horizontalMove = Input.GetAxisRaw("Horizontal");

        // Key listeners
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isLeftClick = false;
            isRightClick = false;
            isJumping = true;
        }
        else if (!animator.GetBool("IsLeftClick") && !animator.GetBool("IsRightClick") && !animator.GetBool("IsJumping") && horizontalMove == 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                isLeftClick = true;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isRightClick = true;
            }
        }
        else if (animator.GetBool("IsLeftClick") || animator.GetBool("IsRightClick"))
        {
            horizontalMove = 0;
        }

        // Animator speed flag set
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    void FixedUpdate()
    {
        // Check for danger zone
        CheckDangerZone();

        // Resolve death due to danger zone or enough hits
        if (!IsAlive() && animator.GetBool("IsAlive"))
        {
            animator.SetBool("IsAlive", false);
            animator.SetBool("IsJumping", false);

            StartCoroutine(OnDeathAnimation());
            return;
        }

        // Check for jumps and grounds
        if (!AirControl())
        {
            if (isLeftClick) // If left click was requested
            {
                animator.SetBool("IsLeftClick", true);

                LeftClick();
                isLeftClick = false;

                StartCoroutine(OnLeftClickAnimation());
            }
            else if (isRightClick) // If right click was requested
            {
                animator.SetBool("IsRightClick", true);

                RightClick();
                isRightClick = false;

                StartCoroutine(OnRightClickAnimation());
            }
            else if (!isJumping)
            {
                // Move the player on the ground while not jumping or casting
                Move();
            }
        }

        isJumping = false;

        // Refresh player stats if implemented
        Refresh();
    }

    // -----------
    // Controllers
    // -----------
    void Move()
    {
        float move = horizontalMove * Time.fixedDeltaTime * runSpeed;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, rigidBody.velocity.y);

        // Smoothing the velocity out and applying it to the character
        rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, movementSmoothing);

        // If the input is moving the player right and the player is facing left, or the opposite, FLIP
        if ((move > 0 && !isFacingRight) || (move < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    bool AirControl()
    {
        // Check if truly on ground
        if (rigidBody.velocity.y < 0.001f)
        {
            // Raycast from the feet of the player directly down
            RaycastHit2D hit2D = Physics2D.Raycast(groundChecker.position, Vector2.down, 0.3f, whatIsGround);

            // If the raycast hit something
            if (hit2D && !isPreAirborn)
            {
                // Check if the distance of the object hit is less than the last distance checked
                if (hit2D.distance < lastDistance)
                {
                    // Update the last distance if the object below is less than the last known distance
                    lastDistance = hit2D.distance;

                    return true;
                }
                else if (isJumping)
                {
                    // If the hit distance is not less than the lass distance, then jump (he isn't going to go any lower)
                    isPreAirborn = true;

                    StartCoroutine(OnJumpAnimation());

                    return true;
                }
                else if (Math.Abs(rigidBody.velocity.y) < 0.1f)
                {
                    animator.SetBool("IsJumping", false);
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }

    void CheckDangerZone()
    {
        // Check danger zone collision
        Collider2D[] dangerColliders = Physics2D.OverlapCircleAll(groundChecker.position, groundOverlapRadius, whatIsDangerZone);
        for (int i = 0; i < dangerColliders.Length; i++)
        {
            if (dangerColliders[i].gameObject != gameObject)
            {
                health = 0;
            }
        }
    }

    protected virtual void LeftClick() { }

    protected virtual void RightClick() { }

    // ------
    // Events
    // ------
    public void OnInputEnabling(bool flag)
    {
        isInputEnabled = flag;
    }

    public virtual void OnGettingHit()
    {
        GotHit();
    }

    IEnumerator OnJumpAnimation()
    {
        yield return new WaitForSeconds(jumpAnimTime);

        rigidBody.AddForce(Vector2.up * jumpForce);
        isPreAirborn = false;
        lastDistance = 100f;
        
        animator.SetBool("IsJumping", true);
    }

    IEnumerator OnLeftClickAnimation()
    {
        yield return new WaitForSeconds(leftClickAnimTime);

        animator.SetBool("IsLeftClick", false);
    }

    IEnumerator OnRightClickAnimation()
    {
        yield return new WaitForSeconds(rightClickAnimTime);

        animator.SetBool("IsRightClick", false);
    }

    IEnumerator OnHurtAnimation()
    {
        isInputEnabled = false;

        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        while (GetComponent<SpriteRenderer>().color.g > 0.2f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g - 0.2f, GetComponent<SpriteRenderer>().color.b - 0.2f);
            yield return new WaitForSeconds(0.03f);
        }

        while (GetComponent<SpriteRenderer>().color.g < 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g + 0.2f, GetComponent<SpriteRenderer>().color.b + 0.2f);
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(hurtAnimTime - 0.24f);

        isInputEnabled = true;

        animator.SetBool("IsBeingHurt", false);
    }

    IEnumerator OnDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimTime);

        if (sceneToLoadOnDeath == "")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoadOnDeath);
        }
    }

    // ------
    // Stats
    // ------
    protected virtual void GotHit()
    {
        if (health > 0 && !animator.GetBool("IsBeingHurt"))
        {
            StopAllCoroutines();
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsLeftClick", false);
            animator.SetBool("IsRightClick", false);
            animator.SetFloat("Speed", 0);

            health--;
            horizontalMove = 0;
            animator.SetBool("IsBeingHurt", true);
            StartCoroutine(OnHurtAnimation());
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    protected virtual void Refresh() { }

    // ---
    // GUI
    // ---
    protected virtual void OnGUI()
    {
        if (!isInputEnabled)
        {
            return;
        }

        // Health
        Rect healthIcon = new Rect(20, 93, 25, 25);
        for (int i = 1; i <= health; i++)
        {
            GUI.DrawTexture(healthIcon, healthTexture);
            healthIcon.x += healthIcon.width + 10;
        }
    }
}

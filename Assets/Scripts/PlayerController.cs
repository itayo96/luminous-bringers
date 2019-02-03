using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody2D rigidBody;
    public Animator animator;

    // Public Movement Members
    public float runSpeed = 10f;
    public float jumpForce = 250f;
    [Range(0, 0.3f)] public float movementSmoothing = 0.05f;

    // Private Movement Members
    private float horizontalMove = 0f;
    private bool isPreAirborn = false;

    // Public Ground Members
    public LayerMask whatIsGround;
    public LayerMask whatIsDangerZone;
    public Transform groundChecker;

    // Private Ground Members
    const float groundOverlapRadius = 0.2f;
    private bool isGrounded;
    private bool isFacingRight = true;
    private Vector3 velocity = Vector3.zero;

    // Public Stats Members
    public int maxHealthCount = 3;
    public Texture2D healthTexture;

    // Private Stats Members
    private int health = 3;

    // Public Animation Timers
    public float jumpAnimTime = 0.25f;
    public float leftClickAnimTime = 0.35f;
    public float rightClickAnimTime = 0.35f;
    public float hurtAnimTime = 0.15f;
    public float deathAnimTime = 0.72f;

    // Animator Flags
    private bool isJumping = false;
    private bool isBeingHurt = false;
    private bool isLeftClick = false;
    private bool isRightClick = false;

    // --------
    // Starters
    // --------
    void Start()
    {
        health = maxHealthCount;
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecker.position, groundOverlapRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
            }
        }
    }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update()
    {
        // Get horizontal button for movement direction
        horizontalMove = Input.GetAxisRaw("Horizontal");

        // Key listeners
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && isGrounded)
        {
            isLeftClick = false;
            isRightClick = false;
            isJumping = true;
        }
        else if (!isLeftClick && !isRightClick && !isJumping && horizontalMove == 0)
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

        // Animator speed flag set
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    void FixedUpdate()
    {
        // Resolve hurt effect set hurt flag to false
        if (isBeingHurt)
        {
            GotHit();
            StartCoroutine(OnHurtAnimation());
        }

        // Resolve death due to danger zone or enough hits
        if (!IsAlive())
        {
            animator.SetBool("IsAlive", false);
            animator.SetBool("IsJumping", false);
            StartCoroutine(OnDeathAnimation());

            return;
        }

        if (!isPreAirborn)
        {
            // Ground checking with circlecast
            bool wasGrounded = isGrounded;
            isGrounded = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecker.position, groundOverlapRadius, whatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    if (!wasGrounded && rigidBody.velocity.y < 0f)
                    {
                        OnLanding();
                    }
                    else if (wasGrounded)
                    {
                        isGrounded = true;
                    }
                }
            }
        }

        // Check danger zone collision
        Collider2D[] dangerColliders = Physics2D.OverlapCircleAll(groundChecker.position, groundOverlapRadius, whatIsDangerZone);
        for (int i = 0; i < dangerColliders.Length; i++)
        {
            if (dangerColliders[i].gameObject != gameObject)
            {
                health = 0;
            }
        }

        // Left Click if requested
        if (isGrounded)
        {
            if (isLeftClick)
            {
                LeftClick();
                animator.SetBool("IsLeftClick", true);
                isLeftClick = false;
                StartCoroutine(OnLeftClickAnimation());
            }
            else if (isRightClick) // Right Click if requested
            {
                RightClick();
                animator.SetBool("IsRightClick", true);
                isRightClick = false;
                StartCoroutine(OnRightClickAnimation());
            }
            else
            {
                // Move the player on the ground if not left or right clicks
                Move();
            }
        }

        // Refresh player stats if implemented
        Refresh();

        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsBeingHurt", isBeingHurt);
    }

    // -----------
    // Controllers
    // -----------
    public void Move()
    {
        if (health == 0)
        {
            return;
        }

        float move = horizontalMove * Time.fixedDeltaTime * runSpeed;

        // Move the player by input if on ground
        if (isGrounded)
        {
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

        // If jump was requested
        if (isGrounded && isJumping && !isPreAirborn)
        {
            // Add a vertical force to the player
            isGrounded = false;
            isPreAirborn = true;
            StartCoroutine(OnJumpAnimation());
        }
    }

    protected virtual void LeftClick() { }

    protected virtual void RightClick() { }

    // ------
    // Events
    // ------
    public void OnLanding()
    {
        isJumping = false;
        isGrounded = true;
    }

    public void OnGettingHit()
    {
        isBeingHurt = true;
    }

    IEnumerator OnJumpAnimation()
    {
        yield return new WaitForSeconds(jumpAnimTime);

        isPreAirborn = false;
        rigidBody.AddForce(new Vector2(0f, jumpForce));
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
        yield return new WaitForSeconds(hurtAnimTime);

        isBeingHurt = false;
    }

    IEnumerator OnDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimTime);

        Destroy(gameObject);
    }

    // ------
    // Stats
    // ------
    public void GotHit()
    {
        if (health > 0)
        {
            health--;
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
        // Text Style
        GUIStyle style = new GUIStyle();
        style.fontSize = 80;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;

        // Health
        Rect healthIcon = new Rect(10, 10, 96, 96);
        Rect healthCountLable = new Rect(healthIcon.xMax, healthIcon.y, 96, 64);

        GUI.DrawTexture(healthIcon, healthTexture);
        GUI.Label(healthCountLable, health.ToString(), style);
    }
}

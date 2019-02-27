using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemeyController : EnemyController
{
    enum StateMachine
    {
        patrol,
        combat,
        death,
    }

    public static bool isActive = true;

    // Components
    public Animator animator;

    // Domain
    public Transform leftWing, rightWing;
    private GameObject player = null;
    private float xLeftWing, xRightWing;
    private float xPatrolTargetPosition = 0f;

    // Axe
    public Transform attackPosition;
    public float axeSlashRange;
    public float damageWaitTime = 0.2f;
    private float axeSlashRefreshRate = 3f;
    private float nextAttackTime = 0f;

    // Public Movement Members
    public float runSpeed = 12f;

    // Private Movement Members
    private float horizontalMove = 0f;
    private float moveBeforeHurt = 0f;
    private bool isFacingLeft = true;

    // Public Stats Members
    public Transform healthLocation;

    // Private Stats Members
    private StateMachine state;
    private bool canBeAttacked = true;

    // Public Animation Timers
    public float attackAnimTime = 0.95f;
    public float hurtAnimTime = 0.65f;
    public float deathAnimTime = 1f;

    // --------
    // Starters
    // --------
    new void Start()
    {
        isActive = true;

        xLeftWing = leftWing.position.x;
        xRightWing = rightWing.position.x;
    }

    new void Awake() { }

    // -------------------
    // Updaters & Checkers
    // -------------------
    new void FixedUpdate() { }

    new void Update()
    {
        if (!isActive)
        {
            return;
        }

        // If player is suddenly null, go to patrol
        if (player == null && state != StateMachine.death)
        {
            state = StateMachine.patrol;
        }

        // Resolve current state
        switch (state)
        {
            case StateMachine.patrol:
                PatrolState();
                break;
            case StateMachine.combat:
                CombatState();
                break;
            case StateMachine.death:
            default:
                return;
        }
    }

    // -----------
    // States
    // -----------
    void CombatState()
    {
        if (animator.GetBool("IsBeingHurt"))
        {
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + Time.deltaTime;
            }
            return;
        }

        Move();

        // Stop for wait time or reached starting x position
        if (Time.time >= nextAttackTime - 0.5f && Time.time < nextAttackTime)
        {
            horizontalMove = 0;
        }

        // Check if enough time had passed since last sword slash then just keep walking
        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = 0f;

        // If player is suddenly null or position is out of bounds, go to patrol
        if (player == null ||
            transform.position.x < xLeftWing ||
            transform.position.x > xRightWing)
        {
            state = StateMachine.patrol;
            return;
        }

        // Player is right to the enemy
        if (player.transform.position.x > transform.position.x)
        {
            if (isFacingLeft)
            {
                isFacingLeft = !isFacingLeft;

                // Multiply the character's x local scale by -1.
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }

            horizontalMove = 1;
        }
        else // Player is left to the enemy
        {
            if (!isFacingLeft)
            {
                isFacingLeft = !isFacingLeft;

                // Multiply the character's x local scale by -1.
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }

            horizontalMove = -1;
        }

        // Player is close, no need to move
        if (Vector3.Distance(attackPosition.transform.position, player.transform.position) < axeSlashRange)
        {
            horizontalMove = 0;
            animator.SetFloat("Speed", 0);

            // Attack
            Attack();

            // Wait alittle (random time) till next slash / next trying to reach the player
            nextAttackTime = Time.time + Random.Range(axeSlashRefreshRate - 4f, axeSlashRefreshRate + 1f);
        }
    }

    void PatrolState()
    {
        horizontalMove = 0;

        // If reached target position / it is zero, generate random X between left and right wing
        if (xPatrolTargetPosition == 0 ||
            (isFacingLeft && transform.position.x <= xPatrolTargetPosition) ||
            (!isFacingLeft && transform.position.x >= xPatrolTargetPosition))
        {
            xPatrolTargetPosition = Random.Range(xLeftWing, xRightWing);
        }

        // Walk towards the location
        if (xPatrolTargetPosition > transform.position.x)
        {
            if (isFacingLeft)
            {
                isFacingLeft = !isFacingLeft;

                // Multiply the character's x local scale by -1.
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }

            horizontalMove = 0.5f;
        }
        else
        {
            if (!isFacingLeft)
            {
                isFacingLeft = !isFacingLeft;

                // Multiply the character's x local scale by -1.
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }

            horizontalMove = -0.5f;
        }

        Move();
    }

    // -----------
    // Controllers
    // -----------
    void Move()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        float move = horizontalMove * Time.fixedDeltaTime * runSpeed;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);

        // Smoothing the velocity out and applying it to the character
        Vector3 velocity = Vector3.zero;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.05f);

        // If the input is moving the player right and the player is facing left, or the opposite, FLIP
        if ((move > 0 && isFacingLeft) || (move < 0 && !isFacingLeft))
        {
            isFacingLeft = !isFacingLeft;

            // Multiply the character's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    void Attack()
    {
        // Check for nearby enemies
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPosition.transform.position, axeSlashRange);
        ArrayList objects = new ArrayList();

        for (int i = 0; i < colliders.Length; i++)
        {
            // If this enemy was already damaged, do nothing
            if (objects.Contains(colliders[i].gameObject))
            {
                continue;
            }

            // Slash the enemy
            if (colliders[i].GetComponent<PlayerController>() != null)
            {
                StartCoroutine(OnAttackAnimation(colliders[i].gameObject));
            }

            // Add to damaged enemies
            objects.Add(colliders[i].gameObject);
        }
    }

    // ------
    // Events
    // ------
    public void OnEnteringDomain(GameObject enteredPlayer)
    {
        state = StateMachine.combat;
        player = enteredPlayer;
    }

    public void OnExitingDomain()
    {
        state = StateMachine.patrol;
        player = null;
    }

    IEnumerator OnAttackAnimation(GameObject player)
    {
        yield return new WaitForSeconds(0.2f);

        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.5f);

        bool hadBlocked = false;
        bool isInRange = false;

        for (float i = 0; i < attackAnimTime - 0.8f; i += 0.05f)
        {
            // Stop if player blocked
            if (player.GetComponent<WarlordController>() != null && player.GetComponent<WarlordController>().IsBlocking())
            {
                hadBlocked = true;
                animator.SetBool("GotParried", true);
                break;
            }

            // Check if still in range while attacking
            if (Vector3.Distance(attackPosition.transform.position, player.transform.position) >= axeSlashRange)
            {
                isInRange = false;
            }
            else
            {
                isInRange = true;
            }

            yield return new WaitForSeconds(0.05f);
        }

        // If player not blocked and still in range of the axe
        if (!hadBlocked && isInRange)
        {
            player.gameObject.GetComponent<PlayerController>().OnGettingHit();
        }

        // Stunned on block
        if (hadBlocked)
        {
            yield return new WaitForSeconds(0.8f);
            animator.SetBool("GotParried", false);
            animator.SetBool("IsAttacking", false);
        }

        yield return new WaitForSeconds(0.35f);
        animator.SetBool("IsAttacking", false);

        horizontalMove = 1;
    }

    IEnumerator OnHurtAnimation()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        while (GetComponent<SpriteRenderer>().color.g > 0.2f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g - 0.2f, GetComponent<SpriteRenderer>().color.b - 0.2f);
            yield return new WaitForSeconds(0.05f);
        }

        while (GetComponent<SpriteRenderer>().color.g < 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g + 0.2f, GetComponent<SpriteRenderer>().color.b + 0.2f);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(hurtAnimTime - 0.4f);

        animator.SetBool("IsBeingHurt", false);

        if (moveBeforeHurt != 0)
        {
            horizontalMove = moveBeforeHurt;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        canBeAttacked = true;
    }

    IEnumerator OnDeathAnimation()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        while (GetComponent<SpriteRenderer>().color.g > 0.2f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g - 0.2f, GetComponent<SpriteRenderer>().color.b - 0.2f);
            yield return new WaitForSeconds(0.05f);
        }

        while (GetComponent<SpriteRenderer>().color.g < 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g + 0.2f, GetComponent<SpriteRenderer>().color.b + 0.2f);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(deathAnimTime - 0.55f);

        Destroy(gameObject);
    }

    // ------
    // Stats
    // ------
    protected override void OnHit(int damage)
    {
        if (!canBeAttacked)
        {
            return;
        }

        health -= damage;

        moveBeforeHurt = horizontalMove;
        animator.SetFloat("Speed", 0);
        horizontalMove = 0;

        canBeAttacked = false;
        
        if (health <= 0)
        {
            if (!animator.GetBool("IsAlive"))
            {
                return;
            }

            animator.SetBool("IsAlive", false);
            state = StateMachine.death;

            StartCoroutine(OnDeathAnimation());
            return;
        }
        else
        {
            animator.SetBool("IsBeingHurt", true);
            StartCoroutine(OnHurtAnimation());
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    // ---
    // GUI
    // ---
    void OnGUI()
    {
        if (!isActive || state == StateMachine.patrol)
        {
            return;
        }

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;

        if (health >= 4)
        {
            style.normal.textColor = Color.white;
        }
        else if (health == 3)
        {
            style.normal.textColor = Color.green;
        }
        else if (health == 2)
        {
            style.normal.textColor = Color.yellow;
        }
        else
        {
            style.normal.textColor = Color.red;
        }

        Vector2 pos = Camera.main.WorldToScreenPoint(healthLocation.position);

        Rect healthCountLable = new Rect(pos.x, pos.y+250f, 0.1f, 0.1f);
        GUI.Label(healthCountLable, health.ToString(), style);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.transform.position, axeSlashRange);
    }
}

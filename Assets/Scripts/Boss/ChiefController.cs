using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiefController : MonoBehaviour
{
    enum StateMachine
    {
        against_warlord,
        against_ranger,
        against_elementalist,
        waiting_for_platform,
        ultimate,
        death,
    }

    // Components
    private Rigidbody2D rigidBody;
    public Animator animator;

    // Private Movement Members
    private float runSpeed = 12f;
    private bool isFacingLeft = true;
    private float startingX = 0f;
    private float startingY = 0f;

    // Public Stats Members
    public int phase = 1;
    public bool isPhaseWithPlatforms = true;
    public const int phaseStartingHealth = 100;
    public const int phaseTransitionHealth = 65;

    public const int warlordHealth = 100, warlordDamage = 1;
    public const int rangerHealth = 93, rangerDamage = 2;
    public const int elementalistHealth = 81, elementalistDamage = 4;

    // Private Stats Members
    private int health = 100;
    private StateMachine state;
    private bool playerEnteredPlatform = false;

    // Public Animation Timers
    public float attackAnimTime = 0.7f;
    public float castAnimTime = 0.7f;
    public float roarAnimTime = 0.7f;
    public float hurtAnimTime = 0.25f;
    public float deathAnimTime = 2f;

    // Animator Flags
    private bool isBeingHurt = false;
    private bool isAttacking = false;
    private bool isCasting = false;
    private bool isRoaring = false;

    // --------
    // Starters
    // --------
    protected void Start()
    {
        health = phaseStartingHealth;

        // Starting against who
        switch (health)
        {
            case warlordHealth:
                state = StateMachine.against_warlord;
                break;
            case rangerHealth:
                state = StateMachine.against_ranger;
                break;
            case elementalistHealth:
                state = StateMachine.against_elementalist;
                break;
            default:
                state = StateMachine.death;
                break;
        }

        startingX = transform.position.x;
        startingY = transform.position.y;

        if (!isPhaseWithPlatforms)
        {
            playerEnteredPlatform = true;
        }
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update() { }

    void FixedUpdate()
    {
        // Resolve hurt effect set hurt flag to false
        if (isBeingHurt)
        {
            GotHit();
            isBeingHurt = false;

            StartCoroutine(OnHurtAnimation());
        }

        // Resolve death due to danger zone or enough hits
        if (!IsAlive())
        {
            animator.SetBool("IsAlive", false);
            state = StateMachine.death;

            StartCoroutine(OnDeathAnimation());
            return;
        }

        // Resolve current state
        switch (state)
        {
            case StateMachine.against_warlord:
                WarlordState();
                break;
            case StateMachine.against_ranger:
                RangerState();
                break;
            case StateMachine.against_elementalist:
                ElementalistState();
                break;
            case StateMachine.waiting_for_platform:
                WaitingState();
                break;
            case StateMachine.ultimate:
                UltimateState();
                break;
            case StateMachine.death:
            default:
                break;
        }

        if (health == phaseStartingHealth)
        {
            return;
        }

        // Transitions
        switch (health)
        {
            case warlordHealth:
                if (!playerEnteredPlatform)
                {
                    state = StateMachine.waiting_for_platform;
                }
                else
                {
                    state = StateMachine.against_warlord;
                }
                break;

            case rangerHealth:
                if (!playerEnteredPlatform)
                {
                    state = StateMachine.waiting_for_platform;
                }
                else
                {
                    state = StateMachine.against_ranger;
                }
                break;

            case elementalistHealth:
                if (!playerEnteredPlatform)
                {
                    state = StateMachine.waiting_for_platform;
                }
                else
                {
                    state = StateMachine.against_elementalist;
                }
                break;

            case phaseTransitionHealth:
                if (!playerEnteredPlatform)
                {
                    state = StateMachine.waiting_for_platform;
                }
                else if (IsAlive())
                {
                    state = StateMachine.ultimate;
                }
                break;

            default:
                state = StateMachine.death;
                break;
        }

        if (playerEnteredPlatform && isPhaseWithPlatforms)
        {
            playerEnteredPlatform = false;
        }
    }

    // -----------
    // States
    // -----------
    void WarlordState()
    {
        // TODO: Check how far from player.
        //          If far - Move
        //          If close - Attack
    }

    void RangerState()
    {
        // TODO: Summon the dance components
    }

    void ElementalistState()
    {
        // TODO: Summon the power ball combo (+ audio + text)
    }

    void WaitingState()
    {
        // TODO: If first time in waiting state, move to starting position
        //              else, do nothing
    }

    void UltimateState()
    {
        // TODO: Ultimate, switch OST and get ready to transition
    }

    // -----------
    // Controllers
    // -----------
    void Move() { }

    void Attack() { }

    void Cast() { }

    void Roar() { }

    // ------
    // Events
    // ------
    public void OnGettingHit()
    {
        isBeingHurt = true;
    }

    public void OnPlayerPlatform()
    {
        playerEnteredPlatform = true;
    }

    IEnumerator OnAttackAnimation()
    {
        yield return new WaitForSeconds(attackAnimTime);

        animator.SetBool("IsAttacking", false);
    }

    IEnumerator OnCastAnimation()
    {
        yield return new WaitForSeconds(castAnimTime);

        animator.SetBool("IsCasting", false);
    }

    IEnumerator OnRoarAnimation()
    {
        yield return new WaitForSeconds(roarAnimTime);

        animator.SetBool("IsRoaring", false);
    }

    IEnumerator OnHurtAnimation()
    {
        yield return new WaitForSeconds(hurtAnimTime);

        animator.SetBool("IsBeingHurt", false);
    }

    IEnumerator OnDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimTime);

        // TODO: Victory screen

        Destroy(gameObject, 5);
    }

    // ------
    // Stats
    // ------
    protected void GotHit()
    {
        if (health > 0)
        {
            switch (state)
            {
                case StateMachine.against_warlord:
                    health -= warlordDamage;
                    animator.SetBool("IsBeingHurt", true);
                    break;
                case StateMachine.against_ranger:
                    health -= rangerDamage;
                    animator.SetBool("IsBeingHurt", true);
                    break;
                case StateMachine.against_elementalist:
                    health -= elementalistDamage;
                    animator.SetBool("IsBeingHurt", true);
                    break;
                default:
                    break;
            }
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
        GUIStyle style = new GUIStyle();
        style.fontSize = 36;
        style.fontStyle = FontStyle.Bold;

        if (health > 75)
        {
            style.normal.textColor = Color.red;
        }
        else if (health > 49)
        {
            style.normal.textColor = Color.yellow;
        }
        else if (health > 20)
        {
            style.normal.textColor = Color.green;
        }
        else
        {
            style.normal.textColor = Color.white;
        }

        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);

        Rect healthCountLable = new Rect(pos.x - 58, pos.y + 245, 0.1f, 0.1f);
        GUI.Label(healthCountLable, health.ToString() + "%", style);
    }
}

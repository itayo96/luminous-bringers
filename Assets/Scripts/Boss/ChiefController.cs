using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiefController : EnemyController
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

    // Player
    public GameObject warlord, ranger, elementalist;

    // Private Movement Members
    private float runSpeed = 12f;
    private bool isFacingLeft = true;
    private float startingX = 0f;
    private float startingY = 0f;

    // Public Stats Members
    public int phase = 1;
    public bool isPhaseWithPlatforms = true;
    public int phaseStartingHealth = 100;
    public int phaseTransitionHealth = 65;

    public int warlordPartHealth = 100;
    public int rangerPartHealth = 93;
    public int elementalistPartHealth = 81;

    // Private Stats Members
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

    // Magic
    public GameObject redPowerBallToRight, redPowerBallToLeft;
    public GameObject bluePowerBallToRight, bluePowerBallToLeft;
    private float fireBallWaitTime;
    private float iceBallWaitTime;
    private float xBallPosition = 0.7f;

    // Platforms
    public GameObject leftProtectivePlatform, middleProtectivePlatform, rightProtectivePlatform;

    // --------
    // Starters
    // --------
    new void Start()
    {
        health = phaseStartingHealth;

        warlord.SetActive(false);
        ranger.SetActive(false);
        elementalist.SetActive(false);

        warlord.GetComponent<PlayerController>().OnInputEnabling(false);
        elementalist.GetComponent<PlayerController>().OnInputEnabling(false);
        ranger.GetComponent<PlayerController>().OnInputEnabling(false);

        // Starting against who
        if (health == warlordPartHealth)
        {
            state = StateMachine.against_warlord;
            warlord.SetActive(true);
            warlord.GetComponent<PlayerController>().OnInputEnabling(true);
        }
        else if (health == rangerPartHealth)
        {
            state = StateMachine.against_ranger;
            ranger.SetActive(true);
            ranger.GetComponent<PlayerController>().OnInputEnabling(true);
        }
        else if (health == elementalistPartHealth)
        {
            state = StateMachine.against_elementalist;
            elementalist.SetActive(true);
            elementalist.GetComponent<PlayerController>().OnInputEnabling(true);
        }
        else
        {
            state = StateMachine.death;
        }

        startingX = transform.position.x;
        startingY = transform.position.y;

        if (!isPhaseWithPlatforms)
        {
            playerEnteredPlatform = true;
        }
    }

    new void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // -------------------
    // Updaters & Checkers
    // -------------------
    new void Update() { }

    new void FixedUpdate()
    {
        // Resolve hurt effect set hurt flag to false
        if (isBeingHurt)
        {
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
        if (!playerEnteredPlatform && 
            (health == warlordPartHealth || health == rangerPartHealth || 
             health == elementalistPartHealth || health == phaseTransitionHealth))
        {
            switch (state)
            {
                case StateMachine.against_warlord:
                    middleProtectivePlatform.SetActive(true);
                    break;
                case StateMachine.against_ranger:
                    leftProtectivePlatform.SetActive(true);
                    break;
                case StateMachine.against_elementalist:
                    rightProtectivePlatform.SetActive(true);
                    break;
            }

            state = StateMachine.waiting_for_platform;
            return;
        }

        if (health == warlordPartHealth)
        {
            warlord.SetActive(true);
            warlord.GetComponent<PlayerController>().OnInputEnabling(true);

            elementalist.GetComponent<PlayerController>().OnInputEnabling(false);
            ranger.GetComponent<PlayerController>().OnInputEnabling(false);

            state = StateMachine.against_warlord;
            phaseStartingHealth = warlordPartHealth;
        }
        else if (health == rangerPartHealth)
        {
            ranger.SetActive(true);
            ranger.GetComponent<PlayerController>().OnInputEnabling(true);

            warlord.GetComponent<PlayerController>().OnInputEnabling(false);
            elementalist.GetComponent<PlayerController>().OnInputEnabling(false);
            
            state = StateMachine.against_ranger;
            phaseStartingHealth = rangerPartHealth;
        }
        else if (health == elementalistPartHealth)
        {
            elementalist.SetActive(true);
            elementalist.GetComponent<PlayerController>().OnInputEnabling(true);

            warlord.GetComponent<PlayerController>().OnInputEnabling(false);
            ranger.GetComponent<PlayerController>().OnInputEnabling(false);

            state = StateMachine.against_elementalist;
            phaseStartingHealth = elementalistPartHealth;
        }
        else if (health == phaseTransitionHealth)
        {
            if (IsAlive())
            {
                state = StateMachine.ultimate;
                warlord.SetActive(false);
                elementalist.SetActive(false);
                ranger.SetActive(false);
            }
            else
            {
                state = StateMachine.death;
            }
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
        // TODO: Ultimate, switch OST and get ready to transition to next phase scene
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
    public override void GotSlashedBySword()
    {
        if (state == StateMachine.against_warlord)
        {
            OnHit(swordHitDamage);
        }
    }

    public override void GotHitByElementalBall()
    {
        if (state == StateMachine.against_elementalist)
        {
            OnHit(elementalHitDamage);
        }
    }

    public override void GotHitByArrow()
    {
        if (state == StateMachine.against_ranger)
        {
            OnHit(arrowHitDamage);
        }
    }

    protected override void OnHit(int damage)
    {
        health -= damage;
        animator.SetBool("IsBeingHurt", true);
        isBeingHurt = true;

        if (health <= 0)
        {
            state = StateMachine.death;
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

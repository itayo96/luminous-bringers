using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChiefController : EnemyController
{
    enum StateMachine
    {
        against_warlord,
        against_ranger,
        against_elementalist,
        waiting_for_platform,
        ultimate,
        waiting_for_ultimate,
        death,
    }

    // Components
    private Rigidbody2D rigidBody;
    public Animator animator;

    // Player
    public GameObject warlord, ranger, elementalist;

    // Main Camera
    public GameObject mainCamera;

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
    public float attackAnimTime = 0.95f;
    public float castAnimTime = 0.95f;
    public float roarAnimTime = 0.95f;
    public float hurtAnimTime = 0.95f;
    public float deathAnimTime = 2f;

    // Animator Flags
    private bool isBeingHurt = false;
    private bool isAttacking = false;
    private bool isCasting = false;
    private bool isRoaring = false;

    // Magic
    public GameObject redPowerBallToLeft;
    public GameObject bluePowerBallToLeft;
    private float fireBallWaitTime;
    private float iceBallWaitTime;
    private float xBallPosition = 0.7f;

    // Platforms
    public GameObject leftProtectivePlatform, middleProtectivePlatform, rightProtectivePlatform;

    // Ultimate
    public GameObject fadeOut;
    public float enrageInSeconds;

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

        // Enrage
        if (state != StateMachine.ultimate && 
            state != StateMachine.waiting_for_ultimate && 
            enrageInSeconds != 0 && 
            Time.timeSinceLevelLoad >= enrageInSeconds)
        {
            state = StateMachine.ultimate;
            enrageInSeconds = 0;
            fadeOut.GetComponent<Image>().color = Color.black;
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
            case StateMachine.waiting_for_ultimate:
                return;
            case StateMachine.ultimate:
                state = StateMachine.waiting_for_ultimate;
                UltimateState();
                return;
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

            if (!isPhaseWithPlatforms)
            {
                ranger.SetActive(false);
                elementalist.SetActive(false);
            }
            else
            {
                playerEnteredPlatform = false;
            }
        }
        else if (health == rangerPartHealth)
        {
            ranger.SetActive(true);
            ranger.GetComponent<PlayerController>().OnInputEnabling(true);

            warlord.GetComponent<PlayerController>().OnInputEnabling(false);
            elementalist.GetComponent<PlayerController>().OnInputEnabling(false);
            
            state = StateMachine.against_ranger;
            phaseStartingHealth = rangerPartHealth;

            if (!isPhaseWithPlatforms)
            {
                warlord.SetActive(false);
                elementalist.SetActive(false);
            }
            else
            {
                playerEnteredPlatform = false;
            }
        }
        else if (health == elementalistPartHealth)
        {
            elementalist.SetActive(true);
            elementalist.GetComponent<PlayerController>().OnInputEnabling(true);

            warlord.GetComponent<PlayerController>().OnInputEnabling(false);
            ranger.GetComponent<PlayerController>().OnInputEnabling(false);

            state = StateMachine.against_elementalist;
            phaseStartingHealth = elementalistPartHealth;

            if (!isPhaseWithPlatforms)
            {
                warlord.SetActive(false);
                ranger.SetActive(false);
            }
            else
            {
                playerEnteredPlatform = false;
            }
        }
        else if (health == phaseTransitionHealth)
        {
            if (IsAlive())
            {
                if (state != StateMachine.waiting_for_ultimate)
                {
                    state = StateMachine.ultimate;
                }
            }
            else
            {
                state = StateMachine.death;
            }
        }
    }

    // -----------
    // States
    // -----------
    void WarlordState()
    {
        // TODO: Check how far from player.
        //          If far - Move
        //          If close - Attack and then return to a random location between current and starting

        health = 93;
    }

    void RangerState()
    {
        // TODO: Summon the dance components

        health = 65;
    }

    void ElementalistState()
    {
        // TODO: Summon the power ball combo (+ audio + text)

        health = 77;
    }

    void WaitingState()
    {
        // TODO: If first time in waiting state, move to starting position (and face left)
        //              else, do nothing
    }

    void UltimateState()
    {
        warlord.GetComponent<PlayerController>().OnInputEnabling(false);
        elementalist.GetComponent<PlayerController>().OnInputEnabling(false);
        ranger.GetComponent<PlayerController>().OnInputEnabling(false);

        // Switch OST
        mainCamera.GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().Play();

        animator.SetBool("IsRoaring", true);
        StartCoroutine(OnRoarAnimation());

        StartCoroutine(UltimateAttack());
    }

    IEnumerator UltimateAttack()
    {
        yield return new WaitForSeconds(5f);

        animator.SetBool("IsRoaring", true);
        StartCoroutine(OnRoarAnimation());

        yield return new WaitForSeconds(5f);

        animator.SetBool("IsRoaring", true);
        StartCoroutine(OnRoarAnimation());

        // Screen shattering
        Vector3 initialPosition = mainCamera.transform.localPosition;
        for (int i = 0; i < 50; i++)
        {
            mainCamera.transform.localPosition = initialPosition + Random.insideUnitSphere * 0.4f;
            yield return new WaitForSeconds(0.1f);
        }
        mainCamera.transform.localPosition = initialPosition;

        // Show some meteor coming down
        mainCamera.transform.localPosition = mainCamera.transform.localPosition + new Vector3(0, -12.13f, 0);

        yield return new WaitForSeconds(10f);

        // Remove boss from the screen
        GetComponent<Animator>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = null;

        // Return to main camera
        mainCamera.transform.localPosition = initialPosition;

        for (int i = 0; i < 70; i++)
        {
            mainCamera.transform.localPosition = initialPosition + Random.insideUnitSphere * 0.4f;
            yield return new WaitForSeconds(0.1f);
        }
        mainCamera.transform.localPosition = initialPosition;

        // White screen
        for (int i = 0; i < 30; i++)
        {
            mainCamera.transform.localPosition = initialPosition + Random.insideUnitSphere * 0.4f;
            fadeOut.GetComponent<RectTransform>().localScale += new Vector3(0.5f, 0.5f, 0.5f);
            yield return new WaitForSeconds(0.2f);
        }

        fadeOut.GetComponent<Image>().material = null;

        yield return new WaitForSeconds(6.4f);

        if (enrageInSeconds == 0f)
        {
            yield return new WaitForSeconds(6.5f);
        }

        // Transition based on player's situation
        // Moving to second phase if all three platforms are protecting the player,
        // else (Timer is up for either phase one or phase two) restart first scene. 
        if (phase == 1 &&
            leftProtectivePlatform.GetComponent<ProtectivePlatform>().WasUsed() &&
            middleProtectivePlatform.GetComponent<ProtectivePlatform>().WasUsed() &&
            rightProtectivePlatform.GetComponent<ProtectivePlatform>().WasUsed())
        {
            SceneManager.LoadScene("BossSecondPhaseScene");
        }
        else
        {
            SceneManager.LoadScene("BossFirstPhaseScene");
        }
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

    public override void GotHitByElementalBall(string ballTag)
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
        if (state == StateMachine.ultimate || state == StateMachine.waiting_for_ultimate)
        {
            return;
        }

        GUIStyle style = new GUIStyle();
        style.fontSize = 36;
        style.fontStyle = FontStyle.Bold;

        if (health > 60)
        {
            style.normal.textColor = Color.white;
        }
        else if (health > 32)
        {
            style.normal.textColor = Color.green;
        }
        else if (health > 9)
        {
            style.normal.textColor = Color.yellow;
        }
        else
        {
            style.normal.textColor = Color.red;
        }

        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);

        Rect healthCountLable = new Rect(pos.x - 58, pos.y + 245, 0.1f, 0.1f);
        GUI.Label(healthCountLable, health.ToString() + "%", style);
    }
}

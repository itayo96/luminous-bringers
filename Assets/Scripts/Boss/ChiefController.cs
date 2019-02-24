using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [System.Serializable]
    public struct Combo
    {
        public string firstBall;
        public string secondBall;
        public string text;
        public AudioSource voice;
        public bool wasUsed;
    }

    // Components
    private Rigidbody2D rigidBody;
    public Animator animator;

    // Player Objects
    public GameObject warlord, ranger, elementalist;

    // Main Camera
    public GameObject mainCamera;

    // Private Movement Members
    public float runSpeed = 12f;
    private float horizontalMove = 0f;
    private float moveBeforeHurt = 0f;
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
    private bool canBeAttacked = false;
    private string damagingElementalBallTag = "";

    // Public Animation Timers
    public float attackAnimTime = 5f;
    public float castAnimTime = 0.95f;
    public float roarAnimTime = 0.95f;
    public float hurtAnimTime = 0.95f;
    public float deathAnimTime = 2f;

    // Platforms
    public GameObject leftProtectivePlatform, middleProtectivePlatform, rightProtectivePlatform;

    // Ultimate
    public GameObject fadeOut;
    public float enrageInSeconds;

    private float nextActionTime = 0f;

    // Axe
    public float axeSlashRange = 1.7f;
    public Transform attackPosition;
    private float axeSlashRefreshRate = 6f;

    // Magic
    public GameObject redPowerBallToLeft;
    public GameObject bluePowerBallToLeft;
    public Dialog dialog;
    public Combo[] combos;

    private float comboRefreshRate = 15f;
    
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
    new void FixedUpdate() { }

    new void Update()
    {
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
                return;
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

            canBeAttacked = false;
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
            nextActionTime = Time.time + 2f;
            dialog.timeout = 7f; // Setting dialog timeout to be 1.5 seconds
            dialog.player = null;

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
        if (animator.GetBool("IsBeingHurt"))
        {
            if (Time.time >= nextActionTime)
            {
                nextActionTime = Time.time + 1;
            }
            return;
        }

        Move();

        // Stop for wait time or reached starting x position
        if (Time.time >= nextActionTime - 0.5f && Time.time < nextActionTime)
        {
            horizontalMove = 0;
        }

        if (transform.position.x >= startingX)
        {
            horizontalMove = 0;
            nextActionTime = Time.time - 1f;
        }

        // Check if enough time had passed since last sword slash then just keep walking
        if (Time.time < nextActionTime)
        {
            return;
        }

        nextActionTime = 0f;

        // On far
        if (Vector3.Distance(attackPosition.transform.position, warlord.transform.position) >= axeSlashRange)
        {
            // Move towards the player
            horizontalMove = -1;

            // Cannot be attacked
            canBeAttacked = false;
        }
        else // On close
        {
            horizontalMove = 0;
            animator.SetFloat("Speed", 0);

            // Attack
            Attack();

            // Wait alittle (random time) till next slash / next trying to reach the player
            nextActionTime = Time.time + Random.Range(axeSlashRefreshRate - 4f, axeSlashRefreshRate + 1f);
        }
    }

    void RangerState()
    {
        // TODO: Summon the dance components

        if (phase == 1)
        {
            health = 65;
        }
        else
        {
            health = 11;
        }
    }

    void ElementalistState()
    {
        // Check if enough time had passed since last combo
        if (Time.time < nextActionTime)
        {
            return;
        }

        // Choose randomly from possible combos
        //      If phase 1: remove the used combo from the list
        int chosenComboIndex;
        List<int> availableComboIndices = new List<int>();

        for (int i = 0; i < combos.Length; i++)
        {
            if (!combos[i].wasUsed)
            {
                availableComboIndices.Add(i);
            }
        }

        if (availableComboIndices.Count == 0) // If all was used, they all are available
        {
            availableComboIndices.AddRange(Enumerable.Range(0, combos.Length));
        }

        chosenComboIndex = availableComboIndices[Random.Range(0, availableComboIndices.Count)];

        dialog.textDisplay.color = Color.black;
        if (phase == 1)
        {
            combos[chosenComboIndex].wasUsed = true;
            dialog.textDisplay.color = (combos[chosenComboIndex].firstBall == "red") ? Color.red : Color.blue;
        }

        // Text
        dialog.Display(combos[chosenComboIndex].text);

        // OST
        if (combos[chosenComboIndex].voice != null)
        {
            combos[chosenComboIndex].voice.Play();
        }

        nextActionTime = Time.time + comboRefreshRate;

        // Can be attacked now by opposite of the first ball
        damagingElementalBallTag = (combos[chosenComboIndex].firstBall == "red") ? "IceBall" : "FireBall";
        canBeAttacked = true;

        // Cast the combo
        StartCoroutine(CastCombo(combos[chosenComboIndex].firstBall, combos[chosenComboIndex].secondBall));

    }

    IEnumerator CastCombo(string first, string second)
    {
        // Wait
        yield return new WaitForSeconds(1f);

        // Cast first ball
        Cast(first);

        // Wait
        yield return new WaitForSeconds(7f);

        // Do not finish combo if moving to next phase
        if (state == StateMachine.against_elementalist)
        {
            // Cast second ball
            Cast(second);

            yield return new WaitForSeconds(0.5f);
        }

        // Cannot be attacked anymore
        damagingElementalBallTag = "";
        canBeAttacked = false;
    }

    void WaitingState()
    {
        horizontalMove = 0;

        if (transform.position.x < startingX - 1.5f)
        {
            horizontalMove = 2;
            Move();
        }
        else
        {
            animator.SetFloat("Speed", 0);
            if (!isFacingLeft)
            {
                isFacingLeft = !isFacingLeft;

                // Multiply the character's x local scale by -1.
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }
        }
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

        dialog.textDisplay.color = Color.black;
        dialog.timeout = 4f;
        dialog.Display("RAAAA!!! MEET YOUR MAKER!");

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
    void Move()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        float move = horizontalMove * Time.fixedDeltaTime * runSpeed;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, rigidBody.velocity.y);

        // Smoothing the velocity out and applying it to the character
        Vector3 velocity = Vector3.zero;
        rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, 0.05f);

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
            if (colliders[i].GetComponent<WarlordController>() != null)
            {
                StartCoroutine(OnAttackAnimation(colliders[i].gameObject));
            }

            // Add to damaged enemies
            objects.Add(colliders[i].gameObject);
        }
    }

    void Cast(string powerBall)
    {
        animator.SetBool("IsCasting", true);

        if (powerBall == "blue")
        {
            StartCoroutine(OnCastAnimation(bluePowerBallToLeft));
        }
        else if (powerBall == "red")
        {
            StartCoroutine(OnCastAnimation(redPowerBallToLeft));
        }
    }

    void Roar() { }

    // ------
    // Events
    // ------
    public void OnPlayerPlatform()
    {
        playerEnteredPlatform = true;
    }

    IEnumerator OnAttackAnimation(GameObject player)
    {
        if (!isFacingLeft)
        {
            isFacingLeft = !isFacingLeft;

            // Multiply the character's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        yield return new WaitForSeconds(0.2f);

        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.5f);

        bool hadBlocked = false;
        bool isInRange = false;

        for (float i = 0; i < attackAnimTime - 0.5f; i+=0.1f)
        {
            // Stop if player blocked
            if (player.GetComponent<WarlordController>().IsBlocking())
            {
                hadBlocked = true;
                canBeAttacked = true;
                animator.SetBool("GotParried", true);
                break;
            }

            // Check if still in range while attacking
            if (Vector3.Distance(attackPosition.transform.position, warlord.transform.position) >= axeSlashRange)
            {
                isInRange = false;
            }
            else
            {
                isInRange = true;
            }

            yield return new WaitForSeconds(0.1f);
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
            yield return new WaitForSeconds(0.3f);
        }

        animator.SetBool("IsAttacking", false);


        // Can be attacked on phase 1 anyway
        if (phase == 1)
        {
            canBeAttacked = true;
        }

        horizontalMove = 1;
    }

    IEnumerator OnCastAnimation(GameObject powerBall)
    {
        yield return new WaitForSeconds(castAnimTime);

        Vector2 powerBallPosition = transform.position;

        powerBallPosition += new Vector2(-2f, -0.5f);
        Instantiate(powerBall, powerBallPosition, Quaternion.identity);

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

        horizontalMove = moveBeforeHurt;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
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
        // Can be damaged only by wanted ball tag
        if (damagingElementalBallTag != ballTag)
        {
            return;
        }

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
        if (!canBeAttacked)
        {
            return;
        }

        health -= damage;
        animator.SetBool("IsBeingHurt", true);

        moveBeforeHurt = horizontalMove;
        animator.SetFloat("Speed", 0);
        horizontalMove = 0;

        canBeAttacked = false;
        StartCoroutine(OnHurtAnimation());

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.transform.position, axeSlashRange);
    }
}

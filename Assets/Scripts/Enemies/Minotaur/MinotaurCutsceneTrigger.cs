using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineCutscene : MonoBehaviour
{
    [Header("Timeline References")]
    public PlayableDirector timelineDirector;
    public GameObject minotaur;
    public GameObject cutsceneSkeleton; // This is the actual skeleton in the scene
    public Transform player;

    [Header("Settings")]
    public bool destroyAfterCutscene = true;

    private bool hasPlayed = false;
    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private MinotaurFight minotaurFight;
    private Animator minotaurAnimator;
    private Animator playerAnimator;
    private Enemy skeletonEnemy; // Store skeleton's enemy component
    private Collider2D skeletonCollider; // Store skeleton's collider

    void Start()
    {
        // Get player components
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerCombat = player.GetComponent<PlayerCombat>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // Get minotaur components and disable them initially
        if (minotaur != null)
        {
            minotaurFight = minotaur.GetComponent<MinotaurFight>();
            minotaurAnimator = minotaur.GetComponent<Animator>();

            if (minotaurFight != null) minotaurFight.enabled = false;
            if (minotaurAnimator != null) minotaurAnimator.enabled = false;
        }

        // NEW: Skeleton starts as normal enemy - no changes until cutscene
        if (cutsceneSkeleton != null)
        {
            // Store references but don't disable anything
            skeletonEnemy = cutsceneSkeleton.GetComponent<Enemy>();
            skeletonCollider = cutsceneSkeleton.GetComponent<Collider2D>();

            // Skeleton remains fully functional - player can fight it normally
            Debug.Log("Skeleton is active and vulnerable before cutscene");
        }

        Debug.Log("Cutscene ready - Minotaur disabled, Skeleton active");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasPlayed)
        {
            StartCutscene();
        }
    }

    public void StartCutscene()
    {
        if (hasPlayed) return;

        hasPlayed = true;
        StartCoroutine(PlayTimelineCutscene());
    }

    private IEnumerator PlayTimelineCutscene()
    {
        Debug.Log("Starting Timeline cutscene...");

        // Step 1: Make skeleton invulnerable and prepare for cutscene
        PrepareSkeletonForCutscene();

        // Step 2: Force player to idle animation
        ForcePlayerToIdle();

        // Step 3: Disable player control
        DisablePlayerControl();

        // Step 4: Enable minotaur animator for Timeline
        if (minotaurAnimator != null)
        {
            minotaurAnimator.enabled = true;
            Debug.Log("Minotaur animator enabled for Timeline");
        }

        // Step 5: Play the Timeline
        timelineDirector.Play();
        Debug.Log("Timeline started playing");

        // Step 6: Wait for Timeline to finish
        yield return new WaitForSeconds((float)timelineDirector.duration);

        // Step 7: End cutscene and start battle
        EndCutscene();

        // Step 8: Destroy this GameObject to prevent interference
        if (destroyAfterCutscene)
        {
            Debug.Log("Destroying cutscene trigger to prevent interference");
            Destroy(gameObject);
        }
    }

    // NEW: Prepare skeleton for cutscene (make invulnerable, disable AI, etc.)
    private void PrepareSkeletonForCutscene()
    {
        if (cutsceneSkeleton != null)
        {
            Debug.Log("Preparing skeleton for cutscene");

            // Make skeleton invulnerable during cutscene
            if (skeletonEnemy != null)
            {
                skeletonEnemy.enabled = false; // Disable enemy behavior
                // Optional: Make sure skeleton is at full health for dramatic effect
                // skeletonEnemy.currentHealth = skeletonEnemy.maxHealth;
            }

            // Disable collisions so player doesn't interfere during cutscene
            if (skeletonCollider != null)
            {
                skeletonCollider.enabled = false;
            }

            // Stop any skeleton movement
            Rigidbody2D skeletonRb = cutsceneSkeleton.GetComponent<Rigidbody2D>();
            if (skeletonRb != null)
            {
                skeletonRb.velocity = Vector2.zero;
                skeletonRb.angularVelocity = 0f;
            }

            // Disable any AI scripts
            MonoBehaviour[] skeletonScripts = cutsceneSkeleton.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in skeletonScripts)
            {
                if (script != null && script != this && script.enabled)
                {
                    script.enabled = false;
                }
            }
        }
    }

    private void ForcePlayerToIdle()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("speed", 0f);

            playerAnimator.ResetTrigger("attack");
            playerAnimator.ResetTrigger("air_attack");
            playerAnimator.ResetTrigger("crouch_attack");
            playerAnimator.Play("Idle");
            Debug.Log("Player forced to idle animation");
        }

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null) playerRb.velocity = Vector2.zero;
    }

    // TIMELINE SIGNAL: Called when skeleton should be destroyed during cutscene
    public void OnSkeletonDestroy()
    {
        Debug.Log("Cutscene: Destroying skeleton dramatically");

        if (cutsceneSkeleton != null)
        {
            PlaySkeletonDestructionEffects();
            Destroy(cutsceneSkeleton, 0.1f); // Small delay for effects to play
        }
    }

    private void PlaySkeletonDestructionEffects()
    {
        if (cutsceneSkeleton != null)
        {
            // Play death particles
            if (skeletonEnemy != null && skeletonEnemy.deathParticles != null)
            {
                ParticleSystem particles = Instantiate(skeletonEnemy.deathParticles,
                    cutsceneSkeleton.transform.position, Quaternion.identity);
                particles.Play();
                Destroy(particles.gameObject, particles.main.duration);
            }

            // Add dramatic physics effects
            Rigidbody2D skeletonRb = cutsceneSkeleton.GetComponent<Rigidbody2D>();
            if (skeletonRb != null)
            {
                skeletonRb.velocity = new Vector2(0f, 12f); // Higher for more drama
                skeletonRb.AddTorque(300f); // More spin

                // Optional: Add some horizontal force based on minotaur position
                Vector2 horizontalForce = (cutsceneSkeleton.transform.position - minotaur.transform.position).normalized * 3f;
                skeletonRb.AddForce(horizontalForce, ForceMode2D.Impulse);
            }

            // Play any skeleton death sound
            AudioSource skeletonAudio = cutsceneSkeleton.GetComponent<AudioSource>();
            if (skeletonAudio != null && skeletonAudio.clip != null)
            {
                skeletonAudio.Play();
            }
        }
    }

    private void DisablePlayerControl()
    {
        if (playerController != null) playerController.enabled = false;
        if (playerCombat != null) playerCombat.enabled = false;

        Debug.Log("Player control disabled");
    }

    private void EnablePlayerControl()
    {
        if (playerController != null) playerController.enabled = true;
        if (playerCombat != null) playerCombat.enabled = true;

        Debug.Log("Player control enabled");
    }

    private void EndCutscene()
    {
        Debug.Log("Ending cutscene, starting battle...");

        // Enable minotaur fight system
        if (minotaurFight != null)
        {
            minotaurFight.enabled = true;
            Debug.Log("Minotaur fight system enabled");
        }

        if (minotaurAnimator != null)
        {
            minotaurAnimator.enabled = true;
            Debug.Log("Minotaur animator enabled for battle");
        }

        // Make sure skeleton is destroyed (in case Timeline signal didn't fire)
        if (cutsceneSkeleton != null)
        {
            Destroy(cutsceneSkeleton);
        }

        // Re-enable player control
        EnablePlayerControl();

        Debug.Log("Minotaur battle begins!");
    }

    void Update()
    {
        if (timelineDirector.state == PlayState.Playing && Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    public void SkipCutscene()
    {
        Debug.Log("Skipping cutscene");
        timelineDirector.Stop();
        EndCutscene();

        if (destroyAfterCutscene)
        {
            Debug.Log("Destroying cutscene trigger after skip");
            Destroy(gameObject);
        }
    }
}
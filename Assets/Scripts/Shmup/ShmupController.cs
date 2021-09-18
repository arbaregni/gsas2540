using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShmupController : QuadMover, IStrikable
{
    [SerializeField]
    int initialLives = 3;

    [SerializeField]
    float fireCoolDown = .2f;
    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    AudioSource hitSound;
    [SerializeField]
    AudioSource gameOverSound;

    public delegate void OnGameOver();
    public static event OnGameOver OnGameOverEvent;

    private IconTracker lifeTracker = null;

    private bool isDead = false;

    private float timeStampOfLastFire = 0f;
    private bool isHoldingFire = false;

    public int ShotsFired { get; private set; }
    public int Lives
    {
        get => lifeTracker.Count;
        set => lifeTracker.Count = value;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        lifeTracker = GameObject.Find("LifeTracker").GetComponent<IconTracker>();
        Lives = initialLives;
        ShotsFired = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (isDead) return;

        horizontality = (int)Input.GetAxisRaw("Horizontal");
        verticality = (int)Input.GetAxisRaw("Vertical");


        if ((Time.time - timeStampOfLastFire > fireCoolDown) && Input.GetButtonDown("Fire"))
        {
            // the player is now aiming the weapon

            isHoldingFire = true;
            Speed = 0f;
            
            animator.SetBool("Attacking", true);
        }
        if (isHoldingFire && Input.GetButtonUp("Fire"))
        {
            // the player has released, so we fire a projectile

            ShotsFired += 1;
            timeStampOfLastFire = Time.time;
            isHoldingFire = false;
            Speed = fullSpeed;

            // the player may have released all directionality since firing,
            // recover the previous direction we were holding so now we need to find what the last nonzero directionality was,
            // which is stored by the animator

            horizontality = animator.GetInteger("Horizontality");
            verticality = animator.GetInteger("Verticality");

            // Debug.Log($"instantiating arrow, heading = {Heading}, rot = {HeadingAsQuaternion}");
            Instantiate(projectilePrefab, transform.position, HeadingAsQuaternion);

            animator.SetBool("Attacking", false);
        }

        base.Update();
    }

    public void StrikeMe(GameObject _projectile)
    {
        if (isDead) return;

        Lives -= 1;
        if (Lives == 0)
        {
            GameOver();
            return;
        }
        hitSound.Play();
        // animation?
    }

    void GameOver()
    {
        rb.isKinematic = true;
        Speed = 0;
        gameOverSound.Play();
        isDead = true;
        Invoke(nameof(_GameOver), gameOverSound.clip.length);
    }

    void _GameOver()
    {
        OnGameOverEvent.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Shmup_GameOver");
    }

    
}

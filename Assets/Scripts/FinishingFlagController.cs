using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishingFlagController : MonoBehaviour
{
    [Header("FlagPoleController")]
    public bool isFinished = false;
    public bool isPlayerTouchingGround;                     //checks if player is touching ground and player touching flag pole

    private Animator finishingFlagAnimator;
    private BoxCollider2D finishingFlagBoxCollider;

    public AudioClip music_Flagpole;
    public AudioClip music_StageClear;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        finishingFlagAnimator = GetComponent<Animator>();
        finishingFlagBoxCollider = GetComponent<BoxCollider2D>();
        finishingFlagAnimator.SetBool("IsFinished", isFinished);
        source = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
         if (collision.gameObject.tag == "Player")
        {
            source.PlayOneShot(music_Flagpole);
            source.PlayOneShot(music_StageClear);
            isFinished = true;
            finishingFlagAnimator.SetBool("IsFinished", isFinished);
            collision.gameObject.GetComponent<PlayerController>().isTouchingFlagpole = true;
            finishingFlagBoxCollider.enabled = false;
            collision.gameObject.GetComponent<PlayerController>().playerRigidbody2D.velocity.Set(0, 1f);
        }
    }

   
}

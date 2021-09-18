using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField,TextArea]
    private string adjacentMovementText;
    [SerializeField,TextArea]
    private string nearMovementText;
    [SerializeField,TextArea]
    private string farMovementText;

    [SerializeField,TextArea]
    private string onKillText;
    [SerializeField,TextArea]
    private string onMissText;


    [SerializeField]
    private float adjacentSwingChance;

    [SerializeField]
    private float nearSwingChance;

    [SerializeField]
    private AudioSource onWalkAudio;

    private Text messageText;

    private PlayerController player;

    // is there a way to edit this through the unity editor?
    private Vector2[] positionSequence = new Vector2[]
    {
        new Vector2(-2,-2),
        new Vector2(-2,-1),
        new Vector2(-2, 0),
        new Vector2(-2, 1),
        new Vector2(-2, 2),
        new Vector2(-1, 2),
        new Vector2( 0, 2),
        new Vector2( 1, 2),
        new Vector2( 2, 2),
        new Vector2( 2, 1),
        new Vector2( 2, 0),
        new Vector2( 2,-1),
        new Vector2( 2,-2),
        new Vector2( 1,-2),
        new Vector2( 0,-2),
        new Vector2(-1,-2),
        // wraps around to (-2,-2)
};
    private int idx;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        messageText = GameObject.Find("MessageText").GetComponent<Text>();

        // choose random location?
        idx = 0;
        transform.position = positionSequence[idx];
    }

    public void TakeTurn()
    {
        // wait for all the audio to stop playing
        // while (ForestManager.Instance.AudioIsPlaying()) ;

        // what we do depends on how close the player is
        float dist = (player.transform.position - transform.position).magnitude;
        float vol = ForestManager.ScaleVolume(dist);
        // it also depends on a random variable
        float r = Random.Range(0f, 1f);

        Debug.Log($"Enemy's turn, dist = {dist}, r = {r}");

        // check for attacking
        if (   player.TurnNum == 2
            || (0.0f <= dist && dist <= 1.1f && r <= adjacentSwingChance)
            || (1.0f <  dist && dist <= 2.0f && r <= nearSwingChance)
            )
        {
            Debug.Log("enemy is attacking");
            // we are attacking this turn

            if (dist <= ForestManager.Instance.adjacentDist)
            {
                // it's a hit! the player is dead.

                ForestManager.Instance.onHitAudio.Play();
                
                messageText.text += $"\n{onKillText}";

                player.StopAcceptingInput();
            }
            else
            {
                // play the miss sound
                ForestManager.Instance.onMissAudio.volume = vol;
                ForestManager.Instance.onMissAudio.Play();

                messageText.text += $"\n{onMissText}";
            }
        }
        else
        {
            Debug.Log("Enemy is moving");
            // we move this turn,
            //  to the next position in the sequence (which wraps around)
            idx += 1;
            transform.position = positionSequence[idx % positionSequence.Length];

            // TODO: change volume based on distance to player
            ForestManager.Instance.onWalkAudio.volume = vol;
            ForestManager.Instance.onWalkAudio.Play();

            if (dist <= ForestManager.Instance.adjacentDist)
            {
                messageText.text += $"\n{adjacentMovementText}";
            }
            else if (dist <= ForestManager.Instance.nearDist)
            {
                messageText.text += $"\n{nearMovementText}";
            }
            else if (dist <= ForestManager.Instance.nearDist)
            {
                messageText.text += $"\n{farMovementText}";
            }

        }



    }
}

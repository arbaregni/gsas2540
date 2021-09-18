using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField,TextArea]
    private string winText;
    [SerializeField,TextArea]
    private string timeOutText;
    [SerializeField, TextArea]
    private string missText = "It misses. There is nothing immediately next to you right now.";
    [SerializeField, TextArea]
    private string swingText = "You swing your axe.";
    [SerializeField, TextArea]
    private string outOfSwingsText = "You are too weary to swing your axe.";


   [SerializeField]
    int totalSwings;

    [SerializeField]
    private Text swingCounter;
    private int _currSwings;
    public int CurrSwings
    {
        private set
        {
            _currSwings = value;
            string plural = CurrSwings == 1 ? "" : "s";
            swingCounter.text = $"You have {_currSwings} axe swing{plural} left.";
        }
        get => _currSwings;
    }

    [SerializeField]
    private Text turnCounter;
    private int _turnNum;
    public int TurnNum
    {
        private set
        {
            _turnNum = value;
            string plural = _turnNum == 1 ? "" : "s";
            turnCounter.text = $"You have survived {_turnNum} turn{plural}.";
        }
        get => _turnNum;
    }

    private bool rejectInput;

    private EnemyController enemy;


    [SerializeField]
    private Text messageText;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GameObject.Find("Enemy").GetComponent<EnemyController>();
        TurnNum = 0;
        CurrSwings = totalSwings;
        rejectInput = false;
    }

    public void StopAcceptingInput()
    {
        rejectInput = true;
        Text restartButtonText = GameObject.Find("RestartButton").GetComponentInChildren<Text>();
        restartButtonText.text = "Play Again?";
    }

    // Delegate the stepping in each of the 4 directions to a more general function
    public void StepNorth()
    {
        Step(Vector3.up, "north");
    }
    public void StepSouth()
    {
        Step(Vector3.down, "south");
    }
    public void StepWest()
    {
        Step(Vector3.left, "west");
    }
    public void StepEast()
    {
        Step(Vector3.right, "east");
    }

    public void Step(Vector3 delta, string direction)
    {
        if (ForestManager.Instance.AudioIsPlaying() || rejectInput) return;

        if (!ForestManager.Instance.InBounds(transform.position + delta))
        {
            // tell player they can't move out of bounds

            ForestManager.Instance.outOfBoundsAudio.Play();
            messageText.text += "\nYou can't go any further.";
            
            return;
        }
        transform.position += delta;

        messageText.text += $"\nYou walk {direction}.";

        EndTurn();
    }

    public void Swing()
    {
        if (ForestManager.Instance.AudioIsPlaying() || rejectInput) return;

        if (CurrSwings <= 0)
        {
            messageText.text += $"\n{outOfSwingsText}";
            ForestManager.Instance.outOfBoundsAudio.Play();
            return;
        }

        CurrSwings -= 1;

        // perform the swing action
        messageText.text += $"\n{swingText}";

        // play the swing sound

        Vector2 toEnemy = enemy.transform.position - transform.position;

        if (toEnemy.magnitude < ForestManager.Instance.adjacentDist)
        {
            // inform player of victory !
            ForestManager.Instance.onHitAudio.Play();

            messageText.text += $"\n{winText}";

            StopAcceptingInput();
            return;
        }
        else
        {
            ForestManager.Instance.onMissAudio.Play();

            messageText.text += $"\n{missText}";
        }

        EndTurn();
    }
    public void Listen()
    {
        // play an ominous message
        messageText.text += "\nYou listen to the sounds of the forest.";

        EndTurn();
    }
    public void EndTurn()
    {
        TurnNum += 1;

        if (TurnNum > 30 && CurrSwings == 0)
        {
            // no point playing after this
            messageText.text += $"\n{timeOutText}";

            StopAcceptingInput();
            return;

        }
        // the enemy can go after all the audio has finished
        Invoke("EnemyTurn", ForestManager.Instance.AudioWait);
    }

    public void EnemyTurn()
    {
        enemy.TakeTurn();
    }
}

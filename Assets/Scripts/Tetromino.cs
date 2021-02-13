using UnityEngine;


public class Tetromino : MonoBehaviour
{
    public float fall = 0;                         // count timer for fall speed
    private float fallSpeed = 1;
    public bool allowRotation = true;              // we use this to specify if we want to allow the tetromino to rotate
    public bool limitRotation = false;             // this is used to limit the rotation of the tetromino to a 90/-90 rotation. (to / from)

    public int individualScore = 100;
    public float individualScoreTime;

    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    private AudioSource audioSource;

    private float continuousVerticalSpeed = 0.054f;  // the speed at which the tetrmino will move when the down button is held down
    private float continuousHorizontalSpeed = 0.1f;  // the speed at which the tetrmino will move when the left or right buttons are held down
    private float buttonDownWaitMax = 0.2f;          // how long to wait before the tetromino recognizes that a button is being held down

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerHorizontal = 0;
    private float buttonDownWaitTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (!Game.isPaused)
        {
            // we need to call the checuserýnput method so that the tetromino will be property positioned
            CheckUserInput();
            UpdateIndividualScore();
            UpdateFallSpeed();
        }
    }

    void UpdateFallSpeed()
    {
        fallSpeed = Game.fallSpeed;
    }

    private void PlayMoveAudio()
    {
        audioSource.PlayOneShot(moveSound);
    }

    void PlayRotateAudio()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }

    public void UpdateIndividualScore()
    {
        if (individualScoreTime < 1)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    public void CheckUserInput()
    {
        // This method checks the keys that the player can press tý manipulate the position of the tetromino
        // The options here will be left, right , up and down
        // Left and right will move the tetromino one unit to the left or right
        // Down will move the tetromino 1 unit down
        // Up will rotate the tetromino

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {

            movedImmediateHorizontal = false;

            horizontalTimer = 0;

            buttonDownWaitTimerHorizontal = 0;

        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.S) || Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Rotate();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SlamDown();
        }

    }

    public void SlamDown()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition())
        {
            
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Game>().UpdateGrid(this);

            Game.currentScore += individualScore;



            enabled = false;
            tag = "Untagged";
            PlayLandAudio();

            // Spawn the next piece
            FindObjectOfType<Game>().SpawnNextTetromino();
            FindObjectOfType<Game>().DeleteRow();

            // Check if there are any minos above the grid
            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game>().GameOver();
            }
        }
    }

    public bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 position = FindObjectOfType<Game>().Round(mino.position);
            if (FindObjectOfType<Game>().CheckIsInsideGrid(position) == false)
            {
                return false;
            }
            else if (FindObjectOfType<Game>().GetTransformAtGridPosition(position) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(position).parent != transform)
            {
                return false;
            }
        }

        return true;
    }

    void MoveLeft()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }

        horizontalTimer = 0;

        // first we attempt to move the tetromino to the left
        transform.position += new Vector3(-1, 0, 0);

        // we then check if the tetromino is at a valid position
        if (CheckIsValidPosition())
        {
            // if it is, we then call the UpdateGrid method which records this tetromino new position
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            // if it isn't we move the tetromino back to the right
            transform.position += new Vector3(1, 0, 0);
        }
    }

    void MoveRight()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }
        horizontalTimer = 0;

        // first we attempt to move the tetromino to the right
        transform.position += new Vector3(1, 0, 0);

        // we then check if the tetromino is at a valid position
        if (CheckIsValidPosition())
        {
            // if it is, we then call the UpdateGrid method which records this tetromino new position
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            // if it isn't we move the tetromino back to the left
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    void MoveDown()
    {
        if (movedImmediateVertical)
        {
            if (buttonDownWaitTimerVertical < buttonDownWaitMax)
            {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateVertical)
        {
            movedImmediateVertical = true;
        }


        verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);
        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            if (Input.GetKey(KeyCode.S))
            {
                PlayMoveAudio();
            }
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            Game.currentScore += individualScore;

            

            enabled = false;
            tag = "Untagged";
            PlayLandAudio();

            // Spawn the next piece
            FindObjectOfType<Game>().SpawnNextTetromino();
            FindObjectOfType<Game>().DeleteRow();

            // Check if there are any minos above the grid
            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game>().GameOver();
            }
        }
        fall = Time.time;
    }

    void Rotate()
    {

        
        // The up key was psressed, lets first check if the tetromino is allowed to rotate.
        if (allowRotation)
        {
            // if it is, we need to check if the rotation is limited to just back and forth
            if (limitRotation)
            {
                // if it is, we need to check what the current rotation is
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    // if it is at 90 then we know it was already rotated, so we rotate it back by -90
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    // if it isn't, then we rotate it to 90
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                // if it isn't, then we rotate it to 90
                transform.Rotate(0, 0, 90);
            }

            // Now we check if the tetromino is at a valid position after attempting a rotation
            if (CheckIsValidPosition())
            {
                // if the position is valid, we update the grid
                FindObjectOfType<Game>().UpdateGrid(this);
                PlayRotateAudio();
            }
            else
            {
                // if it isn't we rotate it back -90
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
        }
    }

}


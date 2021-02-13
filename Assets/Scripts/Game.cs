using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    // the width of the grid
    public static int gridWidth = 10;
    // the height of the grid
    public static int gridHeight = 20;
    // the grid
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingLevelZero;
    public static int startingLevel;

    public static int currentScore = 0;

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

    private int numberOfRowsThisTurn = 0;

    public static float fallSpeed = 1.0f;
    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public Text hud_score;
    public Text hud_level;
    public Text hud_lines;
    private AudioSource audioSource;
    public AudioClip LineClearedSound;

    private GameObject previewTetromino;
    private GameObject nextTetromino;
    private GameObject savedTetromino;
    private GameObject ghostTetromino;

    private bool gameStarted = false;
    public static bool isPaused = false;

    private Vector2 previewTetrominoPosition = new Vector2(-9, 9);
    private Vector2 savedTetrominoPosition = new Vector2(-9, 3);

    public int maxSwaps = 2;
    private int currentSwaps = 0;

    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3;



    public GameObject hudPuase;

    void Start()
    {
        SpawnNextTetromino();
        audioSource = GetComponent<AudioSource>();

        
        currentScore = 0;
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
        

        currentLevel = startingLevel;

        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3");


        if (isPaused == false)
        {
            Time.timeScale = 1;
        }
    }


    void Update()
    {
        UpdateUI();
        UpdateScore();
        UpdateLevel();
        UpdateSpeed();
        CheckUserInput();
    }

    public void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
            
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SavedTetromino(tempNextTetromino.transform);
        }
    }

    public void PauseGame()
    {
        hudPuase.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        audioSource.Pause();
        
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        audioSource.Play();
        hudPuase.SetActive(false);
    }

    public void UpdateLevel()
    {
        if (startingLevelZero == true || startingLevelZero == false && numLinesCleared / 10 > startingLevel)
        {
            currentLevel = numLinesCleared / 10;
        }

    }

    public void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                ClearedOneLine();
            }
            else if (numberOfRowsThisTurn == 2)
            {
                ClearedTwoLine();
            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearedThreeLine();
            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearedFourLine();
            }

            numberOfRowsThisTurn = 0;
            PlayLineClearSound();


        }
    }

    public void ClearedOneLine()
    {
        currentScore += scoreOneLine + (currentLevel * 20);
        numLinesCleared++;
    }

    public void ClearedTwoLine()
    {
        currentScore += scoreTwoLine + (currentLevel * 25);
        numLinesCleared += 2;
    }

    public void ClearedThreeLine()
    {
        currentScore += scoreThreeLine + (currentLevel * 30);
        numLinesCleared += 3;
    }

    public void ClearedFourLine()
    {
        currentScore += scoreFourLine + (currentLevel * 40);
        numLinesCleared += 4;
    }

    public void PlayLineClearSound()
    {
        audioSource.PlayOneShot(LineClearedSound);
    }

    #region update the high score

    public void UpdateHighScore()
    {
        // if score is higher than top 1  score, new top1 score is this score and old top 1 score is top2 score and top 2 score is top 3 score.
        if (currentScore > startingHighScore)
        {
            // change the score board. this block change the score 3 to score 2
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            // change
            PlayerPrefs.SetInt("highscore2", startingHighScore);
            PlayerPrefs.SetInt("highscore", currentScore);
        }
        else if (currentScore > startingHighScore2)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }
        else if (currentScore > startingHighScore3)
        {
            PlayerPrefs.SetInt("highscore3", currentScore);
        }
        PlayerPrefs.SetInt("lastscore",currentScore);
    }
    #endregion

    private bool CheckIsValidPosition(GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform )
        {
            Vector2 position = Round(mino.position);
            if (!CheckIsInsideGrid(position))
            {
                return false;
            }

            if (GetTransformAtGridPosition(position) != null && GetTransformAtGridPosition(position).parent != tetromino.transform)
            {
                return false;
            }
        }

        return true;
    }

    #region check this is above grid
    public bool CheckIsAboveGrid(Tetromino tetromino)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 position = Round(mino.position);
                if (position.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region game over 
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
        UpdateHighScore(); //the update score method must be work when the game finished. that why's it is here
    }
    #endregion

    #region Determine if there is a full row at the specified y
    public bool IsFullRow(int y)
    {
        // the parameter y, is the row we will iterate over in the grid array to check each x position for a transform.
        for (int x = 0; x < gridWidth; x++)
        {
            //if we find a position that returns null instead of a transform, then we know that the row is not full
            if (grid[x, y] == null)
            {
                // so we return false
                return false;
            }
        }

        // Since we found a full wor, we increment the full row variable
        numberOfRowsThisTurn++;

        // if we iterated over the entire loop and didn't encounter any Null positions, then we return true
        return true;
    }
    #endregion

    #region delete mino at y
    public void DeleteMino(int y)
    {
        // the parameter y, is the row we will iterate over in the grid array.
        for (int x = 0; x < gridWidth; x++)
        {
            // we destroy the gameObject of each transform at the current iteration of x,y
            Destroy(grid[x, y].gameObject);

            // we then set the x,y location in the grid array to null
            grid[x, y] = null;
        }
    }
    #endregion

    #region rows down
    public void MoveRowDown(int y)
    {
        // we iterate over each mino in the row of the y coordinate
        for (int x = 0; x < gridWidth; x++)
        {

            // if we check if the current x and y in the grid array does not equal null
            if (grid[x, y] != null)
            {

                // if it doesn't the we have to set the current transform one position below in the grid
                grid[x, y - 1] = grid[x, y];

                // we then set the current transform to null
                grid[x, y] = null;

                // and then we adjust the position of the sprite to move down by 1
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
    #endregion

    #region rows down all
    public void MoveAllRowsDown(int y)
    {
        // the y coordinate is the row above the row that was just deleted. So we iterate over each row starting with that one all the way to the height of the grid
        for (int i = y; i < gridHeight; i++)
        {
            // we then call move row down for each iteration
            MoveRowDown(i);
        }
    }
    #endregion

    #region delete the row
    public void DeleteRow()
    {
        // after receiving a call from the tetromino class, we  iterate over all the y positions to gridHeight
        for (int y = 0; y < gridHeight; y++)
        {

            // at each iteration we check if the row is full
            if (IsFullRow(y))
            {
                // if it is, we call deletemino at and pass in the current y
                DeleteMino(y);

                // we then call MoveAllRowsDown starting with the row above the one we just deleted
                MoveAllRowsDown(y + 1);

                // we then have to decrement y because we just moved all of the rows down by one
                y--;
            }
        }
    }
    #endregion

    #region Updates the grid
    public void UpdateGrid(Tetromino tetromino)
    {

        // in this method, we update our grid array. We do this by starting a for loop that iterates over all the grid rows starting at 0
        for (int y = 0; y < gridHeight; y++)
        {

            // for each row, we iterate over each individual x coordinate that represents a spot on the grid where a mino could be
            for (int x = 0; x < gridWidth; x++)
            {

                // for each iteration, we check the grid array for a null value
                if (grid[x, y] != null)
                {

                    // if there is a transform stored at the current index of the array then we check if the transform parent is the transform of the
                    if (grid[x, y].parent == tetromino.transform)
                    {

                        //if it is then we set that position in the array to null
                        grid[x, y] = null;
                    }
                }
            }
        }

        // we are now creating a foreach loop to step through all of the minos (childiren) of the calling tetromino
        foreach (Transform mino in tetromino.transform)
        {
            // Then we create a vector2 with the rounded position of the current mino
            Vector2 position = Round(mino.position);

            // we then check the position of the mino to make sure it is below the top line of the grid
            if (position.y < gridHeight)
            {

                // if it is, we set the mino (transform) at the position of the mino
                grid[(int)position.x, (int)position.y] = mino;
            }
        }
    }
    #endregion

    #region Gets the transform at grid position
    public Transform GetTransformAtGridPosition(Vector2 position)
    {

        //because we instantiate the tetrominos above the height of the grid which is not part of the array, we have return null instaed of attempting to return a transform doesn't exist
        if (position.y > gridHeight - 1)
        {
            return null;
        }

        //if the tetromino is below the height of the grid, we can return the transform at the position
        return grid[(int)position.x, (int)position.y];

    }
    #endregion

    #region Spawns the next tetromino
    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(4f, 19f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";

            SpawnGhostTetromino();
        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(4.0f, 19f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.tag = "currentActiveTetromino";
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
            
            SpawnGhostTetromino();
        }

        currentSwaps = 0;
    }
    #endregion

    public void SpawnGhostTetromino()
    {
        if (GameObject.FindGameObjectWithTag("currentGhostTetromino") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("currentGhostTetromino"));
        }

       

        ghostTetromino = (GameObject) Instantiate(nextTetromino, nextTetromino.transform.position, Quaternion.identity);

        Destroy(ghostTetromino.GetComponent<Tetromino>());

        ghostTetromino.AddComponent<GhostTetromino>();
    }

    public void SavedTetromino(Transform tetromino)
    {
        currentSwaps++;
        if (currentSwaps > maxSwaps)
        {
            return; 
        }

        if (savedTetromino != null)
        {
            // There is currently a tetromino being held
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavedTetromino");
            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            if (!CheckIsValidPosition(tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;
                return;
            }

            savedTetromino = (GameObject) Instantiate(tetromino.gameObject);
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            nextTetromino = (GameObject) Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(tempSavedTetromino);
            DestroyImmediate(tetromino.gameObject);

            SpawnGhostTetromino();

        }
        else
        {
            // There is currently no tetromino being held
            savedTetromino = (GameObject) Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.transform.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            SpawnNextTetromino();
        }
    }

    #region create the random tetromino
    public string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/Tetromino_Long";
        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/Tetromino_J";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Tetromino_L";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Tetromino_T";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Tetromino_Z";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Tetromino_S";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Tetromino_Square";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Tetromino_Long";
                break;
        }

        return randomTetrominoName;
    }
    #endregion

    #region  Check is inside grid 
    public bool CheckIsInsideGrid(Vector2 position)
    {
        // check if the position that was specified is within the boundaries of the grid
        return ((int)position.x >= 0 && (int)position.x < gridWidth && (int)position.y >= 0);
    }
    #endregion

    #region round the specified position
    public Vector2 Round(Vector2 position)
    {
        // A simple helper method that will round the x and y positions
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }
    #endregion
}

using UnityEngine;

public class GhostTetromino : MonoBehaviour
{
    
    void Start()
    {
        tag = "currentGhostTetromino";
        foreach (Transform mino in transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f,.2f);
        }
    }

    
    void Update()
    {
        FollowActiveTetromino();
        MoveDown();
    }

    void FollowActiveTetromino()
    {
        Transform currentActiveTetrominoTransform =
            GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;

        transform.position = currentActiveTetrominoTransform.position;
        transform.rotation = currentActiveTetrominoTransform.rotation; 

    }

    void MoveDown()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
        }
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 position = FindObjectOfType<Game>().Round(mino.position);
            if (FindObjectOfType<Game>().CheckIsInsideGrid(position) == false)
            {
                return false;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(position) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(position).parent.tag == "currentActiveTetromino")
            {
                return true;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(position) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(position).parent != transform)
            {
                return false;
            }
        }

        return true;
    }


}

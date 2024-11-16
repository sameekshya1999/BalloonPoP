using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class GameMaster : MonoBehaviour
{

    // an enum to hold the type of objects we can have in the maze
    public enum CellType
    {
        space, wall
    };

    // A Point struct to be stored in linked lists by Prim's algorithm.
    public struct Point
    {
        public int x, y;
        public Point(int vx, int vy) : this() { this.x = vx; this.y = vy; }
    };

    public CellType[,] maze;
    //public GameObject[,] bricks;
    public GameObject[,][] bricks;

    public GameObject[] baloons;

    public int baloonNr;

    public int mazeWidth, mazeHeight;
    static int offsetX = 2, offsetZ = 2;
    static float tileSize = 0.6f;

    public GameObject brickRef;
    public GameObject baloonRef;

    float brickWidth, brickY;
    public Text scoreText;  

    private int score = 0;
    public AudioSource balloonPopSound;
    public AudioSource dangerSound;


    public int[,] hood = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
    float[] rotations = { 0, 0, 90, 90 };
    float[,] offsets = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };




    // Create a maze and the 3D objects composing it
    void MakeMaze(int cols, int rows)
    {
        mazeWidth = cols;
        mazeHeight = rows;

        // set the offsets so that (0, 0) is in the center of the maze
        offsetX = mazeWidth / 2;
        offsetZ = mazeHeight / 2;

        // allocate the array
        maze = new CellType[mazeWidth, mazeHeight];

        // create the CellType maze
        Prim();
        // create the maze of wall 3D objects
        BuildMaze();
    }

    // Initialize all the cells in the maze with the same value
    void InitMaze(CellType val)
    {
        for (int i = 0; i < mazeWidth; i++)
            for (int j = 0; j < mazeHeight; j++)
                maze[i, j] = val;
    }


    LinkedList<Point> Neighbors(int cx, int cy)
    {
        LinkedList<Point> neighbs = new LinkedList<Point>();
        if (cx > 1)
        {
            neighbs.AddFirst(new Point(cx - 1, cy));
        }
        if (cx < mazeWidth - 2)
        {
            neighbs.AddLast(new Point(cx + 1, cy));
        }
        if (cy > 1)
        {
            neighbs.AddLast(new Point(cx, cy - 1));
        }
        if (cy < mazeHeight - 2)
        {
            neighbs.AddLast(new Point(cx, cy + 1));
        }
        return neighbs;
    }

    // converts a column number into a position on the stage
    static public float Col2X(int c)
    {
        return c * tileSize - offsetX * tileSize;
    }

    // converts a column number into a position on the stage
    static public float Row2Z(int r)
    {
        return r * tileSize - offsetZ * tileSize;
    }

    // converts an x position to a column number
    static public int X2col(float xVal)
    {
        return offsetX + (int)(xVal / tileSize);
    }

    // converts a y position to a row number
    static public int Z2row(float yVal)
    {
        return offsetZ + (int)(yVal / tileSize);
    }


    void BuildMaze()
    {
        int i, j;
        brickY = brickRef.transform.position.y;
        brickWidth = 2 * brickRef.transform.localScale.x;
        tileSize = brickWidth; 

        bricks = new GameObject[mazeWidth, mazeHeight][];
        for (i = 0; i < mazeWidth; i++)
        {
            for (j = 0; j < mazeHeight; j++)
            {
                if (maze[i, j] == CellType.wall)
                {
                    bricks[i, j] = MakeBrick(i, j); 
                }
                else
                {
                    bricks[i, j] = null;
                }
            }
        }
    }




    // implementation of Prim's algorithm to generate a maze
    void Prim()
    {
        LinkedListNode<Point> node;
        int cx, cy, dx, dy, r;
        LinkedList<Point> neighbs;

        // start by filling up the maze with walls
        InitMaze(CellType.wall);

         int startx = mazeWidth / 2, starty = mazeHeight / 2;

        // the frontier contains all the neighbors of the starting position
        LinkedList<Point> frontier = Neighbors(startx, starty);
        maze[startx, starty] = CellType.space;

        while (frontier.Count > 0)               // while we still have nodes in the frontier
        {
            r = Random.Range(0, frontier.Count); // choose a random one
            node = NodeAtIndex(frontier, r);
            cx = node.Value.x;
            cy = node.Value.y;
            frontier.Remove(node);               
            neighbs = Neighbors(cx, cy);
            if (CountSpaces(neighbs) == 1)       // make sure it doesn't close a cycle
            {
                maze[cx, cy] = CellType.space;   // make it a space

                for (node = neighbs.First; node != null; node = node.Next) // process its neighbors
                {
                    dx = node.Value.x;
                    dy = node.Value.y;
                    // if the neighbor is not a space
                    if (maze[dx, dy] != CellType.space)
                    {
                        LinkedList<Point> n = Neighbors(dx, dy);
                        // if it has exactly one space among its neighbors and is not already in the frontier
                        if (CountSpaces(n) == 1 && !ContainsNode(frontier, dx, dy))
                        {
                            //add this neighbor to the frontier;
                            frontier.AddLast(new Point(dx, dy));
                        }
                    }
                }
            }
        }
    }

    // returns the node at a given index from the list
    LinkedListNode<Point> NodeAtIndex(LinkedList<Point> list, int index)
    {
        LinkedListNode<Point> n = list.First;
        int i = 0;
        while (n != null && i < index)
        {
            n = n.Next;
            ++i;
        }
        return n;
    }

    // counts the spaces in the list
    int CountSpaces(LinkedList<Point> neighb)
    {
        int count = 0;
        for (LinkedListNode<Point> n = neighb.First; n != null; n = n.Next)
        {
            if (maze[n.Value.x, n.Value.y] == CellType.space)
            {
                count++;
            }
        }
        return count;
    }

    // checks if the list contains a node given with x and y
    bool ContainsNode(LinkedList<Point> list, int cx, int cy)
    {
        LinkedListNode<Point> p = list.First;
        while (p != null)
        {
            if (p.Value.x == cx && p.Value.y == cy)
            {
                return true;
            }
            p = p.Next;
        }
        return false;
    }

    // Add the given number of baloons randomly
    void AddBaloons()
    {
        float fx, fz;
        baloons = new GameObject[baloonNr];
        for (int i = 0; i < baloonNr; i++)
        {
            baloons[i] = Instantiate(baloonRef) as GameObject;
            fx = Col2X(Random.Range(0, mazeWidth));
            fz = Row2Z(Random.Range(0, mazeHeight));
            Vector3 pos = new Vector3(fx, 0f, fz);
            baloons[i].transform.position = pos;
        }
    }

    // Use this for initialization
    void Start()
    {
        MakeMaze(mazeWidth, mazeHeight);
        AddBaloons();   
   }


    bool OnTable(int i, int j)
    {
        return i >= 0 && i < mazeWidth && j >= 0 && j < mazeHeight;
    }

        // Create a composite brick for cell [i, j] based on neighboring walls
    GameObject[] MakeBrick(int i, int j)
    {
        GameObject[] br = new GameObject[4];
        GameObject b;
        int bi = 0;
        for (int k = 0; k < hood.Length / 2; k++)
        {
            int c = hood[k, 0];
            int r = hood[k, 1];

            if (OnTable(i + c, j + r) && maze[i + c, j + r] == CellType.wall)
            {
                b = Instantiate(brickRef) as GameObject;
                b.transform.position = new Vector3(Col2X(i) + offsets[k, 0], brickY, Row2Z(j) + offsets[k, 1]);
                b.transform.rotation = Quaternion.Euler(0, rotations[k], 0);
                br[bi++] = b;
            }
        }
        if (bi == 0) // if no neighboring walls were found, create a standalone brick
        {
            b = Instantiate(brickRef) as GameObject;
            b.transform.position = new Vector3(Col2X(i), brickY, Row2Z(j));
            b.transform.localScale = new Vector3(3f, 4f, 3f); // make it bigger
            br[bi++] = b;
        }
        return br;
    }
        public void StartNewGame() // Ensure this is public
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame() // Ensure this is public
    {
        Application.Quit();
    }


        // Check for key presses to start new game or quit
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartNewGame();
            

        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
    }

    // Method to increase the score
    public void IncreaseScore(int points)
    {
        score += points;
        UpdateScoreText();
        if (balloonPopSound != null && balloonPopSound.isActiveAndEnabled)
        {
            balloonPopSound.Play();
        }
    }

    // Update the score text display
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

// Method to decrease the score
public void DecreaseScore(int points)
    {
        score -= points;
        UpdateScoreText();

        // Play the danger sound
        if (dangerSound != null && !dangerSound.isPlaying)
        {
            dangerSound.Play();
        }
    }

    // Method to get the current score
    public int GetScore()
    {
        return score;
    }
    
}









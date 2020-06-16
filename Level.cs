using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Level : MonoBehaviour
{

    #region variables

    public enum SPRITE
    {
        ONEPLAY = 1,
        TWOPLAY,
        THREEPLAY,
        FOURPLAY,
        FIVEPLAY,
        SIXPLAY,
        SEVENPLAY,
        EIGHTPLAY,
        REVEALEDPLAY,

        ONEWON = 21,
        TWOWON,
        THREEWON,
        FOURWON,
        FIVEWON,
        SIXWON,
        SEVENWON,
        EIGHTWON,
        REVEALEDWON,
        MINEWON,

        ONELOST = 41,
        TWOLOST,
        THREELOST,
        FOURLOST,
        FIVELOST,
        SIXLOST,
        SEVENLOST,
        EIGHTLOST,
        REVEALEDLOST,
        MINELOST,

        HIDDEN,
        QUESTION,
        FLAG,

    };

    //block prefab
    public GameObject blockPreFab;
    //block scale
    private float scale = .4f;
    //gap between blocks
    private float gap = .2f;
    //offset from center
    private float leftOffset, topOffset;
    //animations delay
    private float animDelay = 0.05f;

    //sprites
    [HideInInspector]
    public Dictionary<SPRITE, Sprite> sprites;


    //level number
    private int levelNum;
    //rows an cols
    private int rows, cols;

    //number of mines in the level
    private int numMines;
    //fraction of mines based on size
    private float minesFraction = .05f;
    //number of blocks revealed
    private int numBlocksRevealed;
    //total number of blocks
    private int numBlocksMax;



    //data structures
    //block matrix
    public Block[,] allBlocks;
    //all blocks with mines
    public List<Block> allMines;
    //all blocks with no mines
    public List<Block> nonMines;

    public static Level instance = null;


    #endregion variables

    #region startup

    void MakeSingleton()
    {
        //Debug.Log("level instance:");
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad (gameObject);
        }
    }

    void Awake()
    {
        Debug.Log("level awake:");
        MakeSingleton();
    }

    void Start()
    {
        Debug.Log("level start");
    }

    public void setUpLevel(int levelNum)
    {
        this.levelNum = levelNum;
        sprites = Controller.instance.Sprites;
        setLevelVariables();
        generateBlocks();

    }

    private void setLevelVariables()
    {

        switch (levelNum)
        {

            case 1:
                {
                    rows = 8;
                    cols = 10;                   
                }
                break;

            case 2:
                {
                    rows = 9;
                    cols = 16;
                }
                break;

            case 3:
                {
                    rows = 10;
                    cols = 18;
                }
                break;
        }

      
        numMines = (int)(minesFraction * rows * cols);
       
        //offsets from center
        leftOffset = cols * (scale + gap) / 2f - scale;
        topOffset = rows * (scale + gap) / 2f;
        numBlocksMax = rows * cols - numMines;
        numBlocksRevealed = 0;

    }

    private void generateBlocks()
    {
        //Debug.Log("generate blocks");
        GameObject blockHolder = new GameObject("blocks");
        GameObject go;
        allBlocks = new Block[rows, cols];

        float xpos, ypos, zpos = 89;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                xpos = -leftOffset + c * (scale + gap);
                ypos = -topOffset + r * (scale + gap);
                go = Instantiate(blockPreFab, new Vector3(xpos, ypos, zpos), Quaternion.identity) as GameObject;
                go.transform.parent = blockHolder.transform;
                go.transform.localScale = new Vector3(scale, scale, scale);
                //update block script data
                Block block = go.GetComponent<Block>();

                //block.SpriteRenderer.sprite = sprites[SPRITE.HIDDEN];
                block.Row = r;
                block.Col = c;

                //add cube cut matrix
                allBlocks[r, c] = block;
            }
        }
    }

    #endregion startup

    #region gameplay

    //right mouse click on a block 
    public void processRightClick(Block selectedBlock)
    {
        selectedBlock.advanceBlockState();
    }

    //left mouse click on a block 
    public void processLeftClick(Block selectedBlock)
    {

        if (numBlocksRevealed == 0)
        {
            //first click: set mines and reveal block
            generateMines(selectedBlock);
            initMineCount();
        }

        //check if block is playable
        if (selectedBlock.IsPlayable)
        {
            if (selectedBlock.HasMine)
            {
                Debug.Log("block has mine");
                //reveal mine on block right away
                selectedBlock.showBlockSprite(Controller.GAMESTATE.IDLE);
                //game lost procedure
                StartCoroutine(endGame(Controller.GAMESTATE.LOST));
            }
            else
            {
                if (selectedBlock.MineCount > 0)
                {
                    //has mines around it               
                    numBlocksRevealed++;
                    //set block as not playable
                    selectedBlock.IsPlayable = false;
                    selectedBlock.showBlockSprite(Controller.GAMESTATE.PLAYERMOVE);
                }
                else
                {
                    //reveal block
                    Controller.instance.changeGameState(Controller.GAMESTATE.IDLE);
                    StartCoroutine(revealContigousBlocks(selectedBlock));
                    
                    


                }
                //check for win
                checkWin(selectedBlock);
            }
        }
    }

    //check win
    private void checkWin(Block selectedBlock)
    {
        Debug.Log("check win");

        if (numBlocksRevealed == numBlocksMax)
        {
            //reveal block
            selectedBlock.showBlockSprite(Controller.GAMESTATE.IDLE);
            //game win procedure
            StartCoroutine(endGame(Controller.GAMESTATE.WON));
        }
    }

    private IEnumerator revealContigousBlocks(Block selectedBlock)
    {

        Stack<Block> stack = new Stack<Block>();
        //push the selected block onto the stack to start the process
        stack.Push(selectedBlock);
        //iterate until stack is empty
        int failSafe = 0;
        while (stack.Count > 0 && failSafe < 500)
        {
            //Debug.Log("stack: " + stack.Count);
            //pop the block off the top of the stack
            Block block = stack.Pop();
            // Debug.Log("pop block: " + block);
            //reveal block mine count
            block.showBlockSprite(Controller.GAMESTATE.PLAYERMOVE);
            //set block as not playable
            block.IsPlayable = false;
            //update number of blocks reavealed
            numBlocksRevealed++;
            if (block.MineCount == 0)
            {
                //get all the blocks around the given block that are in play and not mines
                List<Block> contigiouusBlocks = getContigiousBlocks(block.Row, block.Col);
                // Debug.Log("contigious blocks: " + contigiouusBlocks.Count);
                //place all surrounding blocks on to the stack, only if not already on the stack, and not mines
                addBlockstoStack(stack, contigiouusBlocks);
                //showStack(stack);
            }
            //Debug.Log("num max/revealed: " + numBlocksMax + " " + numBlocksRevealed);
            failSafe++;
            yield return new WaitForSeconds(animDelay);
        }//while

        Controller.instance.changeGameState(Controller.GAMESTATE.PLAYERMOVE);

    }


    //add all contigious blocks with no contigious mines to the stack, if not already on the stack
    //stack and contigious blocks are not null
    private void addBlockstoStack(Stack<Block> stack, List<Block> contigiousBlocks)
    {
        for (int i = 0; i < contigiousBlocks.Count; i++)
        {
            //Debug.Log("check add to stack: " + contigiousBlocks[i]);
            if (!contigiousBlocks[i].HasMine && !stack.Contains(contigiousBlocks[i]))
            {
                //Debug.Log("add block to stack: " + contigiousBlocks[i]);
                stack.Push(contigiousBlocks[i]);
            }
        }
    }

    private IEnumerator endGame(Controller.GAMESTATE gamestate)
    {
        Debug.Log("endgame");

        //reveal all the mines first
        foreach (Block b in allMines)
        {
            b.showBlockSprite(gamestate);
            yield return new WaitForSeconds(animDelay);

        }
        //reveal no mine blocks

        foreach (Block b in nonMines)
        {
            b.showBlockSprite(gamestate);
            yield return new WaitForSeconds(animDelay);

        }

    }

    #endregion gameplay

    #region generate mines

    //generate mines, no mine on selected block
    public void generateMines(Block selectBlock)
    {
        //number of mines placed
        int mineCounter = 0;

        allMines = new List<Block>();


        //generate random list
        List<int> randomize = randomizeNums();
        //place mines

        //iterate through random values
        foreach (int index in randomize)
        {
            if (mineCounter >= numMines)
                break;

            //calcuate the row and col
            int row = index / cols;
            int col = index % cols;

            //no mine on the selected block
            if (selectBlock.Row == row && selectBlock.Col == col)
                continue;

            //get block in matrix
            Block block = allBlocks[row, col];
            //set mine
            block.HasMine = true;
            allMines.Add(block);
            //show mine temporarily!
            //block.showBlockSprite(Controller.GAMESTATE.PLAY);
            mineCounter++;
            //Debug.Log(mineCounter + " mine set:" + row + " " + col);

        }//place mines

        //set no mines blocks
        nonMines = new List<Block>();
        for (int r = 0; r < allBlocks.GetLength(0); r++)
        {
            for (int c = 0; c < allBlocks.GetLength(1); c++)
            {
                if (allBlocks[r, c].HasMine == false)
                {
                    nonMines.Add(allBlocks[r, c]);
                }
            }
        }//set no mines list    
    }

    //set mine count for all blocks
    private void initMineCount()
    {
        //iterate through all blocks
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {   //get all contigious blocks
                List<Block> blocks = getContigiousBlocks(r, c);
                allBlocks[r, c].MineCount = calcMines(blocks);
                //allBlocks[r, c].revealMineCount();
            }
        }
    }//mine count

    //returned list can be empty, but not null
    private List<Block> getContigiousBlocks(int row, int col)
    {
        List<Block> blocks = new List<Block>();

        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                //avoid adding given position
                if (r != row || c != col)
                {
                    //check if in bounds, playable
                    if (inBounds(r, c) && allBlocks[r, c].IsPlayable)
                    {
                        //Debug.Log("add block: " + r + " " + c);
                        blocks.Add(allBlocks[r, c]);
                    }
                }
            }
        }
        return blocks;
    }//get contigious 

    //count all mines on contigious blocks
    private int calcMines(List<Block> contigiousBlocks)
    {
        int counter = 0;
        for (int i = 0; i < contigiousBlocks.Count; i++)
        {
            //check if in bounds
            if (contigiousBlocks[i].HasMine)
            {
                counter++;
            }
        }
        return counter;
    }//calc mines

    #endregion generate mines

    #region helper methods
    //return true if r, c are in bounds
    private bool inBounds(int r, int c)
    {
        return r >= 0 && r < rows && c >= 0 && c < cols;
    }

    //randomize numbers from 0 to num blocks
    private List<int> randomizeNums()
    {
        List<int> rand = new List<int>();
        for (int i = 0; i < numBlocksMax; i++)
        {
            rand.Add(i);
        }
        for (int i = 0; i < 500; i++)
        {
            int r1 = UnityEngine.Random.Range(0, numBlocksMax);
            int r2 = UnityEngine.Random.Range(0, numBlocksMax);
            int temp = rand[r1];
            rand[r1] = rand[r2];
            rand[r2] = temp;
        }
        return rand;
    }
    #endregion helper methods


}//class


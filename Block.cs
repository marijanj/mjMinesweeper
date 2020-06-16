using UnityEngine;
using System.Collections;
//using System.Collections.Concurrent;


public class Block : MonoBehaviour
{

  

    private SpriteRenderer spriteRenderer;

    //position in matrix
    private int row, col;
    //mine on this block?
    private bool hasMine;
    //number of mines around this block
    private int mineCount;
    //is this block still playable?
    private bool isPlayable;


    //sprite state: hidden, question, mark
    private Level.SPRITE blockState;


    // Use this for initialization
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        isPlayable = true;
        mineCount = 0;
        hasMine = false;
        blockState = Level.SPRITE.HIDDEN;
    }



    //show block sprite
    public void showBlockSprite(Controller.GAMESTATE gameState)
    {
        switch (gameState)
        {
            case Controller.GAMESTATE.PLAYERMOVE:
                {
                    showPlaySprite();
                }
                break;
            case Controller.GAMESTATE.WON:
                {
                    showWinSprite();
                }
                break;
            case Controller.GAMESTATE.LOST:
                {
                    showLoseSprite();
                }
                break;
        }
    }

    private void showPlaySprite()
    {

        if (hasMine)
        {
            spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.MINEWON];
        }
        else if (mineCount == 0)
        {
            spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.REVEALEDPLAY];
        }
        else
        {
            Level.SPRITE spriteType = (Level.SPRITE)mineCount;
            spriteRenderer.sprite = Level.instance.sprites[spriteType];
        }
    }

    private void showWinSprite()
    {

        if (hasMine)
        {
            spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.MINEWON];
        }
        else if (mineCount == 0)
        {
            spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.REVEALEDWON];
        }
        else
        {
            Level.SPRITE spriteType = (Level.SPRITE)(20 + mineCount);
            spriteRenderer.sprite = Level.instance.sprites[spriteType];
        }
    }

    private void showLoseSprite()
    {

        if (hasMine)
        {
            spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.MINELOST];
        }
        else if (mineCount == 0)
        {
            spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.REVEALEDLOST];
        }
        else
        {
            Level.SPRITE spriteType = (Level.SPRITE)(40 + mineCount);
            spriteRenderer.sprite = Level.instance.sprites[spriteType];
        }
    }

    public void advanceBlockState()
    {
        switch (blockState)
        {
            case Level.SPRITE.HIDDEN:
                {
                    blockState = Level.SPRITE.QUESTION;
                    spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.QUESTION];
                }
                break;
            case Level.SPRITE.QUESTION:
                {
                    blockState = Level.SPRITE.FLAG;
                    spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.FLAG];
                }
                break;
            case Level.SPRITE.FLAG:
                {
                    blockState = Level.SPRITE.HIDDEN;
                    spriteRenderer.sprite = Level.instance.sprites[Level.SPRITE.HIDDEN];
                }
                break;
            
        }
    }

    //getters and setter

    //game object in play
    public bool IsPlayable
    {
        get { return isPlayable; }
        set
        {
            isPlayable = value;
        }
    }

    public bool HasMine
    {
        get { return hasMine; }
        set { hasMine = value; }
    }

    public int Row
    {
        get { return row; }
        set { row = value; }
    }

    public int Col
    {
        get { return col; }
        set { col = value; }
    }



    public int MineCount
    {
        get { return mineCount; }
        set
        {
            mineCount = value;
        }
    }


    public SpriteRenderer SpriteRenderer
    {
        get { return spriteRenderer; }
        set { spriteRenderer = value; }
    }


    public Level.SPRITE BlockState
    {
        get { return blockState; }
        set
        {
            blockState = value;
            spriteRenderer.sprite = Level.instance.sprites[blockState];
        }
    }



    public override string ToString()
    {
        return row + " " + col + " " + hasMine + " " + MineCount;
    }
}

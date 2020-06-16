using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{

    #region variables

    public enum GAMESTATE
    {
        IDLE, GAMESETUP, MENU, LEVELSETUP, PLAYERMOVE, WON, LOST
    };

    private GAMESTATE gameStateCurrent;
    private delegate void gameModeDelegate();
    private gameModeDelegate playGame;

    private int levelNum;


    //sprites
    private Dictionary<Level.SPRITE, Sprite> sprites;

    public static Controller instance = null;

    void MakeSingleton()
    {
        Debug.Log("controller instance:");
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion variables

    void Awake()
    {
        Debug.Log("controller awake:");
        MakeSingleton();
    }


    // Use this for initialization
    void Start()
    {
        Debug.Log("controller start:");
        changeGameState(GAMESTATE.GAMESETUP);
    }

    #region delegates


    void Update()
    {
        playGame();

    }



    public void changeGameState(GAMESTATE gameStateCurrent)
    {
        switch (gameStateCurrent)
        {

            case GAMESTATE.GAMESETUP:
                {
                    playGame = new gameModeDelegate(gameSetupfunction);
                }
                break;
            case GAMESTATE.MENU:
                {
                    playGame = new gameModeDelegate(menuFunction);
                }break;
            case GAMESTATE.LEVELSETUP:
                {
                    playGame = new gameModeDelegate(levelSetupFunction);
                }
                break;

            case GAMESTATE.PLAYERMOVE:
                {
                    playGame = new gameModeDelegate(playerMoveFunction);
                }
                break;

            case GAMESTATE.IDLE:
                {
                    playGame = new gameModeDelegate(idleFunction);
                }
                break;
        }
    }


    private void gameSetupfunction()
    {
        Debug.Log("game setup function");
        setSprites();
        changeGameState(GAMESTATE.MENU);
    }
    public void menuFunction()
    {
        Debug.Log("menu");
    }
    private void levelSetupFunction()
    {

        Debug.Log("level setup function");
        if (SceneManager.GetActiveScene().name == "Game Scene")
        {
            Level.instance.setUpLevel(levelNum);
            changeGameState(GAMESTATE.PLAYERMOVE);
        }
    }
    private void playerMoveFunction()
    {
        Debug.Log("player move");
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("left click ");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Block block = hit.collider.gameObject.GetComponent<Block>();
                if (block != null)
                {
                    Level.instance.processLeftClick(block);
                }
            }

        }
        else if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("right click: ");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Block block = hit.collider.gameObject.GetComponent<Block>();
                if (block != null)
                {
                    Level.instance.processRightClick(block);
                }
            }
        }
    }
    public void idleFunction()
    {
        Debug.Log("idle");
    }

    #endregion delegates


    private void setSprites()
    {
        sprites = new Dictionary<Level.SPRITE, Sprite>();

        Sprite s;

        //numbered play sprites

        s = Resources.Load<Sprite>("oneplay");
        sprites.Add(Level.SPRITE.ONEPLAY, s);

        s = Resources.Load<Sprite>("twoplay");
        sprites.Add(Level.SPRITE.TWOPLAY, s);

        s = Resources.Load<Sprite>("threeplay");
        sprites.Add(Level.SPRITE.THREEPLAY, s);

        s = Resources.Load<Sprite>("fourplay");
        sprites.Add(Level.SPRITE.FOURPLAY, s);

        s = Resources.Load<Sprite>("fiveplay");
        sprites.Add(Level.SPRITE.FIVEPLAY, s);

        s = Resources.Load<Sprite>("sixplay");
        sprites.Add(Level.SPRITE.SIXPLAY, s);

        s = Resources.Load<Sprite>("sevenplay");
        sprites.Add(Level.SPRITE.SEVENPLAY, s);

        s = Resources.Load<Sprite>("eightplay");
        sprites.Add(Level.SPRITE.EIGHTPLAY, s);

        //numbered win sprites

        s = Resources.Load<Sprite>("onewon");
        sprites.Add(Level.SPRITE.ONEWON, s);

        s = Resources.Load<Sprite>("twowon");
        sprites.Add(Level.SPRITE.TWOWON, s);

        s = Resources.Load<Sprite>("threewon");
        sprites.Add(Level.SPRITE.THREEWON, s);

        s = Resources.Load<Sprite>("fourwon");
        sprites.Add(Level.SPRITE.FOURWON, s);

        s = Resources.Load<Sprite>("fivewon");
        sprites.Add(Level.SPRITE.FIVEWON, s);

        s = Resources.Load<Sprite>("sixwon");
        sprites.Add(Level.SPRITE.SIXWON, s);

        s = Resources.Load<Sprite>("sevenwon");
        sprites.Add(Level.SPRITE.SEVENWON, s);

        s = Resources.Load<Sprite>("eightwon");
        sprites.Add(Level.SPRITE.EIGHTWON, s);

        //numbered lost sprites

        s = Resources.Load<Sprite>("onelost");
        sprites.Add(Level.SPRITE.ONELOST, s);

        s = Resources.Load<Sprite>("twolost");
        sprites.Add(Level.SPRITE.TWOLOST, s);

        s = Resources.Load<Sprite>("threelost");
        sprites.Add(Level.SPRITE.THREELOST, s);

        s = Resources.Load<Sprite>("fourlost");
        sprites.Add(Level.SPRITE.FOURLOST, s);

        s = Resources.Load<Sprite>("fivelost");
        sprites.Add(Level.SPRITE.FIVELOST, s);

        s = Resources.Load<Sprite>("sixlost");
        sprites.Add(Level.SPRITE.SIXLOST, s);

        s = Resources.Load<Sprite>("sevenlost");
        sprites.Add(Level.SPRITE.SEVENLOST, s);

        s = Resources.Load<Sprite>("eightlost");
        sprites.Add(Level.SPRITE.EIGHTLOST, s);

        //rest of images

        s = Resources.Load<Sprite>("hidden");
        sprites.Add(Level.SPRITE.HIDDEN, s);

        s = Resources.Load<Sprite>("minelost");
        sprites.Add(Level.SPRITE.MINELOST, s);

        s = Resources.Load<Sprite>("minewon");
        sprites.Add(Level.SPRITE.MINEWON, s);

        s = Resources.Load<Sprite>("question");
        sprites.Add(Level.SPRITE.QUESTION, s);

        s = Resources.Load<Sprite>("flag");
        sprites.Add(Level.SPRITE.FLAG, s);

        s = Resources.Load<Sprite>("revealedlost");
        sprites.Add(Level.SPRITE.REVEALEDLOST, s);

        s = Resources.Load<Sprite>("revealedwon");
        sprites.Add(Level.SPRITE.REVEALEDWON, s);

        s = Resources.Load<Sprite>("revealed");
        sprites.Add(Level.SPRITE.REVEALEDPLAY, s);
    }


    //getters and setters

    public int LevelNum
    {
        get { return levelNum; }
        set { levelNum = value; }
    }

    public Dictionary<Level.SPRITE, Sprite> Sprites
    {
        get { return sprites; }
        set { sprites = value; }
    }





}//class

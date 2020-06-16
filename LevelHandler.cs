using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{


  

    public void SelectLevel(int level)
    {
        Controller.instance.LevelNum = level;
        SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
        Controller.instance.changeGameState(Controller.GAMESTATE.LEVELSETUP);
    }

    public void Menu()
    {

        SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
        Controller.instance.changeGameState(Controller.GAMESTATE.MENU);
    }





}

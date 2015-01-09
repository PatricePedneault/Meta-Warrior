// Le pause menu n'utilise plus GAF, afin de permettre d'afficher les instructions Panic dans la scène respective. - Gab

using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour 
{

	private GameObject	camera;

    private GameObject  pauseMenuObject;

    private bool active;


	void Start () 
	{
		camera = GameObject.Find ("Main Camera");
        pauseMenuObject = camera.transform.FindChild("PauseMenu").gameObject;
        active = false;

		cInput.SetKey("buttonBController", Keys.JoystickButton1);
	}

	// Si la méthode est appelé on affiche tout pour faire afficher le menu de pause et on met le jeu en pause
	public void showPauseGameMenu()
	{
        pauseMenuObject.SetActive(true);

		UFE.PauseGame(true);

        UFE.switchPauseMusic(true);

        active = true;

        try
        {
            AkSoundEngine.PostEvent("UI_Menu_Click", GameObject.Find("CameraUIChild"));
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Je n'ai pas pu trouver la caméra!");
        }

	}

	// Si appelé on arrete l'affiche du menu pause et on redémarre le jeu
	public void disablePauseGameMenu()
	{
        UFE.PauseGame(false);

        UFE.switchPauseMusic(false);

        pauseMenuObject.SetActive(false);

        active = false;

        try
        {
            AkSoundEngine.PostEvent("UI_Menu_Back", GameObject.Find("CameraUIChild"));
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Je n'ai pas pu trouver la caméra!");
        }
	}

	void Update () 
	{
		if(active && Input.GetJoystickNames().Length != 0)
		{
			// Si c'est la manette qui est utilisé comme controller et que le joueur 
			//appui sur "B" on arrete l'affiche du menu en appelant la méthode "disablePauseGameMenu()"
			if (cInput.GetButtonDown ("buttonBController")) 
			{
                disablePauseGameMenu();
			}
		}
	}
}

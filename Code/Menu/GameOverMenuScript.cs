using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverMenuScript : MonoBehaviour 
{
	private GameObject				camera;

	private List<Texture2D> 		listTextureRetryButton;
	private List<Texture2D> 		listTextureQuitButton;

	private int 					idRetryButtonTexture;
	private int 					idQuitButtonTexture;

	private GameObject				flashAnimPrefab;
	private GameObject				flashAnimBackground;

	private float					timer;

	private float					lastUpdateTimer;

	private	float					scale;

	private string 					overButtonName;


	// Use this for initialization
	void Start () 
	{
		// On va chercher la camera pour pouvoir positonner les boutons et background en fonction d'elle
		camera = GameObject.Find ("Main Camera");



		//flashAnimPrefab = (GameObject) Resources.LoadAssetAtPath("Assets/Flash Animations/GameOver_V1/GameOver_V1.prefab", typeof(GameObject));

		flashAnimPrefab = Resources.Load<GameObject> ("Menu/GameOver Menu/GameOver_V1_fix");

		// Création du gameobject avec le prefab flash qui contient tous les images de l'anim a jouer
		flashAnimBackground = Instantiate(flashAnimPrefab) as GameObject;
		// Positionnement et scale du background/anim
		flashAnimBackground.transform.localScale = new Vector3 (0.005f,0.005f,0.005f);
		flashAnimBackground.transform.position = new Vector3 (camera.transform.position.x - 4.5f,4.8f,-7.0f);

		// On positionne l'anim a 1
		flashAnimBackground.GetComponent<GAFMovieClip> ().gotoAndStop (1);


		// Crée la liste et load les textures pour le bouton "Retry"
		listTextureRetryButton = new List<Texture2D> ();
		listTextureRetryButton.Add(Resources.Load<Texture2D>("Menu/GameOver Menu/game_over_retry"));
		listTextureRetryButton.Add(Resources.Load<Texture2D>("Menu/GameOver Menu/game_over_retry_selected"));
		idRetryButtonTexture = 0;

		// Crée la liste et load les textures pour le bouton "Qui"
		listTextureQuitButton = new List<Texture2D> ();
		listTextureQuitButton.Add(Resources.Load<Texture2D>("Menu/GameOver Menu/game_over_quitgame"));
		listTextureQuitButton.Add(Resources.Load<Texture2D>("Menu/GameOver Menu/game_over_quitgame_selected"));
		idQuitButtonTexture = 0;

		// On met le jeu en pausse
		UFE.PauseGame(true);

		// On fait afficher le Background
		flashAnimBackground.renderer.enabled = true;


		cInput.SetKey("buttonAController", Keys.JoystickButton0);
		cInput.SetKey("buttonBController", Keys.JoystickButton1);
		
		cInput.SetKey("Joy1 Axis 1-", 		Keys.Joy1Axis1Negative);
		cInput.SetKey("Joy1 Axis 1+", 		Keys.Joy1Axis1Positive);
		cInput.SetAxis ("axisX", Keys.Joy1Axis1Negative, Keys.Joy1Axis1Positive);
		
		cInput.SetKey("Joy1 Axis 2-", 		Keys.Joy1Axis2Negative);
		cInput.SetKey("Joy1 Axis 2+", 		Keys.Joy1Axis2Positive);
		cInput.SetAxis ("axisY", Keys.Joy1Axis2Negative, Keys.Joy1Axis2Positive);
		
		cInput.SetKey("Joy1 Axis 6-", 		Keys.Joy1Axis6Negative);
		cInput.SetKey("Joy1 Axis 6+", 		Keys.Joy1Axis6Positive);
		cInput.SetAxis ("D-Pad-X", Keys.Joy1Axis6Negative, Keys.Joy1Axis6Positive);
		
		cInput.SetKey("Joy1 Axis 7-", 		Keys.Joy1Axis7Negative);
		cInput.SetKey("Joy1 Axis 7+", 		Keys.Joy1Axis7Positive);
		cInput.SetAxis ("D-Pad-Y", Keys.Joy1Axis7Negative, Keys.Joy1Axis7Positive);

        AkSoundEngine.StopAll();

        try
        {
            AkSoundEngine.PostEvent("Mus_GameOver", GameObject.Find("CameraUIChild"));
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Je n'ai pas pu trouver la caméra!");
        }
	}

	// Méthode qui permet de faire afficher la bonne méthode et de faire les bonnes actions si un bouton est utilisé ( avec clavier/souris) 
	void gestionSourisButton()
	{
		// Changement du scale pour mettre tout a la bonne grandeur
		scale = (float)(Screen.width * 0.4) / 779;
		GUIUtility.ScaleAroundPivot (new Vector2 (scale, scale), new Vector2 (Screen.width / 2, Screen.height / 2));

		// Affichage de la bonne texture du bonton "Retry" en fonction du nombre que contient 
		//"idRetryButtonTexture" 0= Bouton non overlapper (bouton sans rouge) 1= Bouton overlapper (bouton rouge) 
		GUI.skin.box.normal.background = listTextureRetryButton[idRetryButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2  + 160,626.0f,100.0f), "");

		// On crée un rectangle a la bonne position pour qu'il soit sur la texture du bouton en question (retry) et le if permet de faire une 
		// action si le rectangle subit un clique 
		if (GUI.Button(new Rect(Screen.width/2 - 300,Screen.height/2 + 160,626.0f,100.0f),new GUIContent("","RetryButton"),GUIStyle.none))
		{
			//Application.LoadLevel(Application.loadedLevel);
			startLoadingScreen();
		}

		// Meme chose que les lignes en haut
		GUI.skin.box.normal.background = listTextureQuitButton[idQuitButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 + 260,626.0f,100.0f), "");

		if (GUI.Button(new Rect(Screen.width/2 - 300,Screen.height/2 + 260,626.0f,100.0f),new GUIContent("","QuitButton"),GUIStyle.none))
		{
			Application.Quit();
		}

		// if qui permet de vérifier si le jeu render en se moment
		if (Event.current.type == EventType.Repaint)
		{
			// Permet de savoir le nom du rectangle qui est overlapper
			overButtonName = GUI.tooltip;

			// Si 1 alors la souris se trouve sur le bouton "Retry" si 0 alors la souris ne se trouve pas sur le bouton
			if(overButtonName=="RetryButton")
			{
				idRetryButtonTexture = 1;
			}
			else 
			{
				idRetryButtonTexture = 0;
			}

			if(overButtonName=="QuitButton")
			{
				idQuitButtonTexture = 1;
			}
			else 
			{
				idQuitButtonTexture = 0;
			}
		}
	}

	// Méthode qui permet de faire afficher la bonne méthode et de faire les bonnes actions si un bouton est utilisé ( avec Manette) 
	void gestionManetteButton()
	{
		// Si aucune sélection est choisi (au début) on met le bouton "retry" comme sélectionné
		if(idRetryButtonTexture == 0 && idQuitButtonTexture == 0)
		{
			idRetryButtonTexture = 1;
		}

		// Changement du scale pour mettre tout a la bonne grandeur
		scale = (float)(Screen.width * 0.4) / 779;
		GUIUtility.ScaleAroundPivot (new Vector2 (scale, scale), new Vector2 (Screen.width / 2, Screen.height / 2));
		
		// Affichage de la bonne texture du bonton "Retry" en fonction du nombre que contient 
		//"idRetryButtonTexture" 0= Bouton non overlapper (bouton sans rouge) 1= Bouton overlapper (bouton rouge) 
		GUI.skin.box.normal.background = listTextureRetryButton[idRetryButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2  + 160,626.0f,100.0f), "");
		
		// Meme chose que les lignes en haut
		GUI.skin.box.normal.background = listTextureQuitButton[idQuitButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 + 260,626.0f,100.0f), "");

		timer += Time.realtimeSinceStartup - lastUpdateTimer;
		lastUpdateTimer = Time.realtimeSinceStartup;

		// Si le bouton "A" de la manette est cliqué
		if (cInput.GetButtonDown ("buttonAController")) 
		{
			// si idRetryButtonTexture == 1 donc retry est sélectionné et on reload la map
			// Sinon c'est quit qui est sélectionné donc on quitte
			if(idRetryButtonTexture == 1)
			{
				//Application.LoadLevel(Application.loadedLevel);
                AkSoundEngine.StopAll();
				startLoadingScreen();
			}
			else
			{
				Application.Quit();
			}
		}


			// Gestion des axes avec la manette 
			if(cInput.GetAxis("axisY") > 0.0 ||  cInput.GetAxis("D-Pad-Y") < 0 )
			{
				// Vers le bas donc le bouton retry doit etre sélectionné
				idRetryButtonTexture = 0;
				idQuitButtonTexture = 1;
				
			}
			else if(cInput.GetAxis("axisY") < -0.0 ||  cInput.GetAxis("D-Pad-Y") > 0)
			{
				idRetryButtonTexture = 1;
				idQuitButtonTexture = 0;
			}

	}

	public void OnGUI()
	{
		// on regarde si l'anim du Background est a 100:
		// si oui on fait afficher les bouton
		// sinon on fait afficher la prochaine anim 
		if(flashAnimBackground.GetComponent<GAFMovieClip> ().getCurrentFrameNumber() != 100)
		{
			// Utiliation de "realtimeSinceStartup" car lorsque le jeu est en pause "UFE.PauseGame(true);" Time.DeltaTime ne fonctionnement pas 
			// Donc on fait par nous meme en prenant le temps now (ime.realtimeSinceStartup) - le temps de la derniere fois (lastUpdateTime)
			timer += Time.realtimeSinceStartup - lastUpdateTimer;
			lastUpdateTimer = Time.realtimeSinceStartup;
			
			if(timer > 0.025)
			{
				timer = 0;
				flashAnimBackground.GetComponent<GAFMovieClip> ().gotoAndStop(flashAnimBackground.GetComponent<GAFMovieClip> ().getCurrentFrameNumber() + 1);
			}
		}
		else
		{
			//condition pour savoir si une manette est connecté 0= non
			if(Input.GetJoystickNames().Length == 0)
			{
				gestionSourisButton ();
			}
			else
			{
				gestionManetteButton();
			}
		}
	}

	public void startLoadingScreen()
	{
		GameObject screenLoading = GameObject.FindGameObjectWithTag ("LoadingScreen");
		screenLoading.transform.localPosition = new Vector3 (0.4979212f, 0.5f, -3.0f);
		screenLoading.GetComponent<LoadingScreenScript> ().enabled = true;
		this.enabled = false;
		

	}
}


















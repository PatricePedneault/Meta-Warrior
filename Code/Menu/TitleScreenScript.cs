using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleScreenScript : MonoBehaviour 
{
	private List<Texture2D> 		listTextureNewGameButton;
	private List<Texture2D> 		listTextureConstrolsButton;
	private List<Texture2D> 		listTextureQuitButton;
	private List<Texture2D> 		listTextureBackButton;
	
	private int 					idNewGameButtonTexture;
	private int 					idConstrolsButtonTexture;
	private int 					idQuitButtonTexture;
	private int 					idBackButtonTexture;
	
	private string 					overButtonName;
	private	float					scale;
	
	private bool					showPrincipalMenu;
	
	private GameObject				principalMenuBackground;
	private GameObject				controlslMenuBackground;
	
	private int						idButton; // 1 = new game , 2 = controls , 3 = exit , 4 = back
	
	private bool 					readyToChange;
	
	private float					timer;

    private AudioSource             musicSource; 

    private AudioClip               highlight, click, back;

    // On fait jouer un son.
    private void playSound(AudioClip toPlay)
    {
        if (musicSource != null) musicSource.PlayOneShot(toPlay);
    }

	public void OnGUI()
	{
		// Scale tout
		scale = (float)(Screen.width * 0.4) / 779;

		GUIUtility.ScaleAroundPivot (new Vector2 (scale, scale), new Vector2 (Screen.width / 2, Screen.height / 2));
		
		// Est-ce qu'on affiche le menu principal ou les controls
		// On fonction du choix on rend enabled = true le bon background
		if (showPrincipalMenu) 
		{
			principalMenuBackground.renderer.enabled = true;
			controlslMenuBackground.renderer.enabled = false;
			
		} 
		else 
		{
			principalMenuBackground.renderer.enabled = false;
			controlslMenuBackground.renderer.enabled = true;
		}
		
		// Si une manette n'est pas connecté = 0
		if(Input.GetJoystickNames().Length == 0)
		{
			// Si vrai on fait afficher le menu principal avec gestion souris
			if(showPrincipalMenu)
			{
				gestionSourisPrincipalMenu();
			}
			else
			{
				gestionSourisControlMenu();
			}
		}
		else
		{
			// Si vrai on fait afficher le menu principal avec gestion manette
			if(showPrincipalMenu)
			{
				gestionManettePrincipalMenu();
			}
			else
			{
				
				gestionManetteControlMenu();
				
			}
		}
		
	}
	
	// Permet l'affichage du menu principal avec la gestion de la souris
	void gestionSourisPrincipalMenu()
	{
		// On fait afficher la bonne texture pour le bouton NewGame ( en fonction du nombre dans la variable idNewGameButtonTexture) 
		GUI.skin.box.normal.background = listTextureNewGameButton[idNewGameButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 - 100,626.0f,100.0f), "");

		// On crée un rectangle a la bonne position pour qu'il soit sur la texture du bouton en question (NewGame) et le if permet de faire une 
		// action si le rectangle subit un clique 
		if (GUI.Button(new Rect(Screen.width/2 - 300,Screen.height/2 - 100,626.0f,100.0f),new GUIContent("","NewGameButton"),GUIStyle.none))
		{
            //cInput.Clear();
			//DestroyImmediate(Camera.main.gameObject);
			//AutoFade.LoadLevel("intro",1,0, /*new Color(0.99f, 0.99f, 0.99f, 1)*/ Color.black);
            playSound(click);
			startLoadingScreen();

			
		}
		
		
		GUI.skin.box.normal.background = listTextureConstrolsButton[idConstrolsButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 + 50,626.0f,100.0f), "");
		
		if (GUI.Button(new Rect(Screen.width/2 - 300,Screen.height/2 + 50,626.0f,100.0f),new GUIContent("","ControlsButton"),GUIStyle.none))
		{
            playSound(click);
			showPrincipalMenu = false;
		}
		
		
		GUI.skin.box.normal.background = listTextureQuitButton[idQuitButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 + 200,626.0f,100.0f), "");
		
		if (GUI.Button(new Rect(Screen.width/2 - 300,Screen.height/2 + 200,626.0f,100.0f),new GUIContent("","QuitButton"),GUIStyle.none))
		{
            playSound(click);
			Application.Quit();
		}
		
		
		// if qui permet de vérifier si le jeu render en se moment
		if (Event.current.type == EventType.Repaint)
		{
			// Permet de savoir le nom du rectangle qui est overlapper
			overButtonName = GUI.tooltip;
			// Si 1 alors la souris se trouve sur le bouton "NewGameButton" si 0 alors la souris ne se trouve pas sur le bouton
			// on affecte donc le bon numero de texture dans la liste
			if(overButtonName=="NewGameButton")
			{
				idNewGameButtonTexture = 1;
			}
			else 
			{
				idNewGameButtonTexture = 0;
			}
			
			if(overButtonName=="ControlsButton")
			{
				idConstrolsButtonTexture = 1;
			}
			else 
			{
				idConstrolsButtonTexture = 0;
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
	
	// Permet l'affichage du menu control avec la gestion de la souris
	void gestionSourisControlMenu()
	{
		// On fait afficher la bonne texture pour le bouton BackButton ( en fonction du nombre dans la variable idBackButtonTexture) 
		GUI.skin.box.normal.background = listTextureBackButton[idBackButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 850,Screen.height/2 + 400,626.0f,100.0f), "");
		
		// On crée un rectangle a la bonne position pour qu'il soit sur la texture du bouton en question (BackButton) et le if permet de faire une 
		// action si le rectangle subit un clique 
		if (GUI.Button(new Rect(Screen.width/2 - 850,Screen.height/2 + 400,626.0f,100.0f),new GUIContent("","BackButton"),GUIStyle.none))
		{
            playSound(back);
			showPrincipalMenu = true;
		}
		
		// if qui permet de vérifier si le jeu render en se moment
		if (Event.current.type == EventType.Repaint) 
		{
			// Permet de savoir le nom du rectangle qui est overlapper
			overButtonName = GUI.tooltip;
			
			// Si 1 alors la souris se trouve sur le bouton "BackButton" si 0 alors la souris ne se trouve pas sur le bouton
			// on affecte donc le bon numero de texture dans la liste
			if(overButtonName=="BackButton")
			{
				idBackButtonTexture = 1;
			}
			else 
			{
				idBackButtonTexture = 0;
			}
		}
	}
	
	// Permet l'affichage du menu principal avec la gestion de la manette
	void gestionManettePrincipalMenu()
	{
		// On fait afficher la bonne texture pour le bouton NewGame ( en fonction du nombre dans la variable idNewGameButtonTexture) 
		GUI.skin.box.normal.background = listTextureNewGameButton[idNewGameButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 - 100,626.0f,100.0f), "");
		
		
		GUI.skin.box.normal.background = listTextureConstrolsButton[idConstrolsButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 + 50,626.0f,100.0f), "");
		
		
		GUI.skin.box.normal.background = listTextureQuitButton[idQuitButtonTexture];
		GUI.Box(new Rect(Screen.width/2 - 300,Screen.height/2 + 200,626.0f,100.0f), "");
		
		// if qui permet de vérifier si le jeu render en se moment
		if (Event.current.type == EventType.Repaint) 
		{
			// Si le bouton "A" est appuyé
			if(cInput.GetButtonDown("buttonAController"))
			{
                playSound(click);
				// En fonction de la valeur de la variable idButton on effectu la bonne action
				switch(idButton)
				{
					case 1: 
					{
						//DestroyImmediate(Camera.main.gameObject);
						//AutoFade.LoadLevel("intro",1,0, /*new Color(0.99f, 0.99f, 0.99f, 1)*/ Color.black);
						startLoadingScreen();
						break;
					}
						
					case 2: 
					{
						timer = 0;
						showPrincipalMenu = false;
						break;
					}
						
					case 3: 
					{
						Application.Quit();
						break;
					}
					default:
					{
						break;
					}
				}
			}

			// La variable readyToChange permet  de savoir si un changement de bouton est possible
			if(readyToChange)
			{
				// Si le joystick est vers le haut, ou le pad
				if(cInput.GetAxis("axisY") > 0.25 ||  cInput.GetAxis("D-Pad-Y") < 0)
				{
                    playSound(highlight);
					readyToChange = false;
					if(idButton != 3)
					{
						idButton++;
					}
				}
				else if(cInput.GetAxis("axisY") < -0.25 ||  cInput.GetAxis("D-Pad-Y") > 0)
				{
                    playSound(highlight);
					readyToChange = false;
					if(idButton != 1)
					{
						idButton--;
					}
				}
				
				// En fonction de la valeur idButton (qui est l'id du bouton sélectionné) on donne au variable 
				// id de texture les bonnes valeurs
				if(idButton == 1)
				{
					idNewGameButtonTexture = 1;
					idConstrolsButtonTexture = 0;
					idQuitButtonTexture = 0;
				}
				else if(idButton == 2)
				{
					idNewGameButtonTexture = 0;
					idConstrolsButtonTexture = 1;
					idQuitButtonTexture = 0;
				}
				else
				{
					idNewGameButtonTexture = 0;
					idConstrolsButtonTexture = 0;
					idQuitButtonTexture = 1;
				}
			}
			else
			{
				// Si un relachement du joystick ou du pad a eu lieu on met la variable readyToChange a true, donc un changement de bouton est possible
				if((cInput.GetAxis("axisY") < 0.2 &&  cInput.GetAxis("axisY") > -0.2) && (cInput.GetAxis("D-Pad-Y") == 0 && cInput.GetAxis("D-Pad-Y") == 0))
				{
					readyToChange = true;
				}
			}
				

		}
		
	}
	
	// Permet l'affichage du menu control avec la gestion de la manette
	void gestionManetteControlMenu()
	{
		// Affichage du bouton back en rouge
		GUI.skin.box.normal.background = listTextureBackButton[1];
		GUI.Box(new Rect(Screen.width/2 - 850,Screen.height/2 + 400,626.0f,100.0f), "");
		
		if (Event.current.type == EventType.Repaint) 
		{
			// On met un compteur car lorsque le joueur a cliqué sur l'option "Control" le jeu revient trop rapidement ici
			// alors le menu controls se ferme toute suite car le joueur n'a pas le temps de relacher le bouton "A" de la manette
			timer += Time.deltaTime;
			if(timer > 0.2)
			{
				// Si "A" ou "B" sont appuyé
				if(cInput.GetButtonDown("buttonBController") || cInput.GetButtonDown("buttonAController"))
				{
                    playSound(back);
					showPrincipalMenu = true;
					idButton = 2;
				}
			}
		}
		
	}
	
	
	// Use this for initialization
	void Start () 
	{	
		// Création des listes de texture
		listTextureNewGameButton = new List<Texture2D> ();
		listTextureConstrolsButton = new List<Texture2D> ();
		listTextureQuitButton = new List<Texture2D> ();
		listTextureBackButton = new List<Texture2D> ();
		
		
		// Load les textures du bouton "NewGame"
		listTextureNewGameButton.Add(Resources.Load<Texture2D>("Menu/Main Menu/New_Game_Button_Not_Selected"));
		listTextureNewGameButton.Add(Resources.Load<Texture2D>("Menu/Main Menu/New_Game_Button_Selected"));
		idNewGameButtonTexture = 0;
		
		// Load les textures du bouton "Controls"
		listTextureConstrolsButton.Add(Resources.Load<Texture2D>("Menu/Main Menu/Controls_Button_Not_Selected"));
		listTextureConstrolsButton.Add(Resources.Load<Texture2D>("Menu/Main Menu/Controls_Button_Selected"));
		idConstrolsButtonTexture = 0;
		
		// Load les textures du bouton "Quit"
		listTextureQuitButton.Add(Resources.Load<Texture2D>("Menu/Main Menu/Quit_Button_Not_Selected"));
		listTextureQuitButton.Add(Resources.Load<Texture2D>("Menu/Main Menu/Quit_Button_Selected"));
		idQuitButtonTexture = 0;
		
		// Load les textures du bouton "Back"
		listTextureBackButton.Add(Resources.Load<Texture2D>("Menu/Controls Menu/Controls_Menu_Back_Not_Selected"));
		listTextureBackButton.Add(Resources.Load<Texture2D>("Menu/Controls Menu/Controls_Menu_Back_Selected"));
		idBackButtonTexture = 0;
		
		// On cherche les gameobject 
		principalMenuBackground = GameObject.Find("principalMenuBackground");
		controlslMenuBackground = GameObject.Find("controlsMenuBackground");
		
		// On veut afficher le menu principal au départ
		showPrincipalMenu = true;
		
		// Création pour la gestion des touches de la manette
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

        // J'ai ajouté ça pour loader des sons. Pas de Wwise dans le menu.
        try
        {
            musicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();

            musicSource.clip = Resources.Load<AudioClip>("TitleScreenSounds/MainTheme1_Orch_Voix_Loopable");

            musicSource.loop = true;

            musicSource.Play();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Je n'ai pas pu trouver la caméra!");
            musicSource = null;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            musicSource = null;
        }

        highlight = Resources.Load<AudioClip>("TitleScreenSounds/UI_Menu_Highlight");
        click = Resources.Load<AudioClip>("TitleScreenSounds/UI_Menu_Click");
        back = Resources.Load<AudioClip>("TitleScreenSounds/UI_Menu_Back");

		
		idButton = 1;
		
		readyToChange = true;
		
	}

	public void startLoadingScreen()
	{
		GameObject screenLoading = GameObject.FindGameObjectWithTag ("LoadingScreen");
		screenLoading.transform.localPosition = new Vector3 (0.4979212f, 0.5f, 0);
		
		screenLoading.GetComponent<LoadingScreenScript> ().directFull = true;
		screenLoading.GetComponent<LoadingScreenScript> ().enabled = true;

		GameObject.FindGameObjectWithTag ("controlsMenuBackground").SetActive(false);
		GameObject.FindGameObjectWithTag ("principalMenuBackground").SetActive(false);
	}
}













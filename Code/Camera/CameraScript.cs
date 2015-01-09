using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	
	public GameObject playerLight;
	public GameObject camera_Grab_V1;
	private GameObject cameraUI;
	[HideInInspector]	public GameObject cameraUIChild;

	[HideInInspector] public bool killCamMove;
	
	private Vector3 targetPosition;
	private Quaternion targetRotation;
	private float targetFieldOfView;
	private float camSpeed;
	private Animator animatorParent;

	private Transform player1;

	private string lastOwner;

	private int idStateNothing;

	private float timerAnim;
	
	private bool 	updateCameraPos = true;
	private bool 	cameraAnimIsPlay = false;


	private float standardGroundHeight;
	private Vector3 camPos;
	
	private Quaternion camRotation;

	public bool		lockCam;

	private Vector3		locationLock;

	private Vector3		locationBack;

	private	bool		startAnimGrab;

	private	bool		startAnimExecute;

	private GameObject	blackSaturationObject;

	private Vector3 velocityCam = Vector3.zero;
	private float dampTime = 0.09f;

	void Start()
	{
		// Si le joueur est dans la scene "mainPanic" on désactive l'objet devant la camera qui permet de faire une saturation noir de l'image
		if( Application.loadedLevelName != "mainPanic" )
		{
			blackSaturationObject = GameObject.FindGameObjectsWithTag("BlackSaturationObjectTag")[0];
			blackSaturationObject.transform.localPosition =  new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z+1);
			blackSaturationObject.transform.renderer.material.color = new Color (0, 0, 0, 0.0f);
		}

		timerAnim = 0;
		playerLight = GameObject.Find("Player Light");
		player1 = GameObject.Find("Player1").transform;
		camPos.y = 0;
		camPos.x = -3.84f;
		camPos.z = 0;

		updateCameraPos = true;
		cameraAnimIsPlay = false;


		cameraUI = GameObject.Find ("CameraUI");
		cameraUIChild = GameObject.Find ("CameraUIChild");
		
		cameraUIChild.transform.parent = cameraUI.transform;
		
		camRotation  = Camera.main.transform.rotation;
		camRotation.x = 0.06f;
		ResetCam();
		Camera.main.GetComponent<Vibration> ().setOriginalPosition ();

		camera_Grab_V1 = Camera.main.transform.parent.transform.parent.gameObject;

		cameraUI.transform.position = camera_Grab_V1.transform.position;
		cameraUI.transform.rotation = camera_Grab_V1.transform.rotation;

		animatorParent = Camera.main.transform.parent.transform.parent.GetComponent<Animator>();

		// On va chercher l'id de l'anim "doNothing"
		idStateNothing = Animator.StringToHash("Base Layer.doNothing");  

		lockCam = false;

		camera_Grab_V1.transform.position = new Vector3 (camPos.x, 0, 0);
		cameraUI.transform.position = new Vector3 (camPos.x, 0, 0);
	}
	
	public void ResetCam(){
		Camera.main.transform.position = UFE.config.cameraOptions.initialDistance;
		Camera.main.transform.localRotation = Quaternion.Euler(UFE.config.cameraOptions.initialRotation);
		Camera.main.fieldOfView = UFE.config.cameraOptions.initialFieldOfView;

		cameraUIChild.transform.position = UFE.config.cameraOptions.initialDistance;
		cameraUIChild.transform.localRotation = Quaternion.Euler(UFE.config.cameraOptions.initialRotation);
		cameraUIChild.GetComponent<Camera>().fieldOfView = UFE.config.cameraOptions.initialFieldOfView;
	}
	
	void FixedUpdate () 
	{
		if (killCamMove) return;

		Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFieldOfView, Time.deltaTime * camSpeed * 2);
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, Time.deltaTime * camSpeed * 2);
		Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, targetRotation, Time.deltaTime * camSpeed * 2);

		cameraUIChild.GetComponent<Camera>().fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFieldOfView, Time.deltaTime * camSpeed * 2);
		cameraUIChild.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, Time.deltaTime * camSpeed * 2);
		cameraUIChild.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, targetRotation, Time.deltaTime * camSpeed * 2);

	}
	
	void Update()
	{
		// Permet de dire si oui ou non on veut que la position de la camera soit mis a jour
		if (updateCameraPos) 
		{

			float cpt = 0;
			float speedTranslate = 55;
			Vector3 somme  = Vector3.zero;

			// For qui regarde tous les ennemis autour du joueur, est qui regarde s'ils ont le script "ControlsScript", ne sont pas mort et s'ils sont dans une certaine distance du joueur
			// si tout est vrai, on augmente la variable "somme" est le compteur "cpt" (compteur du nombre d'ennemi qui se trouve à une distance plus petite que 13)
			for (int i = 0; i < player1.GetComponent<ControlsScript>().opponents.Count; i++) 
			{
				if(player1.GetComponent<ControlsScript> ().opponents [i].transform.parent.GetComponent<ControlsScript>() != null )
				{
					if(!player1.GetComponent<ControlsScript> ().opponents [i].transform.parent.GetComponent<ControlsScript>().isDead)
					{
						// deux calcul de distance car si l'on prend juste 15 par exemple, la camera passera de faux a vrai (ex : 14.96 et 15.5) trop souvant donc on veut une "marge"
						// entre vrai et faux, donc 14 sera oublié
						if (Vector3.Distance (player1.transform.position, player1.GetComponent<ControlsScript> ().opponents [i].transform.position) < 15 &&
						    Vector3.Distance (player1.transform.position, player1.GetComponent<ControlsScript> ().opponents [i].transform.position) < 13 ) 
						{
							somme += player1.GetComponent<ControlsScript> ().opponents [i].transform.position;
							cpt++;
						}
						else 
						{
							speedTranslate = 40;
						}
					}
				}
			}

			// Si le compteur "cpt" n'est pas 0 ( donc des ennemis sont visible), on met la position de la camera le plus au centre de tous les locations d'ennemis, en effectuant un lerp entre la position du joueur
			// et la moyen des locations de tout les ennemis visible ( ex: si nous avons un seul ennemis la camera se positionnera entre le joueur et l'ennemi en question)
			// Si "cpt" = 0 donc aucun ennemi, alors la camera sera centré sur le joueur
			if(cpt != 0 )
			{
				camPos.x = Vector3.Lerp (UFE.GetPlayer1Position (), somme/cpt, 0.5f).x;
			}
			else
			{
				camPos.x = Vector3.Lerp (UFE.GetPlayer1Position (), UFE.GetPlayer1Position(), 0.5f).x;
			}

			// Si aucune anim de caméra n'est en se moment joué et que la camera n'est pas bloqué, on peut la déplacer à l'aide de la méthode "SmoothDamp" vers la direction calculé précédemment (camPos)
			if(!cameraAnimIsPlay && !lockCam)
			{
				camera_Grab_V1.transform.position = Vector3.SmoothDamp(camera_Grab_V1.transform.position, camPos, ref velocityCam, dampTime);
				cameraUI.transform.position = Vector3.SmoothDamp(cameraUI.transform.position, camPos, ref velocityCam, dampTime);
			}

			// Si aucune anim de caméra n'est en se moment joué mais que "lockCam" est a vrai, alors on déplace à la position voulu que la camera sois bloqué (locationLock)
			if(lockCam && !cameraAnimIsPlay)
			{
				camera_Grab_V1.transform.position = Vector3.SmoothDamp(camera_Grab_V1.transform.position, locationLock, ref velocityCam, 0.3f);
				cameraUI.transform.position = Vector3.SmoothDamp(cameraUI.transform.position, locationLock, ref velocityCam, 0.3f);
			}

			Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(7,0,0));
			Camera.main.transform.localPosition = new Vector3(0.5f,1.6f,-12f);

			cameraUIChild.transform.localRotation = Quaternion.Euler(new Vector3(7,0,0));
			cameraUIChild.transform.localPosition = new Vector3(0.5f, 3.0f, -12.0f);
		}
		else
		{

			if(cameraAnimIsPlay)
			{
				// on regarde si dans le controlleur d'anim le "state" du moment est l'id du stateNothing, si oui alors on replace la camera à la position qu'elle était avant le 
				// démarrage de l'Anim
				if (timerAnim > 1 && cameraAnimIsPlay && (animatorParent.GetCurrentAnimatorStateInfo(0).nameHash == idStateNothing)) 
				{
					backToPlace();
					timerAnim = 0;
				}
			}

			if(cameraAnimIsPlay)
			{
				centerCam();
				timerAnim += Time.deltaTime;
			}
		}
		
	}

	// Méthode appelé pour l'activation d'un shake de caméra, chaque parametre permettent de définir le shake
	public void shakeCamera(float xVibe,float yVibe,float zVibe,float xRot,float yRot,float zRot,float speed,float diminish,int numberOfShakes)
	{
		
		Camera.main.GetComponent<Vibration> ().setOriginalPosition ();
		Camera.main.GetComponent<Vibration> ().StartShaking(new Vector3(xVibe, yVibe, zVibe), new Quaternion(xRot, yRot, zRot, 1), speed, diminish, numberOfShakes);
	}
	
	public void MoveCameraToLocation(Vector3 targetPos, Vector3 targetRot, float targetFOV, float speed, string owner)
	{
		targetFieldOfView = targetFOV;
		targetPosition = targetPos;
		targetRotation = Quaternion.Euler(targetRot);
		camSpeed = speed;
		UFE.normalizedCam = false;
		lastOwner = owner;
		if (playerLight != null) playerLight.light.enabled = true;
	}
	
	public void ReleaseCam()
	{
		lastOwner = "";
	}

	// Méthode qui permet de démarrer l'anim de camera "Grab"
	public void grabAnim()
	{
		// Permet un random pour que l'anim ne soit pas toujours joué
		if(Random.Range (0.0F, 5.0F) <= 2)
		{
			if(!lockCam)
			{
				locationBack = camera_Grab_V1.transform.position;
			}
			else
			{
				locationBack = locationLock;
			}

			animatorParent.SetBool ("GrabAnim",true);
			updateCameraPos = false;
			cameraAnimIsPlay = true;
		}
	}

	// Méthode qui permet de démarrer l'anim de camera "execute"
	public void executeAnim()
	{
		if(!lockCam)
		{
			locationBack = camera_Grab_V1.transform.position;
		}
		else
		{
			locationBack = locationLock;
		}

		animatorParent.SetBool ("ExecuteAnim",true);
		updateCameraPos = false;
		cameraAnimIsPlay = true;
	}


	// Méthode qui permet de démarrer l'anim de camera "secondChance"
	public void secondChanceAnim()
	{
		if(!lockCam)
		{
			locationBack = camera_Grab_V1.transform.position;
		}
		else
		{
			locationBack = locationLock;
		}

		animatorParent.SetBool ("SecondChance",true);
		updateCameraPos = false;
		cameraAnimIsPlay = true;
	}
	
	public void SetCameraOwner(string owner)
	{
		lastOwner = owner;
	}
	
	public string GetCameraOwner()
	{
		return lastOwner;
	}
	
	public Vector3 GetRelativePosition(Transform origin, Vector3 position) 
	{
		Vector3 distance = position - origin.position;
		Vector3 relativePosition = Vector3.zero;
		relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
		relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
		relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);
		
		return relativePosition;
	}

	// Méthode appelé lorsque la camera doit être bloqué à une position précise ( si le joueur est à la limite du level, la camera se bloqué à une position, mais le joueur
	// pourra continuer dans cette direction jusqu'à être bloqué au mur invisible)
	public void lockCamera(Vector3 locationRecu)
	{
		lockCam = true;
		locationLock.x = locationRecu.x;
		locationLock.y = 0;
		locationLock.z = 0;
	}

	// Pour débloquer la camera
	public void unlockCamera()
	{
		lockCam = false;
	}

	// Permet de donner comme saturation la valeur de paramètre reçu, ainsi l'object devant la camera sera plus ou moins noir transparent et ainsi tout le visuel du jeu
	public void setBlackSaturation(float valeur)
	{
		blackSaturationObject.transform.renderer.material.color = new Color (0, 0, 0,valeur);
	}

	// Permet de retourner la camera à la position contenu dans la variable "locationBack", par exemple si un anim se termine
	public void backToPlace()
	{
		Vector3 tempLocation = cameraUI.transform.position;
		tempLocation.x = locationBack.x;
		
		cameraUI.transform.position = tempLocation;
		
		camera_Grab_V1.transform.position = locationBack;

		animatorParent.SetBool ("GrabAnim",false);
		animatorParent.SetBool ("ExecuteAnim",false);
		animatorParent.SetBool ("SecondChance",false);

		
		updateCameraPos = true;
		cameraAnimIsPlay = false;
	}

	// Permet de retourner vrai si une anim est en se moment joué
	public bool getIfAnimPlay()
	{
		if (animatorParent.GetBool ("GrabAnim") || animatorParent.GetBool ("ExecuteAnim")  || animatorParent.GetBool ("SecondChance") )
				return true;

		return false;
	}

	// Permet de centrer la camera sur le joueur
	public void centerCam()
	{
		camPos.x = UFE.GetPlayer1Position().x;
		camera_Grab_V1.transform.position = Vector3.SmoothDamp(camera_Grab_V1.transform.position, camPos, ref velocityCam, 0.3f);
		cameraUI.transform.position = Vector3.SmoothDamp(cameraUI.transform.position, camPos, ref velocityCam, 0.3f);
	}


}



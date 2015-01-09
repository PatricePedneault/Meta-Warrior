using UnityEngine;
using System.Collections;

public class BlockSideCamera : MonoBehaviour
{
	// Variable qui permet de savoir qu'elle direction le joueur doit etre blocker
	public orientationLock		directionLock = orientationLock.gauche;
	private CameraScript		cameraScript;

	// Bool qui permet de savoir est-ce que la camera est bloqué en se moment
	private bool				lockCam = false;

	// Location du Player
	private Transform			playerGlobal;

	// enum d'oritentation
	public enum orientationLock{droite,gauche};

	// Use this for initialization
	void Start () 
	{
		// On va chercher le script de la camera pour pouvoir arreter celle-ci lorsque l'on veut
		cameraScript = GameObject.Find("Game").GetComponent<CameraScript>();

		// Rend invisible le GameObject
		transform.renderer.material.color = new Color (1, 1, 1, -1.0f);

	}

	// Méthode qui est appeler lorsque l'on créer un prefab en code du volume pour bloquer la camera et que le player ne touche au GameObject (aucune appelle de OnTriggerEnter)
	// Donc on veut initialisé certaine valeur et que la limite de screen soit pris en compte now dans le update
	public void setCamLockNow()
	{
		playerGlobal = GameObject.Find("Player1").transform;
		lockCam = true;
	}

	// Permet de donner la valeur de directionLock a gauche
	public void setDirectionLockGauche()
	{
		directionLock = orientationLock.gauche;
	}

	// Permet de donner la valeur de directionLock a droite
	public void setDirectionLockDroite()
	{
		directionLock = orientationLock.droite;
	}

	// Appeler lorsque quelque chose entre en collision
	void OnTriggerEnter(Collider other) 
	{
		// Si c'est le player qui est entré en collision
		if(other.name == "Player1")
		{
			// Si la camera n'est pas deja bloqué
			if(!lockCam)
			{
				playerGlobal = GameObject.Find("Player1").transform;

				// Si la direction choisi comme paramettre est gauche
				if(directionLock == orientationLock.gauche)
				{
					// Si le joueur est arrivé par la droite on block la camera car il se dirige vers la location a bbloquer (gauche)
					// Sinon on n'a pas besoin de bloquer car le joueur se dirige vers la droite qui ne doit pas etre bloquer
					if((playerGlobal.transform.position.x-transform.position.x)>0)
					{
						lockCam = true;
						cameraScript.lockCamera(playerGlobal.transform.position);
					}
				}
				else
				{
					// Meme principe que le if plus haut mais de l'autre sense
					if((playerGlobal.transform.position.x-transform.position.x)<0)
					{
						lockCam = true;
						cameraScript.lockCamera(playerGlobal.transform.position);
					}
				}

			}
		}
	}

	// Lorsque le joueur quitte la collision, on regarde dans quelque direction il se dirige et s'il se dirige dans la direction non bloqué 
	// on peut donc remettre la camera qui suit le joueur, sinon rien ne se passe
	void OnTriggerExit(Collider other) 
	{
		if(directionLock == orientationLock.gauche)
		{
			if((playerGlobal.transform.position.x-transform.position.x)>0)
			{
				lockCam = false;
				cameraScript.unlockCamera();
			}
		}
		else
		{
			if((playerGlobal.transform.position.x-transform.position.x)<0)
			{
				lockCam = false;
				cameraScript.unlockCamera();
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Si la camera est bloqué on regarde la limitation du joueur dans l'écran et s'il dépasse on le remet juste avant
		if(lockCam)
		{
			if(playerGlobal.position.x > Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width-30,0,15)).x)
			{
				playerGlobal.position =  new Vector3(Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width-35,0,15)).x,playerGlobal.position.y,playerGlobal.position.z);
			}
			
			if(playerGlobal.position.x  < Camera.main.ScreenToWorldPoint(new Vector3 (30,0,15)).x)
			{
				playerGlobal.position =  new Vector3(Camera.main.ScreenToWorldPoint(new Vector3 (35,0,15)).x,playerGlobal.position.y,playerGlobal.position.z);
			}
		}
	
	}
}

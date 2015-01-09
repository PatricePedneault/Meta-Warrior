using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnnemiWaveScript : MonoBehaviour 
{
	// Valeur qui sera ajouté au niveau de panic du joueur
	public int					panicValue;

	// Variable qui contient le nombre d'ennemis qui doit avoir été tué en tout dans la wave
	public int 					nbAEnnemi;

	// Array qui contient l'ordre des ennemis a faire spawner 
	public CharacterInfo[] 		enemyQueue;

	// Array qui contient les ordres de directions (droite gauche) que les ennemis doivent spawner
	public orientationSpawn[] 	directionEnnemi;

	// Delay de spawn 
	public float				ennemySpawnDelay = 1.0f;

	// Variable qui dit combien d'ennemis peuvent se trouver en meme temps dans l'écran
	public int					nbEnnemyMaxInScreen = 2;

	// Nombre d'ennemis total qui sont mort jusqu'a maintenant dans la wave
	private int 				ennemyTotal;

	// Combien d'ennemis se trouver en se moment in screen
	private int					nbEnnemyNowInScreen;

	private float				cptDeltaTime;

	// Variable qui est a true lorsque le joueur entre en collision avec le volume et ainsi la wave peut commencer
	private bool				startWave;


	// Compteur qui permet de savoir ou on est rendu dans l'ordre des ennemis a spawner dans l'array "enemyQueue"
	private	int					cptEnemyQueue;

	// Compteur qui permet de savoir ou on est rendu dans l'ordre des lieu de spawn des ennemis dans l'array "directionEnnemi"
	private	int					cptEnemyOrientation; 

	private CameraScript		cameraScript;

	// Variable référent au GameObject sous Player1
	private Transform			playerMesh;

	// Variable référent a Player1
	private Transform			playerGlobal;

	private ControlsScript		playerControlScript;

	// Prefab qui sera utiliser pour faire afficher le volume de block du joueur et de la camera dans une direction
	private GameObject			blockLeftOrRightCameraPrefab;

	// GameObject qui contient le volume de block du joueur et de la camera dans une direction
	private GameObject			blockLeftOrRightCamera;

	private List<infoEnemy>		listInfoEnemy;

	public struct infoEnemy
	{

		public GameObject 		enemyGameObject;
		public ControlsScript	enemyControlsScript;

		public infoEnemy(GameObject enemyGameObjectRecu,ControlsScript	enemyControlsScriptRecu)
		{
			enemyGameObject = enemyGameObjectRecu;
			enemyControlsScript = enemyControlsScriptRecu;
		}

	}

	public enum orientationSpawn{droite,gauche};

	// Use this for initialization
	void Start () 
	{
		// Load le prefab
		blockLeftOrRightCameraPrefab = (GameObject) Resources.Load("Camera/BlockLeftOrRightCamera", typeof(GameObject));
		ennemyTotal = 0;
		nbEnnemyNowInScreen = 0;
		cptEnemyOrientation = 0;
		cptEnemyQueue = 0;
		startWave = false;

		listInfoEnemy = new List<infoEnemy>();

		transform.renderer.material.color = new Color (1, 1, 1, -1.0f);

	}

	void OnTriggerEnter(Collider other) 
	{
		// Si le joueur entre en collision
		if(!startWave &&  other.name == "Player1")
		{
			startWave= true;
			cameraScript = GameObject.Find("Game").GetComponent<CameraScript>();

			// Lock la camera et on lui donne la location ou elle doit s'arreter
			cameraScript.lockCamera(transform.position);

			playerGlobal = GameObject.Find("Player1").transform;

			playerControlScript = playerGlobal.GetComponent<ControlsScript>();

			playerMesh = playerGlobal.GetChild(0);
			playerMesh.GetComponent<WaitArrow>().enabled = false;

            UFE.switchBeUMusic(true);


		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		// Si la wave est starter
        if (startWave)
        {
            // On regarde si le joueur dépasse la vision de la screen vers la droite, si oui on le racule un peu
            if (playerGlobal.position.x > Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - 100, 0, 15)).x)
            {
                playerGlobal.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - 105, 0, 15)).x, playerGlobal.position.y, playerGlobal.position.z);
            }

            // On regarde si le joueur dépasse la vision de la screen vers la gauche, si oui on le racule un peu
            if (playerGlobal.position.x < Camera.main.ScreenToWorldPoint(new Vector3(80, 0, 15)).x)
            {
                playerGlobal.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(85, 0, 15)).x, playerGlobal.position.y, playerGlobal.position.z);
            }

            // Pour chaque ennemi 
            for (int i = 0; i < listInfoEnemy.Count; i++)
            {
                // Permet de savoir s'il est mort, si oui on l'enlève de la liste "ennemi" et sa valeur bool de la liste "ennemiIsOnScreen" pour savoir s'il est visible 
                // On diminue le nombre d'ennemis qui se trouve dans l'écran et on augmente le nombre d'ennemis mort au total
                if (listInfoEnemy[i].enemyGameObject.GetComponent<ControlsScript>() == null)
                {
                    cptDeltaTime = 0;
                    listInfoEnemy.RemoveAt(i);


                    ennemyTotal++;
                    nbEnnemyNowInScreen--;
                }
                else
                {
                    // On regarde si l'ennemis en question (i) est visible a l'écran
                    // Si oui on vérifie s'il sort de l'écran et si c'Est le cas on le racule
                    // Sinon on regarde demande a la méthode "enemyIsOnScreen" si l'ennemis est visible
                    if (listInfoEnemy[i].enemyControlsScript.inWaveScreen)
                    {
                        if (listInfoEnemy[i].enemyGameObject.transform.position.x > Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - 55, 0, 15)).x)
                        {
                            listInfoEnemy[i].enemyGameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - 60, 0, 15)).x, playerGlobal.position.y, playerGlobal.position.z);
                            //ennemi[i].transform.Translate(new Vector3(0.1f,0,0) );
                        }

                        if (listInfoEnemy[i].enemyGameObject.transform.position.x < Camera.main.ScreenToWorldPoint(new Vector3(55, 0, 15)).x)
                        {
                            listInfoEnemy[i].enemyGameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(60, 0, 15)).x, playerGlobal.position.y, playerGlobal.position.z);
                            //ennemi[i].transform.Translate(new Vector3(-0.1f,0,0) );
                        }
                    }
                    else
                    {
                        listInfoEnemy[i].enemyControlsScript.inWaveScreen = enemyIsOnScreen(listInfoEnemy[i].enemyGameObject.transform.position.x);
                    }
                }

            }

            // On augmtente la valeur de "cptDeltaTime" pour ainsi savoir si le delay est correct pour l'Affiche d'un nouvelle ennemis si c'Est possible
            cptDeltaTime += Time.deltaTime;

            // On addition le nombre total d'ennemis mort et le nombre d'ennemis en se moment dans l'écran pour ainsi savoir si c'est encore possible d'Ajouter des ennemis

            if ((ennemyTotal + nbEnnemyNowInScreen) < nbAEnnemi)
            {
                // On regarde si le nombre d'ennemis now in screen et plus petit que le max qui est possible d'Avoir en meme temps dans l'écran si oui on ajouter un nouvelle
                // ennemi
                // la condition (nbEnnemyNowInScreen==0 && ennemyTotal==0) permet de faire directement spawner un ennemis au tout début
                if ((nbEnnemyNowInScreen < nbEnnemyMaxInScreen && cptDeltaTime > ennemySpawnDelay) || (nbEnnemyNowInScreen == 0 && ennemyTotal == 0))
                {
                    cptDeltaTime = 0;
                    spawnEnnemi();
                }
            }
            // On regarde si on a attent le max d'ennemi qui doivent etre tuer pour terminer la wave, si oui on retir le volume avec ce script et on ajoute dans le niveau un nouveau
            // volume qui permet de bloquer le joueur dans une direction
            else if (ennemyTotal == nbAEnnemi)
            {
                transform.renderer.enabled = false;
                transform.GetComponent<EnnemiWaveScript>().enabled = false;
                playerMesh.GetComponent<WaitArrow>().enabled = true;

                blockLeftOrRightCamera = Instantiate(blockLeftOrRightCameraPrefab) as GameObject;
                blockLeftOrRightCamera.transform.position = transform.position;
                blockLeftOrRightCamera.GetComponent<BlockSideCamera>().setDirectionLockGauche();
                playerGlobal.GetComponent<PlayerManager>().addPanicToPlayer(panicValue);

                // Si le joueur est dans la direction droite (donc il ne doit pas etre block par la limite de la screen et la camera peut bouger, contrairement a
                // la gauche qui est impossible d'y retourner) alors on peut unlock la camera
                if ((playerGlobal.transform.position.x - transform.position.x) < 0)
                {
                    blockLeftOrRightCamera.GetComponent<BlockSideCamera>().setCamLockNow();

                }
                else
                {
                    cameraScript.unlockCamera();
                }

            }

        }
	}

	// Méthode qui permet de faire spawner des ennemis (code pris sur les spawn point de Gab)
	void spawnEnnemi()
	{
		Vector3 locationSpawn = transform.position;
		GameObject npc = new GameObject("Enemy 0");

		// En fonction de la location du spawn on donne la bonne position ( gauche ou droite)
		if(directionEnnemi[cptEnemyOrientation] == orientationSpawn.gauche)
		{
			locationSpawn.x = Camera.main.ScreenToWorldPoint (new Vector3 (0,0,25)).x; 

		}
		else
		{
			locationSpawn.x = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width,0,25)).x; 
		}


		npc.transform.parent = UFE.getThisAsATransform();

		npc.transform.position = locationSpawn;

		// ajout des script au npc/ennemis
		npc.AddComponent<ControlsScript>();
		npc.AddComponent<PhysicsScript>();
		npc.AddComponent<BoxCollider>();
		BoxCollider c = npc.collider as BoxCollider;

		
		if (c != null)
		{
			c.center = new Vector3(0.0f, 1.8f, 0.0f);
			c.size = new Vector3(2.0f, 3.5f, 1.0f);
			c.isTrigger = true;
		}
		
		npc.AddComponent<Rigidbody>();
		npc.rigidbody.useGravity = false;

		// En fonction du type de l'ennemis le case sera du type
		switch (enemyQueue[cptEnemyQueue].enemyType)
		{
			case EnemyType.EnemyTypeExample:
				npc.AddComponent<EnemyTypeExample>();
				break;
		}

		// Initialisation de variable
		npc.GetComponent<ControlsScript>().myInfo = (CharacterInfo)Instantiate(enemyQueue[cptEnemyQueue]);
		npc.GetComponent<ControlsScript> ().enemyIsInWave = true;

		// On ajout dans la liste "ennemi" le nouvelle ennemis et comme valeur false dans la liste "ennemiIsOnScreen" comme de quoi le nouvelle est ennemis n'Est pas visible

		// Si c'est une chien on fait d'autre manipulation ( on ne l'ajoute pas dans la liste car celui-ci ne reste pas longtemps a l'écran)
		if(enemyQueue [cptEnemyQueue].name != "Dog")
		{
			
			nbEnnemyNowInScreen++;

			listInfoEnemy.Add(new infoEnemy(npc,npc.GetComponent<ControlsScript>()));
		}
		else
		{
			ennemyTotal++;
			cptDeltaTime = 0;
		}

		nextEnemyQueue();
		nextEnemyOrientation();
		

	}

	// Méthode qui est appeler lorsque l'on veut augmenter le compteur de la liste "enemyQueue", la méthode permet de gérer si on atteint le max on retourne au début (0)
	void nextEnemyQueue()
	{
		if(cptEnemyQueue == (enemyQueue.Length - 1))
		{
			cptEnemyQueue = 0;
		}
		else
		{
			cptEnemyQueue++;
		}
	}

	// Méthode qui est appeler lorsque l'on veut augmenter le compteur de la liste "directionEnnemi", la méthode permet de gérer si on atteint le max on retourne au début (0)
	void nextEnemyOrientation()
	{
		if(cptEnemyOrientation == (directionEnnemi.Length - 1))
		{
			cptEnemyOrientation = 0;
		}
		else
		{
			cptEnemyOrientation++;
		}
	}

	// Méthode qui permet de savoir si un certain X dans le jeu est visible dans l'écran du joueur
	bool enemyIsOnScreen(float positionX)
	{
		if(positionX > Camera.main.ScreenToWorldPoint(new Vector3 (55,0,15)).x && positionX < Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width-55,0,15)).x)
		{
			return true;
		}
		return false;
	}

    void OnDisable()
    {
        UFE.switchBeUMusic(false);
    }

}






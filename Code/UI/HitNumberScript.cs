using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitNumberScript : MonoBehaviour 
{
	private List<Texture2D>	numberTexture;
	private List<Texture2D>	importantNbTexture;
	private Texture2D		hitTexte;

	private float 			timerHit;
	private float 			fadeOut;


	private bool			afficherHitNumber;

	private	int				numberHit;
	private int				uniter;
	private int				dixaine;


	void Start () 
	{
		string texteImage = "";

		numberTexture = new List<Texture2D> ();
		importantNbTexture = new List<Texture2D> ();

		afficherHitNumber = false;
		timerHit = 99;
		numberHit = 0;

		hitTexte = Resources.Load<Texture2D>("Hits/hits_hold");

		// On load les images différentes des autres, c'est a dire les images pour les hits 10,25,50
		importantNbTexture.Add(Resources.Load<Texture2D>("Numbers/10_big_hits"));
		importantNbTexture.Add(Resources.Load<Texture2D>("Numbers/25_big_hits"));
		importantNbTexture.Add(Resources.Load<Texture2D>("Numbers/50_big_hits"));

		// On load tous les numero des images de 1 a 9
		for (int i = 0; i<=9; i++) 
		{
			texteImage ="Numbers/"+ i + "_hits";
			numberTexture.Add(Resources.Load<Texture2D>(texteImage));
		}
	}
	

	void Update () 
	{
		// Permet de savoir si le temps d'affichage des hits est dépassé ou non
		if (timerHit > 6) 
		{
			if(timerHit < 99)
			{
				afficherHitNumber = false;
				timerHit = 99;
			}
		} 
		else 
		{
			timerHit += Time.deltaTime;
		}
	}

	// Méthode qui permet l'ajout d'un hit lorsqu'elle est appelé
	public void addHit()
	{
		// Plus haut que 100 on ajout aucun hit
		if(numberHit<100)
		{
			if(fadeOut != 1.0f)
			{
				numberHit = 0;
			}

			numberHit++;
			timerHit = 0;
			afficherHitNumber = true;
			fadeOut = 1.0f;


		}
	}

	// Méthode appelé lorsque le joueur recois des coups, donc le nombre de hit combo doit retourner a 0
	public void playerHit()
	{
		afficherHitNumber = false;
		timerHit = 99;
	}

	void OnGUI()
	{
		if (afficherHitNumber || fadeOut > 0.0f) 
		{
			// Si la variable "afficherHitNumber" est a false doit les images de hit combo doivent disparaitre, donc on diminue la variable "fadeOut" 
			// tranquillement
			if(!afficherHitNumber)
			{
				fadeOut -= 0.005f;
			}

			// Permet de donner la transparence des images de hit, ainsi en fonction de la valeur de la variable "fadeOut" les images seront plus transparente ou non
			// ce qui va permette un fadeout des images
			GUI.color = new Color(1.0f,1.0f,1.0f,fadeOut);

			GUI.skin.box.normal.background = hitTexte;
		

			GUI.Box(new Rect(700.0f,80.0f,200.0f,200.0f), "");

			// Les prochains if permettent l'affichage des gros nombres 10,25,50, de plus quand le nombre de coup et plus petit que 9 les images ne sont pas 
			// situé a la meme place donc un if est seulement pour sa, sinon le dernier else permet l'affichage normal (nombe a deux chiffres)
			if(numberHit==10)
			{
				GUI.skin.box.normal.background = importantNbTexture[0];
				GUI.Box(new Rect(720.0f,90.0f,250.0f,250.0f), "");
			}
			else if(numberHit==25)
			{
				GUI.skin.box.normal.background = importantNbTexture[1];
				GUI.Box(new Rect(720.0f,90.0f,250.0f,250.0f), "");
			}
			else if(numberHit==50)
			{
				GUI.skin.box.normal.background = importantNbTexture[2];
				GUI.Box(new Rect(720.0f,90.0f,250.0f,250.0f), "");
			}
			else if(numberHit<=9)
			{

				GUI.skin.box.normal.background = numberTexture[numberHit];
				GUI.Box(new Rect(700.0f,90.0f,200.0f,200.0f), "");
			}
			else
			{
				// On prend en note la dixaine du nombre de hit et l'uniter aussi pour 
				// par la suite prendre la bonne position dans le tableau des images des nombres
				// de 1 a 9
				dixaine = numberHit/10;
				uniter = numberHit%10;

				GUI.skin.box.normal.background = numberTexture[dixaine];
				GUI.Box(new Rect(665.0f,90.0f,200.0f,200.0f), "");

				GUI.skin.box.normal.background = numberTexture[uniter];
				GUI.Box(new Rect(700.0f,90.0f,200.0f,200.0f), "");
			}


		}
		
	}
}

















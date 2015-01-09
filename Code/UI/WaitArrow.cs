using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaitArrow : MonoBehaviour 
{
	private Vector3 		lastPosition;

	public GameObject		gameObjectflashArrow;

	private GameObject		arrowWait;

	private float 			timerHit;

    private bool            soundPlayed = false;

	void Start () 
	{
		arrowWait = Instantiate(gameObjectflashArrow) as GameObject;
		arrowWait.transform.renderer.enabled = false;
		timerHit = 0;
	}

	// Méthode qui permet de regarder si des ennemis son visible a une certaine distance
	bool ennemiAround()
	{
		for (int i = 0; i < this.transform.parent.GetComponent<ControlsScript>().opponents.Count; i++) 
		{
			if(this.transform.parent.GetComponent<ControlsScript> ().opponents[i].transform.parent.GetComponent<ControlsScript>() != null)
			{
				if (!this.transform.parent.GetComponent<ControlsScript> ().opponents[i].transform.parent.GetComponent<ControlsScript> ().isDead) 
				{
					if (Vector3.Distance (this.transform.position, this.transform.parent.GetComponent<ControlsScript> ().opponents [i].transform.position) < 11) 
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	void Update () 
	{
		// Permet de savoir si le joueur c'est déplacé depuis le dernier update 
		if(transform.position == lastPosition)
		{
			// On demande a la méthode si un ennemis est visible 
			if(!ennemiAround())
			{
				// On augement le compteur pour ainsi savoir depuis combien de temps le joueur ne se déplace pas
				timerHit += Time.deltaTime;
				if(timerHit >= 2)
				{
					// Si cela fait plus de 2 secondes que le joueur n'a pas bougé et que l'anim de la flèche n'est pas démarré, on la démarre
					if (!arrowWait.GetComponent<GAFMovieClip> ().isPlaying ()) 
					{
						arrowWait.transform.renderer.enabled = true;
						arrowWait.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-250,Screen.height-100,10));
						Camera.main.WorldToViewportPoint(this.transform.parent.transform.position);
						arrowWait.GetComponent<GAFMovieClip>().play ();

						// On joue le son
                        if (!soundPlayed)
                        {
                            UFE.PlaySound("UI_ThisWay", gameObject);
                            soundPlayed = true;
                        }
					}
				}
			}
		}
		else
		{
			// Si le joueur c'est déplacer (n'est pas à la meme position depuis le dernier update) on arrête l'anim
			arrowWait.GetComponent<GAFMovieClip>().stop();
			arrowWait.transform.renderer.enabled = false;
			timerHit = 0;
			lastPosition = transform.position;
            soundPlayed = false;
		}
	}

}

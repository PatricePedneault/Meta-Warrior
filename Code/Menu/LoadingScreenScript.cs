using UnityEngine;
using System.Collections;

public class LoadingScreenScript : MonoBehaviour 
{
	public  string		levelToLoad = "intro";

	public	GameObject	whiteBackground;

	public  bool		directFull = false;

	private	bool		start = false;

	// Le loading screen fonctionne avec 3 images une par-dessus l'autre, une background tout en arrière, une image devant qui a comme transparence MetaWarrior (donc on voir l'arrière) et pour finir une 
	// image au centre ( rouge ) qui sera déplacer de gauche a droite plus le loading de la scene avancera, ce qui fera comme si la bar de loading augmenterait 
	
	// Méthode qui permet a une coroutine de loader une scene et en meme temps mettre a jour la screen du jeu qui est toujours mis a jour et disponible 
	IEnumerator DisplayLoadingScreen(string level)
	{
		// Si directFull=true alors la bar de loading doit deja être full, certaine fois il n'y a vraiment pas beaucoup à loader est la bar de loading n'a pas le temps d'apparaitre pleine, donc on la met direct
		// pleine
		if(directFull)
		{
			// Remplit directement la bar de loading
			whiteBackground.transform.localPosition = new Vector3(1,whiteBackground.transform.localPosition.y,whiteBackground.transform.localPosition.z);
		}

		// Démarrage du loading
		AsyncOperation async = Application.LoadLevelAsync (level);

		// T'en que le loading n'est pas terminé
		while(!async.isDone)
		{
			// Si le loading est mis comme directement plein on ne fait aucun déplacement
			if(!directFull)
			{
				whiteBackground.transform.localPosition = new Vector3((async.progress)*1.6f,whiteBackground.transform.localPosition.y,whiteBackground.transform.localPosition.z);
			}
			yield return null;
		}
	}
	

	void Update () 
	{
		// Si le loading d'une nouvelle scene est démarré on ne le refait pas
		if(!start)
		{	
			// Start une coroutine qui fera le loading de la map
			start = true;
			StartCoroutine (DisplayLoadingScreen (levelToLoad));
		}


	}
}

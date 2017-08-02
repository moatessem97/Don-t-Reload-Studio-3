using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

	public Image SplashImg;
	public string loadLevel;

	IEnumerator Start()
	{
			SplashImg.canvasRenderer.SetAlpha (0.0f);

			FadeIn ();
			yield return new WaitForSeconds(2.5f);

			FadeOut ();
			yield return new WaitForSeconds(2.5f);
			SceneManager.LoadScene(loadLevel);

	}

	
	void FadeIn()
	{
		SplashImg.CrossFadeAlpha (1.0f, 1.5f, false);
	}

	void FadeOut()
	{
		SplashImg.CrossFadeAlpha (0.0f, 2.5f, false);
	}
}

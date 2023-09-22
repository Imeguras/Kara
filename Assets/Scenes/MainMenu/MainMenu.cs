using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UIElements;
using Kara_.Assets.Settings.General_Settings;
using System;
using System.Collections;

public class MainMenu : MonoBehaviour{ 
    Button btnQuit=null;
	Button btnMp=null;
	//This is used to check if its doing any loading task in this class's context, dont use it to check if the game is doing some process
	public static Boolean isRunning=false;
	
	public void Start(){
		VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
		btnQuit = root.Q<Button>(name: "BtnQuit");
		btnMp = root.Q<Button>(name: "BtnMp");
		//Disregard name, its just to spare code
		RageEnd();
	}
	public IEnumerator CreateWorld(){
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
		while (!asyncLoad.isDone)
        {
            yield return null;
        }
		
	}
	public void Multiplayer(){
		RageSetup();
		StartCoroutine(CreateWorld()); 
		RageEnd();
	}
	public void Exit(){
		
		RageSetup();
		Debug.Log("Exiting gracefully");
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit();
		//This should be in theory unreachable
		RageEnd();
	}
	// Avoids users clicking exit and then clicking other buttons making the game ultra slow and confused, on exchange it makes the user rage more as a misclick causes them to have to commit to the decision hence the name
	public void RageSetup(){
		isRunning = true;
		btnQuit.clickable.clicked -= Exit; 
		btnMp.clickable.clicked -= Multiplayer;
		
	}
	public void RageEnd(){
		isRunning = false;
		btnQuit.clickable.clicked += Exit; 
		btnMp.clickable.clicked += Multiplayer; 
	}

}

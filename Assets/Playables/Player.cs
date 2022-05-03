
using System.Linq;
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kara.Playables{
	public class Spectator{
		protected int uuid;
		protected string playerName;
		protected GameObject pCamera;
		protected GameObject curCamera; 
		
		public Spectator(string playerName){
			this.uuid=playerName.GetHashCode();
			this.playerName=playerName; 
			creatCamera(ref pCamera);
		
			curCamera=pCamera; 
		}
		public int getUUID(){
			return this.uuid;

		}
		public string getIG_Name(){
			return this.playerName;
		}
		public void getCurCamera(ref GameObject camera){
			camera=curCamera; 
		}
		public void creatCamera(ref GameObject go){
			go= new GameObject(uuid+"_MainCamera");
			go.AddComponent<Camera>();
		
		}
		public void resetCamera(){
			if(pCamera!=null){
				curCamera=pCamera;
			}else{
				Debug.LogWarning("Default Player Camera(DPC) was erased creating a new one");
				pCamera= new GameObject();
				resetCamera(); 
			}
		}
    }
	public class Player : Spectator {
		private Color32 playerColor; 
		private Team team;
		private Resources playerResources; 
		private Research playerResearch;
		public Player(string playerName, Color32 igColor): base(playerName){
			this.playerColor=igColor;
			this.team=Team.defaultTeam; 
			this.playerResources=new Resources();
			this.playerResearch= new Research();
			this.pCamera.AddComponent<GameEyes>();
			UpdateAvailableBuilds();
			
		}
		public void UpdateAvailableBuilds(){
			int i=0;
			GameObject gm=UnityEngine.Resources.Load<GameObject>("ButtonBuilding");
			foreach (var item in playerResearch.availableBuild){
				GameObject button=GameObject.Instantiate(gm,Vector3.zero, Quaternion.identity);
				button.name=("ButtonBuilding"+(++i));
				
				button.transform.SetParent(GameObject.Find("Buildings").transform);
				button.transform.position=new Vector3(500,128,0);
								

			}
		}
	}

}

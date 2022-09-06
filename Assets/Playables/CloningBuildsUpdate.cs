using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Security;
using Unity.VisualScripting;

using UnityEngine; 
using UnityEngine.UIElements; 
using Mirror;

namespace Kara.Playables{
	public class CloningBuildsUpdate : NetworkBehaviour{
		//public static CloningBuildsUpdate Instance;
		public override void OnStartClient(){
			base.OnStartClient();
			isReady();
		}
		
		/*void Awake()
		{
			if(Instance == null)
				Instance = this;
			else
				Destroy(this);
		}*/
		[Command]
		public void isReady(){
			TargetUpdateAvailableBuilds(connectionToClient, GameBoardMan.players.Find(x=>x.connID==connectionToClient.connectionId).getPlayerResearch());
		}
		//TODO REMOVE THIS 
		[TargetRpc]
		public void test(NetworkConnection target){
			Debug.Log("test");
			
		}
	
		[TargetRpc]
		public void TargetUpdateAvailableBuilds(NetworkConnection target, Research playerResearch){	
			//Debug.Log("yay");
			Player player= target.identity.GetComponent<GameEyes>().target;
			//this.player=(Player)this.netIdentity.GetComponent<GameEyes>().target;
			
				foreach (var item in playerResearch.availableBuild){
					var k=item.getIcn(); 

					UIDocument docum = GameObject.Find("Canvas").GetComponent<UIDocument>();
					var cloningUI = docum.rootVisualElement.Q<VisualElement>(name: "CloningInstances");
					//apend k to cloningUI
					Button b = new Button();
					b.text = "";
					b.style.backgroundImage = new StyleBackground(k);
					b.style.width = 32;
					b.style.height = 32;
					cloningUI.Add(b);
				
					b.clicked += () => {
						item._gm_inst_now(); 
						player.cur_buildProto=item;
						//For MP purposes i will continue to use Game eyes to get the proper controls
						player.addProc(); 
					};			
					
				}
				
		}
	}
}
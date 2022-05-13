using System.Globalization;
using System.Reflection;
using System;
using UnityEngine;
namespace Kara.Playables
{
	public class Resources_Tab : MonoBehaviour{
		public Player tracking; 
		public void setTrackingPlayer(Player track){
			tracking=track;
			foreach(GameObject t in GameObject.FindGameObjectsWithTag("ResourcesUI")){
				updateResource(t); 
			}
			
		}
		public void updateResource(GameObject t){
			UnityEngine.UI.Text lbl_txt= t.GetComponent<UnityEngine.UI.Text>();
				_Resources playerTrackResources =tracking.getPlayerResources();
				switch (t.name){
					case ("Food"):
						lbl_txt.text=playerTrackResources.food.ToString();
					break;
					case ("Wood"):
						lbl_txt.text=playerTrackResources.wood.ToString();
					break;
					case ("Stone"):
						lbl_txt.text=playerTrackResources.stone.ToString();
					break;
					case ("Iron"):
						lbl_txt.text=playerTrackResources.iron.ToString();
					break; 
					case ("Gold"):
						lbl_txt.text=playerTrackResources.gold.ToString();
					break;
					default:
					break;
				}
		}
		public class _Resources{
			public uint food{get; set;}
			public uint stone{get; set;}
			public uint wood{get; set;}
			public uint gold{get; set;}
			public uint iron{get; set;}

			public _Resources(uint food=0, uint stone=0, uint wood=0, uint gold=0, uint iron=0){
				this.food=food; 
				this.stone=stone; 
				this.wood=wood; 
				this.gold=gold; 
				this.iron=iron;
			}

    	}
	}
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kara.Playables
{
	public class Resources_Tab : MonoBehaviour{
		public Player tracking; 
		public void setTrackingPlayer(Player track){
			tracking=track;
			track.getPlayerResources();
			GameObject trackCanvas = GameObject.Find("Canvas");
			UIDocument docum= trackCanvas.GetComponent<UIDocument>();
				//TODO: unboogaloo this
				var insideCanvas = docum.rootVisualElement.Children().First().Children().First().Children().First().Children();
				
				//insideCanvas.ElementAt(0).ElementAt(1).Bind(tracking.getPlayerResources().food);
					//resourcesUI.Add(new Tuple<string, VisualElement>(k.name, k));
				
			}

		public class _Resources{
			
			public uint food{get; set;}
			
			public uint wood {get; set;}
			public uint stone{get; set;}
			public uint gold{get; set;}
			public uint iron{get; set;}

			public _Resources(uint food=0, uint wood=0,uint stone=0, uint gold=0, uint iron=0){
				
				this.food=food; 
				this.wood=wood; 
				this.stone=stone; 
				this.gold=gold; 
				this.iron=iron;
				//setUiLink();
				//updateUiResources();
			}
		
			
    	}
	}   
}

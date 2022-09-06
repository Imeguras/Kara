using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements; 
//using UnityEditor;
//using UnityEditor.UIElements;

namespace Kara.Playables{
	public class Resources_Tab : NetworkBehaviour{
		public Player tracking; 
		_Resources playerResources;
		private Action<object> _refFood; 
		private Action<object> _refWood; 
		private Action<object> _refStone; 
		private Action<object> _refIron; 
		private Action<object> _refGold;
		
	    
		public void setTrackingPlayer(Player track){
			if(tracking!=null){
				removeTrackingPlayer();
			}
			tracking=track;
			playerResources = track.getPlayerResources();
			GameObject trackCanvas = GameObject.Find("Canvas");
			UIDocument docum= trackCanvas.GetComponent<UIDocument>();
			Label foodUI = docum.rootVisualElement.Q<Label>(name: "FoodVar");
			Label woodUI = docum.rootVisualElement.Q<Label>(name: "WoodVar");
			Label stoneUI = docum.rootVisualElement.Q<Label>(name: "StoneVar");
			Label ironUI= docum.rootVisualElement.Q<Label>(name: "IronVar");
			Label goldUI = docum.rootVisualElement.Q<Label>(name: "GoldVar");
			
			 if(playerResources!= null){
			 	
			 	_refFood=foodUI.BindProperty(playerResources.food);
			 	_refWood=woodUI.BindProperty(playerResources.wood);
			  	_refStone=stoneUI.BindProperty(playerResources.stone);
			  	_refIron=ironUI.BindProperty(playerResources.iron);
			  	_refGold=goldUI.BindProperty(playerResources.gold);

 			 }else{
				//TODO: most likely a spectator hide UI stuff here
			 	Debug.LogWarning("Player Resources is null!! Is it a spectator?");
			
			 }
		}
		public void removeTrackingPlayer(){
			if(tracking!=null){
				GameObject trackCanvas = GameObject.Find("Canvas");
				UIDocument docum= trackCanvas.GetComponent<UIDocument>();
				Label foodUI = docum.rootVisualElement.Q<Label>(name: "FoodVar");
				Label woodUI = docum.rootVisualElement.Q<Label>(name: "WoodVar");
				Label stoneUI = docum.rootVisualElement.Q<Label>(name: "StoneVar");
				Label ironUI= docum.rootVisualElement.Q<Label>(name: "IronVar");
				Label goldUI = docum.rootVisualElement.Q<Label>(name: "GoldVar");
				foodUI.Unbind(_refFood,playerResources.food);
				woodUI.Unbind(_refWood,playerResources.wood);
				stoneUI.Unbind(_refStone, playerResources.stone);
				ironUI.Unbind(_refIron, playerResources.iron);
				goldUI.Unbind(_refGold, playerResources.gold);
			}
		}
	}
	public class _Resources : ScriptableObject{
		public Property<int> food; 
		public Property<int> wood; 
		public Property<int> stone;
		public Property<int> iron;
		public Property<int> gold;
		//basically a constructor cause unity has a problem with it 
		public void Init(int food=0, int wood=0,int stone=0, int iron=0, int gold=0){
			this.food=new Property<int>(food); 
			this.wood=new Property<int>(wood); 
			this.stone=new Property<int>(stone); 
			this.iron=new Property<int>(iron);
			this.gold=new Property<int>(gold); 
		}
		
		
	}

}

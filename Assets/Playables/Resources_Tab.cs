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
			_Resources playerResources = track.getPlayerResources();
			GameObject trackCanvas = GameObject.Find("Canvas");
			UIDocument docum= trackCanvas.GetComponent<UIDocument>();
			
		
				var foodUI = docum.rootVisualElement.Q<Label>(name: "FoodVar");
				var woodUI = docum.rootVisualElement.Q<Label>(name: "WoodVar");
				var stoneUI = docum.rootVisualElement.Q<Label>(name: "StoneVar");
				var ironUI= docum.rootVisualElement.Q<Label>(name: "IronVar");
				var goldUI = docum.rootVisualElement.Q<Label>(name: "GoldVar");
			if(playerResources!= null){
				
				SerializedObject pR = new UnityEditor.SerializedObject(playerResources);
				//Next time read the fine print... Unity and its internall boogaloo never cease to amuse me...
				//The comment above was because i thought that you had to translate the your variables to some kind of internal meta variable one, aparently not unity cannot handle int blah{get;set;}... 
				SerializedProperty foodProp = pR.FindProperty("food");
				SerializedProperty woodProp = pR.FindProperty("wood");
				SerializedProperty stoneProp = pR.FindProperty("stone");
				SerializedProperty ironProp = pR.FindProperty("iron");
				SerializedProperty goldProp = pR.FindProperty("gold");

				foodUI.BindProperty(foodProp);
				woodUI.BindProperty(woodProp);
				stoneUI.BindProperty(stoneProp);
				ironUI.BindProperty(ironProp);
				goldUI.BindProperty(goldProp);

 			}else{
				foodUI.Unbind();
				woodUI.Unbind();
				stoneUI.Unbind();
				ironUI.Unbind();
				goldUI.Unbind();
			}	
		}
	}
	public class _Resources : ScriptableObject{
		public int food=0; 
		public int wood=0; 
		public int stone=0;
		public int iron=0;
		public int gold=0;
		//basically a constructor cause unity has a problem with it 
		public void Init(int food=0, int wood=0,int stone=0, int iron=0, int gold=0){
				
			this.food=food; 
			this.wood=wood; 
			this.stone=stone; 
			this.iron=iron;
			this.gold=gold; 
		
		}
		
		
	}

}

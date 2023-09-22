
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Mirror; 
using UnityEngine;
using UnityEngine.UIElements;

using Kara.Entities;
using Kara_.Assets.Settings.Player_Settings;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

namespace Kara.Playables{
	public class Spectator{
		protected int uuid;
		protected string playerName;
		protected List<GameObject> sleepingCameras; 
		protected GameObject pCamera;
		protected GameObject curCamera; 
		protected ControlSettings settings;
		protected IDictionary<InputAction, Action<InputAction.CallbackContext>> _ref_callbacks;
		protected NetworkIdentity _ref_networkIdentity;
		public int connID {get;} 
		public GameEyes gameEyes; 
		public Spectator(string playerName,int connID,  ControlSettings settings, GameObject pCamera=null){
			
			this.connID=connID;
			this.uuid=playerName.GetHashCode();
			this.playerName=playerName; 
			this._ref_callbacks = new Dictionary<InputAction, Action<InputAction.CallbackContext>>();
			sleepingCameras=new List<GameObject>();
			if(pCamera!=null){
				this.pCamera=pCamera;
				gameEyes=this.pCamera.GetComponent<GameEyes>();
			}else{
				creatCamera(ref pCamera);
				gameEyes=this.pCamera.AddComponent<GameEyes>();
				_ref_networkIdentity= this.pCamera.AddComponent<NetworkIdentity>();
			
			}
			this.settings=settings;
			
			
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
		public NetworkIdentity getIdentity(){
			return _ref_networkIdentity;
		}
		
		public void switchCamera(ref GameObject toCamera){
			curCamera.SetActive(false);
			sleepingCameras.Add(curCamera); 
			curCamera=toCamera;
			curCamera.SetActive(true);
		}
		public void creatCamera(ref GameObject go){
			go= new GameObject(uuid+"_MainCamera");
			go.AddComponent<Camera>();
		}
		public GameObject resetCamera(){
			if(pCamera!=null){
				//make every camera inactive on scene
				foreach(var cam in GameObject.FindObjectsOfType<Camera>()){
					cam.gameObject.SetActive(false);
				}
				curCamera=pCamera;
				curCamera.SetActive(true);
				return curCamera;  
			}else{
				Debug.LogWarning("Default Player Camera(DPC) was erased creating a new one");
				creatCamera(ref pCamera);
				return resetCamera(); 

			}
		}
    }
	public class Player : Spectator {
		protected Color32 playerColor; 
		protected Team team;
		protected _Resources playerResources; 
		protected Research playerResearch;
		protected PlayerControls controlSettings;
		 
		// IFFUCKED: check here this might be a cause, its not a great idea to do this but i don't have a clue how else i can do it(without changing the code in its entirity)
		public _Prototype<Building_Abstract> cur_buildProto;
		
		public Player(string playerName, int connID,  Color32 igColor, ControlSettings settings, GameObject camera=null): base(playerName, connID, settings, camera){
	     
			this.playerColor=igColor;
			this.team=Team.defaultTeam; 
			this.playerResources= _Resources.CreateInstance<_Resources>(); 
			this.playerResources.Init(100,100,100,100,100);
			//TODO THIS IS RETARDED SHOULD BE SPECTATOR
			this.pCamera.GetComponent<GameEyes>().bindIdentity(this); 
			this.controlSettings= this.pCamera.GetComponent<GameEyes>().controls;
			
			this.playerResearch= new Research();
			
			
		}
		
		public void addProc(){
			//_ref_callbacks.Add()
			Action<InputAction.CallbackContext> actionone= (ctx => updatePos(ctx.ReadValue<Vector2>()));
			Action<InputAction.CallbackContext> actiontwo = (ctx => buildProc(ctx.ReadValue<float>()));
			//Debug.Log(controlSettings.TERRA_INT.AwaitMouse); 
			_ref_callbacks.Add(controlSettings.TERRA_INT.AwaitMouse, actionone);
			_ref_callbacks.Add(controlSettings.TERRA_INT.HoldTillMouse, actiontwo);

			controlSettings.TERRA_INT.AwaitMouse.performed += actionone; 
			controlSettings.TERRA_INT.HoldTillMouse.performed += actiontwo;
			
		}
		// This seems to not be working... which is ultra annoying
		public void endProc(){
			//aparently dictionaries dont rearrange themselves after removing an element
			var k = _ref_callbacks.Keys.ToList();
			foreach(var item in k){
				item.performed -= _ref_callbacks[item];
				//remove key value from dictionary
				_ref_callbacks.Remove(item);
			}

		}
		public void buildProc(float val){
			if(val==1){
				instProt();

			}else{
				cancProt();
			}		
		}

		public void updatePos(Vector2 pos){
			RaycastHit hit;
			Ray ray = pCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(pos.x,pos.y,0));
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				cur_buildProto.obj_Preview(hit.point);
			}
		}

		
		void cancProt(){
			endProc();
			cur_buildProto._gm_dest_now(); 
		}		
		void instProt(){
			endProc();
			cur_buildProto._gm_orig_jit(cur_buildProto.getObj_Prtp().transform);
			GameObject t=GameObject.Instantiate<GameObject>(Capital._Capital_Prototype._getObj_Prtp(), Capital._Capital_Prototype._getObj_Prtp().transform.position,  Capital._Capital_Prototype._getObj_Prtp().transform.rotation);
			t.GetComponent<MeshRenderer>().material=UnityEngine.Resources.Load<Material>("WoodHouse");
			cur_buildProto._gm_dest_now(); 
		}
		
		
		
		
		public _Resources getPlayerResources(){
			return this.playerResources;
		}
		public Research getPlayerResearch(){
			return this.playerResearch;
		}
	}

}

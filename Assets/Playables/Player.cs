
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kara.Entities;
namespace Kara.Playables{
	public class Spectator{
		protected int uuid;
		protected string playerName;
		protected List<GameObject> sleepingCameras; 
		protected GameObject pCamera;
		protected GameObject curCamera; 
		
		public Spectator(string playerName){
			this.uuid=playerName.GetHashCode();
			this.playerName=playerName; 
			sleepingCameras=new List<GameObject>();
			 
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
		public void resetCamera(){
			if(pCamera!=null){
				if(curCamera){
					curCamera.SetActive(false); 
				}
				curCamera=pCamera;
				curCamera.SetActive(true); 
			}else{
				Debug.LogWarning("Default Player Camera(DPC) was erased creating a new one");
				creatCamera(ref pCamera);
				resetCamera(); 
			}
		}
    }
	public class Player : Spectator {
		protected Color32 playerColor; 
		protected Team team;
		protected _Resources playerResources; 
		protected Research playerResearch;
		// IFFUCKED: check here this might be a cause, its not a great idea to do this but i don't have a clue how else i can do it(without changing the code in its entirity)
		protected static _Prototype<Building_Abstract> cur_buildProto;
		public Player(string playerName, Color32 igColor): base(playerName){
			this.playerColor=igColor;
			this.team=Team.defaultTeam; 
			this.playerResources= _Resources.CreateInstance<_Resources>(); 
			this.playerResources.Init(100,100,100,100,100);

			this.playerResearch= new Research();
			this.pCamera.AddComponent<GameEyes>();
			UpdateAvailableBuilds();
			
		}
		public void UpdateAvailableBuilds(){
			//int i=0;
			//GameObject gm=UnityEngine.Resources.Load<GameObject>("ButtonBuilding");

			foreach (var item in playerResearch.availableBuild){
				item.getIcn(); 
				/*GameObject button=GameObject.Instantiate(gm,Vector3.zero, Quaternion.identity);
				button.name=("ButtonBuilding"+(++i));

				button.transform.SetParent(GameObject.Find("Buildings").transform);
				button.GetComponent<RectTransform>().anchoredPosition3D=Vector3.zero;
				//button.transform.positionVector3.zero;
				button.GetComponent<Button>().image.sprite=item.getIcn(); 
				button.GetComponent<Button>().onClick.AddListener(()=>{
						item._gm_inst_now(); 
						cur_buildProto=item;
						GameEyes.AoClickEsq += (instProt);
						GameEyes.AoClickDir+= (cancProt);
						GameEyes.Waiting+=(updatePos);
					});			
					*/	
				
			}
		}
	 
		public void updatePos(){
			RaycastHit hit;
			Ray ray = pCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				cur_buildProto.obj_Preview(hit.point);
				
			}
		}
		void cancProt(){
			GameEyes.Waiting -=(updatePos); 
			GameEyes.AoClickEsq -= (instProt);
			GameEyes.AoClickDir -=(cancProt);
			cur_buildProto._gm_dest_now(); 
			
		}		
		void instProt(){
			GameEyes.Waiting -= (updatePos);
			GameEyes.AoClickEsq -= (instProt);
			GameEyes.AoClickDir -= (cancProt);
			cur_buildProto._gm_orig_jit(cur_buildProto.getObj_Prtp().transform);
			//GameObject t=GameObject.Instantiate<GameObject>(Capital._Capital_Prototype._getObj_Prtp(), Capital._Capital_Prototype._getObj_Prtp().transform.position,  Capital._Capital_Prototype._getObj_Prtp().transform.rotation);
			//t.GetComponent<MeshRenderer>().material=UnityEngine.Resources.Load<Material>("WoodHouse");
			cur_buildProto._gm_dest_now(); 
			
			
			
		}
		
		void checkInteractable(){
			RaycastHit hit;
			Ray ray = pCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				cur_buildProto.obj_Preview(hit.point);
				
			}
		}
		
		public _Resources getPlayerResources(){
			return this.playerResources;
		}
		
	}

}

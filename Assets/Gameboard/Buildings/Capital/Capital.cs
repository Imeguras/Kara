using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using kp=Kara.Playables; 
using UnityEngine;

namespace Kara.Entities{
	public class Capital: MonoBehaviour{
		_Capital inn_object;
		
		void Start(){
			inn_object= new _Capital("Capital", 2000, 100, 500); 
			gameObject.AddComponent<MeshFilter>().mesh= Resources.Load<Mesh>("WoodHouse");
			gameObject.AddComponent<MeshRenderer>().material=Resources.Load<Material>("WoodHouse");
			gameObject.AddComponent<BoxCollider>();
			
		}
		public class _Capital : Building_Abstract{
			
			public _Capital(string name, uint baseHP, uint morale_receive, uint morale_transmit):base(name, baseHP, morale_receive, morale_transmit){
				building_Icon=UnityEngine.Resources.Load<Texture2D>("Icn_Capital"); 
			}	
		}
		public class _Capital_Prototype : kp._Prototype<_Capital>{
			
			private static GameObject _cpt_prtp;
			// TODO: Better integration between Prototypes and non behaviour typed generic scripts
			// BODY: I've got no idea how "Generics(java)", in this case its called covariant's work on C# 
			public Sprite getIcn(){
				return UnityEngine.Resources.Load<Sprite>("Icn_Capital"); 
			}
			public GameObject getObj_Prtp(){
				return _getObj_Prtp();
			}
			public static GameObject _getObj_Prtp(){
				return _cpt_prtp;
			}

			/*
			* @Summary Basically true instantiates the game object, false just creates a fake intantiation of the whole object
			*/
			public _Capital_Prototype(bool inst){
				if(inst){
					_gm_inst_now(); 
				}
			}
			public void obj_Preview(Vector3 pos){
				_cpt_prtp.transform.position= pos; 
			}
			public void _gm_inst_now(){
				if (_cpt_prtp==null){
					_cpt_prtp=Instantiate(Resources.Load<GameObject>("CapitalPrototype"), Vector3.zero, Quaternion.identity);
				}
			}
			public void _gm_dest_now(){
				DestroyImmediate(_cpt_prtp.gameObject);
			}

			public void _gm_orig_jit(Transform space){
				GameObject NewCapitalJIT = new GameObject();
				NewCapitalJIT.AddComponent<Capital>();
				NewCapitalJIT.transform.position=space.position;
				NewCapitalJIT.transform.rotation=space.rotation;

			}
		}
		
		
	}
}
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
			inn_object= new _Capital(2000, 100, 500); 
			gameObject.AddComponent<MeshFilter>().mesh= Resources.Load<Mesh>("WoodHouse");
			gameObject.AddComponent<MeshRenderer>().material=Resources.Load<Material>("WoodHouse");
			gameObject.AddComponent<BoxCollider>();
			
		}
		public class _Capital :Building_Abstract{
			public _Capital(uint baseHP, uint morale_receive, uint morale_transmit):base(baseHP, morale_receive, morale_transmit){
			
			}	
		}
		public class _Capital_Prototype : kp._Prototype{
			private static GameObject _cpt_prtp=Resources.Load<GameObject>("CapitalPrototype");
			public GameObject getObj_Prtp(){
				return _getObj_Prtp();
			}
			public static GameObject _getObj_Prtp(){
				return _cpt_prtp;
			}
			public void _Capital_Preview(Vector3 pos){
				Instantiate(_cpt_prtp, pos, Quaternion.identity);
				//_cpt_prtp.gameObjec
				//capital.AddComponent<MeshFilter>().mesh= Resources.Load<Mesh>("WoodHouse");
				//capital.AddComponent<MeshRenderer>().material=Resources.Load<Material>("PrototypeBuild");
			}
		}
		
		
	}
}
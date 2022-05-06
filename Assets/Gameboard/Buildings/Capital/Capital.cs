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
			private static GameObject _cpt_prtp;
			public GameObject getObj_Prtp(){
				return _getObj_Prtp();
			}
			public static GameObject _getObj_Prtp(){
				return _cpt_prtp;
			}
			public _Capital_Prototype(){
				_cpt_prtp=Instantiate(Resources.Load<GameObject>("CapitalPrototype"), Vector3.zero, Quaternion.identity);
			}
			public static void _Capital_Prototype_Destroy(){
				Destroy(_cpt_prtp.gameObject);
			}
			public void capital_Preview(Vector3 pos){
				_capital_Preview(pos);
			}
			public static void _capital_Preview(Vector3 pos){
				_cpt_prtp.transform.position= pos; 
			}
		}
		
		
	}
}
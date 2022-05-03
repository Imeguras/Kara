using System.IO;
using System;
using UnityEngine;
namespace Kara.Entities{
    public class Citizen : MonoBehaviour{
		private _Citizen innerBehaviour;
		private Mesh cMesh; 
        
		void Start(){
			
			cMesh=Resources.Load<Mesh>("Citizen");
			this.gameObject.transform.Rotate(new Vector3(-90,0,0));
			
			innerBehaviour=new _Citizen(50,20f);
			this.gameObject.AddComponent<MeshFilter>();
			this.gameObject.AddComponent<MeshRenderer>();
			this.gameObject.AddComponent<BoxCollider>();
			
			this.GetComponent<MeshFilter>().mesh= cMesh;
			this.GetComponent<MeshRenderer>().material=Resources.Load<Material>("citizen_material");
			
		}
		void Update(){

		}

		public class _Citizen: Unit {
			public _Citizen(uint baseHP, float baseSpeed): base(baseHP, baseSpeed){

			}
		}	
    }

}

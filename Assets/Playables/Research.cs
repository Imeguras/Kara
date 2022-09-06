using System;
using System.Collections.Generic;
using Kara.Entities;
using UnityEngine;
using Mirror;
namespace Kara.Playables
{
    public class Research{
		public List<_Prototype<Building_Abstract>> availableBuild;

		public Research(bool start=true){
			availableBuild= new List<_Prototype<Building_Abstract>>();
			if(start){
				availableBuild.Add(new Capital._Capital_Prototype(false));
			
			}
			//availableBuild.Add(Capital._Capital_Prototype); 
		}
		
		public _Prototype<Building_Abstract> getType(_BuildingType type){
			switch(type){
				case _BuildingType.CAPITAL:
					
					return new Capital._Capital_Prototype(false);
				case _BuildingType.ERR_0:
				default:
					Debug.LogError("Research.getType: type not found");
					return null;
			}
		}
		public _BuildingType getEnum(_Prototype<Building_Abstract> type){
			switch(type){
				case Capital._Capital_Prototype:
					return _BuildingType.CAPITAL;
				default:
					Debug.LogError("Research.getEnum: type not found");
					return _BuildingType.ERR_0;
			}
			
		}

	
    }
	public static class CustomReadWriteFunctions{

		public static void WriteResearch(this NetworkWriter writer, Research value){
			int len= value.availableBuild.Count; 
			_BuildingType[] types=new _BuildingType[len];
		
			for (int i = 0; i < len; i++){
				types[i]=value.getEnum(value.availableBuild[i]);
			}
			
			writer.WriteArray<_BuildingType>(types);
		
			
		}

		public static Research ReadResearch(this NetworkReader reader){
			Research r= new Research(false);
			foreach(_BuildingType b in reader.ReadArray<_BuildingType>()){
				r.availableBuild.Add(r.getType(b));
			}
			
			return r;
		}
		public static void WriteBuildingType(this NetworkWriter writer, _BuildingType value){
		
			writer.WriteInt((int)value);
			
		}
		public static _BuildingType ReadBuildingType(this NetworkReader reader){
			_BuildingType k=_BuildingType.ERR_0;
			k = (_BuildingType)reader.ReadInt();
			
			return k; 
			
		}
	}
	public interface _Prototype<out R>{
		GameObject getObj_Prtp();
		void obj_Preview(Vector3 pos);
		Sprite getIcn();
		void _gm_inst_now(); 
		void _gm_dest_now();
		void _gm_orig_jit(Transform space); 
	}

	public enum _BuildingType{
		ERR_0=0, 
		CAPITAL=1
	}
}

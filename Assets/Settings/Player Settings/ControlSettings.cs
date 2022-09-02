using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kara_.Assets.Settings.Player_Settings
{
    public class ControlSettings: ScriptableObject{
		
		//public ControlSettingsSerializable serializableSettings;
		private float _MousePanSensitivity;
		private float _BoxPanBorder;
		

		public static readonly string advSettings=Application.dataPath+"/Settings/Player Settings/AdvSettings.json"; 
		public static readonly string keySettings=Application.dataPath+"/Settings/Player Settings/KeyBinds.json";
		public void Init(){
			Debug.Log("Init");
			refreshValues();
		}
		//TODO: Add a function to check if all necessary keys exist
		public bool checkIntegrity(){
			//Check if file AdvSettings.json exists
			if(!System.IO.File.Exists(advSettings)||!System.IO.File.Exists(advSettings)){
				return false;
			}
			return true;
		}
		public void refreshValues(){
			Debug.Log(advSettings);

			if(File.Exists(advSettings)){
				var json=File.ReadAllText(advSettings);
				var settings = ControlSettingsSerializable.FromJson(json);
				this._MousePanSensitivity=settings.__MousePanSensitivity;
				this._BoxPanBorder=settings.__BoxPanBorder;
			}

		}
		//getter and setter for scrollWheelSpeed
		public float mousePanSensitivity{
			get{
				return _MousePanSensitivity;
			}
			set{
				_MousePanSensitivity=Mathf.Clamp(value, 20f, 100f);
			}
		}
		public float boxPanBorder{
			get{
				return _BoxPanBorder;	
			}
			set{
				_BoxPanBorder=Mathf.Clamp(value, 20f, 200f);
			}
		}
		[System.Serializable]
		public class ControlSettingsSerializable{
			public ControlSettingsSerializable(){
				
			}
			public float __MousePanSensitivity;
			public float __BoxPanBorder;
			public static ControlSettingsSerializable FromJson(string json){
				return JsonUtility.FromJson<ControlSettingsSerializable>(json); 

			}
			
		}
    }
}


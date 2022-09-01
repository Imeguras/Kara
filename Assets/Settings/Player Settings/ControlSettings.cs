using UnityEngine; 
namespace Kara_.Assets.Settings.Player_Settings
{
    public class ControlSettings: ScriptableObject{
		
		public static readonly string advSettings=Application.dataPath+"/Assets/Settings/Player Settings/AdvSettings.json"; 
		public static readonly string keySettings=Application.dataPath+"/Assets/Settings/Player Settings/KeyBinds.json";
		public void Init(){
			//if(checkIntegrity()){
			//	refreshValues();
			//}
			 
		}
		public bool checkIntegrity(){
			//Check if file AdvSettings.json exists
			if(!System.IO.File.Exists(advSettings)||!System.IO.File.Exists(advSettings)){
				return false;
			}
			return true;
		}
		public void refreshValues(){
			
		}
		//getter and setter for scrollWheelSpeed
		public float scrollWheelSpeed{
			get{
				return scrollWheelSpeed;
			}
			set{
				scrollWheelSpeed=value;
				Mathf.Clamp(scrollWheelSpeed, 20f, 100f);
			}
		}
		public float boxPanBorder{
			get{
				return boxPanBorder;	
			}
			set{
				boxPanBorder=value;
				Mathf.Clamp(boxPanBorder, 20f, 200f);
			}
		}
		public float sensitivityMousePan{
			get{
				return sensitivityMousePan;
			}
			set{
				sensitivityMousePan=value;
				Mathf.Clamp(sensitivityMousePan, 0.1f, 1f);
			}
		}
    }
}


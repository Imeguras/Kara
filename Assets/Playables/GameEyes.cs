using System;
using Kara.Playables;
using Kara.Entities;  
using UnityEngine;
using UnityEngine.InputSystem;
using Kara_.Assets.Settings.Player_Settings;
using Mirror;

namespace Kara.Playables{
    public class GameEyes : NetworkBehaviour{
		//TODO THIS SHOULD BE SPECTATOR
		public Player target; 
		public PlayerControls controls;
		protected ControlSettings controlSettings;
		
		private Vector3 desiredPosition;
		
		private Vector3 mins; 
		private Vector3 maxs; 
		private float speed;
		
		
		private void OnEnable(){
			controls.Enable();
		}
		
		void Awake(){
			// controls is equal load Assets/Settings/PlayerControls
			controls= new PlayerControls();
			controls.TERRA_INT.Pan.performed += vec => pan(vec.ReadValue<Vector2>());
			controls.TERRA_INT.PanMouse.performed += vec => {
				Vector2 mouse = vec.ReadValue<Vector2>();
					//Normalize the mouse input based on screen size
					mouse.x = (2*mouse.x/Screen.width)-1;
					mouse.y = (2*mouse.y/Screen.height)-1;
					
					if(Math.Abs(mouse.x)>=0.8f||Math.Abs(mouse.y)>=0.6f){
						pan(mouse);
					}
			};
			

		}
		
		public override void OnStartClient() {
			mins = new Vector3(0f, 215f, 4f);
			maxs = new Vector3(500f, 290f, 504f);
			speed =  controlSettings.mousePanSensitivity * Time.deltaTime;
			desiredPosition = transform.position;
			
		}
		public void bindIdentity(Player spec){
			target=spec;
		}
		
		public void bindSettings(ControlSettings settings){
			controlSettings=settings;
		}
		
		void pan(Vector2 value){
			//desired position is the current position plus the value of the mouse movement
			desiredPosition+= new Vector3(value.x*speed, 0, value.y*speed);
			desiredPosition.x=Math.Clamp(desiredPosition.x, mins.x, maxs.x);
			desiredPosition.z=Math.Clamp(desiredPosition.z, mins.z, maxs.z);
		}
		
		private void FixedUpdate(){
			transform.position = Vector3.Lerp(transform.position,desiredPosition,0.2f);
		}
		
		private void OnDisable(){
			controls.Disable();
		}
    }
}

using System.Collections;
using System.Collections.Generic;
using Kara.Playables; 
using UnityEngine;

namespace Kara.Playables{
    public class GameEyes : MonoBehaviour{
		public delegate void OnClick();
    	public static event OnClick AoClickEsq;
    	public static event OnClick AoClickDir;
		public static event OnClick Waiting;
		private Vector3 desiredPosition;

		public float boxPanBorder = 30;
		public float scrollSpeed = 200;
		public float treedrise = 200;
		
		private Vector3 mins; 
		private Vector3 maxs; 
		void Start () {
			mins = new Vector3(0f, 15f, 4f);
			maxs = new Vector3(500f, 90f, 504f);
			desiredPosition = transform.position;
			
		}
		
		
		void Update () {
			if(Waiting!= null){
				Waiting();

			}
			if (Input.GetMouseButtonDown(0) && AoClickEsq != null ){
           		AoClickEsq();
        	}
        	if (Input.GetMouseButtonDown(1) && AoClickDir != null){
            	AoClickDir();
        	}
			float x = 0, y = 0, z = 0;
			float speed = scrollSpeed * Time.deltaTime;

			if (Input.mousePosition.x < boxPanBorder)
				x -= speed;
			else if (Input.mousePosition.x > Screen.width - boxPanBorder)
				x += speed;
				
			if (Input.mousePosition.y < boxPanBorder)
				z -= speed;
			else if (Input.mousePosition.y > Screen.height - boxPanBorder)
				z += speed;
			y += Input.GetAxis("Mouse ScrollWheel") * treedrise;
			Vector3 move = new Vector3(x,y,z) + transform.position;
			move.x = Mathf.Clamp (move.x, mins.x, maxs.x);
			//move.y = Mathf.Clamp (move.y, mins.y, maxs.y)+(int)terrainTrack.GetHeight((int)x,(int)z);
			move.y = Mathf.Clamp (move.y, mins.y, maxs.y);
			move.z = Mathf.Clamp (move.z, mins.z, maxs.z);
			
			desiredPosition = move;
			transform.position = Vector3.Lerp(transform.position,desiredPosition,0.2f);
		}
    }
}

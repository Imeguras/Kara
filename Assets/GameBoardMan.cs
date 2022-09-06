using System.Collections;
using System.Collections.Generic;
using Kara.Playables; 
using Kara.Entities;
using Kara.ProceduralGen;
using UnityEngine;
using Mirror; 
using Kara_.Assets.Settings.General_Settings;
using Kara_.Assets.Settings.Player_Settings;

public class GameBoardMan : NetworkManager{
    

	protected GameSettings gameSettings;
	protected GameObject gameBoard;
	public static List<Player> players;
	
	
	public override void OnStartServer(){
			
			gameSettings=GameSettings.CreateInstance<GameSettings>();
			gameSettings.Init(); 
			
			players = new List<Player>();

			var res=Resources.Load("GameBoard") as GameObject;
			gameBoard=Instantiate(res);
			
		
	}
	

	public Vector3 getAvailableSpawnPoint(){
		Debug.LogWarning("TODO-LACKS IMPLEMENTATION!!!!"); 
		return new Vector3 (100, 50, 100);
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn){
		base.OnServerAddPlayer(conn);
		
		GameObject tempRefCamera=conn.identity.gameObject;
		conn.identity.hideFlags=HideFlags.HideInHierarchy;
		
		ControlSettings controlSettings=ControlSettings.CreateInstance<ControlSettings>();
		controlSettings.Init();

		Player me = new Player("Imeguras", conn.connectionId, new Color32(0,0,255,255), controlSettings, tempRefCamera);
		me.gameEyes.bindSettings(controlSettings);
		
		//tempRefCamera=me.resetCamera(); 
		
		//me.getIdentity().AssignClientAuthority(conn);
		//me.getIdentity().observers.Add(conn.connectionId, conn);
		//var CameraNetworkTransform= tempRefCamera.AddComponent<NetworkTransform>();
		
		var resource_tab = tempRefCamera.AddComponent<Resources_Tab>();
		resource_tab.setTrackingPlayer(me);
		//NetworkServer.Respawn(me.getIdentity());
		//Spawn tempRefCAmera
		
		tempRefCamera.transform.position=getAvailableSpawnPoint(); 
		
		players.Add(me); 
	

		tempRefCamera.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
		//gameBoard.GetComponent<GameBoard>().setVisibilityTo(conn); 
		//tempRefCamera.GetComponent<CloningBuildsUpdate>().test(conn);
		//CloningBuildsUpdate.Instance.TargetUpdateAvailableBuilds(conn, me.getPlayerResearch());
		
	}
	
	public override void OnServerDisconnect(NetworkConnectionToClient conn){
	
		base.OnServerDisconnect(conn);
		//destroy all objects owned by conn
		foreach(var obj in conn.clientOwnedObjects){
			NetworkServer.Destroy(obj.gameObject);
		}
		//destroy blank
		players.RemoveAll(x => x.getIdentity() == conn.identity);
		
	}

    
}

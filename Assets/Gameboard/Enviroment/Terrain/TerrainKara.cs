using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Kara.ProceduralGen{
	#if UNITY_EDITOR
		using UnityEditor;
		[CustomEditor(typeof(TerrainKara))]
		public class TerrainKaraEdit : Editor{
			private TerrainKara script;
			private void OnEnable()
			{
				// Method 1
				script = (TerrainKara) target;
				// Method 2
				script = target as TerrainKara;
			}
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();
				if (GUILayout.Button("Draw Terrain")){
					//script.LoadTerrain();
				}
				if (GUILayout.Button("Apply Water"))
				{
					//script.ApplyWater();
				}
			}
		}

	#endif
	public class TerrainKara : NetworkBehaviour{
		// Start is called before the first frame update
		_Seas seas_obj;
		public override void OnStartServer() {
			base.OnStartServer();
			seas_obj = new _Seas(50);
		}
				

	}
	public class _Seas{
			//public List<GameObject> Seas;
			private uint tile_size; 

			public float seaLevel;
			
			public _Seas(uint tile_size, float seaLevel=5.0f){
				this.tile_size=tile_size;
				this.seaLevel=seaLevel;
				//Seas= new List<GameObject>();
				

			}
			//public Mesh tileOcean( uint sizeX, uint sizeZ){
				
			public Mesh tileOcean(uint sizeX, uint sizeZ, Mesh _temp, Vector3[] _vertices, Vector2[] uvs){
				uint toInstX=(uint)(sizeX/tile_size);
				uint toInstZ=(uint)(sizeZ/tile_size);
				
				
				//_vertices = new Vector3[vertex_cnt];
				int[] triangles= new int[ (int)(sizeX-1)*(int)(sizeZ/2)*3];
				//uvs = new Vector2[vertex_cnt];
				int i=0;
				for (int y = 0; y < (int)sizeZ/2; y++){
					for (int x = 0; x < (int)sizeX/2; x++){
						Vector3 vex_Tmp=Vector3.zero;
						vex_Tmp.x=x*2+((y+1)%2); 
						vex_Tmp.y=1; 
						vex_Tmp.z=(y*2);
						_vertices[i]=vex_Tmp;
						i++; 
					}
					
				}
				
				for (i = 0; i < uvs.Length; i++)
				{
					uvs[i] = new Vector2(_vertices[i].x, _vertices[i].z);
				}
				
				int newline;
				for (i = 0; i < (((sizeX/2)-1)*(sizeZ/2))-1; i++){
						newline=(int)sizeX/2;
						triangles[i*6]=i;
						triangles[i*6+1]=newline+i;
						triangles[i*6+2]=newline+(i+1);
						triangles[i*6+3]=i;
						triangles[i*6+4]=newline+(i+1); 
						triangles[i*6+5]=(i+1); 
						
					//Debug.Log("I:  "+triangles[i*6]+" I2: "+triangles[i*6+1]+" I3: "+triangles [i*6+2]+"I4:  "+triangles[i*6+3]+" I5: "+triangles[i*6+4]+" I6: "+triangles [i*6+5]);
				}
				
				//new int[12]{0,250,251,0,1,251,1,251,252,1,2,252}
				_temp.vertices=_vertices;
				_temp.uv = uvs;
				_temp.triangles=triangles;
				
				return _temp;
			}
			
		}
}
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

using UnityEngine; 

namespace Kara.ProceduralGen
{
    public class OceanGeneral : MonoBehaviour{
		private _Seas seas_obj;
		public uint tile_size=50; 
		
		public float [,] heightmap;
        void Start(){
			seas_obj = new _Seas(tile_size); 
		}
		public void OceanGeneral_tileOcean(_par_OceanGeneral paramsOcGn){
			this.heightmap=paramsOcGn.heightMap; 
			//this.seas_obj.sizeX=paramsOcGn.sizeX;
			//this.seas_obj.sizeZ=paramsOcGn.sizeZ;
			uint _sizeX=paramsOcGn.sizeX;
			uint _sizeZ=paramsOcGn.sizeZ;
			Mesh _temp=new Mesh();
			int vertex_cnt = (int)(_sizeZ/2*_sizeX/2); 
			Vector3[] _vertices = new Vector3[vertex_cnt];
			Vector2[] _uvs = new Vector2[vertex_cnt];
			Mesh _block= this.seas_obj.tileOcean(_sizeX, _sizeZ, _temp, _vertices, _uvs);
			//Mesh blocks=this.seas_obj.tileOcean();
			this.GetComponent<MeshFilter>().mesh=_block;	
			this.GetComponent<MeshRenderer>().material=Resources.Load<Material>("WaterMaterial");
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
	public class _par_OceanGeneral{
		public float[,] heightMap;
		public uint sizeX; 
		public uint sizeZ;
		public _par_OceanGeneral(float[,] heightMap, uint sizeX, uint sizeZ){
			this.heightMap=heightMap;
			this.sizeX=sizeX;
			this.sizeZ=sizeZ;
		}
	}
}

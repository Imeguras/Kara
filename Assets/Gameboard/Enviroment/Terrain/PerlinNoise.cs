using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System;
using UnityEngine; 
using TestSimpleRNG; 

namespace Kara.ProceduralGen{
	public class _PerlinNoise{
		public uint octaves;
		public float lacunarity; 
		public double[,] map; 
		public double[,] rnd1; 
		public double[,] rnd2; 
		private int width;
		private int lenght; 

		double maxHeight;
		System.Random rnd; 
		public _PerlinNoise(int width=500, int lenght=500, double maxHeight=1000, uint octaves=20, float lacunarity=0.1f ){
			rnd= new System.Random();
			Math.Clamp(octaves, 2, 255); 
			Math.Clamp(width, 32, int.MaxValue);
			Math.Clamp(lenght, 32, int.MaxValue);
			this.width=width;
			this.lenght=lenght;
			map=new Double[lenght,width];
			rnd1=new Double[lenght,width];
			rnd2=new Double[lenght,width];
			this.octaves=octaves;
			this.lacunarity= lacunarity;
			this.maxHeight=maxHeight;
			
		}
		public Task<double[,]>  setPerlinData(){
			GenNoise(ref rnd1, ref rnd2);
		
			for(int u=1; u<octaves; u++){ 
				GenPerlin(u);
			}
			return Task.FromResult<double[,]>(map);
			
		}
		/*public Task<float[,]> retHeightsMap(float min, float max){

			float[,] hmap= new float[lenght, width];
			for (int y = 0; y < this.lenght; y++){	
				for (int x = 0; x < this.width; x++){
					
						hmap[y,x]=Math.Abs( ( ((float)map[y,x])-min ) / (max-min) );
					
				}
			}
			return Task.FromResult<float[,]>(hmap); 
		}*/
		public void GenNoise(ref double[,] vertex3d1, ref double[,] vertex3d2, int width_=0, int lenght_=0){
			
			int _width = this.width; 
			int _lenght = this.lenght;
			if(width_!=0&&lenght_!=0){
				_width=width_;
				_lenght=lenght_;
			}
			
			for (int y = 0; y < _lenght; y++){
				for (int x = 0; x < _width; x++){
					double rdr1=SimpleRNG.GetNormal(0.5f, 0.2f);
					double rdr2=SimpleRNG.GetNormal(0.5f, 0.2f);
					
					vertex3d1[y,x]= rdr1;
					vertex3d2[y,x]= rdr2;
				}
			}
		}
		
		public void GenPerlin(int u=0,int width_=0, int lenght_=0){
			int _width = this.width; 
			int _lenght = this.lenght;
			if(width_!=0&&lenght_!=0){
				_width=width_;
				_lenght=lenght_;
			}
			Double value=0;
				for(int y=0; y<_lenght-1; y++){
					for(int x=0; x<_width-1; x++){
						value=perlin(x*lacunarity/u,y*lacunarity/u);
						///u	
						map[y,x]+= value/octaves;
					}
					
					
			
				}
			
			
		}
		public int getWidth(){
			return this.width;
		}
		public int getLenght(){
			return this.lenght;
		}
		public uint getOctaves(){
			return this.octaves;
		}
		// Compute Perlin noise at coordinates x, y
		double perlin(double x, double y) {

			// Determine grid cell coordinates
			int x0 = (int)x;
			int x1 = x0 + 1;
			int y0 = (int)y;
			int y1 = y0 + 1;

			// Determine interpolation weights
			// Could also use higher order polynomial/s-curve here
			double sx = x - (double)x0;
			double sy = y - (double)y0;

			// Interpolate between grid point gradients
			double n0, n1, ix0, ix1, value;

			n0 = dotGridGradient(x0, y0, x, y, rnd1, rnd2);
			n1 = dotGridGradient(x1, y0, x, y, rnd1, rnd2);
			ix0 = lerp(n0, n1, sx);

			n0 = dotGridGradient(x0, y1, x, y, rnd1, rnd2);
			n1 = dotGridGradient(x1, y1, x, y, rnd1, rnd2);
			ix1 = lerp(n0, n1, sx);
			
			value = lerp(ix0, ix1, sy);
			return value;
		}
		double lerp(double a0, double a1, double w) {
			return (1.0f - w)*a0 + w*a1;
		}
		
		// Computes the dot product of the distance and mapa vectors.
		double dotGridGradient(int ix, int iy, double x, double y, double[,] grd1, double[,] grd2) {
			
			// Precomputed (or otherwise) mapa vectors at each grid node
			

			// Compute the distance vector
			double dx = x - (double)ix;
			double dy = y - (double)iy;

			// Compute the dot-product
			return (dx*grd1[iy,ix] + dy*grd2[iy,ix]);
		}
		
	}

    
}

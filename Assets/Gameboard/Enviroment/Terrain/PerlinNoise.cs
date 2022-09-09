using System.Threading.Tasks;
using System;
using UnityEngine; 
using TestSimpleRNG;
using Kara.MathS; 
namespace Kara.ProceduralGen{
	public class _PerlinNoise{
		public uint octaves;
		public float lacunarity; 
		public double[,] map; 
		public double[,] rnd1; 
		public double[,] rnd2; 
		private int width;
		private int length; 
		float persistance; 
		double maxHeight;
		System.Random rnd; 
		public _PerlinNoise(int width=500, int length=500, double maxHeight=1000, uint octaves=20, float lacunarity=0.8f, float persistance=0.5f ){
			rnd= new System.Random();
			Math.Clamp(octaves, 2, 255); 
			Math.Clamp(width, 32, int.MaxValue);
			Math.Clamp(length, 32, int.MaxValue);
			this.width=width;
			this.length=length;
			map=new Double[length,width];
			rnd1=new Double[length,width];
			rnd2=new Double[length,width];
			this.octaves=octaves;
			this.lacunarity= lacunarity;
			this.maxHeight=maxHeight;
			this.persistance=persistance;
		}
		//TODO: Optimize
		public Task<double[,]>  setPerlinData(){
			GenNoise(ref rnd1, ref rnd2);
			float k = octaves; 
			for(int u=1; u<octaves; u++){
				GenPerlin(ref k, u );
				k *= persistance;  
			}
			Strech();
			return Task.FromResult<double[,]>(map);
			
		}
		public void Strech(){
			for(int i=0; i<length; i++){
				for(int j=0; j<width; j++){
					map[i,j]+=0.5;
				}
			}
		}
		public void GenNoise(ref double[,] vertex3d1, ref double[,] vertex3d2, int width_=0, int length_=0){
			
			int _width = this.width; 
			int _length = this.length;
			if(width_!=0&&length_!=0){
				_width=width_;
				_length=length_;
			}
			SimpleRNG.SetSeedFromSystemTime();
			for (int y = 0; y < _length; y++){
				for (int x = 0; x < _width; x++){
					double rdr1=SimpleRNG.GetNormal(0.5f, 0.2f);
					double rdr2=rnd.NextDouble();
					
					vertex3d1[y,x]= rdr1;
					vertex3d2[y,x]= rdr2;
				}
			}
		}
		

		
		public void GenPerlin(ref float weight, int currentOctave=1, int width_=0, int length_=0 ){
			int _width = this.width; 
			int _length = this.length;
			//float weight= 1f;
			float _lacunarity=lacunarity/(2f*currentOctave);
			if(width_!=0&&length_!=0){
				_width=width_;
				_length=length_;
			}
			double random=0; 
			for (int y = 0; y < _length; y++){
				for (int x = 0; x < _width; x++){
					
					random=perlin(x*_lacunarity,y*_lacunarity);
					map[y,x]+=Math.Clamp(((random)*2/octaves), 0f, 1f); 

				}
			}
		}

		public int getWidth(){
			return this.width;
		}
		public int getLength(){
			return this.length;
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
			ix0 = KaraMath.INSTANCE.lerp(n0, n1, sx);

			n0 = dotGridGradient(x0, y1, x, y, rnd1, rnd2);
			n1 = dotGridGradient(x1, y1, x, y, rnd1, rnd2);
			ix1 = KaraMath.INSTANCE.lerp(n0, n1, sx);
			
			value = KaraMath.INSTANCE.lerp(ix0, ix1, sy);
			return value;
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

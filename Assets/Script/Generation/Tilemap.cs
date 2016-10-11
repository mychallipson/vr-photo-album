﻿using UnityEngine;

public class Tilemap : MonoBehaviour {

	const float TileLength=2;
	int rows,columns;	

	Tile [][] tileGrid;
	StreakPlacement [] streakPlacements;

	// Use this for initialization
	void Start () {
		ReadTileMapFile("Assets/Script/Generation/tilemap.csv");//relative to the project
		BuildFromTilemap();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void BuildFromTilemap(){
		string containerPath="Tilemap/";//we might also have several themes which can be put in seperate folders

		float startX=0;
		float startY=0;
		//insert the art gallery entry (which contains the player)
		//the art gallery is a 3 X 4 tile prefab which should be placed before the
		//generated art gallery
		// string entryPath=containerPath+"ArtGalleryEntry";
		// GameObject entryObject=Instantiate(Resources.Load(entryPath,typeof(GameObject))) as GameObject;
		// entryObject.transform.position=new Vector3(startX-1.5f*TileLength,startY-4*TileLength,0);
		// entryObject.transform.Rotate(0,0,0,Space.Self);

		for(int i=0;i<rows;i++){
			for(int j=0;j<columns;j++){
				Tile tile=tileGrid[i][j];
				//load Floor prefab
				string floorPath=containerPath+"Floor/"+Floor.StringFor(tile.floor.type);
				GameObject floorObject=Instantiate(Resources.Load(floorPath,typeof(GameObject))) as GameObject;
				floorObject.transform.position=new Vector3(startX+j*TileLength,0,startY+i*TileLength);
				floorObject.transform.Rotate(0,AngleFor(tile.floor.direction),0,Space.Self);

				//load Ceiling prefab
				string ceilingPath=containerPath+"Ceiling/"+Ceiling.StringFor(tile.ceiling.type);
				GameObject ceilingObject=Instantiate(Resources.Load(ceilingPath,typeof(GameObject))) as GameObject;
				ceilingObject.transform.position=new Vector3(startX+i*TileLength,0,startY+j*TileLength);
				ceilingObject.transform.Rotate(0,AngleFor(tile.floor.direction),0);
				
			}
		}		
	}

	private void ReadTileMapFile(string path){
		//read the contents of a CSV file
		string csv=System.IO.File.ReadAllText(path);//must be in the same folder

		//split each new line
		string [] lines=csv.Split("\n"[0]);

		//first line contains the size of the grid
		string []dimensions=lines[0].Split(',');
		rows=int.Parse(dimensions[0]);
		columns=int.Parse(dimensions[1]);

		tileGrid=new Tile[rows][];

		//instantiate the row of tiles first
		for(int i=0;i<rows;i++){
			tileGrid[i]=new Tile[columns];	
		}


		TileLayer currentTileLayer=TileLayer.FloorMap;//default
		int currentMapRow=0;

		for (int i=1;i<lines.Length;i++){
						
			string []tiles=lines[i].Split(',');

			//beginning of each grid contains the type: floormap, ceilingmap, propmap
			if(string.Equals(tiles[0],"FloorMap")){
				currentTileLayer=TileLayer.FloorMap;
				currentMapRow=0;
			}else if(string.Equals(tiles[0],"PropMap")){
				currentTileLayer=TileLayer.PropMap;
				currentMapRow=0;
			}else if(string.Equals(tiles[0],"CeilingMap")){
				currentTileLayer=TileLayer.CeilingMap;
				currentMapRow=0;
			}else{
				//this means its a continuation of the currentTileLayer
				for(int j=0;j<columns;j++){

					if(tileGrid[currentMapRow][j]==null){
						tileGrid[currentMapRow][j]=new Tile();
					}

					if(tileGrid[currentMapRow][j]==null){
						tileGrid[currentMapRow][j]=new Tile();
					}

					switch(currentTileLayer){
						case TileLayer.FloorMap:
							tileGrid[currentMapRow][j].floor=Floor.GetTile(tiles[j]);
							break;
						case TileLayer.CeilingMap:
							tileGrid[currentMapRow][j].ceiling=Ceiling.GetTile(tiles[j]);
							break;
						case TileLayer.PropMap:
							tileGrid[currentMapRow][j].prop=Prop.GetTile(tiles[j]);
							break;
							
					}
					
				}
				currentMapRow++;
			}
			
		}
	}

	public static Direction GetDirection(int angle){
		switch(angle){
			case 0:return Direction.North;
			case 90:return Direction.East;
			case 180:return Direction.South;
			case 270:return Direction.West;
			default:
				Debug.Log("Invalid angle for direction "+angle);
				return Direction.North;
		}
	}

	public static int AngleFor(Direction direction){
		switch(direction){
			case Direction.North:
				return 0;
			case Direction.East:
				return 90;
			case Direction.South:
				return 180;
			case Direction.West:
				return 270;
			default:
				return -1;//indicates unknown,although its a valid angle, this is mostly useful while debugging
		}
	} 
}

public enum Direction{
	North,
	East,
	South,
	West
}
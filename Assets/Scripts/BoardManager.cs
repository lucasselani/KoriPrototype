using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count{
		public int minimum;
		public int maximum;

		public Count(int min, int max){
			minimum = min;
			maximum = max;
		}			
	}

	public int columns = 64;
	public int rows = 64;
	public Count blockingCount = new Count(20,25);
	public Count objectCount = new Count(5,9); 
	public Count monsterCount = new Count(4,7);
	public GameObject[] wallTiles;
	public GameObject[] floorTiles;
	public GameObject[] blockingTiles;
	public GameObject[] monsterTiles;
	public GameObject[] objectTiles;

	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();

	void InitializeList(){
		gridPositions.Clear ();
		for (int x=1; x<columns-1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup(){
		boardHolder = new GameObject ("Board").transform;
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstanciate = floorTiles[Random.Range(0,floorTiles.Length)];
					if(x==-1 || y==-1 || x==columns || y==rows) toInstanciate = wallTiles[Random.Range(0,wallTiles.Length)];
				GameObject instace = Instantiate(toInstanciate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;
				instace.transform.SetParent(boardHolder);
			}
		}		
	}

	Vector3 RandomPosition(){
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 RandomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return RandomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max){
		int objectCount = Random.Range(min, max+1);
		for(int i=0; i<objectCount; i++){
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range(0,tileArray.Length)];
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	public void SetupScene(){
		BoardSetup(); //Seta walls de borda e tiles
		InitializeList(); //Inicializa grid com posições do meio
		LayoutObjectAtRandom(blockingTiles, blockingCount.minimum, blockingCount.maximum);
		LayoutObjectAtRandom(monsterTiles, monsterCount.minimum, monsterCount.maximum);
		LayoutObjectAtRandom(objectTiles, objectCount.minimum, objectCount.maximum);
	}
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

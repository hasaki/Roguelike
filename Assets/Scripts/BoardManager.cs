using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count(int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int Columns = 8;
	public int Rows = 8;
	
	public Count WallCount = new Count(5, 9);
	public Count FoodCount = new Count(1, 5);

	public GameObject Exit;
	public GameObject[] FloorTiles;
	public GameObject[] WallTiles;
	public GameObject[] EnemyTiles;
	public GameObject[] FoodTiles;
	public GameObject[] OuterWallTiles;

	private Transform _boardHolder;
	private readonly List<Vector3> _gridPositions = new List<Vector3>();

	public void SetupScene(int level)
	{
		_boardHolder = new GameObject("Board").transform;

		BoardSetup();
		InitializeList();
		LayoutObjectAtRandom(WallTiles, WallCount.minimum, WallCount.maximum);
		LayoutObjectAtRandom(FoodTiles, FoodCount.minimum, FoodCount.maximum);

		var enemyCount = (int) Mathf.Log(level, 2f);
		LayoutObjectAtRandom(EnemyTiles, enemyCount, enemyCount);

		var instance = Instantiate(Exit, new Vector3(Columns - 1, Rows - 1, 0f), Quaternion.identity) as GameObject;
		Debug.Assert(instance != null, "Could not instantiate exit");
		instance.transform.SetParent(_boardHolder);
	}

	private void BoardSetup()
	{
		for (int x = -1; x < Columns + 1; x++)
		{
			for (int y = -1; y < Rows + 1; y++)
			{
				var toInstantiate = FloorTiles[Random.Range(0, FloorTiles.Length)];
				if (x == -1 || x == Columns || y == -1 || y == Rows)
					toInstantiate = OuterWallTiles[Random.Range(0, OuterWallTiles.Length)];

				var instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				Debug.Assert(instance != null, "Could not instantiate object");
				instance.transform.SetParent(_boardHolder);
			}
		}
	}

	private void InitializeList()
	{
		_gridPositions.Clear();

		for (var x = 1; x < Columns - 1; x++)
		{
			for (var y = 1; y < Rows - 1; y++)
			{
				_gridPositions.Add(new Vector3(x, y, 0f));
			}
		}
	}

	private Vector3 RandomPosition()
	{
		var randomIndex = Random.Range(0, _gridPositions.Count);
		var randomPosition = _gridPositions[randomIndex];

		_gridPositions.RemoveAt(randomIndex);

		return randomPosition;
	}

	private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		var objCount = Random.Range(minimum, maximum + 1);

		for(var index = 0; index < objCount; index++)
		{
			var randomPosition = RandomPosition();
			var randomTile = tileArray[Random.Range(0, tileArray.Length)];

			var instance = Instantiate(randomTile, randomPosition, Quaternion.identity) as GameObject;
			Debug.Assert(instance != null, "Could not instantiate random object");
			instance.transform.SetParent(_boardHolder);
		}
	}
}

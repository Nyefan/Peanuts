using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	public Sprite sp_Empty; 
	public Sprite sp_Water;
	public Sprite sp_Grass;
	public Sprite sp_Desert;
	public Sprite sp_Plains;
	public Sprite sp_Rough;
	public Sprite sp_Lava;
	public Sprite sp_Snow;
	public Sprite sp_Marsh;

	public World World { get; protected set; }

	Dictionary<Tile, GameObject> map_Tile2GameObject;

	// Use this for initialization
	void Start () {

		// Verify that WorldController is unique
		if(Instance != null) { Debug.LogError ("WorldController has been instantiated more than once."); }
		Instance = this;

		World = new World();

		// Instantiate the currentLayer Dictionary
		map_Tile2GameObject = new Dictionary<Tile, GameObject> ();

		// Create a GameObject for each Tile
		InstantiateTiles ();

		// Apply the correct properties to each Tile
		World.InitializeTiles ();
		
	}
		
	// Update is called once per frame
	void Update () {
		
	}

	// Query Functions
	public Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt (coord.x+0.5f);
		int y = Mathf.FloorToInt (coord.y+0.5f);

		return WorldController.Instance.World.GetTileAt (x, y);
	}

	// Callback Functions

	// Changes the Tile's associated GameObject to render correctly
	void OnTileTypeChanged(Tile tile_data) {

		// Sanity Check
		if (map_Tile2GameObject.ContainsKey(tile_data) == false) {
			Debug.LogError ("WorldController.OnTileTypeChanged - This function has been called from a Tile that is not in map_Tile2GameObject.");
			return;
		}

		GameObject tile_go = map_Tile2GameObject [tile_data];

		if (tile_go == null) {
			Debug.LogError ("WorldController.OnTiletypeChanged - This function has been called from a Tile that has no associated GameObject.");
			return;
		}


		if (tile_data.Type == Tile.TileType.Grass) {
			tile_go.GetComponent<SpriteRenderer>().sprite = sp_Grass;
		} else if (tile_data.Type == Tile.TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer>().sprite = null;
		} else {
			Debug.LogError("WorldController.OnTiletypeChanged - Unrecognized tile type.");
		}
	}

	// Internal Functions

	void InstantiateTiles() {
		for (int w = 0; w < World.Width; w++) {
			for (int h = 0; h < World.Height; h++) {
				Tile tile_data = World.GetTileAt (w, h);

				GameObject tile_go = new GameObject ();
				tile_go.name = "Tile_" + w + "_" + h;
				tile_go.transform.position = new Vector3 ( tile_data.X, tile_data.Y );
				tile_go.transform.SetParent (this.transform, true);

				tile_go.AddComponent<SpriteRenderer> ();

				tile_data.RegisterCB_OnTileTypeChanged ( OnTileTypeChanged );

				map_Tile2GameObject.Add (tile_data, tile_go);
			}
		}
	}
}
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class WorldController : MonoBehaviour {

	/// <summary>
	/// Used to ensure that only one instance of WorldController has been created.
	/// </summary>
	public static WorldController Instance { get; protected set; }

	/// <summary>
	/// Gets or sets the world.
	/// </summary>
	/// <value>The world map.  This might be changed later to access a list of levels in the map.</value>
	public World World { get; protected set; }

	/// <summary>
	/// A map of Tiles in a world to their associated GameObjects used for rendering.
	/// </summary>
	Dictionary<Tile, GameObject> map_Tile2GameObject;
	/// <summary>
	/// The map of currently existing InstalledObjects to their associated GameObjects used for rendering.
	/// </summary>
	Dictionary<InstalledObject, GameObject> map_InstalledObject2GameObject;

	Dictionary<string, Sprite> map_SpriteName2Sprite;

	// Use this for initialization
	void Start () {

		// Verify that WorldController is unique
		if(Instance != null) { Debug.LogError ("WorldController has been instantiated more than once."); }
		Instance = this;

		// Make a new map
		World = new World();
		World.RegisterCB_OnInstalledObjectCreated (OnInstalledObjectCreated);
		World.RegisterCB_OnInstalledObjectDestroyed (OnInstalledObjectDestroyed);


		// Instantiate the currentLayer Dictionary
		map_Tile2GameObject = new Dictionary<Tile, GameObject> ();
		map_InstalledObject2GameObject = new Dictionary<InstalledObject, GameObject> ();
		map_SpriteName2Sprite = new Dictionary<string, Sprite> ();

		// Load all the sprites into the dictionary
		LoadSprites ();

		// Create a GameObject for each Tile
		InstantiateTiles ();

		// Apply the correct properties to each Tile
		World.InitializeTiles ();
		
	}
		
	// Update is called once per frame
	void Update () {
		
	}

	// Query Functions

	/// <summary>
	/// Gets the tile at the Unity world coordinate.
	/// </summary>
	/// <returns>The tile at world coordinate.</returns>
	/// <param name="coord">The position under which to find a tile</param>
	public Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt (coord.x+0.5f);
		int y = Mathf.FloorToInt (coord.y+0.5f);

		return WorldController.Instance.World.GetTileAt (x, y);
	}

	// Callback Functions

	/// <summary>
	/// Changes the Tile's associated GameObject to render correctly when its type is changed.
	/// </summary>
	/// <param name="tile_data">The Tile which type has been changed.</param>
	void OnTileTypeChanged(Tile tile_data) {

		// Sanity Check
		if (map_Tile2GameObject.ContainsKey(tile_data) == false) {
			Debug.LogError ("WorldController.OnTileTypeChanged - This function has been called from a Tile that is not in map_Tile2GameObject.");
			return;
		}

		// Find the Tile's GameObject
		GameObject tile_go = map_Tile2GameObject [tile_data];

		// Sanity Check
		if (tile_go == null) {
			Debug.LogError ("WorldController.OnTiletypeChanged - This function has been called from a Tile that has no associated GameObject.");
			return;
		}
			
		//TODO: filled tiles with unrecognized types should have a recognizeable default texture
		if (!Tile.TileType.ContainsKey (tile_data.Type)) {
			string defaultTileType = Tile.TileType.Keys.ElementAt (0);
			Debug.LogError("WorldController.OnTiletypeChanged - Unrecognized tile type.  The Tile at ("+tile_data.X+","+tile_data.Y+") has been defaulted to " + defaultTileType);
			tile_data.Type = defaultTileType;
		} else {
			tile_go.GetComponent<SpriteRenderer> ().sprite = map_SpriteName2Sprite ["sp_Terrain_" + tile_data.Type];
		}

	}

	void OnInstalledObjectCreated(InstalledObject io_data) {
		// Create a visual GameObject to display the InstalledObject
		// Place that GameObject in the correct position
		// Add a SpriteRenderer
		// Registers the necessary callback functions,
		// and Stashes the relationship between the InstalledObject and its GameObject in the local map_InstalledObject2GameObject
		GameObject io_go = new GameObject ();
		io_go.name = io_data.ObjectType + "_" + io_data.Tiles.First().X + "_" + io_data.Tiles.First().Y;
		io_go.transform.position = new Vector3 ( io_data.Tiles.First().X, io_data.Tiles.First().Y );
		io_go.transform.SetParent (this.transform, true);

		io_go.AddComponent<SpriteRenderer>().sprite = map_SpriteName2Sprite ["sp_IO_Trees"];

		io_data.RegisterCB_OnStateChanged(OnInstalledObjectStateChanged);

		map_InstalledObject2GameObject.Add (io_data, io_go);
	}

	void OnInstalledObjectStateChanged(InstalledObject io_data) {
		Debug.LogError ("OnInstalledObjectStateChanged - NOT IMPLEMENTED.");
	}

	void OnInstalledObjectDestroyed(InstalledObject io_data) {
		if ( !map_InstalledObject2GameObject.ContainsKey(io_data) ) {
			Debug.LogError ("Tried to destroy an InstalledObject that doesn't exist - this should never happen.");
			return;
		}

		// Destroy the visual representation of the InstalledObject
		Destroy (map_InstalledObject2GameObject [io_data]);

		// Remove the record of the InstalledObject
		map_InstalledObject2GameObject.Remove (io_data);


		io_data.UnregisterCB_OnStateChanged (OnInstalledObjectStateChanged);
		io_data = null;
	}

	// Internal Functions

	/// <summary>
	/// Creates GameObjects for each Tile in the World,
	/// Places those GameObjects in the correct position,
	/// Adds a SpriteRenderer,
	/// Registers the necessary callback functions,
	/// and Stashes the relationship between the Tile and its GameObject in the local map_Tile2GameObject.
	/// </summary>
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

	void LoadSprites() {
		Sprite[] sp_Terrain = Resources.LoadAll<Sprite>("Terrain");
		foreach (var sprite in sp_Terrain) {
			map_SpriteName2Sprite.Add ("sp_" + "Terrain_" + sprite.name, sprite);
		}
		// Allow empty to default to null without having to check for it everywhere
		map_SpriteName2Sprite.Add ("sp_Terrain_Empty", null);

		Sprite[] sp_InstalledObjects = Resources.LoadAll<Sprite>("Misc");
		foreach (var sprite in sp_InstalledObjects) {
			map_SpriteName2Sprite.Add ("sp_" + "IO_" + sprite.name, sprite);
		}
	}
}
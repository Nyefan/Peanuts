using UnityEngine;
using System.Collections;

/// <summary>
/// This class contains and maintains a record all of the Tiles, InstalledObjects, and LooseObjects in a 
/// given layer/map.
/// </summary>
public class World {

	/// <summary>
	/// The Map.
	/// </summary>
	Tile[,] tiles;
	/// <summary>
	/// The width of the map, starting at 1.
	/// </summary>
	int width;
	/// <summary>
	/// The height of the map, starting at 1.
	/// </summary>
	int height;

	//TODO: refactor this to no longer require the explicit private width and height
	/// <summary>
	/// Gets the width.
	/// </summary>
	/// <value>The width of the map, starting at 1.</value>
	public int Width {
		get { return width; }
	}
	/// <summary>
	/// Gets the height.
	/// </summary>
	/// <value>The height of the map, starting at 1.</value>
	public int Height {
		get { return height; }
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="World"/> class.
	/// </summary>
	/// <param name="width">Width - defaults to 100.</param>
	/// <param name="height">Height - defaults to 100.</param>
	public World(int width = 100, int height = 100) {
		this.width = width;
		this.height = height;
		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles [x, y] = new Tile (this, x, y);
			}
		}

		Debug.Log ("World created with " + (width * height) + " tiles.");
	}

	/// <summary>
	/// Currently, this initializes all of the tiles' types to Grass.  Eventually, this should take a saved map as 
	/// the input and use that to initialize all fo the Tiles, LooseObjects, and InstalledObjects in that map.
	/// </summary>
	public void InitializeTiles() {
		foreach (var tile in tiles) {
			tile.Type = Tile.TileType.Grass;
		}
	}

//	public void RandomizeTiles() {
//		Debug.Log("called RandomizeTiles()");
//		for (int x = 0; x < width; x++) {
//			for (int y = 0; y < height; y++) {
//				if (Random.Range(0,2) == 0) {
//					tiles [x, y].Type = Tile.TileType.Empty;
//				} else {
//					tiles [x, y].Type = Tile.TileType.Grass;
//				}
//			}
//		}
//	}

	/// <summary>
	/// Gets the tile at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate - 0-indexed.</param>
	/// <param name="y">The y coordinate - 0-indexed.</param>
	public Tile GetTileAt(int x, int y) {
		if (x >= width || x < 0 || y >= height || y < 0) {
			//TODO: Re-enable this once the UI and Camera are far enough along to make sure it isn't called constantly
			//  OR: have some editor flag in the WorldController that disables this error log while in editor mode
			//Debug.LogError ("Tile (" + x + "," + y + ") is out of range.");
			return null;
		}
		return tiles [x, y];
	}
}

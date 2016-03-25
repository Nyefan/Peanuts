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
	/// Gets or sets the width.
	/// </summary>
	/// <value>The width of the map, starting at 1.</value>
	public int Width { get; protected set; }
	/// <summary>
	/// Gets or sets the height.
	/// </summary>
	/// <value>The height of the map, starting at 1.</value>
	public int Height { get; protected set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="World"/> class.
	/// </summary>
	/// <param name="width">Width - defaults to 100.</param>
	/// <param name="height">Height - defaults to 100.</param>
	public World(int width = 100, int height = 100) {
		Width = width;
		Height = height;
		tiles = new Tile[Width, Height];

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				tiles [x, y] = new Tile (this, x, y);
			}
		}

		Debug.Log ("World created with " + (Width * Height) + " tiles.");
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

	/// <summary>
	/// Gets the tile at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate - 0-indexed.</param>
	/// <param name="y">The y coordinate - 0-indexed.</param>
	public Tile GetTileAt(int x, int y) {
		if (x >= Width || x < 0 || y >= Height || y < 0) {
			//TODO: Re-enable this once the UI and Camera are far enough along to make sure it isn't called constantly
			//  OR: have some editor flag in the WorldController that disables this error log while in editor mode
			//Debug.LogError ("Tile (" + x + "," + y + ") is out of range.");
			return null;
		}
		return tiles [x, y];
	}
}

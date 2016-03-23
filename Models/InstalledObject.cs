using UnityEngine;
using System.Collections.Generic;
using System.Linq;


// This class is used to represent objects that are not interractable but are also not Terrain
// Examples include: mountains, ponds, bridges, and trees
public class InstalledObject {
	string objectType;

	List<Tile> tiles;
	List<float> movementCost;

	int width;
	int height;

	public InstalledObject( string objectType, List<float> movementCost, int width = 1, int height = 1 ) {
		if ( movementCost.Count == 0 ) {
			this.movementCost = new List<float> ();
			for (int i = 0; i < width*height; i++) {
				this.movementCost.Add (0);
			}
		} else if ( movementCost.Count != width*height ) {
			
		}

		this.objectType = objectType;
		this.width = width;
		this.height = height;
	}

	public InstalledObject( InstalledObject prototype, Tile baseTile ) {
		objectType = prototype.objectType;
		movementCost = prototype.movementCost;
		width = prototype.width;
		height = prototype.height;

		tiles = new List<Tile> ();

		// The base tile should be in the bottom left of the sprite/installed object
		// List<Tile> tiles contains the tiles in order from left to right, bottom to top
		for (int h = 0; h < height; h++) {
			for (int w = 0; w < width; w++) {
				tiles.Add (baseTile.World.GetTileAt(baseTile.X + w, baseTile.Y + h));
			}
		}
	}
}

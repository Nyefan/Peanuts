using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class is used to represent objects that are not interractable but are also not Terrain
/// Examples include: mountains, ponds, bridges, and trees
/// </summary>
public class InstalledObject {
	/// <summary>
	/// The type of the object.
	/// </summary>
	string objectType;

	//TODO: if no other list properties are added, look into changing this to a map/dict
	/// <summary>
	/// A list of all tiles that have their behavior altered or overwritten by the InstalledObject
	/// The list reads from left to right, bottom to top (i.e. - instances are anchored in the bottom left)
	/// </summary>
	List<Tile> tiles;
	/// <summary>
	/// A list of movement cost changes for tiles under or around the InstalledObject sprite.
	/// These values will be multiplied by the movementCost for the tile
	/// A movementCost of 0 makes the underlying Tile unpathable
	/// </summary>
	List<float> movementCost;

	int width;
	int height;

	//TODO: enforce global uniqueness of objectType OR change description
	//TODO: once InstalledObjects handle their own sprites, log a warning when sprites are not the same size as
	//      their associated InstalledObject prototypes
	/// <summary>
	/// Creates a prototype of the InstalledObject class.
	/// <para></para>
	/// NOTE: The caller is currently responsible for making sure that the width and height are not swapped,
	///      which will not log an error and will not result in any default behavior.  Once InstalledObjects handle
	///      their own sprites, this incorrect use will be logged as a warning, but it will not be corrected, since
	///      it's possible that the caller will want to have behavior associated with the object outside of the
	///      assigned sprite.
	/// </summary>
	/// <param name="objectType">
	/// A string representing the type of the object.  Currently, the caller is responsible for ensuring that the 
	/// string is unique, but that will be enforced down the line.
	/// </param>
	/// <param name="movementCost">
	/// A list of movement cost changes for tiles under or around the InstalledObject sprite.  These values will
	/// be multiplied by the movementCost for the underlying Tile.  A movementCost of 0 makes the Tile unpathable.
	/// Passing an empty list or a list of the wrong length (i.e. list.Count!=width*height) will create a list
	/// of the correct length with all values set to 0f, and the latter action will log an error.
	/// </param>
	/// <param name="width">
	/// The number of tiles wide the InstalledObject should be.  This is 1-indexed.  Defaults to 1.
	/// </param>
	/// <param name="height">
	/// The number of tiles high the InstalledObject should be.  This is 1-indexed.  Defaults to 1.
	/// </param>
	public InstalledObject( string objectType, List<float> movementCost, int width = 1, int height = 1 ) {
		// If passed an empty list, make object unpathable
		// else if the list length is unreconcileable with the width and height, log an error and make the object unpathable
		if ( movementCost.Count == 0 ) {
			this.movementCost = new List<float> ();
			for (int i = 0; i < width*height; i++) {
				this.movementCost.Add (0f);
			}
		} else if ( movementCost.Count != width*height ) {
			Debug.LogError ("InstalledObject Prototype Constructor for " + objectType + " has been called incorrectly.  Tiles are defaulting to be unpathable.");
			this.movementCost = new List<float> ();
			for (int i = 0; i < width*height; i++) {
				this.movementCost.Add (0f);
			}
		}

		this.objectType = objectType;
		this.width = width;
		this.height = height;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InstalledObject"/> class.
	/// </summary>
	/// <param name="prototype">A properly instantiated prototype of the InstalledObject.  This is assumed to be correct. </param>
	/// <param name="baseTile">The bottom left tile onto which this object is to be installed. </param>
	public InstalledObject( InstalledObject prototype, Tile baseTile ) {
		// TODO: Consider whether it makes more sense to link these values to the prototype directly rather than to make copies.
		// TODO: Look up whether strings are immutable in C#
		objectType = prototype.objectType;
		movementCost = new List<float> ();
		movementCost.AddRange(prototype.movementCost);
		width = prototype.width;
		height = prototype.height;

		tiles = new List<Tile> ();

		// The base tile should be in the bottom left of the sprite/installed object
		// List<Tile> tiles contains the tiles in order from left to right, bottom to top
		for (int h = 0; h < height; h++) {
			for (int w = 0; w < width; w++) {
				// These lines are separate to add sanity checks and pathing requirements later
				Tile t = baseTile.World.GetTileAt (baseTile.X + w, baseTile.Y + h);
				tiles.Add (t);
			}
		}
	}
}

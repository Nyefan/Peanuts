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

	// The size of the InstalledObject, anchored to the bottom left
	public int Width { get; protected set; }
	public int Height { get; protected set; }

	// Disallow instantiating InstalledObjects outside of this class or its children
	protected InstalledObject() { }

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
	public static InstalledObject CreatePrototype( string objectType, List<float> movementCost, int width = 1, int height = 1 ) {

		InstalledObject io = new InstalledObject ();

		// If passed an empty list, make object unpathable
		// else if the list length is unreconcileable with the width and height, log an error and make the object unpathable
		if ( movementCost.Count == 0 ) {
			io.movementCost = new List<float> ();
			for (int i = 0; i < width*height; i++) {
				io.movementCost.Add (0f);
			}
		} else if ( movementCost.Count != width*height ) {
			Debug.LogError ("InstalledObject Prototype Constructor for " + objectType + " has been called incorrectly.  Tiles are defaulting to be unpathable.");
			io.movementCost = new List<float> ();
			for (int i = 0; i < width*height; i++) {
				io.movementCost.Add (0f);
			}
		}

		io.objectType = objectType;
		io.Width = width;
		io.Height = height;
		return io;
	}

	/// <summary>
	/// Places an instance of the prototype in the world.
	/// </summary>
	/// <param name="prototype">A properly instantiated prototype of the InstalledObject.  This is assumed to be correct. </param>
	/// <param name="baseTile">The bottom left tile onto which this object is to be installed. </param>
	public static InstalledObject PlaceInstance( InstalledObject prototype, Tile baseTile ) {

		InstalledObject io = new InstalledObject ();

		// TODO: Consider whether it makes more sense to link these values to the prototype directly rather than to make copies.
		// TODO: Look up whether strings are immutable in C#
		io.objectType = prototype.objectType;
		io.movementCost = new List<float> ();
		io.movementCost.AddRange(prototype.movementCost);
		io.Width = prototype.Width;
		io.Height = prototype.Height;

		io.tiles = new List<Tile> ();

		// The base tile should be in the bottom left of the sprite/installed object
		// List<Tile> tiles contains the tiles in order from left to right, bottom to top
		for (int h = 0; h < io.Height; h++) {
			for (int w = 0; w < io.Width; w++) {
				// These lines are separate to add sanity checks and pathing requirements later
				Tile t = baseTile.World.GetTileAt (baseTile.X + w, baseTile.Y + h);
				io.tiles.Add (t);
			}
		}

		return io;
	}
}

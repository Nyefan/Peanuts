using UnityEngine;
using System;
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
	public string ObjectType { get; protected set; }

	//TODO: if no other list properties are added, look into changing this to a map/dict
	/// <summary>
	/// A list of all tiles that have their behavior altered or overwritten by the InstalledObject
	/// The list reads from left to right, bottom to top (i.e. - instances are anchored in the bottom left)
	/// </summary>
	public List<Tile> Tiles { get; protected set; }
	/// <summary>
	/// A list of movement cost changes for tiles under or around the InstalledObject sprite.
	/// These values will be multiplied by the movementCost for the tile
	/// A movementCost of 0 makes the underlying Tile unpathable
	/// </summary>
	List<float> movementCost;

	// The size of the InstalledObject, anchored to the bottom left
	public int Width { get; protected set; }
	public int Height { get; protected set; }

	//TODO: granulate this if there is too much going on with it.
	Action<InstalledObject> cb_StateChanged;

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
			Debug.LogWarning ("InstalledObject.CreatePrototype has been called with an empty list.  Tiles are defaulting to be unpathable.");
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
		} else { //I'm a dumbass - spent 20 minutes trying to figure out where my nullArgumentExceptions were coming from.
			io.movementCost = movementCost;
		}

		io.ObjectType = objectType;
		io.Width = width;
		io.Height = height;
		return io;
	}

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
	/// The movement cost of ALL tiles associated with the InstalledObject. These values will be multiplied by the 
	/// movementCost for the underlying Tile.  A movementCost of 0 makes the Tile unpathable.  Defaults to 0f.
	/// </param>
	/// <param name="width">
	/// The number of tiles wide the InstalledObject should be.  This is 1-indexed.  Defaults to 1.
	/// </param>
	/// <param name="height">
	/// The number of tiles high the InstalledObject should be.  This is 1-indexed.  Defaults to 1.
	/// </param>
	public static InstalledObject CreatePrototype( string objectType, float movementCost = 0, int Width = 1, int Height = 1 ) {
		List<float> tmp = new List<float> ();
		for (int i = 0; i < Width*Height; i++) {
			tmp.Add (movementCost);
		}
		return CreatePrototype (objectType, tmp, Width, Height);
	}

	/// <summary>
	/// Places an instance of the prototype in the world.
	/// </summary>
	/// <param name="prototype">A properly instantiated prototype of the InstalledObject.  This is assumed to be correct. </param>
	/// <param name="baseTile">The bottom left tile onto which this object is to be installed. </param>
	public static InstalledObject PlaceInstance( InstalledObject prototype, Tile baseTile ) {

		InstalledObject io = new InstalledObject ();

		// TODO: Consider whether it makes more sense to link these values to the prototype directly rather than to make copies.
		io.ObjectType = prototype.ObjectType;
		io.Width = prototype.Width;
		io.Height = prototype.Height;
		io.Tiles = new List<Tile> ();
		io.movementCost = new List<float> ();
		io.movementCost.AddRange(prototype.movementCost);


		// The base tile should be in the bottom left of the sprite/installed object
		// List<Tile> tiles contains the tiles in order from left to right, bottom to top
		for (int h = 0; h < io.Height; h++) {
			for (int w = 0; w < io.Width; w++) {
				// These lines are separate to add sanity checks and pathing requirements later
				Tile t = baseTile.World.GetTileAt (baseTile.X + w, baseTile.Y + h);
				io.Tiles.Add (t);
				// This should be sufficient to handle multi-tile objects,
				// though it may not make sense to handle that here
				t.InstalledObject = io;
			}
		}

		//io.tiles.ElementAt (0).installedObject = io;
		//baseTile.InstalledObject = io;

		return io;
	}

	public void RegisterCB_OnStateChanged( Action<InstalledObject> cbfun ) {
		cb_StateChanged += cbfun;
	}

	public void UnregisterCB_OnStateChanged( Action<InstalledObject> cbfun ) {
		cb_StateChanged -= cbfun;
	}
}

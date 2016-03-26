using UnityEngine;
using System;
using System.Collections.Generic;

public class Tile  {

	/// <summary>
	/// Available base terrain types.
	/// </summary>
	public enum TileType                           { Empty, Water, Grass, Desert, Plains, Rough, Lava, Snow, Marsh };
	/// <summary>
	/// The base move cost of each terrain type.
	/// </summary>
	static List<float> moveCost = new List<float>(){     0,    5f,    1f,   1.2f,   1.2f,  1.5f,   2f,   2f,    2f };

	/// <summary>
	/// Container for callback functions that should be called when the tile type changes
	/// These are registered by the caller, who is also responsible for unregistering them before destroying the tile
	/// </summary>
	Action<Tile> cb_TileTypeChanged;

	/// <summary>
	/// The base terrain type of the tile.
	/// </summary>
	TileType type = TileType.Empty;

	/// <summary>
	/// Gets or sets the base terrain type of the Tile.  If any render layer modifications need to be made as a 
	/// result of changing this value, they should be registered as a callback function
	/// </summary>
	/// <value>The type.</value>
	public TileType Type {
		get { return type; }
		set { 
			TileType old = type;
			type = value; 
			if (cb_TileTypeChanged != null && old != type) {
				cb_TileTypeChanged (this);
			}
		}
	}

	//TODO: make this dependent on modifiers from InstalledObjects and LooseObjects
	/// <summary>
	/// Returns the total move cost of the tile, including all object-based modifiers.  Player-based modifiers should be 
	/// handled by the caller.
	/// </summary>
	/// <value>The move cost.</value>
	public float MoveCost { get { return moveCost[(int)type]; } }

	public InterractableObject InterractableObject { get; set; }

	InstalledObject io;
	public InstalledObject InstalledObject { 
		get { return io; } 
		set {
			if(value == null) {
				Debug.Log ("Removed InstalledObject " + io.ObjectType + " from Tile " + X + ", " + Y + ".");
			} else if (io != null) {
				Debug.LogWarning ("Tried to place InstalledObject" + value.ObjectType + " in a Tile that already has an installedObject.");
			} else {
				io = value;
				Debug.Log ("InstalledObject " + value.ObjectType + " has been placed in Tile " + X + ", " + Y + ".");
			}
		} 
	}

	/// <summary>
	/// Gets or sets the world.
	/// </summary>
	/// <value>The world (i.e. map/layer) that the Tile occupies.</value>
	public World World { get; protected set; }

	/// <summary>
	/// Returns the x coordinate of the Tile.
	/// </summary>
	public int X { get; protected set; }
	/// <summary>
	/// Returns the y coordinate of the Tile.
	/// </summary>
	public int Y { get; protected set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="world">The World representing the layer/map in which the Tile resides</param>
	/// <param name="x">The x coordinate - 0-Indexed.</param>
	/// <param name="y">The y coordinate - 0-Indexed.</param>
	public Tile(World world, int x, int y) {
		this.World = world;
		this.X = x;
		this.Y = y;
	}

	/// <summary>
	/// Registers a callback function to be run when the TileType is changed.
	/// </summary>
	/// <param name="cbfun">A function of the form foo(Tile tile).</param>
	public void RegisterCB_OnTileTypeChanged(Action<Tile> cbfun) {
		cb_TileTypeChanged += cbfun;
	}

	//TODO: log a warning when this is called on a function that is not in cb_TileTypeChanged
	/// <summary>
	/// Unregisters a callback function to be run when the TileType is changed.
	/// </summary>
	/// <param name="cbfun">A function of the form foo(Tile tile).</param>
	public void UnregisterCB_OnTileTypeChanged(Action<Tile> cbfun) {
		cb_TileTypeChanged -= cbfun;
	}
}

using UnityEngine;
using System;
using System.Collections.Generic;

public class Tile  {

	public enum TileType      {          Empty, Water, Grass, Desert, Plains, Rough, Lava, Snow, Marsh };
	static List<float> moveCost = new List<float>(){ Int32.MaxValue,    2f,    1f,   1.2f,   1.2f,  1.5f,   2f,   2f,    2f };


	TileType type = TileType.Empty;

	Action<Tile> cb_TileTypeChanged;

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

	public float MoveCost { get { return moveCost[(int)type]; } }

	LooseObject looseObject;
	InstalledObject installedObject;

	World world;
	//TODO: restructure this class so that World supplants world
	public World World { get { return world; } }

	int x;
	int y;

	public int X { get { return x; } }
	public int Y { get { return y; } }

	public Tile(World world, int x, int y) {
		this.world = world;
		this.x = x;
		this.y = y;
	}

	public void RegisterCB_OnTileTypeChanged(Action<Tile> cbfun) {
		cb_TileTypeChanged += cbfun;
	}

	public void UnregisterCB_OnTileTypeChanged(Action<Tile> cbfun) {
		cb_TileTypeChanged -= cbfun;
	}
}

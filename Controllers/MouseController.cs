using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class MouseController : MonoBehaviour {

	/// <summary>
	/// The selection box prefab - this is set through Unity.
	/// </summary>
	public GameObject selectionBoxPrefab;

	// Flags used for adjusting the behavior of the editor
	bool          buildModeIsInstalledObject = false;
	bool          buildModeIsLooseObject = false;

	/// <summary>
	/// The current TileType to be painted while in build mode.  Defaults to Grass.
	/// </summary>
	Tile.TileType buildModeType = Tile.TileType.Grass;

	// Camera positions used for dragging and selecting objects
	Vector3 lastFramePosition;
	Vector3 currentFramePosition;

	/// <summary>
	/// The point at which the mouse was originally clicked when dragging in build mode.
	/// </summary>
	Vector3 bandSelectionBoxStartPosition;

	/// <summary>
	/// Flag used to disable dragging and building when clicking over a UI element.
	/// </summary>
	bool noDragFlag = false;

	/// <summary>
	/// A local record of the currently displayed selection box graphics.
	/// </summary>
	List<GameObject> selectionBoxInstances;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		// Keep a record of currently used selection boxes
		selectionBoxInstances = new List<GameObject> ();
	}
	
	/// <summary>
	/// Update this instance - called once per frame.
	/// </summary>
	void Update () {
		currentFramePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		currentFramePosition.z = 0;

		UpdateDragging ();
		UpdateCamera ();

		lastFramePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		lastFramePosition.z = 0;
	}

	//TODO: once editor and gameplay modes are separate, check for the editor flag before doing anything
	//TODO: alter this to work as expected when the build mode is not set to Tile
	/// <summary>
	/// Handles the creation, destruction, and alteration of the selection box graphic and selected tiles
	/// while in editor mode.
	/// </summary>
	void UpdateDragging () {
		
		// Determine whether we should start a selection box
		if (Input.GetMouseButtonDown(0)) {
			// Prevents selection box from instantiating during mouse clicks on UI elements
			if (EventSystem.current.IsPointerOverGameObject()) { 
				noDragFlag = true;	
				return; 
			} else { 
				noDragFlag = false; 
			} 

			bandSelectionBoxStartPosition = currentFramePosition;
		}

		// Figure out the bounds of the selection box
		int start_x = Mathf.FloorToInt (bandSelectionBoxStartPosition.x + 0.5f);
		int start_y = Mathf.FloorToInt (bandSelectionBoxStartPosition.y + 0.5f);
		int end_x = Mathf.FloorToInt (currentFramePosition.x + 0.5f);
		int end_y = Mathf.FloorToInt (currentFramePosition.y + 0.5f);
		if (start_x > end_x) { Swap (ref start_x, ref end_x); }
		if (start_y > end_y) { Swap (ref start_y, ref end_y); }

		// Turn off all existing drag boxes - this should probably be changed once there is a more sophisticated map editing system
		while(selectionBoxInstances.Count > 0) {
			GameObject go = selectionBoxInstances [0];
			selectionBoxInstances.RemoveAt (0);
			SimplePool.Despawn (go);
		}

		// Display a preview of the drag area by turning on existing dragbox sprites or creating new ones if there are no more in the pool
		if (Input.GetMouseButton(0) && !noDragFlag) {
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if(t != null) {
						GameObject go = SimplePool.Spawn (selectionBoxPrefab, new Vector3 (x, y, 0), Quaternion.identity);
						go.transform.SetParent (this.transform, true);
						selectionBoxInstances.Add (go);
					}
				}
			}
		}

		// Change the Type of selected Tiles when the mouse is released after clicking and dragging
		if (Input.GetMouseButtonUp (0) && !noDragFlag) {
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if(t != null) { 
						if (t.Type != null) {
							t.Type = buildModeType;
						} 
					}
				}
			}
		}
	}
		
	/// <summary>
	/// Updates the camera - currently this just drags the screen when middle mouse is held down.
	/// </summary>
	void UpdateCamera () {
		if (Input.GetMouseButton(2)) { // 2 = middle mouse button
			Vector3 diff = lastFramePosition - currentFramePosition;
			Camera.main.transform.Translate (diff);
		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * 3;
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 3f, 20f);
	}

	// Setter functions for build that don't rely on user knowing what types are valid
	// Available types are: Empty, Water, Grass, Desert, Plains, Rough, Lava, Snow, Marsh
	// TODO: Probably should change this to take advantage of the underlying enumeration, 
	//	     but that allows the render layer to dictate the behaviour of the model layer
	public void SetTilePainter_Empty() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Empty; 
	}
	public void SetTilePainter_Water() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Water; 
	}
	public void SetTilePainter_Grass() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Grass; 
	}
	public void SetTilePainter_Desert() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Desert; 
	}
	public void SetTilePainter_Plains() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Plains; 
	}
	public void SetTilePainter_Rough() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Rough; 
	}
	public void SetTilePainter_Lava() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Lava; 
	}
	public void SetTilePainter_Snow() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Snow; 
	}
	public void SetTilePainter_Marsh() { 
		SetBuildMode_T ();
		buildModeType = Tile.TileType.Marsh; 
	}

	// Internal Functions

	// These three functions set the build mode more concisely

	// Installed Object
	/// <summary>
	/// Sets the build mode to work with InstalledObjects.
	/// </summary>
	void SetBuildMode_IO() { 
		bool buildModeIsInstalledObject = true;
		bool buildModeIsLooseObject = false;
	}

	// LooseObject
	/// <summary>
	/// Sets the build mode to work with LooseObjects.
	/// </summary>
	void SetBuildMode_LO() {
		bool buildModeIsInstalledObject = false;
		bool buildModeIsLooseObject = true;
	}

	// Tile
	/// <summary>
	/// Sets the build mode to work with Tiles.
	/// </summary>
	void SetBuildMode_T() {
		bool buildModeIsInstalledObject = false;
		bool buildModeIsLooseObject = false;
	}

	// Why isn't this included in the base language?
	void Swap<T> (ref T l, ref T r) { T t = l; l = r; r = t; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStateManager : MonoBehaviour {
  public TileState[] tile_states_;
  public Texture2D[] tile_textures_;

  private int BLUE = 0;
  private int YELLOW = 1;

  public Texture2D GetTexture(int id) {
    return tile_textures_[id];
  }

  // Use this for initialization
  void Start () {
    tile_states_ = new TileState[16];

    tile_states_[0] = new TileState(tile_textures_[0], 0.06f);
    tile_states_[0].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[0].AddConstraint(Direction.EAST, BLUE);
    tile_states_[0].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[0].AddConstraint(Direction.WEST, BLUE);

    tile_states_[1] = new TileState(tile_textures_[1], 0.07f);
    tile_states_[1].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[1].AddConstraint(Direction.EAST, BLUE);
    tile_states_[1].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[1].AddConstraint(Direction.WEST, BLUE);

    tile_states_[2] = new TileState(tile_textures_[2], 0.07f);
    tile_states_[2].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[2].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[2].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[2].AddConstraint(Direction.WEST, BLUE);

    tile_states_[3] = new TileState(tile_textures_[3], 0.06f);
    tile_states_[3].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[3].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[3].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[3].AddConstraint(Direction.WEST, BLUE);

    tile_states_[4] = new TileState(tile_textures_[4], 0.065f);
    tile_states_[4].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[4].AddConstraint(Direction.EAST, BLUE);
    tile_states_[4].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[4].AddConstraint(Direction.WEST, BLUE);

    tile_states_[5] = new TileState(tile_textures_[5], 0.05f);
    tile_states_[5].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[5].AddConstraint(Direction.EAST, BLUE);
    tile_states_[5].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[5].AddConstraint(Direction.WEST, BLUE);

    tile_states_[6] = new TileState(tile_textures_[6], 0.06f);
    tile_states_[6].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[6].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[6].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[6].AddConstraint(Direction.WEST, BLUE);

    tile_states_[7] = new TileState(tile_textures_[7], 0.06f);
    tile_states_[7].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[7].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[7].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[7].AddConstraint(Direction.WEST, BLUE);

    tile_states_[8] = new TileState(tile_textures_[8], 0.06f);
    tile_states_[8].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[8].AddConstraint(Direction.EAST, BLUE);
    tile_states_[8].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[8].AddConstraint(Direction.WEST, YELLOW);

    tile_states_[9] = new TileState(tile_textures_[9], 0.065f);
    tile_states_[9].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[9].AddConstraint(Direction.EAST, BLUE);
    tile_states_[9].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[9].AddConstraint(Direction.WEST, YELLOW);

    tile_states_[10] = new TileState(tile_textures_[10], 0.06f);
    tile_states_[10].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[10].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[10].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[10].AddConstraint(Direction.WEST, YELLOW);

    tile_states_[11] = new TileState(tile_textures_[11], 0.06f);
    tile_states_[11].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[11].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[11].AddConstraint(Direction.SOUTH, BLUE);
    tile_states_[11].AddConstraint(Direction.WEST, YELLOW);

    tile_states_[12] = new TileState(tile_textures_[12], 0.065f);
    tile_states_[12].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[12].AddConstraint(Direction.EAST, BLUE);
    tile_states_[12].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[12].AddConstraint(Direction.WEST, YELLOW);

    tile_states_[13] = new TileState(tile_textures_[13], 0.07f);
    tile_states_[13].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[13].AddConstraint(Direction.EAST, BLUE);
    tile_states_[13].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[13].AddConstraint(Direction.WEST, YELLOW);

    tile_states_[14] = new TileState(tile_textures_[14], 0.065f);
    tile_states_[14].AddConstraint(Direction.NORTH, BLUE);
    tile_states_[14].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[14].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[14].AddConstraint(Direction.WEST, YELLOW);

    tile_states_[15] = new TileState(tile_textures_[15], 0.06f);
    tile_states_[15].AddConstraint(Direction.NORTH, YELLOW);
    tile_states_[15].AddConstraint(Direction.EAST, YELLOW);
    tile_states_[15].AddConstraint(Direction.SOUTH, YELLOW);
    tile_states_[15].AddConstraint(Direction.WEST, YELLOW);
  }

  // Update is called once per frame
  void Update () {
		
	}
}

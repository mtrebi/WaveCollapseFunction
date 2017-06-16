using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStateManager : MonoBehaviour {
  public TileState[] tile_states_;
  public Texture2D[] tile_textures_;

  public bool enable_rotations_;
  // TODO public bool enable_reflections_;


  public Texture2D GetTexture(int id) {
    return tile_textures_[id];
  }

  // Use this for initialization
  void Start () {
    tile_states_ = new TileState[16];

    tile_states_[0] = new TileState(new Bin("0000"), tile_textures_[0], 0.06f);
    tile_states_[1] = new TileState(new Bin("0001"), tile_textures_[1], 0.07f);
    tile_states_[2] = new TileState(new Bin("0010"), tile_textures_[2], 0.07f);
    tile_states_[3] = new TileState(new Bin("0011"), tile_textures_[3], 0.06f);
    tile_states_[4] = new TileState(new Bin("0100"), tile_textures_[4], 0.065f);
    tile_states_[5] = new TileState(new Bin("0101"), tile_textures_[5], 0.05f);
    tile_states_[6] = new TileState(new Bin("0110"), tile_textures_[6], 0.06f);
    tile_states_[7] = new TileState(new Bin("0111"), tile_textures_[7], 0.06f);
    tile_states_[8] = new TileState(new Bin("1000"), tile_textures_[8], 0.06f);
    tile_states_[9] = new TileState(new Bin("1001"), tile_textures_[9], 0.065f);
    tile_states_[10] = new TileState(new Bin("1010"), tile_textures_[10], 0.06f);
    tile_states_[11] = new TileState(new Bin("1011"), tile_textures_[11], 0.06f);
    tile_states_[12] = new TileState(new Bin("1100"), tile_textures_[12], 0.065f);
    tile_states_[13] = new TileState(new Bin("1101"), tile_textures_[13], 0.07f);
    tile_states_[14] = new TileState(new Bin("1110"), tile_textures_[14], 0.065f);
    tile_states_[15] = new TileState(new Bin("1111"), tile_textures_[15], 0.06f);

    if (enable_rotations_) {
      //GenerateRotations();
    }
  }
}

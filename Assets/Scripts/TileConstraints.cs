using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction {
  NORTH,
  EAST,
  SOUTH,
  WEST
}

static class DirectionMethods {
  public static Direction Opposite(this Direction direction) {
    return (int) direction < 2 ? direction + 2 : direction - 2;
  }
}


public class TileConstraint {
  public Direction direction_;
  public int id_; // TODO > Typedef

  public TileConstraint(Direction direction, int id) {
    direction_ = direction;
    id_ = id;
  }

  public static bool ConstraintSatisfies(TileState state, Direction direction, TileState neighbor) {
    return state.GetConstraint(direction) == neighbor.GetConstraint(direction.Opposite());
  }
}
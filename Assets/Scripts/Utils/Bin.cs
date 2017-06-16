using UnityEngine;

public class Bin {
  int kBITS = 4;

  string string_binary_;
  int int_binary_;


  public Bin(string s, int bits = 4) {
    kBITS = bits;
    string_binary_ = s;
    int_binary_ = System.Convert.ToInt32(string_binary_, 2);
  }

  public Bin(int num, int bits = 4) {
    kBITS = bits;
    int_binary_ = num;
    string_binary_ = System.Convert.ToString(int_binary_, 2);
  }

  public int Length() {
    return kBITS;
  }

  public char GetBit(int n) {
    return string_binary_[kBITS - 1 - n];
  }

  /// <summary>
  /// Left rotate of a binary literal
  /// </summary>
  /// <param name="n"> Number of rotations </param>
  /// <returns> The result of rotating this Bin N times to the left </returns>
  public Bin Rotate(int n) {
    int rotated_num = (int_binary_ << n) | (int_binary_ >> (kBITS - n));
    return new Bin(rotated_num, kBITS);
  }

  public override string ToString() {
    return "0b" + string_binary_;
  }

  public override bool Equals(object obj) {
    var item = obj as Bin;

    if (item == null) {
      return false;
    }

    return this.int_binary_ == item.int_binary_;
  }

  public override int GetHashCode() {
    return this.int_binary_.GetHashCode();
  }
}

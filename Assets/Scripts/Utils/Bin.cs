using UnityEngine;

public class Bin {
  int kBITS = 4;
  uint kBITMASK;

  string string_binary_;
  uint int_binary_;


  public Bin(string s, int bits = 4) {
    kBITS = bits;
    kBITMASK = ((uint)kBITS * (uint)kBITS) - 1;
    string_binary_ = s;
    int_binary_ = System.Convert.ToUInt32(string_binary_, 2);
  }

  public Bin(uint num, int bits = 4) {
    kBITS = bits;
    int_binary_ = num;
    string_binary_ = System.Convert.ToString(int_binary_, 2);

    int diff = kBITS - string_binary_.Length;
    if (diff != 0) {
      // Fill with zeros on the left
      for (int i = 0; i < diff; ++i) {
        string_binary_ = string_binary_.Insert(0, "0");
      }
    }
  }

  public int Length() {
    return kBITS;
  }

  public char GetBit(int n) {
    return string_binary_[kBITS - 1 - n];
  }

  public void SetBit(int n, char bit) {
    char[] arr = string_binary_.ToCharArray();
    arr[kBITS - 1 - n] = bit;
    string_binary_ = new string(arr);
  }

  /// <summary>
  /// Left rotate of a binary literal
  /// </summary>
  /// <param name="n"> Number of rotations </param>
  /// <returns> The result of rotating this Bin N times to the left </returns>
  public Bin Rotate(int n) {
    uint rotated_num = kBITMASK & ((int_binary_ << n) | (int_binary_ >> (kBITS - n)));
    return new Bin(rotated_num, kBITS);
  }

  public override string ToString() {
    return "0b" + string_binary_;
  }

  public override bool Equals(object obj) {
    var item = obj as Bin;
    if (item == null) return false;
    return this.int_binary_.Equals(item.int_binary_);
  }

  public override int GetHashCode() {
    return this.int_binary_.GetHashCode();
  }
}

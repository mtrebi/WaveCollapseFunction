using UnityEngine;

[System.Serializable]
public class Bin {
  [SerializeField] string string_binary_;
  uint kBITMASK;
  uint int_binary_;

  /// <summary>
  /// Constructor from binary string
  /// </summary>
  /// <param name="s"> binary string </param>
  public Bin(string s) {
    string_binary_ = s;
    int_binary_ = System.Convert.ToUInt32(string_binary_, 2);
    kBITMASK = ((uint)string_binary_.Length * (uint)string_binary_.Length) - 1;
  }

  /// <summary>
  /// Constructor from size
  /// </summary>
  /// <param name="bits"> Number of bits used to represent this number</param>
  public Bin(int bits = 8) {
    int_binary_ = 0;
    string_binary_ = System.Convert.ToString(int_binary_, 2);

    int diff = bits - string_binary_.Length;
    if (diff != 0) {
      // Fill with zeros on the left
      for (int i = 0; i < diff; ++i) {
        string_binary_ = string_binary_.Insert(0, "0");
      }
    }
    kBITMASK = ((uint)string_binary_.Length * (uint)string_binary_.Length) - 1;
  }

  /// <summary>
  /// Constructor from unsigned int
  /// </summary>
  /// <param name="num"> Number to be represented in binary</param>
  /// <param name="bits"> Number of bits used to represent this number</param>
  public Bin(uint num, int bits = 8) {
    int_binary_ = num;
    string_binary_ = System.Convert.ToString(int_binary_, 2);

    int diff = bits - string_binary_.Length;
    if (diff != 0) {
      // Fill with zeros on the left
      for (int i = 0; i < diff; ++i) {
        string_binary_ = string_binary_.Insert(0, "0");
      }
    }
    kBITMASK = ((uint)string_binary_.Length * (uint)string_binary_.Length) - 1;
  }

  /// <summary>
  /// Get binary string literal
  /// </summary>
  /// <returns> Binary string </returns>
  public string GetLiteral() {
    return string_binary_;
  }


  /// <summary>
  /// Number of bits of the binary word
  /// </summary>
  /// <returns> Number of bits of the binary word</returns>
  public int Length() {
    return string_binary_.Length;
  }

  /// <summary>
  /// Get value of bit at a given position
  /// </summary>
  /// <param name="position">Position of the bit</param>
  /// <returns> Value of the bit </returns>
  public string GetBit(int position) {
    if (position > string_binary_.Length - 1) {
      return "0";
    }
    return string_binary_[string_binary_.Length - 1 - position].ToString();
  }

  /// <summary>
  /// Get just a sub part of the binary literal
  /// </summary>
  /// <param name="start_pos">Start position of the range (lowest bit)</param>
  /// <param name="end_pos">End position of the range (highest bit)</param>
  /// <returns> A new binary literal that goes from start_pos to end_pos</returns>
  public Bin GetBitRange(int start_pos, int end_pos) {
    int length = start_pos - end_pos;
    string substring = string_binary_.Substring(start_pos, length);
    return new Bin(substring);
  }

  /// <summary>
  /// Set the value of a bit at the given position
  /// </summary>
  /// <param name="position">Position of the bit</param>
  /// <param name="bit">Value of the bit</param>
  public void SetBit(int position, string value) {
    char[] arr = string_binary_.ToCharArray();
    arr[string_binary_.Length - 1 - position] = value[0];
    string_binary_ = new string(arr);
    int_binary_ = System.Convert.ToUInt32(string_binary_, 2);
  }

  /// <summary>
  /// Left rotate of a binary literal
  /// </summary>
  /// <param name="n"> Number of rotations </param>
  /// <returns> The result of rotating this Bin N times to the left </returns>
  public Bin Rotate(int n) {
    uint rotated_num = kBITMASK & ((int_binary_ << n) | (int_binary_ >> (string_binary_.Length - n)));
    return new Bin(rotated_num, string_binary_.Length);
  }

  /// <summary>
  /// Get the negate of this bin (change 0 by 1 and 1 by 0)
  /// </summary>
  /// <returns> Negation of this bin </returns>
  public Bin Not() {
    char[] arr = new char[string_binary_.Length];
    for (int i = 0; i < string_binary_.Length; ++i) {
      char c = string_binary_[i] == '1' ? '0' : '1';
      arr[i] = c;
    }

    return new Bin(new string(arr));
  }

  public override string ToString() {
    return string_binary_;
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

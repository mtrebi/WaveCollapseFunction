using UnityEngine;

[System.Serializable]
public class Bin {
  [SerializeField] string string_binary_;

  /// <summary>
  /// Constructor from binary string
  /// </summary>
  /// <param name="s"> binary string </param>
  public Bin(string s) {
    string_binary_ = s;
  }

  /// <summary>
  /// Constructor from size
  /// </summary>
  /// <param name="bits"> Number of bits used to represent this number</param>
  public Bin(int bits = 8) {
    string_binary_ = System.Convert.ToString(0, 2);

    int diff = bits - string_binary_.Length;
    if (diff != 0) {
      // Fill with zeros on the left
      for (int i = 0; i < diff; ++i) {
        string_binary_ = string_binary_.Insert(0, "0");
      }
    }
  }

  /// <summary>
  /// Constructor from unsigned int
  /// </summary>
  /// <param name="num"> Number to be represented in binary</param>
  /// <param name="bits"> Number of bits used to represent this number</param>
  public Bin(uint num, int bits = 8) {
    string_binary_ = System.Convert.ToString(num, 2);

    int diff = bits - string_binary_.Length;
    if (diff != 0) {
      // Fill with zeros on the left
      for (int i = 0; i < diff; ++i) {
        string_binary_ = string_binary_.Insert(0, "0");
      }
    }
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
  /// <param name="position">Position of the bit starting from the end</param>
  /// <returns> Value of the bit </returns>
  public string GetBit(int position) {
    if (position > string_binary_.Length - 1) {
      return "0";
    }
    return string_binary_[string_binary_.Length - 1 - position].ToString();
  }

  /// <summary>
  /// Set the value of a bit at the given position
  /// </summary>
  /// <param name="position">Position of the bit starting from the end</param>
  /// <param name="bit">Value of the bit</param>
  public void SetBit(int position, string value) {
    char[] arr = string_binary_.ToCharArray();
    arr[string_binary_.Length - 1 - position] = value[0];
    string_binary_ = new string(arr);
  }

  public override string ToString() {
    return string_binary_;
  }

  public uint ToInt() {
    return System.Convert.ToUInt32(string_binary_, 2);
  }

  public override bool Equals(object obj) {
    var item = obj as Bin;
    if (item == null) return false;
    return this.string_binary_.Equals(item.string_binary_);
  }

  public override int GetHashCode() {
    return this.ToInt().GetHashCode();
  }
}

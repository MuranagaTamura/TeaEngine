using System;
using System.Collections.Generic;
using System.Text;

namespace TeaEngine.Core
{
  public class BinaryBuilder
  {
    List<byte> _codes = new List<byte>();
    Dictionary<string, uint> _callIds = new Dictionary<string, uint>();

    public void Init()
    {
      _callIds = Intrinsic.InitAndGetIds();
    }

    public byte[] GenerateCodes() => _codes.ToArray();

    public void LetInterger(int value)
    {
      List<byte> bins = new List<byte>(5);
      bins.Add((byte)Engine.Type.Integer);
      bins.AddRange(BitConverter.GetBytes(value));
      Let(4, bins.ToArray());
    }

    public void Let(uint size, byte[] context)
    {
      if(size != context?.Length - 1)
      {
        // 指定されたサイズと内容のサイズが一致していない
        return;
      }

      _codes.Add((byte)Engine.Opcode.Let);
      _codes.AddRange(BitConverter.GetBytes(size));
      _codes.AddRange(context);
    }

    public void SetArgs()
    {
      _codes.Add((byte)Engine.Opcode.SetArgs);
    }

    public void Call(string name)
    {
      if(_callIds.TryGetValue(name, out uint id))
      {
        _codes.Add((byte)Engine.Opcode.Call);
        _codes.AddRange(BitConverter.GetBytes(id));
      }
    }
  }
}

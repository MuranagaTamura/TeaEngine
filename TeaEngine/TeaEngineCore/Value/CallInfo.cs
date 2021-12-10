using System;
using System.Collections.Generic;
using System.Text;

namespace TeaEngine.Core.Value
{
  public class CallInfo : IValue
  {
    private uint? _preBp = null;
    private uint? _prePc = null;

    public uint PreBp
    {
      get => _preBp.HasValue ? _preBp.Value : uint.MaxValue;
      set
      {
        if (!_preBp.HasValue) _preBp = value;
      }
    }
    public uint PrePc
    {
      get => _prePc.HasValue ? _prePc.Value : uint.MaxValue;
      set
      {
        if (!_prePc.HasValue) _prePc = value;
      }
    }

    public Engine.Type Type => Engine.Type.CallReturn;
    public IList<IValue> Args { get; private set; } = new List<IValue>();

    public IValue Return = NullValue.Null;

    public bool CompareTo(IValue value) => false;

    public bool SetMember(byte[] context) => false;
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TeaEngine.Core.Value
{
  public class CallReturn : IValue
  {
    public Engine.Type Type => Engine.Type.CallReturn;

    public uint PreBp { get; private set; } = 0;
    public uint PrePc { get; private set; } = 0;

    public IValue Return = NullValue.Null;

    public CallReturn(uint bp, uint pc)
    {
      PreBp = bp;
      PrePc = pc;
    }

    public bool CompareTo(IValue value) => false;

    public bool SetMember(byte[] context) => false;
  }
}

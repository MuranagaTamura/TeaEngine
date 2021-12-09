using System;
using System.Collections.Generic;
using System.Text;
using TeaEngine.Core.Value;

namespace TeaEngine.Core
{
  public enum RunStatus
  {
    Continue,
    End,

    Error,
  }

  public class ParamContext
  {
    private Engine _engine = null;

    public ParamContext(Engine engine)
    {
      _engine = engine;
    }

    public IValue GetArg(uint argNum)
    {
      uint argPtr = _engine.Bp - argNum - 1;
      if (Math.Clamp(argPtr, 0, _engine.Stack.Length - 1) != argPtr)
      {
        return NullValue.Null;
      }

      return _engine.Stack[argPtr];
    }

    public void SetReturn(IValue value)
    {
      ((CallReturn)_engine.Stack[_engine.Bp]).Return = value;
    }
  }
}

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
      if (Math.Clamp(_engine.Bp, 0, _engine.Stack.Length - 1) != _engine.Bp)
      {
        // _engine.Bpがスタック範囲外を指している
        return NullValue.Null;
      }

      CallInfo info = _engine.Stack[_engine.Bp] as CallInfo;
      if (info == null)
      {
        // _engine.Bpが指している先がCallInfoではなかった
        return NullValue.Null;
      }

      if (Math.Clamp(argNum, 0, info.Args.Count - 1) != argNum)
      {
        // argNumとして指定された引数は存在しませんでした
        return NullValue.Null;
      }

      return info.Args[(int)argNum];
    }

    public void SetReturn(IValue value)
    {
      if (Math.Clamp(_engine.Bp, 0, _engine.Stack.Length - 1) != _engine.Bp)
      {
        // _engine.Bpがスタック範囲外を指している
        return;
      }

      CallInfo info = _engine.Stack[_engine.Bp] as CallInfo;
      if (info == null)
      {
        // _engine.Bpが指している先がCallInfoではなかった
        return;
      }

      // _engine.Bpが指している先がCallInfoだった
      info.Return = value;
    }
  }
}

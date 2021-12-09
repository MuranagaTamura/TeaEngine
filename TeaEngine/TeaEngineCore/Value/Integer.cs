using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TeaEngine.Core.Value
{
  public class Integer : IValue
  {
    private int _value = 0;

    public Engine.Type Type => Engine.Type.Integer;

    public Integer(int value)
    {
      _value = value;
    }

    public static (RunStatus, string) Add(ParamContext context)
    {
      return Calc(context, (a, b) => a + b, "+");
    }


    public static (RunStatus, string) Sub(ParamContext context)
    {
      return Calc(context, (a, b) => a - b, "-");
    }


    public static (RunStatus, string) Mul(ParamContext context)
    {
      return Calc(context, (a, b) => a * b, "*");
    }

    public static (RunStatus, string) Div(ParamContext context)
    {
      return Calc(context, (a, b) => a / b, "/");
    }

    public static (RunStatus, string) Calc(ParamContext context, Func<int, int, int> func, string op)
    {
      IValue a = context.GetArg(0);
      IValue b = context.GetArg(1);
      if (a is Integer aValue && b is Integer bValue)
      {
        context.SetReturn(new Integer(func(aValue._value, bValue._value)));
        return (RunStatus.End, "");
      }

      return (RunStatus.Error, $"サポートされていない演算です: {a.GetType()} {op} {b.GetType()}");
    }

    public bool CompareTo(IValue other)
    {
      if(other is Integer otherValue)
      {
        return _value == otherValue._value;
      }

      return false;
    }

    public bool SetMember(byte[] context)
    {
      if(context?.Length != 4)
      {
        return false;
      }
      _value = BitConverter.ToInt32(context);
      return true;
    }
  } // class
} // namespace

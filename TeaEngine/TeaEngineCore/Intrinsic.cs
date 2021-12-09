using System;
using System.Collections.Generic;
using System.Text;
using TeaEngine.Core.Value;

namespace TeaEngine.Core
{
  public class Intrinsic
  {
    public Func<ParamContext, (RunStatus, string)> Action { get; private set; }

    private Intrinsic() { }

    public static Intrinsic NewIntrinsic(Func<ParamContext, (RunStatus, string)> action)
    {
      uint id = (uint)Engine.Intrinsics.Count;
      Intrinsic result = new Intrinsic();
      result.Action = action;
      Engine.Intrinsics[id] = result;
      return result;
    }

    public static Intrinsic NewIntrinsic(uint id, Func<ParamContext, (RunStatus, string)> action)
    {
      if(Engine.Intrinsics.TryGetValue(id, out Intrinsic val))
      {
        return val;
      }
      Intrinsic result = new Intrinsic();
      result.Action = action;
      Engine.Intrinsics[id] = result;
      return result;
    }

    public static void Init()
    {
      InitAndGetIds();
    }

    public static Dictionary<string, uint> InitAndGetIds()
    {
      Engine.Intrinsics.Clear();
      Dictionary<string, uint> result = new Dictionary<string, uint>();

      uint id = 0xFFFFFFFF;
      Register(ref id, "Uint8.+", Integer.Add, result);
      Register(ref id, "Uint8.-", Integer.Sub, result);
      Register(ref id, "Uint8.*", Integer.Mul, result);
      Register(ref id, "Uint8./", Integer.Div, result);

      // TODO: 以降に追加していく

      return result;
    }

    private static void Register(
      ref uint id,
      string name,
      Func<ParamContext, (RunStatus, string)> action,
      Dictionary<string, uint> result)
    {
      NewIntrinsic(id, action);
      result.Add(name, id);
      id--;
    }
  }
}

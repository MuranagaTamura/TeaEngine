using System;
using System.Collections.Generic;
using System.Text;
using TeaEngine.Core.Value;

namespace TeaEngine.Core
{
  public class Engine
  {
    public enum Opcode
    {
      None,
      
      Let,
      SetArgs,
      Call,
    }

    public enum Type
    {
      Null,

      Integer,
      CallReturn,
    }

    private byte[] _codes = null;
    private Intrinsic _runIntrinsic = null;
    private CallInfo _callInfo = null;

    public static Dictionary<uint, Intrinsic> Intrinsics { get; private set; } 
      = new Dictionary<uint, Intrinsic>();

    public IValue[] Stack { get; private set; } = null;
    public uint Pc { get; private set; } = 0;
    public uint Bp { get; private set; } = 0;
    public uint Sp { get; private set; } = 0;

    public void Init()
    {
      // スタック領域の初期化
      Stack = new IValue[2048];
      Bp = 0;
      Sp = 0;

      // 実行箇所の初期化
      Pc = 0;

      if (Intrinsics.Count == 0)
      {
        // Intrinsicがまだ初期化されていないので，初期化する
        Intrinsic.Init();
      }
    }

    public void SetupCode(byte[] codes)
    {
      _codes = codes;
    }

    public (bool, string) AllRun()
    {
      if (_codes?.Length == 0)
      {
        // 何もコードが設定されていない
        return (false, "実行するコードがありません");
      }

      while (true)
      {
        (bool isSuccess, string message) = StepRun();
        if (!string.IsNullOrEmpty(message))
        {
          // エラーメッセージがあるので，強制停止する
          return (false, message);
        }
        if (!isSuccess) break;
      }

      return (true, "");
    }

    public (bool, string) StepRun()
    {
      if(_runIntrinsic != null)
      {
        (RunStatus status, string message) =_runIntrinsic.Action(new ParamContext(this));
        switch(status)
        {
          case RunStatus.Continue:
            {
              return (true, "");
            }
          case RunStatus.End:
            {
              CallInfo info = Stack[Bp] as CallInfo;
              Sp = Bp;
              Bp = info.PreBp;
              Pc = info.PrePc;
              if(info.Return != null || info.Return != NullValue.Null)
              {
                // 戻り値があるなら，先頭に格納する
                Push(info.Return);
              }
              _runIntrinsic = null;
              return (true, "");
            }
          case RunStatus.Error:
            {
              _runIntrinsic = null;
              return (false, message);
            }
        }
      }

      if (Pc < 0 || Pc >= _codes.Length)
      {
        // 実行位置が範囲外
        return (false, "");
      }

      Opcode opcode = (Opcode)_codes[Pc++];

      switch (opcode)
      {
        case Opcode.Let:
          {
            (IValue value, string message) = NewValue();
            if(!string.IsNullOrEmpty(message))
            {
              // Valueを作成するのに失敗しました
              return (false, message);
            }

            if(_callInfo != null)
            {
              // 引数の情報を格納する
              _callInfo.Args.Add(value);
              return (true, "");
            }

            if (!Push(value))
            {
              // 変数を確保に失敗した
              return (false, "スタックに格納することに失敗しました");
            }
            return (true, "");
          }
        case Opcode.SetArgs:
          {
            // 引数を設定するために、スタックではなく関数情報に伝える
            _callInfo = new CallInfo();
            return (true, "");
          }
        case Opcode.Call:
          {
            uint id = GetUint32();

            // 関数の情報をスタックに積む
            if (_callInfo == null) _callInfo = new CallInfo();
            _callInfo.PreBp = Bp;
            _callInfo.PrePc = Pc;
            Push(_callInfo);
            _callInfo = null;
            
            Bp = Sp - 1;

            if (Intrinsics.TryGetValue(id, out Intrinsic val))
            {
              _runIntrinsic = val;
              return (true, "");
            }
            else
            {
              // TODO: 通常の関数呼び出し
            }

            return (false, "指定された関数IDは存在しません");
          }
      }

      return (false, "識別不可能なオペコードに到達しました");
    }

    public bool Push(IValue value)
    {
      if (Math.Clamp(Sp, 0, Stack.Length - 1) != Sp)
      {
        return false;
      }
      Stack[Sp++] = value;
      return true;
    }

    private uint GetUint32()
    {
      uint size = BitConverter.ToUInt32(_codes, (int)Pc);
      Pc += 4;
      return size;
    }

    private Type GetObjType()
    {
      return (Type)_codes[Pc++];
    }

    private (IValue, string) NewValue()
    {
      uint size = GetUint32();
      IValue newValue = NewValueOfPbjType(GetObjType());
      if(!newValue.SetMember(CloneRange(_codes, Pc, size)))
      {
        return (NullValue.Null, "メンバーの設定に失敗しました");
      }
      Pc += size;
      return (newValue, "");
    }

    private byte[] CloneRange(byte[] context, uint start, uint size)
    {
      if (context.Length < start + size)
      {
        return null;
      }

      byte[] result = new byte[size];
      for (int i = 0; i < size; ++i)
      {
        result[i] = context[start + i];
      }

      return result;
    }

    private IValue NewValueOfPbjType(Type type)
    {
      switch (type)
      {
        case Type.Integer: return new Integer(0x00);
      }

      return NullValue.Null;
    }
  }
}

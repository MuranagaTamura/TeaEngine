namespace TeaEngine.Core.Value
{
  public class NullValue : IValue
  {
    public Engine.Type Type => Engine.Type.Null;
    public static IValue Null => new NullValue();

    private NullValue() { }

    public bool CompareTo(IValue value) => false;

    public bool SetMember(byte[] context) => false;
  }
}

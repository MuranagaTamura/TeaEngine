namespace TeaEngine.Core.Value
{
  public interface IValue
  {
    Engine.Type Type { get; }

    bool CompareTo(IValue value);
    bool SetMember(byte[] context);
  }
}

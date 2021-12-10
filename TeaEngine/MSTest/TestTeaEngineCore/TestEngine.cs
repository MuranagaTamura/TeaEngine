using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeaEngine.Core;
using TeaEngine.Core.Value;

[TestClass]
public class TestEngine
{
  [TestMethod]
  public void SmallTest()
  {
    Engine engine = new Engine();
    engine.Init();
    Assert.AreNotEqual((uint)0, Engine.Intrinsics.Count, "初期化に失敗しました");
  }

  [TestMethod]
  public void LetTest()
  {
    // メモリレイアウトは[ size, type, member value ... ]となっている
    // 例えば，Integer(0x10)なら[ 4, (byte)Engine.Type.Uint8, 0x10 ]となっている
    BinaryBuilder builder = new BinaryBuilder();
    builder.LetInterger(1);

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual((uint)0, engine.Sp-1, "スタックに確保されませんでした");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(0x01)), "確保されたが確保された値が違います");
  }

  [TestMethod]
  public void LetTests()
  {
    LetTestsHelper(new int[] { 0x00, 0x00, 0x01, 0x00 });
    LetTestsHelper(new int[] { 0x00, 0x01, 0x02, 0x03 });
    LetTestsHelper(new int[] { 0xff, 0xff, 0xff, 0xff });
  }

  private void LetTestsHelper(int[] letContext)
  {
    BinaryBuilder builder = new BinaryBuilder();
    for(int i = 0; i < letContext.Length; ++i)
    {
      builder.LetInterger(letContext[i]);
    }

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual(0, engine.Sp - letContext.Length, "スタックに確保されませんでした");

    for(int i = 0; i < letContext.Length; ++i)
    {
      Assert.IsTrue(
        engine.Stack[engine.Sp - i - 1]
          .CompareTo(new Integer(letContext[letContext.Length - i - 1])),
        "確保されたが確保された値が違います");
    }
  }

  [TestMethod]
  public void CallAddTest()
  {
    // 引数の格納順は，前からCallInfoに積まれる（第0引数，第1引数，…，第n引数）
    // 戻り値はCallInfo.Retrunに格納されている
    // （基本的に第一引数にselfがあり，第二引数にその関数の引数が並ぶ）
    BinaryBuilder builder = new BinaryBuilder();
    builder.Init();
    builder.SetArgs();
    builder.LetInterger(0x01);
    builder.LetInterger(0x02);
    builder.Call("Uint8.+");

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 1, "スタックに確保されませんでした");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(0x03)), "計算された結果が違います");
  }

  [TestMethod]
  public void CallAddTests()
  {
    CallAddTestsHelper(3, 4, 7);
    CallAddTestsHelper(-1, 1, 0);
  }

  public void CallAddTestsHelper(int a, int b, int result)
  {
    // 基本的に第一引数にselfがあり，第二引数にその関数の引数が並ぶ
    // 戻り値は必ずスタックの先頭に配置される
    BinaryBuilder builder = new BinaryBuilder();
    builder.Init();
    builder.SetArgs();
    builder.LetInterger(b);
    builder.LetInterger(a);
    builder.Call("Uint8.+");

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 1, "スタックに確保されませんでした");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(result)), "計算された結果が違います");
  }

  [TestMethod]
  public void CallCalcTests()
  {
    // 基本的に第一引数にselfがあり，第二引数にその関数の引数が並ぶ
    // 戻り値は必ずスタックの先頭に配置される
    BinaryBuilder builder = new BinaryBuilder();
    builder.Init();
    builder.SetArgs();
    builder.LetInterger(0x05);
    builder.LetInterger(0x01);
    builder.Call("Uint8.-");
    builder.SetArgs();
    builder.LetInterger(0x03);
    builder.LetInterger(0x02);
    builder.Call("Uint8.*");
    builder.SetArgs();
    builder.LetInterger(0x0A);
    builder.LetInterger(0x02);
    builder.Call("Uint8./");

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 3, "スタックに確保されませんでした");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(0x05)), "計算された結果が違います");
    Assert.IsTrue(engine.Stack[engine.Sp - 2].CompareTo(new Integer(0x06)), "計算された結果が違います");
    Assert.IsTrue(engine.Stack[engine.Sp - 3].CompareTo(new Integer(0x04)), "計算された結果が違います");
  }
}


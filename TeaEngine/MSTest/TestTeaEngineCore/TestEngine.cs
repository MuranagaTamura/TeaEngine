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
    // 引数の格納順は，後ろからスタックに積むようにする（第n引数，第n-1引数，...，第1引数）
    // 戻り値は必ずスタックの先頭に配置される（関数リターン先の情報である箇所にスワップされて配置）
    // （基本的に第一引数にselfがあり，第二引数にその関数の引数が並ぶ）
    BinaryBuilder builder = new BinaryBuilder();
    builder.Init();
    builder.LetInterger(0x02);
    builder.LetInterger(0x01);
    builder.Call("Uint8.+");

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 3, "スタックに確保されませんでした");
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
    builder.LetInterger(b);
    builder.LetInterger(a);
    builder.Call("Uint8.+");

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 3, "スタックに確保されませんでした");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(result)), "計算された結果が違います");
  }

  [TestMethod]
  public void CallCalcTests()
  {
    // 基本的に第一引数にselfがあり，第二引数にその関数の引数が並ぶ
    // 戻り値は必ずスタックの先頭に配置される
    BinaryBuilder builder = new BinaryBuilder();
    builder.Init();
    builder.LetInterger(0x01);
    builder.LetInterger(0x05);
    builder.Call("Uint8.-");
    builder.LetInterger(0x02);
    builder.LetInterger(0x03);
    builder.Call("Uint8.*");
    builder.LetInterger(0x02);
    builder.LetInterger(0x0A);
    builder.Call("Uint8./");

    // エンジン側を初期化，設定
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"実行に失敗しました => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 9, "スタックに確保されませんでした");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(0x05)), "計算された結果が違います");
    Assert.IsTrue(engine.Stack[engine.Sp - 4].CompareTo(new Integer(0x06)), "計算された結果が違います");
    Assert.IsTrue(engine.Stack[engine.Sp - 7].CompareTo(new Integer(0x04)), "計算された結果が違います");
  }
}


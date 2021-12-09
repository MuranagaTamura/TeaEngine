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
    Assert.AreNotEqual((uint)0, Engine.Intrinsics.Count, "�������Ɏ��s���܂���");
  }

  [TestMethod]
  public void LetTest()
  {
    // ���������C�A�E�g��[ size, type, member value ... ]�ƂȂ��Ă���
    // �Ⴆ�΁CInteger(0x10)�Ȃ�[ 4, (byte)Engine.Type.Uint8, 0x10 ]�ƂȂ��Ă���
    BinaryBuilder builder = new BinaryBuilder();
    builder.LetInterger(1);

    // �G���W�������������C�ݒ�
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"���s�Ɏ��s���܂��� => {message}");

    Assert.AreEqual((uint)0, engine.Sp-1, "�X�^�b�N�Ɋm�ۂ���܂���ł���");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(0x01)), "�m�ۂ��ꂽ���m�ۂ��ꂽ�l���Ⴂ�܂�");
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

    // �G���W�������������C�ݒ�
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"���s�Ɏ��s���܂��� => {message}");

    Assert.AreEqual(0, engine.Sp - letContext.Length, "�X�^�b�N�Ɋm�ۂ���܂���ł���");

    for(int i = 0; i < letContext.Length; ++i)
    {
      Assert.IsTrue(
        engine.Stack[engine.Sp - i - 1]
          .CompareTo(new Integer(letContext[letContext.Length - i - 1])),
        "�m�ۂ��ꂽ���m�ۂ��ꂽ�l���Ⴂ�܂�");
    }
  }

  [TestMethod]
  public void CallAddTest()
  {
    // �����̊i�[���́C��납��X�^�b�N�ɐςނ悤�ɂ���i��n�����C��n-1�����C...�C��1�����j
    // �߂�l�͕K���X�^�b�N�̐擪�ɔz�u�����i�֐����^�[����̏��ł���ӏ��ɃX���b�v����Ĕz�u�j
    // �i��{�I�ɑ�������self������C�������ɂ��̊֐��̈��������ԁj
    BinaryBuilder builder = new BinaryBuilder();
    builder.Init();
    builder.LetInterger(0x02);
    builder.LetInterger(0x01);
    builder.Call("Uint8.+");

    // �G���W�������������C�ݒ�
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"���s�Ɏ��s���܂��� => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 3, "�X�^�b�N�Ɋm�ۂ���܂���ł���");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(0x03)), "�v�Z���ꂽ���ʂ��Ⴂ�܂�");
  }

  [TestMethod]
  public void CallAddTests()
  {
    CallAddTestsHelper(3, 4, 7);
    CallAddTestsHelper(-1, 1, 0);
  }

  public void CallAddTestsHelper(int a, int b, int result)
  {
    // ��{�I�ɑ�������self������C�������ɂ��̊֐��̈���������
    // �߂�l�͕K���X�^�b�N�̐擪�ɔz�u�����
    BinaryBuilder builder = new BinaryBuilder();
    builder.Init();
    builder.LetInterger(b);
    builder.LetInterger(a);
    builder.Call("Uint8.+");

    // �G���W�������������C�ݒ�
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"���s�Ɏ��s���܂��� => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 3, "�X�^�b�N�Ɋm�ۂ���܂���ł���");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(result)), "�v�Z���ꂽ���ʂ��Ⴂ�܂�");
  }

  [TestMethod]
  public void CallCalcTests()
  {
    // ��{�I�ɑ�������self������C�������ɂ��̊֐��̈���������
    // �߂�l�͕K���X�^�b�N�̐擪�ɔz�u�����
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

    // �G���W�������������C�ݒ�
    Engine engine = new Engine();
    engine.Init();
    engine.SetupCode(builder.GenerateCodes());
    (bool hasError, string message) = engine.AllRun();
    Assert.IsTrue(hasError, $"���s�Ɏ��s���܂��� => {message}");

    Assert.AreEqual((uint)0, engine.Sp - 9, "�X�^�b�N�Ɋm�ۂ���܂���ł���");
    Assert.IsTrue(engine.Stack[engine.Sp - 1].CompareTo(new Integer(0x05)), "�v�Z���ꂽ���ʂ��Ⴂ�܂�");
    Assert.IsTrue(engine.Stack[engine.Sp - 4].CompareTo(new Integer(0x06)), "�v�Z���ꂽ���ʂ��Ⴂ�܂�");
    Assert.IsTrue(engine.Stack[engine.Sp - 7].CompareTo(new Integer(0x04)), "�v�Z���ꂽ���ʂ��Ⴂ�܂�");
  }
}


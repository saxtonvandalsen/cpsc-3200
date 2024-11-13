namespace DurableStreamTest;

using P3;

[TestClass]
public class DurableStreamTest
{
    private const string TestFilePath = "durablestreamtestfile.txt";
    private DurableStream durableStream;

    [TestInitialize]
    public void Setup()
    {
        durableStream = new DurableStream(10, TestFilePath);
    }

    [TestCleanup]
    public void Cleanup()
    {
        durableStream.Dispose();
        if (File.Exists(TestFilePath))
        {
            File.Delete(TestFilePath);
        }
    }

    [TestMethod]
    public void ValidConstructor_ValidCapacity_FilePath()
    {
        Assert.IsNotNull(durableStream);
    }

    [TestMethod]
    public void AppendMessage_WritesMessageToFile()
    {
        durableStream.AppendMessage("Test Message");
        durableStream.AppendMessage("Test Message 2");
        durableStream.AppendMessage("Test Message 3");
        durableStream.Dispose();

        var fileContents = File.ReadAllText(TestFilePath);
        Assert.IsTrue(fileContents.Contains("Test Message"));
    }

    [TestMethod]
    public void SyncMessagesFromFile_LoadsMessagesOnInit()
    {
        File.WriteAllText(TestFilePath, "Persisted Message\n");
        durableStream = new DurableStream(10, TestFilePath);

        Assert.AreEqual(1, durableStream.GetMessageCount());
        Assert.AreEqual("Persisted Message", durableStream.ReadMessages(0, 1)[0]);
    }

    [TestMethod]
    public void DeepCopy_CreatesExactCopyOfStream()
    {
        durableStream.AppendMessage("Message 1");
        durableStream.AppendMessage("Message 2");

        DurableStream copy = (DurableStream)durableStream.DeepCopy();

        Assert.AreEqual(durableStream.GetMessageCount(), copy.GetMessageCount());
        for (int i = 0; i < durableStream.GetMessageCount(); i++)
        {
            Assert.AreEqual(durableStream.ReadMessages(i, i + 1)[0], copy.ReadMessages(i, i + 1)[0]);
        }
    }

    [TestMethod]
    public void Dispose_ClosesFileStreams()
    {
        durableStream.Dispose();
        try
        {
            durableStream.AppendMessage("Test After Dispose");
            Assert.Fail("Expected ObjectDisposedException");
        }
        catch (ObjectDisposedException)
        {
            Assert.IsTrue(true);
        }
    }
    
    [TestMethod]
    public void AppendMessage_ExceedsCapacity()
    {
        for (int i = 0; i < 10; i++)
        {
            durableStream.AppendMessage($"Message {i}");
        }
        Assert.ThrowsException<InvalidOperationException>(() => durableStream.AppendMessage("Overflow Message"));
    }

    [TestMethod]
    public void AppendMessage_AfterDispose_ThrowsException()
    {
        durableStream.Dispose();
    
        Assert.ThrowsException<ObjectDisposedException>(() => durableStream.AppendMessage("Message After Dispose"));
    }

    [TestMethod]
    public void DeepCopy_AfterDispose_ThrowsException()
    {
        durableStream.Dispose();
        Assert.ThrowsException<ObjectDisposedException>(() => durableStream.DeepCopy());
    }
    
    [TestMethod]
    public void Dispose_MultipleCalls_DoesNotThrowException()
    {
        durableStream.Dispose();
        durableStream.Dispose();
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void SyncMessagesFromFile_HandlesEmptyFileGracefully()
    {
        File.WriteAllText(TestFilePath, string.Empty);
        durableStream = new DurableStream(10, TestFilePath);

        Assert.AreEqual(0, durableStream.GetMessageCount());
    }
}
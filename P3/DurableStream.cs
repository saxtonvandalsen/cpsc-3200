// Saxton Van Dalsen
// 10/27/2024

using System;
using Systems.Collections.Generic;
using System.IO;
using System.Linq;

namespace DurablStreamLibrary
{
    public class DurableStream : MsgStream
{
    private string filePath;
    private FileStream fileStream;
    private BufferedStream bufferedStream;
    private int capacity;

    private void SyncMessagesFromFile()
    {
        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (IsValidMessage(line))
                {
                    AppendMessage(line);
                }    
            }    
        }    
    }

    private void WriteMessageToFile(string message)
    {
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message + Environment.NewLine);
        bufferedStream.Write(messageBytes, 0, messageBytes.Length);
    }
    
    public DurableStream(int capacity, string filePath) : base(capacity)
    {
        this.filePath = filePath;

        if (File.Exists(filePath))
        {
            SyncMessagesFromFile();
        }

        fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
        bufferedStream = new BufferedStream(fileStream);
    }

    public override void AppendMessage(string message)
    {
        base.AppendMessage(message);

        WriteMessageToFile(message);
    }

    public void Reset()
    {
        base.Reset();
        File.WriteAllText((filePath), string.Empty);
    }

    public void Dispose()
    {
        bufferedStream.Close();
        fileStream.Close();
    }

    public override MsgStream DeepCopy()
    {
        DurableStream copy = new DurableStream(this.capacity, this.filePath);
        for (int i = 0; i < this.GetMessageCount(); i++)
        {
            copy.AppendMessage(this.ReadMessages(i, i +1)[0]);
        }
        return copy;
    }
    // Implementation invariant:
    //
}
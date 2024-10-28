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
        // Class invariant:
        // Client must know this class is based on MsgStream and extends its functionality to include file I/O, ensuring 
        // messages are both stored in memory and written to a backing file efficiently
        // The buffered stream and file stream must be properly initialized or closed when Dispose() is called
        // The filePath must not be null or empty
        // The capacity must be greater than 0
        // The initialState list must accurately reflect the messages that have been synced from the backing file

        private string filePath;
        private FileStream fileStream;
        private BufferedStream bufferedStream;
        private List<string> initialState;
        private int capacity;
        private int appendCounter = 0;
        private int writeThreshold = 3;
        private bool disposed = false;

        // Preconditions:
        // - fil path must be a valid and accessible path
        // - The file at filePath must be readable
        // Postconditions:
        // - The initialState list is populated with messages from the file, if it exists
        private void SyncMessagesFromFile()
        {
            initialState = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (IsValidMessage(line))
                    {
                        base.AppendMessage(line);
                        initialState.Add(line);
                    }    
                }    
            }    
        }

        // Preconditions:
        // - message must not be null or empty.
        // Postconditions:
        // - The message is converted to byte data, appended to a new line and writes to underlying file
        private void WriteMessageToFile(List<string> messages)
        {
            foreach (var message in messages)
            {
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message + Environment.NewLine);
                bufferedStream.Write(messageBytes, 0, messageBytes.Length);
            }
            bufferedStream.Flush();
        }

        // Preconditions:
        // - The file path provided should be valid and accessible for reading and writing.
        // Postconditions:
        // - After initialization, the in-memory message list is populated if the backing file exists.
        // - Messages appended through AppendMessage() are written to the backing file.
        public DurableStream(int capacity, string filePath) : base(capacity)
        {
            this.filePath = filePath;
            this.capacity = capacity;
            initialState = new List<string>();

            if (File.Exists(filePath))
            {
                SyncMessagesFromFile();
            }

            fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            bufferedStream = new BufferedStream(fileStream);
        }

        // Preconditions:
        // - message must not be null or empty
        // Postconditions:
        // - The message is appended to the in-memory list of messages
        // - The message is written to the backing file
        public override void AppendMessage(string message)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("DurableStream");
            }    
            
            if (!IsValidMessage(message))
            {
                throw new ArgumentException("Invalid message: message is either null, empty, or exceeds the maximum allowed length.");
            }
            
            base.AppendMessage(message);
            appendCounter++;

            if (appendCounter >= writeThreshold)
            {
                WriteMessageToFile(new List<string> { message });
                appendCounter = 0;
            }
        }

        // Preconditions:
        // - The stream must have been initialized and should contain messages
        // Postconditions:
        // - The stream is reset, and the initial messages are restored from initialState
        // - The backing file is updated to reflect the restored messages
        public override void Reset()
        {
            if (disposed) throw new ObjectDisposedException("DurableStream");
            
            base.Reset();

            foreach (var msg in initialState)
            {
                base.AppendMessage(msg);
            }
            
            using (StreamWriter writer = new StreamWriter(filePath, false)) // Overwrite the file
            {
                foreach (string msg in initialState)
                {
                    writer.WriteLine(msg);
                }
            }
        }

        // Preconditions:
        // - The streams must be open and initialized
        // Postconditions:
        // - The buffered stream and file stream are closed, releasing any resources
        public void Dispose()
        {
            if (!disposed)
            {
                bufferedStream.Close();
                fileStream.Close();
            }    
            disposed = true;
        }

        // Preconditions:
        // - The current instance must contain messages
        // Postconditions:
        // - A deep copy of the DurableStream instance is created, including the associated file path
        public override MsgStream DeepCopy()
        {
            if (disposed) throw new ObjectDisposedException("DurableStream");
            
            DurableStream copy = new DurableStream(this.capacity, this.filePath);
            for (int i = 0; i < this.GetMessageCount(); i++)
            {
                copy.AppendMessage(this.ReadMessages(i, i + 1)[0]);
            }
            return copy;
        }
        // Implementation invariant:
        // - The capacity must be greater than 0, ensuring the DurableStream can hold messages
        // - The filePath must be a valid and accessible path for reading and writing operations
        // - The bufferedStream must be correctly initialized, and messages must be written to it efficiently 
        //   without immediate disk writes on every append, minimizing the cost of file I/O
        // - The initialState list must accurately reflect the messages that have been synced from the backing file, 
        //   allowing for consistent behavior upon reset and deep copy operations
    }
}
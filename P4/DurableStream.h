// Saxton Van Dalsen
// 11/14/2024

#ifndef DURABLESTREAM_H
#define DURABLESTREAM_H

#include "MsgStream.h"
#include <memory>
#include <string>
#include <fstream>
#include <stdexcept>

using namespace std;

class DurableStream : public MsgStream
{
    // Class invariant:
    // - DurableStream extends MsgStream with additional file I/O to persist messages both in memory and on disk.
    // - The filePath must be a valid, non-empty path that specifies where messages are stored.
    // - The capacity must be greater than 0, setting a limit on the number of messages stored in memory and on file.
    // - The WRITE_THRESHOLD determines the frequency of file writes to balance efficiency with data durability.
    // - The initialState accurately reflects the messages synced from the file, allowing reset operations to restore this state.

    private:
        const int WRITE_THRESHOLD = 3;

        string filePath;
        ofstream outFile;
        ifstream inFile;
        unique_ptr<string[]> initialState;
        int capacity;
        int appendCounter;
        bool disposed;

        DurableStream(const DurableStream& other);
        DurableStream& operator=(const DurableStream& other);
        DurableStream(DurableStream&& other) noexcept;
        DurableStream& operator=(DurableStream& other) noexcept;     
        void syncMessages();
        void writeMessageToFile(unique_ptr<string[]> messages);
        unique_ptr<string[]> getLastMessages(int count) const;

    public:
        DurableStream(int capacity, const string& filePath);
        void appendMessage(const string& message) override;
        unique_ptr<string[]> readMessages(int startRange, int endRange) override;
        void reset() override;

    // Implementation invariant:
    // - The capacity must remain above 0, ensuring DurableStream has space for message storage.
    // - The filePath must point to a readable and writable file, used consistently for data synchronization.
    // - The WRITE_THRESHOLD ensures that the file isn’t written on each append, optimizing I/O performance.
    // - The initialState holds the initial file-synced messages, supporting consistent reset behavior and enabling accurate deep copies.
};

#endif
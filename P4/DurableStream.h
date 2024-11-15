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
    // - DurableStream extends reading to both in-memory messages and any that is maintained in the backing file.
    // - Resetting will restore both in-memory and file messages to the original state of when the object was first created.

    private:
        const int WRITE_THRESHOLD = 3;

        string filePath;
        ofstream outFile;
        ifstream inFile;
        unique_ptr<string[]> initialState;
        int capacity;
        int appendCounter;

        // Preconditions:
        // - other must be a valid DurableStream object with initialized members.
        // - other must have a valid file path and in-memory storage state to copy from.
        // Postconditions:
        // - Current object is deeply copied with its own unique in-memory messages and file path.
        // - All applicable and current resources are copied over to the target object.
        DurableStream(const DurableStream& other);

        // Preconditions:
        // - other must be a valid DurableStream object with initialized members.
        // - other must have a valid file path and in-memory storage state to copy from.
        // Postconditions:
        // - Checks for self assignment initially; if true, returns *this unchanged.
        // - The current DurableStream object is deeply copied from "other" with its own unique in-memory message storage and file path.
        // - "messages", "initialState", and "filePath" are all copied to ensure the new object has an independent, consistent state.
        // - Any previously stored data in the current object is replaced by the other objects data, with exclusive ownership of all resources.
        DurableStream& operator=(const DurableStream& other);

        // Preconditions:
        // - other must be a valid DurableStream object ready to transfer ownership.
        // Postconditions:
        // - Transfers ownership of the source object to target object including base class resources (MsgStream).
        // - Leaving other in a valid but empty state with its resources nullified or reset
        DurableStream(DurableStream&& other) noexcept;

        // Preconditions:
        // - other must be a valid DurableStream object ready to transfer ownership
        // Postconditions:
        // - Checks for self assignment initially; if true, returns *this unchanged.
        // - Transfers all resources from other to target object including base class resources (MsgStream).
        // - Returns *this to allow for chaining of move assignment operations.
        DurableStream& operator=(DurableStream&& other) noexcept;

        // Preconditions:
        // - filePath must be a valid and readable file path.
        // - object and capacity must have valid state.
        // Postconditions:
        // - Synchronizes messages from filePath into in-memory message storage
        // - Populates initialState array with synced messages to preserve original state for reset
        void syncMessages();

        // Preconditions:
        // - messages must be valid containing at least the write threshold messages
        // Postconditions:
        // - The messagess are appeneded to the file.
        void writeMessageToFile(unique_ptr<string[]> messages);
        unique_ptr<string[]> getLastMessages(int count) const;
        bool isValidFilePath(const string& file) const;

    public:
        // Preconditions:
        // - capacity must be greater than 0 to allocate space for message storage.
        // - filePath must be a valid, non-empty string, pointing to a readable and writable file location.
        // Postconditions:
        // - Instantiated with initialized in-memory message storage and an associated file for persistence.
        // - If the file at file path exists, its contents are synced to the in-memory storage.
        // - The initialState is set to match the original file content, supporting reset functionality.
        DurableStream(int capacity, const string& filePath);

        // Preconditions:
        // - The message must be a valid string and pass vaild message check.
        // - Stream must not be full and operation limit must not be reached.
        // Postconditions:
        // - message is appended to in-memory storage if valid
        // - if the write threshold is reached, those messages are written to the file, and append counter is reset.
        void appendMessage(const string& message) override;

        // Preconditions:
        // - start and end range must be a valid range with current message count.
        // - filePath must be accessible if syncing is required.
        // Postconditions:
        // - in-memory storage is up-to-date by syncing messages from the file.
        // - Returns a pointer containing messages within the specified range from in-memory storage.
        unique_ptr<string[]> readMessages(int startRange, int endRange) override;

        // Preconditions:
        // - initialState must contain original messages from the file or be initialized.
        // - filePath must be valid and writable.
        // Postconditions:
        // - in-memory messages are cleared and restored to initialState.
        // - filePath is cleared and rewritten; append counter is reset
        void reset() override;

    // Implementation invariant:
    // - DurableStream leverages MsgStream for core message storage and management.
    // - The capacity must remain above 0, ensuring DurableStream has space for message storage.
    // - The filePath must point to a readable and writable file, used consistently for data synchronization.
    // - The WRITE_THRESHOLD ensures that the file isn’t written on each append, optimizing I/O performance.
    // - The initialState holds the initial file-synced messages, supporting consistent reset behavior and enabling accurate deep copies.
    // - Copy and move operations have been suppressed to ensure each DurableStream instance is uniquely owned and manages its
    //   own file, maintaining data integrity, and avoiding resource contention.
    // - unique_ptr<string[]> messages provides exclusive ownership of in-memory messages to ensure safe and automatic memory management.
};

#endif
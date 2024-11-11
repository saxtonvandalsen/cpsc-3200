// Saxton Van Dalsen
// 11/14/2024

#include "DurableStream.h"
#include "MsgStream.h"

#include <memory>
#include <string>
#include <fstream>
#include <stdexcept>

using namespace std;

DurableStream::DurableStream(int capacity, const string& filePath) : appendCounter(0), MsgStream(capacity)
{
    ifstream inFile(filePath);
    if (inFile.is_open())
    {
        string line;
        while (getline(inFile, line))
        {
            this->appendMessage(line);
        }
        inFile.close();
    }

    outFile.open(filePath, ios::app);
    if (!outFile.is_open())
    {
        throw runtime_error("Failed to open file for writing.");
    }
}

void DurableStream::appendMessage(const string& message)
{
    if (isFull())
        throw runtime_error("Capacity has been reached.");

    if (operationLimit())
        throw runtime_error("Operation limit has been reached.");

    if (!isValidMessage(message))
        throw runtime_error("Invalid message.");

    MsgStream::appendMessage(message);
    appendCounter++;

    if (appendCounter >= WRITE_THRESHOLD)
    {
        unique_ptr<string[]> messagesToWrite = getLastMessages(WRITE_THRESHOLD);
        writeMessageToFile(move(messagesToWrite));
        appendCounter = 0;
    }
}

unique_ptr<string[]> DurableStream::readMessages(int startRange, int endRange)
{
    syncMessages();

    return MsgStream::readMessages(startRange, endRange);
}

void DurableStream::reset()
{
    // step 1: clear in-memory messages
    messages = make_unique<string[]>(capacity);
    messageCount = 0;

    //step 2: reload messages from initialState
    for (int i = 0; i < capacity && !initialState[i].empty(); i++)
    {
        messages[i] = initialState[i];
        messageCount++;
    }

    // step 3: clear and rewrite the file
    ofstream outFile(filePath, ios::trunc); // truncate to clear file
    if (outFile.is_open())
    {
        for (int i = 0; i < messageCount; i++)
        {
            outFile << messages[i] << endl;
        }
        outFile.close();
    }

    appendCounter = 0;
}

void DurableStream::syncMessages()
{
    initialState = make_unique<string[]>(capacity);

    ifstream inFile(filePath);
    if (inFile.is_open())
    {
        string line;
        int index = 0;
        while (getline(inFile, line) && index < capacity)
        {
            if (isValidMessage(line))
            {
                MsgStream::appendMessage(line);
                initialState[index++] = line;
            }
        }
        inFile.close();
    }
}

void DurableStream::writeMessageToFile(unique_ptr<string[]> messages)
{
    for (int i = 0; i < WRITE_THRESHOLD; i++)
    {
        outFile << messages[i] << endl;
    }
}

unique_ptr<string[]> DurableStream::getLastMessages(int count) const
{
    unique_ptr<string[]> messages = make_unique<string[]>(count);
    int startIndex = messageCount - count;
    for (int i = 0; i < count; i++)
    {
        messages[i] = this->messages[startIndex + i];
    }
    return messages;
}
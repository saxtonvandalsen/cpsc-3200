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

bool DurableStream::isValidMessage(const string& message) const
{
    return message != "" && message.length() <= MAX_STRING_LENGTH;
}
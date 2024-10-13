// Saxton Van Dalsen
// 10/15/2024

#include "MsgStream.h"
#include <string>

using namespace std;

MsgStream::MsgStream(int initialCapacity) : messageCount(0), operationCount(0)
{
    capacity = validateCapacity(initialCapacity);
    
    messages = new char*[capacity];
    for (int i = 0; i < capacity; i++)
        messages[i] = nullptr;
}

MsgStream::~MsgStream()
{
    for (int i = 0; i < messageCount; i++)
        delete[] messages[i];

    delete[] messages;
}

int MsgStream::validateCapacity(int cap)
{
    if (cap > MAX_CAPACITY)
    {
        cap = MAX_CAPACITY;
        throw invalid_argument("Capacity must not exceed 200. Setting to max capacity.");
    }
    if (cap <= 0) {
        cap = 1;
        throw invalid_argument("Capacity must be greater than zero. Setting to minimum capacity of 1.");
    }
    return cap;
}
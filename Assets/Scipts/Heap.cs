using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    
    T[] items; // Array to store heap items
    int currentItemCount; // Tracks the current number of items in the heap

    public Heap(int maxHeapSize) // Constructor that initializes the heap with a maximum size
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item) // Adds a new item to the heap and ensures the heap property is maintained
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst() // Removes and returns the first (highest-priority) item from the heap
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    
    public void UpdateItem(T item) // Updates the position of an item in the heap if its priority changes
    {
        SortUp(item);
    }
    
    public int Count // Returns the count of items currently in the heap
    {
        get
        {
            return currentItemCount;
        }
    }
    
    public bool Contains(T item) // Checks if the heap contains a specific item
    {
        return Equals(items[item.HeapIndex], item); // Compares the item at the specified index with the given item
    }

    void SortDown(T item) // Rearranges the heap from top to bottom to maintain the heap property after removal
    {
        while (true)
        {
            // Calculate the indices of the left and right child nodes
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // If the left child exists...
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                // If the right child also exists, compare left and right children
                if (childIndexRight < currentItemCount)
                {
                    // Use the child with the higher priority (greater value)
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                // If the item is smaller than the larger child, swap them
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
  
    void SortUp(T item) // Rearranges the heap from bottom to top to maintain the heap property after addition or update
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            // If the item has higher priority than its parent, swap them
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            // Update the parent index and continue
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
 
    void Swap(T itemA, T itemB) // Swaps two items in the heap and updates their indices
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T> // An interface for items that can be stored in the heap
{
    int HeapIndex
    {
        get;
        set;
    }
}
﻿using System;
using System.Buffers;

namespace Sera.Utils;

/// <summary>It is safe to leak array allocations</summary>
public class DirectAllocationArrayPool<T> : ArrayPool<T>
{
    public static DirectAllocationArrayPool<T> Instance { get; } = new();

    public override T[] Rent(int minimumLength) => new T[minimumLength];

    public override void Return(T[] array, bool clearArray = false)
    {
        if (clearArray) Array.Clear(array);
    }
}
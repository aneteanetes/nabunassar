﻿using Nabunassar.Entities;

namespace Nabunassar
{
    internal interface IClonable<T>
    {
        T Clone();
    }
}

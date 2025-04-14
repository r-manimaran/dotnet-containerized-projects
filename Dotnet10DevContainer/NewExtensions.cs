using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Dotnet10DevContainer;

public static class NewExtensions
{
    // extension(IEnumerable<int> source)
    // {
    //     public IEnumerable<int> WhereGreaterThan(int threshold)
    //         => source.Where(x=>x > threshold);

    //     public bool IsEmpty => source.Any();
    // }

    extension<T>(IEnumerable<T> source) where T: INumber<T>
    {
        // Extension Method
        public IEnumerable<T> WhereGreaterThan(T threshold)
            => source.Where(x=> x > threshold);

        // Extension Property
        public bool IsEmpty => !source.Any();
    }

    extension(List<int>)
    {
        public static List<int> Create() =>[];
    }
    
}

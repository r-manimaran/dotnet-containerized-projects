
using Dotnet10DevContainer;

Console.WriteLine("New Way of Extensions Functions in .NET 10");
Console.WriteLine("Target Framework: " + typeof(object).Assembly.ImageRuntimeVersion);

List<int> numbers = [1,2,3,4,5,6,7,8,9,10];
int search = 5;

var greater = numbers.WhereGreaterThan(search);

Console.WriteLine($"Number of number greater than {search} is {greater.Count()}");

Console.WriteLine(greater.IsEmpty);

var newList = List<int>.Create();

Console.WriteLine(newList.IsEmpty);

// Old Approach of defining Extension methods
/*
public static class Extensions
{
    public static IEnumerable<int> WhereGreaterThan(this IEnumerable<int> source, int threshold)
      => source.Where(x=> x > threshold);
}*/


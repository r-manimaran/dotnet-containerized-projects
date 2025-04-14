using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet10DevContainer;

public class Customer
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public class UpdateCustomer
{
    public static void UpdateAge(Customer? customer, int newAge)
    {
        customer?.Age = newAge;
    }

    // No need to check like below
    // public static void UpdateAge(Customer? customer, int newAge)
    // {
    //     if(customer is not null)
    //     {
    //         customer.Age = newAge;
    //     }
    // }
}
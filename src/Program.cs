namespace DynamicTypeLinqQuery;

using System;
using System.Collections.Generic;
using System.Linq;
using DynamicTypeLinqQuery.Data;
using DynamicTypeLinqQuery.Expressions;

class Program {
    static void Main(string[] _) {
        // Array of concrete types assigned to dynamic enumerable
        IEnumerable<dynamic> enumerable = [
            new Person { BirthDate = new DateTime(1975, 8, 14), FirstName = "Alan",  LastName = "Smith" },
            new Person { BirthDate = new DateTime(2006, 1, 26), FirstName = "Elisa",  LastName = "Ridley" },
            new Person { BirthDate = new DateTime(1993, 12, 1), FirstName = "Randy",  LastName = "Knowles" },
            new Person { BirthDate = new DateTime(1946, 5, 8), FirstName = "Melissa",  LastName = "Fincher" }
        ];


        IQueryable<dynamic> queryable = enumerable
            .AsQueryable();

        // Property access on the dynamic object
        var youngsters = enumerable
            .Where(m => m.Age < 21);

        Write("People below 21y to test property access:", youngsters);

        // Index access on the dynamic object
        var adults = enumerable
            .Where(m => m["Age"] >= 21 && m["Age"] <= 59);

        Write("People between 21y and 59y to test index access:", adults);

        // Composed lambda expression for queryable (be aware it is not going to work with EF)
        var seniors = queryable
            .Where(ExpressionFactory.PropertyGreaterThanPredicate("Age", 59));

        Write("People older than 59y to test property access lambda expression:", seniors);

        // Composed lambda expression for queryable (be aware it is not going to work with EF)
        var under60 = queryable
            .Where(ExpressionFactory.IndexLessThanPredicate("Age", 60));

        Write("People younger than 60y to test index access lambda expression:", under60);

        Console.ReadKey();

        // Helper method to write data to console
        static void Write(string header, IEnumerable<dynamic> collection) {
            Console
                .WriteLine(header);

            foreach (var item in collection) {
                Console
                    .WriteLine($"{item.FullName} was born {item.BirthDate.ToShortDateString()}");
            }

            Console
                .WriteLine();
        }
    }
}

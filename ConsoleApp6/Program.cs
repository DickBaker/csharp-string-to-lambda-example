using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ConsoleApp6;

static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Roslyn approach:");
        const string discountFilter = "album => album.Quantity > 100";
        ScriptOptions options = ScriptOptions.Default.AddReferences(typeof(Album).Assembly);
        Func<Album, bool> discountFilterExpression = await CSharpScript.EvaluateAsync<Func<Album, bool>>(discountFilter, options);

        var albums = new List<Album>
        {
            new() { Quantity = 10, Artist = "Betontod", Title = "Revolution" },
            new() { Quantity = 50, Artist = "The Dangerous Summer", Title = "The Dangerous Summer" },
            new() { Quantity = 200, Artist = "Depeche Mode", Title = "Spirit" },
        };

        foreach (Album album in albums.Where(discountFilterExpression))
        {
            Console.WriteLine($"Discounted album: {album.Artist,-20} - {album.Title}");
        }

        Console.WriteLine("\n****\n");

        // approach with manual expression building
        Console.WriteLine("Manual expression building approach:");
        ParameterExpression parameter = Expression.Parameter(
            typeof(Album), "album");
        BinaryExpression comparison = Expression.GreaterThan(Expression.Property(
            parameter,
            typeof(Album)
                .GetProperty(nameof(Album.Quantity))!),
            Expression.Constant(100));
        Func<Album, bool> lambda = Expression.Lambda<Func<Album, bool>>(
            comparison, parameter).Compile();
        foreach (Album album in albums.Where(lambda))
        {
            Console.WriteLine($"Discounted album: {album.Artist,-20} - {album.Title}");
        }

        _ = Console.ReadKey();
    }
}

public class Album
{
    public required int Quantity { get; set; }
    public required string Title { get; set; }
    public required string Artist { get; set; }
}

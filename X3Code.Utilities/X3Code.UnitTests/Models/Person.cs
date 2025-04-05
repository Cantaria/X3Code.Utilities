using System;

namespace X3Code.UnitTests.Models;

internal class Person
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime BirthDate { get; set; }
    public int Age { get; set; }
    public bool IsEvil { get; set; }
    public char CheckMark { get; set; }
    public decimal SomeDecimal { get; set; }
    public float SomeFloat { get; set; }
    public double SomeDouble { get; set; }

    #region Dummy Data

    public static Person Michael => new()
    {
        Name = "Michael",
        LastName = "Scott",
        Age = 25,
        Email = "michael.scott@xyz.com",
        BirthDate = new DateTime(1985, 1, 11)
    };

    public static Person Viktoria => new()
    {
        Name = "Viktoria",
        LastName = "Scott",
        Age = 32,
        Email = "viktoria.scott@xyz.com",
        BirthDate = new DateTime(1980, 7, 16)
    };

    public static Person John => new()
    {
        Name = "John",
        LastName = "Doe",
        Age = 35,
        Email = "john.doe@xyz.com",
        BirthDate = new DateTime(1975, 8, 24)
    };

    public static Person Alexander => new()
    {
        Name = "Alexander",
        LastName = "Doe",
        Age = 27,
        Email = "alexander.doe@xyz.com",
        BirthDate = new DateTime(1987, 3, 22)
    };

    public static Person Clemens => new()
    {
        Name = "Clemens",
        LastName = "Skywalker",
        Age = 29,
        Email = "clemens.skywalker@xyz.com",
        BirthDate = new DateTime(1991, 5, 15)
    };

    #endregion
}
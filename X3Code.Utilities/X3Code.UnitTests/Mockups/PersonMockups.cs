using System;
using X3Code.UnitTests.Models;

namespace X3Code.UnitTests.Mockups;

internal static class PersonMockups
{
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
}
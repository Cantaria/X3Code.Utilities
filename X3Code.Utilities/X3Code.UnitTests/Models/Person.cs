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
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions
{
    // ReSharper disable once InconsistentNaming
    public class DataTableExtension_specs
    {
        public IEnumerable<Spaceship> GenerateMixedTestData()
        {
            var result = new List<Spaceship>();
            for (var i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    var ship = CreateSpaceShip(i);
                    result.Add(ship);
                }
                else
                {
                    var deathStar = CreateDeathStar(i);
                    result.Add(deathStar);
                }
            }

            return result;
        }

        public IEnumerable<Deathstar> GenerateDeathStars()
        {
            var result = new List<Deathstar>();

            for (var i = 0; i < 10; i++)
            {
                var deathStar = CreateDeathStar(i);
                result.Add(deathStar);
            }

            return result;
        }

        private Deathstar CreateDeathStar(int i)
        {
            DateTime? date;
            if (i % 2 == 0)
            {
                date = null;
            }
            else
            {
                date = new DateTime(2340, 7, 30);
            }

            return new Deathstar("Dagobah")
            {
                BuildCosts = 1500.50M,
                BuildDate = new DateTime(2340, 9, 4),
                CanBeDestroyedBySkywalker = (i % 2 == 0),
                Firepower = i * 1000,
                Name = $"Ship Nr. {i}",
                ShipId = Guid.NewGuid(),
                WeaponCount = i * 25,
                LifeTimeEnd = date
            };
        }

        private Spaceship CreateSpaceShip(int i)
        {
            DateTime? date;
            if (i % 2 == 0)
            {
                date = null;
            }
            else
            {
                date = new DateTime(2340, 7, 30);
            }
            return new Spaceship("tatooine")
            {
                BuildDate = new DateTime(2340, 9, 4),
                CanBeDestroyedBySkywalker = (i % 2 == 0),
                Name = $"Ship Nr. {i}",
                ShipId = Guid.NewGuid(),
                LifeTimeEnd = date
            };
        }

        private DataRow GetFirstRowWithColumnContains(DataRowCollection rows, string columnName, string key)
        {
            foreach (DataRow row in rows)
            {
                if (row[columnName].ToString() == key)
                {
                    return row;
                }
            }

            return null;
        }

        [Fact]
        public void MixedListShouldContainAllRows()
        {
            var referenceData = GenerateMixedTestData();
            var asDataTable = referenceData.ToDataTable();
            Assert.Equal(10, asDataTable.Rows.Count);
        }

        [Fact]
        public void DataTableShouldContainAllRows()
        {
            var referenceData = GenerateDeathStars();
            var asDataTable = referenceData.ToDataTable();
            Assert.Equal(10, asDataTable.Rows.Count);
        }

        [Fact]
        public void DataTableShouldHaveAllcolumns()
        {
            var referenceData = GenerateDeathStars();
            var asDataTable = referenceData.ToDataTable();
            Assert.Equal(8, asDataTable.Columns.Count);
        }

        [Fact]
        public void ShouldNotContainTheShipYardName()
        {
            var referenceData = GenerateDeathStars();
            var asDataTable = referenceData.ToDataTable();

            var columns = asDataTable.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToArray();

            Assert.DoesNotContain("ShipyardName", columns);
        }

        [Theory]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(175)]
        public void ShouldContainShipsWithSpeificWeaponCounts(int weaponcount)
        {
            var referenceData = GenerateDeathStars();
            var asDataTable = referenceData.ToDataTable();

            Assert.NotNull(GetFirstRowWithColumnContains(asDataTable.Rows, "WeaponCount", weaponcount.ToString()));
        }

        [Fact]
        public void ChouldContain5ShipsWithoutLifeEnd()
        {
            var referenceData = GenerateDeathStars();
            var asDataTable = referenceData.ToDataTable();

            var count = 0;
            foreach (DataRow row in asDataTable.Rows)
            {
                if (row["LifeTimeEnd"] == DBNull.Value)
                {
                    count++;
                }
            }

            Assert.Equal(5, count);
        }

        [Fact]
        public void ChouldContain5ShipsWithLifeEnd()
        {
            var referenceData = GenerateDeathStars();
            var asDataTable = referenceData.ToDataTable();

            var count = 0;
            foreach (DataRow row in asDataTable.Rows)
            {
                if (row["LifeTimeEnd"] != DBNull.Value)
                {
                    count++;
                }
            }

            Assert.Equal(5, count);
        }

        [Fact]
        public void ChouldContain5ShipsWhichCanBedestroyedBySkywalker()
        {
            var referenceData = GenerateDeathStars();
            var asDataTable = referenceData.ToDataTable();

            var count = 0;
            foreach (DataRow row in asDataTable.Rows)
            {
                if ((bool)row["CanBeDestroyedBySkywalker"])
                {
                    count++;
                }
            }

            Assert.Equal(5, count);
        }

        [Fact]
        public void ChouldContainAShipsWhithSpecificId()
        {
            var referenceData = GenerateDeathStars().ToList();
            var asDataTable = referenceData.ToDataTable();

            var contains = false;
            foreach (DataRow row in asDataTable.Rows)
            {
                if ((Guid)row["ShipId"] == referenceData[5].ShipId)
                {
                    contains = true;
                }
            }

            Assert.True(contains);
        }
        
        [Fact]
        public void CanConvertWithTableName()
        {
            const string Name = "MyDataTable";
            var referenceData = GenerateDeathStars().ToList();
            var asDataTable = referenceData.ToDataTable(Name);

            Assert.Equal(Name, asDataTable.TableName);
        }

        [Theory]
        [InlineData("Name", typeof(string))]
        [InlineData("BuildDate", typeof(DateTime))]
        [InlineData("CanBeDestroyedBySkywalker", typeof(bool))]
        [InlineData("ShipId", typeof(Guid))]
        [InlineData("WeaponCount", typeof(int))]
        [InlineData("Firepower", typeof(long))]
        [InlineData("BuildCosts", typeof(decimal))]
        public void ShouldHaveaColumnASpecificType(string columnname, Type columntype)
        {
            var referenceData = GenerateDeathStars().ToList();
            var asDataTable = referenceData.ToDataTable();

            var column = asDataTable.Columns[columnname];
            Assert.Equal(columntype, column.DataType);
        }
    }

    #region Testclasses

    //A long time ago in a galaxy far, far away....
    public class Spaceship
    {
        public Spaceship(string shipyardName)
        {
            ShipyardName = shipyardName;
        }

        public string ShipyardName { get; }
        public string Name { get; set; }
        public DateTime BuildDate { get; set; }
        public DateTime? LifeTimeEnd { get; set; }
        public bool CanBeDestroyedBySkywalker { get; set; }
        public Guid ShipId { get; set; }
    }

    public class Deathstar : Spaceship
    {
        public Deathstar(string shipyardName) : base(shipyardName)
        {
        }

        public int WeaponCount { get; set; }
        public long Firepower { get; set; }
        public decimal BuildCosts { get; set; }
    }

    #endregion
}

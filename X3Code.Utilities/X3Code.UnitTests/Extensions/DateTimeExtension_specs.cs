using System;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions
{
    // ReSharper disable once InconsistentNaming
    public class DateTimeExtension_specs
    {
        private readonly DateTime _referenceDate = new DateTime(2010, 11, 15, 12, 43, 0);
        private readonly DateTime _referenceZeroClock = new DateTime(2010, 11, 15, 0, 0, 0);
        private readonly DateTime _referenceMidnightClock = new DateTime(2010, 11, 15, 23, 59, 59);

        [Fact]
        public void IfAFullDateStringIsNeeded()
        {
            var result = _referenceDate.ToFullShortDateString();
            var referenceString = "15.11.2010 12:43";

            Assert.Equal(referenceString, result);
        }

        [Fact]
        public void IfaFullLongDateStringIsNeeded()
        {
            var result = _referenceDate.ToFullLongDateString();
            var referenzString = "Montag, 15. November 2010 12:43:00";

            Assert.Equal(referenzString, result);
        }

        [Fact]
        public void TheDateShotNotBeLowerThanSqlMin()
        {
            var wrongDate = DateTimeExtension.SqlMinDate.AddDays(-1);
            var result = wrongDate.ToSqlMinDateTimeIfOutOfRange();

            Assert.Equal(DateTimeExtension.SqlMinDate, result);
        }

        [Fact]
        public void IfTheClockNeedsToBeSetToZero()
        {
            var result = _referenceDate.ClockTo00();

            Assert.Equal(_referenceZeroClock, result);
        }

        [Fact]
        public void IfTheClockNeedsToBeSetToMidnight()
        {
            var result = _referenceDate.ClockTo24();

            Assert.Equal(_referenceMidnightClock, result);
        }

        [Fact]
        public void IfTheNextTuesdayIsNeeded()
        {
            var result = _referenceDate.GetNextTuesday();

            Assert.Equal(new DateTime(2010, 11, 16, 12, 43, 0), result);
        }
    }
}

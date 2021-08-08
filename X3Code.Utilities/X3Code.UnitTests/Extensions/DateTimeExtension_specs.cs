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
            var wrongResult = wrongDate.ToSqlMinDateTimeIfOutOfRange();
            Assert.Equal(DateTimeExtension.SqlMinDate, wrongResult);
            
            var rightDate = new DateTime(2021,1,1);
            var result = rightDate.ToSqlMinDateTimeIfOutOfRange();
            Assert.Equal(rightDate, result);
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
        public void CanGetNextWeekDays()
        {
            var monday = _referenceDate.GetNextMonday();
            var tuesday = _referenceDate.GetNextTuesday();
            var wednesday = _referenceDate.GetNextWednesday();
            var thursday = _referenceDate.GetNextThursday();
            var friday = _referenceDate.GetNextFriday();
            var saturday = _referenceDate.GetNextSaturday();
            var sunday = _referenceDate.GetNextSunday();

            Assert.Equal(new DateTime(2010, 11, 15, 12, 43, 0), monday);
            Assert.Equal(new DateTime(2010, 11, 16, 12, 43, 0), tuesday);
            Assert.Equal(new DateTime(2010, 11, 17, 12, 43, 0), wednesday);
            Assert.Equal(new DateTime(2010, 11, 18, 12, 43, 0), thursday);
            Assert.Equal(new DateTime(2010, 11, 19, 12, 43, 0), friday);
            Assert.Equal(new DateTime(2010, 11, 20, 12, 43, 0), saturday);
            Assert.Equal(new DateTime(2010, 11, 21, 12, 43, 0), sunday);
        }
        
        [Fact]
        public void CanJumpDays()
        {
            var fourteenEarlier = _referenceDate.GetFourteenDaysEarlier();
            var fourteenLater = _referenceDate.GetFourteenDaysLater();
            var sevenEarlier = _referenceDate.GetSevenDaysEalier();
            var sevenLater = _referenceDate.GetSevenDaysLater();
            Assert.Equal(new DateTime(2010, 11, 01, 12, 43, 0), fourteenEarlier);
            Assert.Equal(new DateTime(2010, 11, 29, 12, 43, 0), fourteenLater);
            Assert.Equal(new DateTime(2010, 11, 8, 12, 43, 0), sevenEarlier);
            Assert.Equal(new DateTime(2010, 11, 22, 12, 43, 0), sevenLater);
        }
    }
}

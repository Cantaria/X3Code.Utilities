using X3Code.Utils.Serializing;
using Xunit;

namespace X3Code.UnitTests.IO
{
    // ReSharper disable once InconsistentNaming
    public class InMemorySerializer_specs
    {
        [Fact]
        public void IfTheInMemorySerializerSerializesAndDeserializesAnArray()
        {
            var referenceData = GetTestData();

            //Should not throw an Exception
            var serialized = GenericMemorySerializer.Serialize(referenceData);
            var testData = GenericMemorySerializer.Deserialize(serialized);

            Assert.NotNull(serialized);
            Assert.Equal(testData, referenceData);
        }

        [Fact]
        public void IfTheMemorySerializerGetsCalledAboutTheExtension()
        {
            var referenceData = GetTestData();

            //Should not throw an Exception
            var serialized = referenceData.ToBinaryMemoryStream();
            var testData = serialized.ToObjectArray();

            Assert.NotNull(serialized);
            Assert.Equal(testData, referenceData);
        }

        private object[] GetTestData()
        {
            var result = new object[5];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = $"The cake nr {i} is a lie";
            }

            return result;
        }
    }
}

namespace Quantum.Tempo.Tests;

public class StringDateToIsoFormatTests
{
    [Theory]
    [InlineData("2013/02/27", "2013-02-27")]
    [InlineData("2013/27/02", "2013-02-27")]
    [InlineData("2013/27/2", "2013-02-27")]
    [InlineData("2013/2/2", "2013-02-02")]

    [InlineData("13/27/02", "2013-02-27")]
    [InlineData("13/02/27", "2013-02-27")]
    [InlineData("13/2/27", "2013-02-27")]

    [InlineData("2017.02.27", "2017-02-27")]
    [InlineData("2017.27.02", "2017-02-27")]
    [InlineData("2017.27.2", "2017-02-27")]
    [InlineData("2017.2.27", "2017-02-27")]
    [InlineData("2017.2.28", "2017-02-28")]
    public void normalizeDate(string actualDate, string expectedIsoFormatted)
    {
        actualDate
            .ToIso()
            .Should()
            .Be(expectedIsoFormatted);
    }
}
namespace Quantum.Tempo.Tests;

public class NestingTimeTests
{
    // 2017-04 nest-day => 30  
    // 2017 nest-day => 365  
    // 2016 nest-day => 366
    // 2017 nest-month => 12
    // 2017 nest-month first => 2017-01
    // 2017 nest-month second => 2017-02
    // 2017 nest-month last => 2017-12

    // 2017 nest-day first=> 1
    // 2017 nest-day last => 365



    // 2017-04-09 enclosing-immediate 2017-04
    // 2017-04-09 enclosing-year 2017

    // 2017-04-09 end-of-month 2017-04-30
    // 2017-04 end-of-month 2017-04-30
    // 2017-04-19T15:12 end-of-month 2017-04-30
    // 2017-04 end-of-month 2017-04-30

    // 2017 end-of-month throw error


    [Fact]
    public void nesting()
    {
        var timeSequence = "2017-02".NestMonth();

        Assert.Equal(1, timeSequence.Count);
        Assert.Equal(timeSequence.First(), timeSequence.First());
        Assert.Equal("2017-02", timeSequence.First().ToIso());
        Assert.Equal("2017-02", timeSequence.Last().ToIso());
        Assert.Equal("2017-02", timeSequence.nth(1).ToIso());

        Assert.Equal(1, "2017-02".NestMonth().Count);

        Assert.Equal(1, "2017-02-03".NestDay().Count);
        Assert.Equal(1, "2017-02-03T20".NestDay().Count);
        Assert.Equal(1, "2017-02-03T20:30".NestDay().Count);
        Assert.Equal(1, "2017-02-03T20:30:20".NestDay().Count);
        
        Assert.Equal(1, "2017-02-03".NestMonth().Count);
        Assert.Equal(1, "2017-02-03T20".NestMonth().Count);
        Assert.Equal(1, "2017-02-03T20:30".NestMonth().Count);
        Assert.Equal(1, "2017-02-03T20:30:20".NestMonth().Count);
    }

    [Fact]
    public void name()
    {
        Assert.Equal("2018-012", "2018-012".NestDay().First().ToIso());
        Assert.Equal("2018-012", "2018-012".NestDay().Last().ToIso());

        Assert.Equal("2018-02-12", "2018-02-12T20:30:30".NestDay().Last().ToIso());

        Assert.Equal("2018-W52", "2018-W52".NestDay().Last().ToIso());
        Assert.Equal("2018-W52", "2018-W52-1".NestDay().Last().ToIso());

        Assert.Equal("2018-012", "2018-012".NestMonth().First().ToIso());
        Assert.Equal("2018-012", "2018-012".NestMonth().Last().ToIso());

        Assert.Equal("2018-W52", "2018-02-12T20:30:30".NestMonth().Last().ToIso());

        Assert.Equal("2018-W52", "2018-W52".NestMonth().Last().ToIso());
        Assert.Equal("2018-W52", "2018-W52-1".NestMonth().Last().ToIso());
    }


    [Fact]
    public void nestingOutOfRangeShouldRaiseException()
    {
        var timeSequence = "2017-02".NestMonth();

        Action action = () => timeSequence.nth(2);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }


    [Fact]
    public void nestingMonthOfYearMonthIsIdentitca()
    {
        Assert.Equal(1, "2017-02".NestMonth().Count);
        Assert.Equal("2017-02".NestMonth().First(), "2017-02".NestMonth().First());
        Assert.Equal("2017-02", "2017-02".NestMonth().First().ToIso());
    }

    [Fact]
    public void nestingDay_Of_A_YearMonthDay()
    {
        Assert.Equal(1, "2017-04-01".NestDay().Count);
        Assert.Equal(1, "2017-04-28".NestDay().Count);
        Assert.Equal("2017-04-01", "2017-04-01".NestDay().First().ToIso());
        Assert.Equal("2017-04-01", "2017-04-01".NestDay().Last().ToIso());
        Assert.Equal("2017-04-20", "2017-04-20".NestDay().Last().ToIso());

        Assert.Equal(1, "2017-04-20".NestMonth().Count);
        Assert.Equal("2017-04", "2017-04-20".NestMonth().First().ToIso());
        Assert.Equal("2017-04", "2017-04-20".NestMonth().Last().ToIso());
    }

    [Theory]
    [InlineData("2017-04-01", "2017-04-30")]
    [InlineData("2017-02-20", "2017-02-28")]
    [InlineData("2016-02-20", "2016-02-29")]
    [InlineData("2016-02", "2016-02-29")]
    [InlineData("2017-02", "2017-02-28")]
    [InlineData("2018-12", "2018-12-31")]
    [InlineData("2018-12-20T20", "2018-12-31")]
    [InlineData("2018-12-20T20:30", "2018-12-31")]
    [InlineData("2018-12-20T20:30:20", "2018-12-31")]

    public void endOfMonth(string actual, string expected)
        => Assert.Equal(expected, actual.NestDay().EndOfMonth().ToIso());

    [Fact]
    public void endOfMonthOf_a_yearTime_WillThrowException()
    {
        Action action = () => "2018".NestDay().EndOfMonth().ToIso();

        Assert.Throws<EndOfMonthOnYearTimeException>(action);
    }
    
    [Theory]
    [InlineData("2017-04-01", "2017-04-01")]
    [InlineData("2017-02-20", "2017-02-01")]
    [InlineData("2016-02-20", "2016-02-01")]
    [InlineData("2016-02", "2016-02-01")]
    [InlineData("2017-02", "2017-02-01")]
    [InlineData("2018-12-20T20", "2018-12-01")]
    [InlineData("2018-12-20T20:30", "2018-12-01")]
    [InlineData("2018-12-20T20:30:20", "2018-12-01")]
    public void beginingOfMonth(string actual, string expected)
        => Assert.Equal(expected, actual.NestDay().BeginningOfMonth().ToIso());
    
    [Theory]
    [InlineData("2017-04-09 ", "2017-04")]
    [InlineData("2016-02-09 ", "2016-02")]
    [InlineData("2016-02", "2016")]
    [InlineData("2016-02-21T20", "2016-02-21")]
    [InlineData("2016-02-21T20:30", "2016-02-21T20")]
    [InlineData("2016-02-21T20:30:30", "2016-02-21T20:30")]
    [InlineData("2016", "2016")]
    public void enclosing_immediate(string actual, string expected)
    {
        // 2017-04-09 enclosing-immediate 2017-04
        // 2017-04-09 enclosing-year 2017

        Assert.Equal(expected, actual.EnclosingImmediate().ToIso());
    }

    [Theory]
    [InlineData("2017-04-09 ", "2017")]
    [InlineData("2016-02-09 ", "2016")]
    [InlineData("2016-02", "2016")]
    [InlineData("2016-02-21T20", "2016")]
    [InlineData("2016-02-21T20:30", "2016")]
    [InlineData("2016-02-21T20:30:30", "2016")]
    [InlineData("2016", "2016")]
    public void enclosing_year(string actual, string expected)
        => Assert.Equal(expected, actual.EnclosingYear().ToIso());
    
    [Theory]
    [InlineData("2017-04-09 ", "2017-04")]
    [InlineData("2016-02-09 ", "2016-02")]
    [InlineData("2016-02", "2016-02")]
    [InlineData("2016-02-21T20", "2016-02")]
    [InlineData("2016-02-21T20:30", "2016-02")]
    [InlineData("2016-02-21T20:30:30", "2016-02")]
    public void enclosing_month(string actual, string expected)
        => Assert.Equal(expected, actual.EnclosingMonth().ToIso());

    [Fact]
    public void enclosing_month_Of_a_yearTime_will_throw_exception()
    {
        var actual = "2016";
        Action action = () => actual.EnclosingMonth().ToIso();
        Assert.Throws<EnclosingMonthOfYearTimeException>(action);
    }
}
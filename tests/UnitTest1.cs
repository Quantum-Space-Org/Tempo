namespace Quantum.Tempo.Tests
{
    public class DateTimeTests
    {
        [Fact]
        public void Test1()
        {
            var addMonths = new DateTime(2017, 01, 30).AddMonths(1);
            var b = new DateTime(2017, 01, 20).AddMonths(1);
            var c = new DateTime(2017, 01, 20).AddMonths(2);


            var nextDay = new DateTime(2020, 02, 28).AddDays(1);


            // 2017-04-30
            var d = new DateTime(2017, 03, 30).AddMonths(1);
            
            
            // 2017-04-30
            var e = new DateTime(2017, 03, 31).AddMonths(1);





            // homoiconic
            // code as data

            // [1 , 2 ]
            // new int[] = {1,2}
            // [1 , 2 ]

            // "2017-05"
            // A + 2 != A + 1 + 1
            // temporal coupling
            // 2017-05-31
            var f = new DateTime(2017, 03, 31).AddMonths(2);
            // 2017-05-30
            var g = new DateTime(2017, 03, 31).AddMonths(1).AddMonths(1);

            // 2017-05-31
            var h = new DateTime(2017, 03, 30).AddMonths(2);
            var i = new DateTime(2017, 03, 30).AddMonths(1).AddMonths(1);
        }
    }
}
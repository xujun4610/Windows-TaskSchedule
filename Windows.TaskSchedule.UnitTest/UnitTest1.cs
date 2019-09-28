using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.TaskSchedule.Utility;

namespace Windows.TaskSchedule.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCronTrigger()
        {
            var cronExpressString = "0 36 13 * * ?"; 
            var date = new DateTime(2019, 9, 28, 5, 51, 0, DateTimeKind.Utc);
            var dateLocal = date.ToLocalTime();
            var dateUTC = date.ToUniversalTime();
            var flag = CornUtility.Trigger(cronExpressString, DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));

            Assert.AreEqual(flag, true);
        }
    }
}

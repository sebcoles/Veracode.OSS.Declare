using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Veracode.OSS.Declare.Shared;
using Veracode.OSS.Wrapper.Rest;

namespace Veracode.OSS.Declare.Tests.Unit
{
    [TestFixture]
    public class CronHelperTests
    {
        [Test]
        public void IsANewScanDue_ShouldReturnTrue()
        {
            var pattern = "0 0 * * *";
            var last_scan_date = DateTime.Now.AddDays(-2);
            Assert.IsTrue(CronHelper.IsANewScanDue(pattern, last_scan_date));
        }

        [Test]
        public void IsANewScanDue_ShouldReturnFalse()
        {
            var pattern = "0 0 * * *";
            var last_scan_date = DateTime.Now;
            Assert.IsFalse(CronHelper.IsANewScanDue(pattern, last_scan_date));
        }

        [Test]
        public void HowManyDaysAgo_ShouldReturn10()
        {
            var pattern = "0 0 * * *";
            var last_scan_date = DateTime.Now.AddDays(-10);
            var days = CronHelper.HowManyDaysAgo(pattern, last_scan_date);
            Assert.IsTrue(9.0 < days && days < 10.0);
        }
    }
}

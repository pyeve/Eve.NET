using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Eve.EC;

namespace Eve.Tests
{
    [TestClass]
    public class Fluent
    {
        [TestMethod]
        public void GetMany()
        {
            var date = DateTime.Now;
            var challenge = Get<Company>()
                .Where(x => (x.StateOrProvince != "tag" || x.Name == "nik") && x.Password == "pw")
                .IfModifiedSince(date)
                .IncludeDeleted()
                .BuildQuery();

            Assert.AreEqual(
                //$"{{$or: [{{\"state_or_province\": {{\"$ne\":\"tag\"}}}}, {{\"name\": \"nik\"}}], \"password\": \"pw\"}}",
                //$"{{$or: [{{\"state_or_province\": {{\"$ne\":\"tag\"}}}}, {{\"name\": \"nik\"}}], \"password\": \"pw\", \"_updated\": \"{date.ToString("r")}\"}}",
                $"{{$or: [{{\"state_or_province\": {{\"$ne\":\"tag\"}}}}, {{\"name\": \"nik\"}}], \"password\": \"pw\", \"_updated\": \"{date.ToString("r")}\"}}?show_deleted",
                challenge);
        }
    }
}

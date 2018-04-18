using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eve.EC;

namespace Eve.Tests
{
    [TestFixture]
    class Fluent
    {
        [Test]
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

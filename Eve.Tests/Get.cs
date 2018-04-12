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
        public void IfModifiedSince()
        {
            var r = Get<Company>().Where(x => x.Name == "ciao" && x.ETag == "tag");
        }
    }
}

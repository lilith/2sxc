﻿using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Services;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToPagePartUnknownTests : LinkHelperTestBase
    {
        [TestMethod]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual(LinkServiceUnknown.NiceCurrentUrl, Link.TestTo());
        }

        [TestMethod]
        public void ToPageCommonsTest()
        {
            //AreEqual($"{LinkHelperUnknown.MockHost}/page", Link.TestTo(pageId: null));
            AreEqual($"{LinkServiceUnknown.DefRoot}/page{27}", Link.TestTo(pageId: 27));
        }

        [TestMethod]
        public void ToPageParametersTest()
        {
            AreEqual($"{LinkServiceUnknown.DefRoot}/page27", Link.TestTo(pageId: 27));
            AreEqual($"{LinkServiceUnknown.DefRoot}/page27", Link.TestTo(pageId: 27, parameters: null));
            AreEqual($"{LinkServiceUnknown.DefRoot}/page27?a=1&b=2#fragment", Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/page27?a=1&b=2&c=3", Link.TestTo(pageId: 27, parameters: new Parameters(new NameValueCollection
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            })));
        }
    }
}
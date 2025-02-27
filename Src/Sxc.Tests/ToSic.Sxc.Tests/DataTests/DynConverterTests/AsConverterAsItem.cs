﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    [TestClass]
    public class AsConverterAsItem: AsConverterTestsBase
    {
        [TestMethod]
        public void AsItemWithFakeOk()
        {
            var item = Cdf.AsItem(Cdf.FakeEntity(0), noParamOrder: Protector, propsRequired: true);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AsItemWithAnonFail()
        {
            var data = new
            {
                Title = "This is a title",
                Birthday = new DateTime(2012, 02, 07)
            };

            var item = Cdf.AsItem(data, noParamOrder: Protector, propsRequired: true);
        }
    }
}

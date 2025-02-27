﻿using System;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Dnn.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static readonly FeatureDefinition DnnPageWorkflow = new FeatureDefinition(
            "DnnPageWorkflow",
            new Guid("da68d954-5220-4f9c-a485-86f16b98629a"),
            "Support for Dnn / Evoq Page Workflow",
            false,
            false,
            "Enables support for Page Workflow in Evoq; on by default. It seems that in Evoq ca. v9.4+ it has become buggy. So you can disable it now. ",
            FeaturesCatalogRules.Security0Neutral,

            // Todo: SHOULD MOVE TO CorePlus, but must wait till PinWheel adjusted their setup - ask 2dm
            Eav.Configuration.BuiltInFeatures.ForAllEnabled
        );

    }
}

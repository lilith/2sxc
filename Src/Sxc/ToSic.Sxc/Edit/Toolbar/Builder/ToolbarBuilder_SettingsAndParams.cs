﻿using System;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        public IToolbarBuilder Settings(
            string noParamOrder = Eav.Parameters.Protector,
            string show = default,
            string hover = default,
            string follow = default,
            string classes = default,
            string autoAddMore = default,
            object ui = default,
            object parameters = default)
            => this.AddInternal(new ToolbarRuleSettings(show: show, hover: hover, follow: follow, classes: classes, autoAddMore: autoAddMore,
                ui: PrepareUi(ui), parameters: Utils.Par2Url.Serialize(parameters)));


        public IToolbarBuilder Parameters(
            object target = default,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = default,
            object parameters = default,
            object prefill = default,
            string context = default
        )
        {
            TargetCheck(target);
            var clone = new ToolbarBuilder(this);

            // see if we already have a params rule, if yes remove to then later clone and add again
            var previous = clone.FindRule<ToolbarRuleForParams>();
            if (previous != null)
                clone.Rules.Remove(previous);

            // detect if the first parameter (target) is a parameters object
            (target, parameters) = FixTargetIsParameters(target, parameters);

            // Use new or previous target
            target = target ?? previous?.Target;

            // Must create a new one, to not change the original which is still in the original object
            var uiWithPrevious = PrepareUi(previous?.Ui, ui);
            var partsWithPrevious = Utils.Par2Url.SerializeWithChild(previous?.Parameters, parameters);
            var parts = PreCleanParams(tweak, defOp: ToolbarRuleOps.OprNone, ui: uiWithPrevious, parameters: partsWithPrevious, prefill: prefill);

            //var parsWithPrefill = Utils.Prefill2Url.SerializeWithChild(partsWithPrevious, prefill, PrefixPrefill);

            var newParamsRule = new ToolbarRuleForParams(target,
                parts.Ui,
                parts.Parameters,
                GenerateContext(target, context) ?? previous?.Context,
                Services.ToolbarButtonHelper.Value);

            clone.Rules.Add(newParamsRule);

            return clone;
        }

        private void TargetCheck(object target)
        {
            if (target is IAsset)
                throw new Exception("Got a 'target' parameter which seems to be an adam-file. " +
                                    "This is not allowed. " +
                                    "Were you trying to target the .Metadata of this file? if so, add .Metadata to the target object.");

        }

        private (object target, object parameters) FixTargetIsParameters(object target, object parameters)
        {
            // No target, or parameters supplied
            if (parameters != null || target == null) return (target, parameters);

            // Basically only keep the target as is, if it's a known target
            if (target is IEntity || target is ICanBeEntity)
                return (target, null);

            return (null, target);
        }
    }
}

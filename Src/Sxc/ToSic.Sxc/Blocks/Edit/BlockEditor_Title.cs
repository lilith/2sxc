﻿using System.Linq;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Blocks.Edit
{
    public partial class BlockEditorBase
    {
        internal void UpdateTitle()
        {
            Log.A("update title");
            // check the blockConfiguration as to what should be the module title, then try to set it
            // technically it could have multiple different groups to save in, 
            // ...but for now we'll just update the current modules title
            // note: it also correctly handles published/unpublished, but I'm not sure why :)

            var cms = Services.CmsRuntime.Value;
            var contentGroup = cms.Blocks.GetBlockConfig(BlockConfiguration.Guid);

            var titleItem = contentGroup.Header.FirstOrDefault() ?? contentGroup.Content.FirstOrDefault();
            
            if (titleItem != null) UpdateTitle(titleItem);
        }

    }
}

﻿//using System.Collections.Generic;
//using static System.StringComparer;

//namespace ToSic.Sxc.Blocks
//{
//    public partial class BlockBuilder
//    {
//        internal IDictionary<string, string> BuildSpecsForLogHistory()
//        {
//            var specs = new Dictionary<string, string>(InvariantCultureIgnoreCase);
//            var block = Block;
//            if (block != null)
//            {
//                specs.Add(nameof(block.ContentBlockId), block.ContentBlockId.ToString());

//                var app = block.App;
//                if (app != null)
//                {
//                    specs.Add(nameof(app.AppId), app.AppId.ToString());
//                    specs.Add("AppPath", app.Path);
//                    specs.Add("AppName", app.Name);
//                }

//                var view = block.View;
//                if (view != null)
//                {
//                    specs.Add("ViewId", view.Id.ToString());
//                    specs.Add("ViewEdition", view.Edition);
//                    specs.Add("ViewPath", view.Path);
//                }

//                var ctx = block.Context;
//                if (ctx != null)
//                {
//                    var site = ctx.Site;
//                    if (site != null)
//                    {
//                        specs.Add("SiteId", site.Id.ToString());
//                        specs.Add("SiteUrl", site.Url);
//                    }

//                    var page = ctx.Page;
//                    if (page != null)
//                    {
//                        specs.Add("PageId", page.Id.ToString());
//                        specs.Add("PageUrl", page.Url);
//                    }

//                    var module = ctx.Module;
//                    if (module != null) specs.Add("ModuleId", module.Id.ToString());

//                    var usr = ctx.User;
//                    if (usr != null)
//                    {
//                        specs.Add("UserId", usr.Id.ToString());
//                    }
//                }

//                return specs;
//            }

//            return null;
//        }
//    }
//}

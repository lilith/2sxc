﻿using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Images
{
    public class ImageDecorator: EntityBasedType
    {
        // Marks Requirements Metadata 13.04
        public static string TypeNameId = "cb27a0f2-f921-48d0-a3bc-37c0e77b1d0c";
        public static string NiceTypeName = "ImageDecorator";

        public const string FieldDescription = "Description";
        public const string FieldCropBehavior = "CropBehavior";
        public const string FieldCompass = "CropTo";
        public const string NoCrop = "none";

        public static ImageDecorator GetOrNull(IHasMetadata source, string[] dimensions)
        {
            var decItem = source?.Metadata?.FirstOrDefaultOfType(TypeNameId);
            return decItem != null ? new ImageDecorator(decItem, dimensions) : null;
        }

        public ImageDecorator(IEntity entity, string[] languageCodes) : base(entity, languageCodes) { }

        public string CropBehavior => Get(FieldCropBehavior, "");

        public string CropTo => Get(FieldCompass, "");

        public string Description => Get(FieldDescription, "");

        public (string Param, string Value) GetAnchorOrNull()
        {
            var b = CropBehavior;
            if (b != "to") return (null, null);
            var direction = CropTo;
            if(string.IsNullOrWhiteSpace(direction)) return (null, null);
            var dirLong = ResolveCompass(direction);
            if (string.IsNullOrWhiteSpace(dirLong)) return (null, null);
            return ("anchor", dirLong);
        }

        #region Private Gets


        private string ResolveCompass(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 2) return null;
            return GetRow(code[0]) + GetCol(code[1]);
        }

        private static string GetRow(char code)
        {
            switch (code)
            {
                case 't': return "top";
                case 'm': return "middle";
                case 'b': return "bottom";
                default: return null;
            }
        }
        private static string GetCol(char code)
        {
            switch (code)
            {
                case 'c': return "center";
                case 'l': return "left";
                case 'r': return "right";
                default: return null;
            }
        }

        #endregion

        #region AddRecommendations

        /// <summary>
        /// Optionally add image-metadata recommendations
        /// </summary>
        public static void AddRecommendations(IMetadataOf mdOf, string path)
        {
            if (mdOf?.Target == null || !path.HasValue()) return;
            var ext = Path.GetExtension(path);
            if (ext.HasValue() && Classification.IsImage(ext))
                mdOf.Target.Recommendations = new[] { TypeNameId };
        }


        #endregion
    }
}

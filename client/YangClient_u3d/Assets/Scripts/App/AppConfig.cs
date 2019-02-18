using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class AppConfig
    {
        public enum AssetLoadMode
        {
            Editor,
            AssetBundle,
        }

        public static AssetLoadMode mAssetLoadMode = AssetLoadMode.AssetBundle;
    }

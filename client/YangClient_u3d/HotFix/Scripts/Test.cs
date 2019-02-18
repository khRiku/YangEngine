using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotfix
{
    public class HotFixTest :MonoBehaviour
    {
        public string mAssetIndex = "0,1";

        public UnityEngine.Object mObject;
        [ContextMenu("加载")]
        public void Load()
        {
            //UnityEngine.Object tObj = ManagerIndex.AssetManager.LoadAsset(mAssetIndex);
            //mObject = Object.Instantiate(tObj);
            ManagerIndex.AssetManager.LoadAssetAsync(mAssetIndex, (pObject, pUseCache) =>
            {
             //   mObject = UnityEngine.Object.Instantiate(pObject);

            });
        }

        [ContextMenu("卸载")]
        public void Unload()
        {
            UnityEngine.Object.Destroy(mObject);
            ManagerIndex.AssetManager.UnloadAsset(mAssetIndex);

        }
    }
}

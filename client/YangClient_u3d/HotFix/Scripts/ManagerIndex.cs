using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotfix;

namespace Hotfix
{
    public class ManagerIndex
    {
        private static List<object> mManagerList = new List<object>();
        #region 核心的管理器
        /// <summary>
        /// 启动管理器
        /// </summary>
        private static StartUpManager mStartUpManager = null;

        public static StartUpManager StartUpManager
        {
            get
            {
                if (mStartUpManager == null)
                {
                    mStartUpManager = new StartUpManager();
                    mStartUpManager.InitInInstance();

                    mManagerList.Add(mStartUpManager);
                }

                return mStartUpManager;
            }
        }


        /// <summary>
        /// 资源管理器
        /// </summary>
        private static AssetManager mAssetManager = null;
        public static AssetManager AssetManager
        {
            get
            {
                if (mAssetManager == null)
                {
                    mAssetManager = new AssetManager();
                    mAssetManager.InitInCreateInstance();

                    mManagerList.Add(mAssetManager);

                }

                return mAssetManager;
            }
        }

        /// <summary>
        /// UI管理器
        /// </summary>
        private static UIManager mUIManager = null;
        public static UIManager UIManager
        {
            get
            {
                if (mUIManager == null)
                {
                    mUIManager = new UIManager();
                    mUIManager.InitInCreateInstance();

                    mManagerList.Add(mUIManager);

                }

                return mUIManager;
            }
        }

        #endregion


        #region 业务相关的管理器


        #endregion
    }
}

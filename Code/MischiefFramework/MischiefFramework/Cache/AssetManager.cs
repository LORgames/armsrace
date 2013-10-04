using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MischiefFramework.Cache {
    internal class AssetManager {
        private static List<Asset> assets = new List<Asset>();
        private static List<Asset> destroyQueue = new List<Asset>();
        
        private static int _nextAssetID = 0;
        private static object ACCESS_LOCK = new Object();

        public static void AddAsset(Asset newAsset) {
            lock (ACCESS_LOCK) {
                assets.Add(newAsset);
                newAsset.SetAssetID(_nextAssetID++);
            }
        }

        public static void RemoveAsset(Asset deadAsset) {
            lock (ACCESS_LOCK) {
                destroyQueue.Add(deadAsset);
            }
        }

        public static void Update(float dt) {
            lock (ACCESS_LOCK) {
                int totalObjects = assets.Count;

                for (int i = 0; i < totalObjects; i++) {
                    assets[i].Update(dt);
                    assets[i].AsyncUpdate(dt);
                }

                //TODO: Flip this
                while (destroyQueue.Count > 0) {
                    assets.Remove(destroyQueue[0]);
                    destroyQueue.RemoveAt(0);
                }
            }
        }

        internal static void Flush() {
            assets.Clear();
        }
    }

    internal partial class Asset {
        private int _assetID;

        public void SetAssetID(int newID) { _assetID = newID; }
        public int GetAssetID() { return _assetID; }

        public virtual void Update(float dt) { }
        public virtual void AsyncUpdate(float dt) { }
    }
}

using UnityEngine;
using System.Collections;

namespace GuideEditor
{
    public class GuideSubData
    {
        public int id;
        public int btnType = 0;
        public string desc = "";
        public string path = "";
        public int direction;
        public string content = "";
        public int offsetX;
        public int offsetY;
        public string systemOpenFuncName = "";
        public bool isKeyFrame;
        public bool isTriggerMainTask;
        /// <summary>
        /// 语音
        /// </summary>
        public string dialogMusic = "";
        /// <summary>
        /// 延迟触发显示引导  目前只有弱引导会用
        /// </summary>
        public float delayShowTime = 0.0f;
        /// <summary>
        /// 当超过某个等级 就不触发了 0就代表不检查
        /// </summary>
        public int triggerMaxPlayerLevel = 0;
        /// <summary>
        /// vip就不触发?
        /// </summary>
        public bool vipNotTrigger = false;
        /// <summary>
        /// 引导一出来 是否有黑背景显示
        /// </summary>
        public bool isNeedBlack = false;
        /// <summary>
        /// 副本内的时候是否可以尝试触发引导
        /// </summary>
        public bool inDupCanTrigger = true;
    }
}

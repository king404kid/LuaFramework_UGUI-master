using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GuideEditor
{
    public class GuideData
    {
        /// <summary>
        /// 大引导的id
        /// </summary>
        public int id;
        /// <summary>
        ///大引导的说明
        /// </summary>
        public string desc = "";

        /// <summary>
        /// 是否强引导
        /// </summary>
        public bool isForce = true;
        /// <summary>
        /// 引导步骤的列表
        /// </summary>
        public List<GuideSubData> subData;

        public int GetCount() {
            if (subData == null) {
                return 0;
            } else {
                return subData.Count;
            }
        }

        public List<GuideSubData> GetData() {
            return subData;
        }

        public void InsertData(int ind = -1) {
            if (subData == null) {
                subData = new List<GuideSubData>();
            }
            GuideSubData d = new GuideSubData();
            if (ind < 0) {
                subData.Add(d);
            } else {
                subData.Insert(ind, d);
            }
            SortSubList();
        }

        public void DelData(int ind) {
            if (subData == null) {
                return;
            }
            if (ind > subData.Count) {
                return;
            }
            ind = ind - 1;
            if (ind < 0) {
                ind = 0;
            }
            subData.RemoveAt(ind);
            SortSubList();
        }

        /// <summary>
        /// 排序
        /// </summary>
        private void SortSubList() {
            if (subData == null) {
                return;
            }
            for (int i = 0; i < subData.Count; i++) {
                subData[i].id = i + 1;
            }
        }
    }
}
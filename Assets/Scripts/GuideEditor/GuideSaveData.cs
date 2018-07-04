using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GuideEditor
{
    public class GuideSaveData
    {
        public Dictionary<int, GuideData> data;

        public int GetCount() {
            if (data == null) {
                return 0;
            } else {
                return data.Count;
            }
        }

        public List<GuideData> GetDataList() {
            if (data == null) {
                return null;
            } else {
                List<GuideData> list = new List<GuideData>();
                foreach (KeyValuePair<int, GuideData> item in data) {
                    int ind = list.Count;
                    for (int i = 0; i < list.Count; i++) {
                        if (list[i].id > item.Key) {
                            ind = i;
                            break;
                        }
                    }
                    list.Insert(ind, item.Value);
                }
                return list;
            }
        }

        public bool IsGuideExist(int id) {
            if (data == null || data.ContainsKey(id) == false) {
                return false;
            } else {
                return true;
            }
        }

        public GuideData GetGuideData(int id) {
            if (data == null || data.ContainsKey(id) == false) {
                return null;
            } else {
                return data[id];
            }
        }

        public void AddGuide(int id) {
            if (data == null) {
                data = new Dictionary<int, GuideData>();
            }
            GuideData d = new GuideData();
            d.id = id;
            if (data.ContainsKey(id)) {

                data[id] = d;
            } else {
                data.Add(id, d);
            }
        }

        public void DelGuide(int id) {
            if (data == null || data.ContainsKey(id) == false) {
                return;
            }
            data.Remove(id);
        }
    }
}
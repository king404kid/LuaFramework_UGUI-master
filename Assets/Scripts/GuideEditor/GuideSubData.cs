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
        /// ����
        /// </summary>
        public string dialogMusic = "";
        /// <summary>
        /// �ӳٴ�����ʾ����  Ŀǰֻ������������
        /// </summary>
        public float delayShowTime = 0.0f;
        /// <summary>
        /// ������ĳ���ȼ� �Ͳ������� 0�ʹ������
        /// </summary>
        public int triggerMaxPlayerLevel = 0;
        /// <summary>
        /// vip�Ͳ�����?
        /// </summary>
        public bool vipNotTrigger = false;
        /// <summary>
        /// ����һ���� �Ƿ��кڱ�����ʾ
        /// </summary>
        public bool isNeedBlack = false;
        /// <summary>
        /// �����ڵ�ʱ���Ƿ���Գ��Դ�������
        /// </summary>
        public bool inDupCanTrigger = true;
    }
}

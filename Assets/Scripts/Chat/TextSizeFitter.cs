using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MyStuff
{
    [AddComponentMenu("Layout/TMP TextFitter")]
    public class TextSizeFitter : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI m_TextMeshPro;
        public TMPro.TextMeshProUGUI TextMeshPro
        {
            get
            {
                if(m_TextMeshPro == null&&transform.GetComponentInChildren<TMPro.TextMeshProUGUI>())
                {
                    m_TextMeshPro=transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    rectTransform = m_TextMeshPro.rectTransform;
                }
                return m_TextMeshPro;
            }
        }
        private RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get { return rectTransform; }
        }
        private RectTransform m_rect;
        public RectTransform m_Rect
        {
            get 
            { 
                if(m_rect==null)
                {
                    m_rect=GetComponent<RectTransform>();
                }
                return m_rect; 
            }
        }

        private float preferedHight;
        public float PreferedHight
        {
            get { return preferedHight; }
        }

        void SetHight()
        {
            if(TextMeshPro==null)
            {
                return;
            }
            preferedHight = TextMeshPro.preferredHeight;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferedHight);
        }

        private void OnEnable()
        {
            SetHight();
        }
        private void Start()
        {
            SetHight();
        }
        private void Update()
        {
            if(preferedHight!= TextMeshPro.preferredHeight)
            {
                SetHight();
            }
        }
    }
}


/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2022 Binary Charm - All Rights Reserved
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@                  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@                        @@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@                           @@@@@@@@@@@@@@@@@@
             @@@@@@@@@   @@@@@@@@@@@  @@@@@        @@@@@@@@@@@@@@@
             @@@@@@@@@@@  @@@@@@@@@  @@@@@@@@@@       (@@@@@@@@@@@
             @@@@@@@@@@@@  @@@@@@@@& @@@@@@@@@@ @@@@     @@@@@@@@@
             @@@@@@@@@@@@@ @@@@@@@@@@ *@@@@@@@ @@@@@@@@@*   @@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@      @@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
*/

using UnityEngine;

using System.Collections.Generic;

using TMPro;

using BinaryCharm.TextMeshProAlpha;

namespace BinaryCharm.Samples.TextMeshProAlpha.TextAnimation.Effects {
    public class DialogueAppear : ATextAnimation {

        private string m_sParsedText; 
        private float m_fVisibleChars;

        private float m_fElapsedTime;

        private float m_fPrevCharTime;
        private float m_fNextCharTime;
        private int m_iPrevChar;
        private int m_iNextChar;

        private float m_fSecsPerChar;
        private Dictionary<char, float> m_rPausesMap;

        private const float fCHARACTERS_PER_SEC = 30f;
        private static Dictionary<char[], float> s_rDefaultPauses =
            new Dictionary<char[], float>() {
                { new char[] { ',', '-', ':' } , 0.3f },
                { new char[] { '.', '…', '!', '?', ';' }, 0.6f }
            };

        public DialogueAppear(TMP_Text rTextMesh, string sText,
                float fCharsPerSec = fCHARACTERS_PER_SEC,
                Dictionary<char[], float> rPausesAfterChars = null
                ) : base(rTextMesh, sText) {

            m_fSecsPerChar = 1f / fCharsPerSec;

            if (rPausesAfterChars == null) {
                rPausesAfterChars = s_rDefaultPauses;
            }

            m_rPausesMap = new Dictionary<char, float>();
            foreach (var kv in rPausesAfterChars) {
                foreach (char c in kv.Key) {
                    m_rPausesMap.Add(c, kv.Value);
                }
            }
        }

        protected override void onStart() {
            m_fVisibleChars = 0f;
            m_fElapsedTime = 0f;

            m_iPrevChar = 0;
            m_iNextChar = 0;
            m_fNextCharTime = 0f;
            m_fPrevCharTime = 0f;

            // we need this to fetch the string without rich text tags
            m_rTextMesh.ForceMeshUpdate();
            m_sParsedText = m_rTextMesh.GetParsedText();
        }

        protected override void onUpdate(float fDT) {
            m_fElapsedTime += fDT;

            while (m_fElapsedTime > m_fNextCharTime && m_iPrevChar < m_sParsedText.Length) {
                char c = m_sParsedText[m_iPrevChar];
                float fPause;
                if (!m_rPausesMap.TryGetValue(c, out fPause)) {
                    fPause = 0f;
                }

                m_fNextCharTime = m_fPrevCharTime + fPause + m_fSecsPerChar;
                m_iNextChar = m_iPrevChar + 1;

                if (m_fElapsedTime > m_fNextCharTime) {
                    m_fPrevCharTime = m_fNextCharTime;
                    m_iPrevChar = m_iNextChar;
                }
            }
            float fFracCharTime = m_fElapsedTime - m_fPrevCharTime;
            float fFracChar = Mathf.InverseLerp(0, m_fSecsPerChar, fFracCharTime);
            float fNextVisibleChars = m_iPrevChar + fFracChar;

            m_fVisibleChars = fNextVisibleChars;
        }

        public override bool isComplete() {
            return m_iPrevChar == m_sParsedText.Length;
        }

        protected override void updateVisualization() {
            int iIntPart = Mathf.FloorToInt(m_fVisibleChars);
            float fFracPart = m_fVisibleChars - iIntPart;

            m_rTextMesh.setAlphaBegin();
            m_rTextMesh.setAlphaForCharsRange(0, iIntPart, 255);
            if (iIntPart < m_sParsedText.Length) {
                m_rTextMesh.setCharAlphaFadeHorizEx(iIntPart, fFracPart, 0.5f, 255, 0);
                m_rTextMesh.setAlphaForCharsRange(iIntPart + 1, m_sParsedText.Length, 0);
            }
            m_rTextMesh.setAlphaEnd();
        }
    }
}
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

using TMPro;

using BinaryCharm.TextMeshProAlpha;

namespace BinaryCharm.Samples.TextMeshProAlpha.TextAnimation.Effects
{
    public class SmoothIncrementalAppear : ATextAnimation {

        private string m_sParsedText;
        private float m_fVisibleChars;

        private float m_fElapsedTime;
        private float m_fCharsPerSec;

        private const float fCHARACTERS_PER_SEC = 30f;

        public SmoothIncrementalAppear(TMP_Text rTextMesh, string sText,
                float fCharsPerSec = fCHARACTERS_PER_SEC
                ) : base(rTextMesh, sText) {

            m_fCharsPerSec = fCharsPerSec;
        }

        protected override void onStart() {
            m_fVisibleChars = 0f;
            m_fElapsedTime = 0f;

            // we need this to fetch the string without rich text tags
            m_rTextMesh.ForceMeshUpdate();
            m_sParsedText = m_rTextMesh.GetParsedText();
        }

        protected override void onUpdate(float fDT) {
            m_fElapsedTime += fDT;

            float fNextVisibleChars = m_fElapsedTime * m_fCharsPerSec;
            if (fNextVisibleChars > m_sParsedText.Length) {
                fNextVisibleChars = m_sParsedText.Length;
            }
            m_fVisibleChars = fNextVisibleChars;
        }

        public override bool isComplete() {
            return m_fVisibleChars == m_sParsedText.Length;
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
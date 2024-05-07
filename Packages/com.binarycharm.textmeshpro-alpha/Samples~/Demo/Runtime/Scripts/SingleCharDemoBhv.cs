/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2020 Binary Charm - All Rights Reserved
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

using BinaryCharm.Samples.UI;
using BinaryCharm.TextMeshProAlpha;

namespace BinaryCharm.Samples.TextMeshProAlpha
{
    public class SingleCharDemoBhv : ACornersDemoBhv
    {
        ///////////////////////////////////////////////////////////////////////
        #region INSPECTOR_REFS
        #pragma warning disable 649
        [SerializeField]
        private VarDisplayBhv m_rCharIdVarDisplay;
        #pragma warning restore 649
        #endregion
        ///////////////////////////////////////////////////////////////////////

        private int m_iCharId;

        private const string sSAMPLE_CODE =
@"textMesh.setAlphaBegin();
textMesh.setCharAlphaCornersFade(charIdx,
    topLeftAlpha, topRightAlpha,
    bottomRightAlpha, bottomLeftAlpha);
rTextMesh.setAlphaEnd();";

        ///////////////////////////////////////////////////////////////////////
        #region MONOBHV

        protected new void Awake() {
            base.Awake();
            m_rCodePreview.text = sSAMPLE_CODE;
            m_rCodePreview.readOnly = true;

            TMP_Text rTMP = m_rTextMesh.GetComponent<TMP_Text>();
            m_rCharIdVarDisplay.setup("charIdx", true,
                0f, (float)rTMP.text.Length - 1, 2f,
                (float f) => {
                    m_iCharId = (int)f;
                }
            );
        }

        void Update() {
            updateFadingText();
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
        
        private void updateFadingText() {
            m_rTextMesh.setAlphaBegin();
            m_rTextMesh.setCharAlphaCornersFade(m_iCharId,
                m_iTopLeftAlpha, m_iTopRightAlpha,
                m_iBottomRightAlpha, m_iBottomLeftAlpha);
            m_rTextMesh.setAlphaEnd();
        }

        ///////////////////////////////////////////////////////////////////////
    }
}
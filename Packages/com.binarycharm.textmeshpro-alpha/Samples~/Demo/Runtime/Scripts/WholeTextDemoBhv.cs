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

using BinaryCharm.TextMeshProAlpha;

namespace BinaryCharm.Samples.TextMeshProAlpha
{
    public class WholeTextDemoBhv : ACornersDemoBhv
    {
        ///////////////////////////////////////////////////////////////////////

        private const string sSAMPLE_CODE =
@"textMesh.setAlphaBegin();
textMesh.setTextAlphaCornersFade(
    topLeftAlpha, topRightAlpha,
    bottomRightAlpha, bottomLeftAlpha);
textMesh.setAlphaEnd();";

        ///////////////////////////////////////////////////////////////////////
        #region MONOBHV

        private new void Awake() {
            base.Awake();
            m_rCodePreview.text = sSAMPLE_CODE;
            m_rCodePreview.readOnly = true;
        }

        void Update() {
            updateFadingText();
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////

        private void updateFadingText() {
            m_rTextMesh.setAlphaBegin();
            m_rTextMesh.setTextAlphaCornersFade(
                m_iTopLeftAlpha, m_iTopRightAlpha,
                m_iBottomRightAlpha, m_iBottomLeftAlpha);
            m_rTextMesh.setAlphaEnd();
        }

        ///////////////////////////////////////////////////////////////////////
    }
}
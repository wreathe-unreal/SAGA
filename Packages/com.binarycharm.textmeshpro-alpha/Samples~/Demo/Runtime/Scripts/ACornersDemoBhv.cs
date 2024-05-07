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

namespace BinaryCharm.Samples.TextMeshProAlpha
{
    public abstract class ACornersDemoBhv : MonoBehaviour
    {
        #pragma warning disable 649
        [SerializeField]
        protected TMP_Text m_rTextMesh;

        [SerializeField]
        private VarDisplayBhv m_rTopLeftAlphaVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rTopRightAlphaVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rBottomRightAlphaVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rBottomLeftAlphaVarDisplay;

        [SerializeField]
        protected TMP_InputField m_rCodePreview;
        #pragma warning restore 0649

        protected byte m_iTopLeftAlpha;
        protected byte m_iBottomLeftAlpha;
        protected byte m_iTopRightAlpha;
        protected byte m_iBottomRightAlpha;

        protected void Awake() {
            m_rTopLeftAlphaVarDisplay.setup("topLeftAlpha",
                true, 0f, 255f, 255f,
                (float f) => m_iTopLeftAlpha = (byte)f
            );
            m_rTopRightAlphaVarDisplay.setup("topRightAlpha",
                true, 0f, 255f, 112f,
                (float f) => m_iTopRightAlpha = (byte)f
            );
            m_rBottomRightAlphaVarDisplay.setup("bottomRightAlpha",
                true, 0f, 255f, 0f,
                (float f) => m_iBottomRightAlpha = (byte)f
            );
            m_rBottomLeftAlphaVarDisplay.setup("bottomLeftAlpha",
                true, 0f, 255f, 112f,
                (float f) => m_iBottomLeftAlpha = (byte)f
            );
        }

    }
}
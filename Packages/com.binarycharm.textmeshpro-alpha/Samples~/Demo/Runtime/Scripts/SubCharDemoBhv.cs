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
using UnityEngine.UI;

using TMPro;

using BinaryCharm.Samples.UI;
using BinaryCharm.TextMeshProAlpha;

namespace BinaryCharm.Samples.TextMeshProAlpha
{
    public class SubCharDemoBhv : MonoBehaviour
    {

        ///////////////////////////////////////////////////////////////////////
        #region INSPECTOR_REFS
        #pragma warning disable 649
        [SerializeField]
        TMP_Text m_rTextMesh;

        [SerializeField]
        private Button m_rPlayBtn;

        [SerializeField]
        private Button m_rPauseBtn;

        [SerializeField]
        private VarDisplayBhv m_rVisibleCharsVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rLeftAlphaVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rRightAlphaVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rIntPartVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rFracPartVarDisplay;

        [SerializeField]
        private VarDisplayBhv m_rFadeWidthVarDisplay;

        [SerializeField]
        private TMP_InputField m_rCodePreview;
        #pragma warning restore 649
        #endregion
        ///////////////////////////////////////////////////////////////////////

        private float fFADE_IN_DURATION_SECS = 3f;

        private int m_iIntPart;
        private float m_f01FracPart;
        private float m_f01FadeWidth;
        private float m_fVisibleChars;
        private byte m_iLeftAlpha;
        private byte m_iRightAlpha;

        private bool m_bIsPlaying;

        private const string sSAMPLE_CODE =
@"textMesh.setAlphaBegin();
textMesh.setAlphaForCharsRange(0, intPart,
    leftAlpha);
textMesh.setCharAlphaFadeHorizEx(intPart,
    fracPart, fadeWidth, leftAlpha, rightAlpha);
textMesh.setAlphaForCharsRange(intPart + 1,
    textMesh.text.Length, rightAlpha);
textMesh.setAlphaEnd();";

        private const string sDEMO_TEXT =
            "This text smoothly fades in progressively!";

        ///////////////////////////////////////////////////////////////////////
        #region MONOBHV

        private void Awake() {
            m_rCodePreview.text = sSAMPLE_CODE;
            m_rCodePreview.readOnly = true;


            // set demo text preventing it to be drawn at full alpha for the
            // first frame
            m_rTextMesh.setAlphaText(sDEMO_TEXT, 0);

            // if you need a more sophisticated alpha setup from the first
            // frame, you can do something like this:
            // m_rTextMesh.setAlphaTextDontRender(sDEMO_TEXT);
            // m_rTextMesh.setAlphaBegin();
            // m_rTextMesh.setTextAlpha(0); // other stuff here
            // m_rTextMesh.setAlphaEnd();
            // m_rTextMesh.setAlphaTextRender();

            
            m_rPlayBtn.onClick.AddListener(() => {
                setPlaying(true);
            });
            m_rPauseBtn.onClick.AddListener(() => {
                setPlaying(false);
            });
            setPlaying(true);

            
            m_rVisibleCharsVarDisplay.setup("visibleChars", false, 
                0f, (float)m_rTextMesh.text.Length, 0f,
                (float f) => {
                    m_fVisibleChars = f;
                    int iPart = (int)f;
                    float fPart = f - iPart;
                    m_rIntPartVarDisplay.setValue(iPart);
                    m_rFracPartVarDisplay.setValue(fPart);
                }
            );

            m_rLeftAlphaVarDisplay.setup("leftAlpha", true,
                0f, 255f, 255f,
                (float f) => m_iLeftAlpha = (byte) f
            );

            m_rRightAlphaVarDisplay.setup("rightAlpha", true,
                0f, 255f, 0f,
                (float f) => m_iRightAlpha = (byte) f
            );

            m_rIntPartVarDisplay.setup("intPart", true,
                0f, (float)m_rTextMesh.text.Length, 0f,
                (float f) => {
                    m_iIntPart = (int)f;
                    m_rVisibleCharsVarDisplay.setValue(m_iIntPart + m_f01FracPart);
                }
            );

            m_rFracPartVarDisplay.setup("fracPart", false,
                0f, 0.999f, 0f,
                (float f) => {
                    m_f01FracPart = f;
                    m_rVisibleCharsVarDisplay.setValue(m_iIntPart + m_f01FracPart);
                }
            );

            m_rFadeWidthVarDisplay.setup("fadeWidth", false,
                0f, 1f, 0.5f,
                (float f) => m_f01FadeWidth = f
            );
        }

        void Update() {
            if (m_bIsPlaying) {
                float fValChange = (m_rTextMesh.text.Length / fFADE_IN_DURATION_SECS) * Time.deltaTime;

                m_fVisibleChars = Mathf.Clamp(m_fVisibleChars + fValChange,
                    0f, (float)m_rTextMesh.text.Length);
                if (m_fVisibleChars == (float)m_rTextMesh.text.Length) {
                    m_fVisibleChars = 0f;  // loop
                }
                m_rVisibleCharsVarDisplay.setValue(m_fVisibleChars);
            }
            updateFadingText();
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////

        private void setPlaying(bool bPlaying) {
            m_bIsPlaying = bPlaying;
            m_rPlayBtn.gameObject.SetActive(!bPlaying);
            m_rPauseBtn.gameObject.SetActive(bPlaying);
        }

        private void updateFadingText() {
            m_rTextMesh.setAlphaBegin();

            m_rTextMesh.setAlphaForCharsRange(0, m_iIntPart, m_iLeftAlpha);
            m_rTextMesh.setCharAlphaFadeHorizEx(m_iIntPart, m_f01FracPart,
                m_f01FadeWidth, m_iLeftAlpha, m_iRightAlpha);
            m_rTextMesh.setAlphaForCharsRange(m_iIntPart + 1,
                m_rTextMesh.text.Length, m_iRightAlpha);
            m_rTextMesh.setAlphaEnd();

            // Alternate version that automatically calls
            // setAlphaBegin() / setAlphaEnd(), but generates garbage:
            //
            //m_rTextMesh.setAlphaExecOps(new System.Action[] {
            //    () => m_rTextMesh.setAlphaForCharsRange(0, m_iIntPart, 
            //        m_iLeftAlpha),
            //    () => m_rTextMesh.setCharAlphaFadeHorizEx(m_iIntPart, 
            //        m_f01FracPart, m_f01FadeWidth, m_iLeftAlpha, m_iRightAlpha),
            //    () => m_rTextMesh.setAlphaForCharsRange(m_iIntPart + 1, 
            //        m_rTextMesh.text.Length, m_iRightAlpha)
            //});
        }
    }
}
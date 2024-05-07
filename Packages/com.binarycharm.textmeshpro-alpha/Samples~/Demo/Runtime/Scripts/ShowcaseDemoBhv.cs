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
using BinaryCharm.Samples.TextMeshProAlpha.TextAnimation;
using BinaryCharm.Samples.TextMeshProAlpha.TextAnimation.Effects;

namespace BinaryCharm.Samples.TextMeshProAlpha
{

    public class ShowcaseDemoBhv : MonoBehaviour
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
        private BulletSelectorBhv m_rBulletSelector;
        #pragma warning restore 649
        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region MONOBHV
        private void Awake() {
            setupShowcase();

            m_rPlayBtn.onClick.AddListener(() => {
                setPlaying(true);
            });
            m_rPauseBtn.onClick.AddListener(() => {
                setPlaying(false);
            });
            setPlaying(true);
        }

        void Update() {
            if (!m_bIsPlaying) return;

            IAnimation rCurrAnim = m_rSteps[m_iCurrStep];
            if (rCurrAnim.isComplete()) {
                int iNextStep = (m_iCurrStep + 1) % m_rSteps.Length;
                m_rBulletSelector.selectBullet(iNextStep);
            } else {
                rCurrAnim.update(Time.deltaTime);
            }
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region PLAYBACK

        private bool m_bIsPlaying;

        private void setPlaying(bool bPlaying) {
            m_bIsPlaying = bPlaying;
            m_rPlayBtn.gameObject.SetActive(!bPlaying);
            m_rPauseBtn.gameObject.SetActive(bPlaying);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region SHOWCASE
        private int m_iCurrStep;
        private IAnimation[] m_rSteps;

        private void setupShowcase() {
            string sWelcomeText = "Welcome to <b>TextMeshPro Alpha</b> Demo!";
            AnimationSequence rWelcomeAnim = new AnimationSequence(
                new IAnimation[] {
                    new LinearFade(m_rTextMesh, sWelcomeText, 3, 0, 255),
                    new AnimationPause(0.5f),
                    new LinearFade(m_rTextMesh, sWelcomeText, 1, 255, 0),
                    new AnimationPause(1f)
                }
            );

            string sMainDescText =
                "The included <b>TMP_Text_AlphaExtensions</b> static class " +
                "features a set of exension methods for fine grained, code " +
                "based control of text alpha values.\n" +
                "The class is based on native TextMeshPro features, and " +
                "works with no custom shaders or masks.";
            AnimationSequence rMainDescAnim = new AnimationSequence(
                new IAnimation[] {
                    new DialogueAppear(m_rTextMesh, sMainDescText),
                    new AnimationPause(1f),
                    new LinearFade(m_rTextMesh, sMainDescText, 1, 255, 0),
                    new AnimationPause(1f)
                }
            );

            string sWholeMeshText =
                "You can easily implement fading effects on a whole text, " +
                "setting alpha values for the four corners of the mesh, " +
                "like this...";
            AnimationSequence rWholeMeshAnim = new AnimationSequence(
                new IAnimation[] {
                    new WholeTextClockwiseFade(m_rTextMesh, sWholeMeshText,
                        4f, 0, 255, 1f),
                    new AnimationPause(0.5f),
                    new WholeTextClockwiseFade(m_rTextMesh, sWholeMeshText,
                        2f, 255, 0, 1.5f, 3),
                }
            );

            string sSingleCharText =
                "Or you can set the alpha values for the four corners of each " +
                "character, for more sophisticated effects, like this.";
            AnimationSequence rSingleCharAnim = new AnimationSequence(
                new IAnimation[] {
                    new RandomFade(m_rTextMesh, sSingleCharText, 3f, 0, 255, 1f),
                    new RandomFade(m_rTextMesh, sSingleCharText, 0.5f, 255, 60, 1f),
                    new RandomFade(m_rTextMesh, sSingleCharText, 0.5f, 60, 255, 1f),
                    new RandomFade(m_rTextMesh, sSingleCharText, 2f, 255, 0, 1f)
                }
            );

            string sSubCharText =
                "Two methods for smooth, horizontal or vertical " +
                "sub-character fading are also included.";
            AnimationSequence rSubCharAnim = new AnimationSequence(
                new IAnimation[] {
                    new VerticalRandomFade(m_rTextMesh, sSubCharText,
                        1.5f, 0, 255, 1f),
                    new SubCharTestFade(m_rTextMesh, sSubCharText, 8f),
                    new AnimationPause(1f),
                    new VerticalRandomFade(m_rTextMesh, sSubCharText,
                        1f, 255, 0, 0.7f),
                    new AnimationPause(2f)
                }
            );

            m_rSteps = new IAnimation[] {
                rWelcomeAnim,
                rMainDescAnim,
                rWholeMeshAnim,
                rSingleCharAnim,
                rSubCharAnim
            };

            m_rBulletSelector.setup(m_rSteps.Length, (int iStepId) => {
                m_iCurrStep = iStepId;
                m_rSteps[m_iCurrStep].start();
                setPlaying(true);
            });
            m_rBulletSelector.selectBullet(0);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////
    }
}
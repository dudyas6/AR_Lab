/* -------------------------------
   Created by : Hamza Herbou
   hamza95herbou@gmail.com
---------------------------------- */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EasyUI.Popup;
using UnityEngine.Events;
using System.Collections.Generic;
using System;


namespace EasyUI.Helpers
{

    public class PopupUI : MonoBehaviour
    {
        [Header("UI References :")]
        [SerializeField] private GraphicRaycaster uiCanvasGraphicRaycaster;
        [SerializeField] private CanvasGroup uiCanvasGroup;
        [SerializeField] private GameObject uiHeader;
        [SerializeField] private Text uiTitle;
        [SerializeField] private Text uiText;
        [SerializeField] private Image uiImage;
        [SerializeField] private Image uiButtonImage;
        [SerializeField] private Text uiButtonText;
        private Button uiButton;
        [Header("Popup Colors :")]
        [SerializeField] private Color[] colors;

        [Header("Popup Fade Duration :")]
        [Range(.1f, .8f)][SerializeField] private float fadeInDuration = .3f;
        [Range(.1f, .8f)][SerializeField] private float fadeOutDuration = .3f;

        [Header("Page Indicators:")]
        [SerializeField] private Transform indicatorsParent;
        [SerializeField] private GameObject emptyDotPrefab;
        [SerializeField] private GameObject filledDotPrefab;
        private List<GameObject> pageIndicators = new List<GameObject>();

        [Space]
        public int defaultMaxTextLength = 200;
        private float swipeThreshold = 50f;
        public string defaultButtonText = "CLOSE";
        private Vector2 mouseStartPos;
        private Vector2 mouseEndPos;
        private int amountPages = 0;
        private bool isPopupActive = false;
        private int currentPage = 0;
        private List<string> textPages;
        private UnityAction onCloseAction;

        void Awake()
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGraphicRaycaster.enabled = false;
            uiButton = uiButtonImage.GetComponent<Button>();
            uiButton.onClick.RemoveAllListeners();
            uiButton.onClick.AddListener(() => {
                if (onCloseAction != null)
                {
                    onCloseAction.Invoke();
                    onCloseAction = null;
                }

                StartCoroutine(FadeOut(fadeOutDuration));
            });
        }

        // added optional parameters pageDelimeter and customMaxTextLength, in order to have more control over pop up page displays, if not passed previous default behavior will apply.
        public void Show(string title, string text, string buttonText, PopupColor color, UnityAction action, string pageDelimiter = null, int customMaxTextLength = -1, bool exitFlag = false)
        {
            int maxTextLength = (customMaxTextLength == -1) ? defaultMaxTextLength : customMaxTextLength;
            ResetAllValues();
            ScreensManager.Instance().setPopupActive(true);
            if (string.IsNullOrEmpty(title.Trim()))
                uiHeader.SetActive(false);
            else
            {
                uiHeader.SetActive(true);
                uiTitle.text = title;
            }
            uiImage.sprite = null; //to not save the sprite if privious popup showed picture

            if (text.Length > maxTextLength || pageDelimiter != null)
            {
                uiText.text = text.Substring(0, maxTextLength);
                textPages = SplitTextIntoPages(text, maxTextLength, pageDelimiter);
                amountPages = textPages.Count;
            }
            else
            {
                uiText.text = text;
                textPages = null;
                amountPages = 1;
            }
            CreatePageIndicators(amountPages);
            UpdatePageIndicators();
            uiButtonText.text = buttonText;

            Color c = colors[(int)color];
            Color ct = c;
            ct.a = .75f;
            uiTitle.color = ct;
            uiButtonImage.color = c;

            onCloseAction = action;

            Dismiss();
            StartCoroutine(FadeIn(fadeInDuration));
        }

        private void ResetAllValues()
        {
            amountPages = 0;
            currentPage = 0;
            isPopupActive = false;
            textPages = null;
        }

        //*****added*******
        public void ShowWithImage(string title, string text, string buttonText, PopupColor color, UnityAction action, Sprite sprite2)
        {
            if (string.IsNullOrEmpty(title.Trim()))
                uiHeader.SetActive(false);
            else
            {
                uiHeader.SetActive(true);
                uiTitle.text = title;
            }
            Debug.Log("3 --> " + sprite2);
            uiImage.sprite = sprite2;

            Debug.Log("4 --> " + sprite2);
            uiText.text = (text.Length > defaultMaxTextLength) ? text.Substring(0, defaultMaxTextLength) + "..." : text;

            uiButtonText.text = buttonText;

            Color c = colors[(int)color];
            Color ct = c;
            ct.a = .75f;
            uiTitle.color = ct;
            uiButtonImage.color = c;
            onCloseAction = action;

            Dismiss();
            StartCoroutine(FadeIn(fadeInDuration));

        }

        private IEnumerator FadeIn(float duration)
        {
            uiCanvasGraphicRaycaster.enabled = true;
            yield return Fade(uiCanvasGroup, 0f, 1f, duration);
            uiCanvasGroup.interactable = true;
        }

        private IEnumerator FadeOut(float duration)
        {
            yield return Fade(uiCanvasGroup, 1f, 0f, duration);
            uiCanvasGroup.interactable = false;
            uiCanvasGraphicRaycaster.enabled = false;
            ScreensManager.Instance().setPopupActive(false);
        }

        private IEnumerator Fade(CanvasGroup cGroup, float startAlpha, float endAlpha, float duration)
        {
            float startTime = Time.time;
            float alpha = startAlpha;

            if (duration > 0f)
            {
                //Anim start
                while (alpha != endAlpha)
                {
                    alpha = Mathf.Lerp(startAlpha, endAlpha, (Time.time - startTime) / duration);
                    cGroup.alpha = alpha;

                    yield return null;
                }
            }

            cGroup.alpha = endAlpha;
        }

        public void Dismiss()
        {
            StopAllCoroutines();
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGraphicRaycaster.enabled = false;
        }

        private void OnDestroy()
        {
            EasyUI.Popup.Popup._isLoaded = false;
        }

        void Update()
        {
            if (isPopupActive)
                HandleSwipe();
        }

        void HandleSwipe()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseStartPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseEndPos = Input.mousePosition;
                CheckSwipe();
            }
        }

        void CheckSwipe()
        {
            if (textPages == null)
                return;
            float swipeDistance = Vector2.Distance(mouseStartPos, mouseEndPos);
            if (swipeDistance > swipeThreshold)
            {
                float swipeDirection = Mathf.Sign(mouseEndPos.x - mouseStartPos.x);
                if (swipeDirection > 0)
                {
                    if (currentPage == 0)
                        return;
                    currentPage -= 1;
                }
                else if (swipeDirection < 0)
                {
                    if (currentPage == amountPages - 1)
                        return;
                    currentPage += 1;
                }
                uiText.text = textPages[currentPage];
                UpdatePageIndicators();
            }
        }
        private List<string> SplitTextIntoPages(string text, int maxLength, string pageDelimiter = null)
        {
            List<string> pages = new List<string>();
            List<string> valid_blocks = new List<string>();

            if (pageDelimiter != null)
            {
                string[] splittedByDelimeter = text.Split(pageDelimiter);
                foreach (string block in splittedByDelimeter)
                {
                    if (block.Length > maxLength)
                    {
                        valid_blocks = getValidLengthTextBlocks(block, maxLength);
                        foreach (string valid_block in valid_blocks)
                        {
                            pages.Add(valid_block);
                        }
                    }
                    else
                    {
                        pages.Add(block);
                    }
                }
                return pages;
            }
            if (text.Length <= maxLength)
            {
                pages.Add(text);
            }
            else
            {
                pages = getValidLengthTextBlocks(text, maxLength);
            }

            return pages;
        }

        List<string> getValidLengthTextBlocks(string text, int maxLength)
        {
            List<string> textBlocks = new List<string>();

            for (int i = 0; i < text.Length; i += maxLength)
            {
                int length = Mathf.Min(maxLength, text.Length - i);
                string textBlock = text.Substring(i, length);
                textBlocks.Add(textBlock);
            }

            return textBlocks;
        }

        void CreatePageIndicators(int pageCount)
        {
            foreach (GameObject p in pageIndicators)
            {
                if (p != null)
                {
                    Destroy(p);
                }
            }
            pageIndicators.Clear();
            if (pageCount == 1)
            {
                return;
            }
            // Create indicators based on the number of pages
            for (int i = 0; i < pageCount; i++)
            {
                GameObject indicator = Instantiate(emptyDotPrefab, indicatorsParent);
                pageIndicators.Add(indicator);
            }
        }

        void UpdatePageIndicators()
        {
            float totalWidth = (pageIndicators.Count - 1) * 80;
            float startX = -totalWidth / 2f;
            for (int i = 0; i < pageIndicators.Count; i++)
            {
                GameObject indicatorPrefab = (i == currentPage) ? filledDotPrefab : emptyDotPrefab;
                GameObject indicator = Instantiate(indicatorPrefab, indicatorsParent);
                indicator.transform.localPosition = new Vector3(startX + i * 80, -100f, 0f);
                Destroy(pageIndicators[i]);
                pageIndicators[i] = indicator;
            }
        }


    }

}

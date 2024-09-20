using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Netherlands3D.Interface
{
    public class TooltipDialog : MonoBehaviour
    {
        [Tooltip("Offset of the tooltip in relation to the element")]
        [SerializeField] private Vector2 offset;
        
        private TextMeshProUGUI tooltiptext;
        private RectTransform rectTransform;
        private RectTransform lastTarget;
        private RectTransform currentTarget;
        private ContentSizeFitter contentSizeFitter;
        private Sequence animationSequence;
        private const float animationDuration = 0.25f;
        private Vector3[] worldCorners = new Vector3[4];

        #region Singleton

        private static TooltipDialog instance;

        public static TooltipDialog Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("No tooltip object instance found. Make it is active in your scene.");
                }

                return instance;
            }
        }

        #endregion

        private void Awake()
        {
            instance = this;

            contentSizeFitter = GetComponent<ContentSizeFitter>();
            tooltiptext = GetComponentInChildren<TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();
            Hide();
        }

        private void FollowPointer()
        {
            rectTransform.position = Mouse.current.position.ReadValue();

            SwapPivot();
        }

        private void SwapPivot()
        {
            var pivot = Vector2.zero;
            //Swap pivot based on place in screen (to try to stay in the screen horizontally)
            if (rectTransform.position.x + GetRectTransformBounds(rectTransform).size.x > Screen.width)
            {
                pivot.x = 1;
            }

            //Swap pivot based on place in screen (to try to stay in the screen vertically)
            if (rectTransform.position.y + GetRectTransformBounds(rectTransform).size.y > Screen.height)
            {
                pivot.y = 1;
            }

            rectTransform.pivot = pivot;
        }

        public void AlignOnElement(RectTransform element)
        {
            if (!element) return;

            lastTarget = currentTarget;
            currentTarget = element;

            var elementCenter = GetRectTransformBounds(element).center;
            var elementMax = GetRectTransformBounds(element).max;
            
            var tooltipPosition = new Vector2(elementMax.x, elementCenter.y) + offset;
            rectTransform.position = tooltipPosition;
            
            SwapPivot();
        }

        public void ShowMessage(string message = "Tooltip", RectTransform hoverTarget = null)
        {
            gameObject.transform.SetAsLastSibling(); //Make sure we are in front of all the UI

            AlignOnElement(hoverTarget);

            gameObject.SetActive(false);
            gameObject.SetActive(true);

            tooltiptext.text = message;

            StartCoroutine(FitContent());

            StartAnimation(1f, () => 
            {
               
            });
        }

        private IEnumerator FitContent()
        {
            yield return new WaitForEndOfFrame();
            contentSizeFitter.enabled = false;
            contentSizeFitter.enabled = true;
        }

        public void Hide()
        {   
            StartAnimation(0f, ()=> {
                if(lastTarget == currentTarget || currentTarget == null)
                    gameObject.SetActive(false);
            });
        }

        private Bounds GetRectTransformBounds(RectTransform transform)
        {
            transform.GetWorldCorners(worldCorners);
            Bounds bounds = new Bounds(worldCorners[0], Vector3.zero);
            for (int i = 1; i < 4; ++i)
            {
                bounds.Encapsulate(worldCorners[i]);
            }

            return bounds;
        }


        private void StartAnimation(float targetScale, Action onFinish = null)
        {
            // If the animation is playing, quickly complete it and then start a new one
            if (animationSequence != null && animationSequence.IsPlaying())
            {
                animationSequence.Complete(true);
            }  
            animationSequence = CreateAnimationSequence(targetScale, onFinish);
            animationSequence.Play();
        }

        private Sequence CreateAnimationSequence(float scale, Action onFinish = null)
        {
            Sequence sequence = DOTween.Sequence(rectTransform);
            sequence.SetEase(scale > 0 ? Ease.OutBounce : Ease.InBack);            
            sequence.Join(rectTransform.DOScale(scale, scale > 0 ? animationDuration : 0.5f * animationDuration));

            // Ensure animation sequence is nulled after completing to clean up
            sequence.OnComplete(() =>
            {
                onFinish?.Invoke();
                animationSequence = null;                
            });

            return sequence;
        }
    }
}
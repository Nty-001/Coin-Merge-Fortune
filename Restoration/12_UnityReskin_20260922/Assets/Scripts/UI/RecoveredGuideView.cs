using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    [DefaultExecutionOrder(50)] public sealed class RecoveredGuideView : MonoBehaviour
    {
        public GameObject stepZero,stepOne,stepThree,stepFour,mask;
        public Button oneButton,threeButton,fourButton,backdropButton;
        public Text balance;
        [Header("Cash tutorial target")]
        public RectTransform cashHand,cashTarget,cashPanel;
        public Vector2 cashTargetPoint=new Vector2(.65f,.45f);
        public float cashPanelGap=16;
        [Header("Coin tutorial target")]
        public RectTransform coinAnchor,coinTarget,coinContent;
        public RecoveredLocalization Locale {get;set;}
        public event Action Advanced;
        void Awake(){oneButton.onClick.AddListener(Advance);threeButton.onClick.AddListener(Advance);fourButton.onClick.AddListener(Advance);backdropButton.onClick.AddListener(Advance);}
        void Advance(){Advanced?.Invoke();}
        void LateUpdate()
        {
            if(stepThree.activeSelf&&cashHand&&cashTarget&&cashPanel)AlignCashGuide();
            if(stepFour.activeSelf&&coinAnchor&&coinTarget&&coinContent)
                coinContent.position+=coinTarget.position-coinAnchor.position;
        }
        public void AlignCashGuide()
        {
            // Use the actual responsive HUD, not coordinates from the 750x1624 source canvas.
            Rect target=cashTarget.rect;
            cashHand.position=cashTarget.TransformPoint(new Vector3(
                Mathf.Lerp(target.xMin,target.xMax,cashTargetPoint.x),
                Mathf.Lerp(target.yMin,target.yMax,cashTargetPoint.y),0));
            var root=(RectTransform)transform;
            Vector3 bottom=root.InverseTransformPoint(cashHand.TransformPoint(new Vector3(0,cashHand.rect.yMin,0)));
            Vector3 panelTop=root.InverseTransformPoint(cashPanel.TransformPoint(new Vector3(0,cashPanel.rect.yMax,0)));
            Vector3 center=root.InverseTransformPoint(cashPanel.position);
            cashPanel.position=root.TransformPoint(new Vector3(root.rect.center.x,
                bottom.y-cashPanelGap-(panelTop.y-center.y),center.z));
        }
        public void Show(int step,double money)
        {
            stepZero.SetActive(step==0);stepOne.SetActive(step==1);stepThree.SetActive(step==3);stepFour.SetActive(step==4);
            mask.SetActive(step!=0);balance.text=Locale.Money(money);
            gameObject.SetActive(step==0||step==1||step==3||step==4);
        }
        void OnDestroy(){oneButton.onClick.RemoveListener(Advance);threeButton.onClick.RemoveListener(Advance);fourButton.onClick.RemoveListener(Advance);backdropButton.onClick.RemoveListener(Advance);}
    }
}

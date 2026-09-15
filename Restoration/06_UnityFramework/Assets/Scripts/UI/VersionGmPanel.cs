using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class VersionGmPanel : MonoBehaviour
    {
        public GameVersionRouter router;
        public GameObject popup;
        public Button open,close,backdrop,apply,content,cohort,regionPrevious,regionNext,shareDown,shareUp,reset;
        public Text contentValue,cohortValue,regionValue,shareValue,status;
        public bool IsOpen=>popup.activeSelf;
        public VersionProfile Draft {get;private set;}
        void Awake()
        {
            open.onClick.AddListener(Show);close.onClick.AddListener(Hide);backdrop.onClick.AddListener(Hide);
            apply.onClick.AddListener(Apply);content.onClick.AddListener(CycleContent);cohort.onClick.AddListener(CycleCohort);
            regionPrevious.onClick.AddListener(PreviousRegion);regionNext.onClick.AddListener(NextRegion);
            shareDown.onClick.AddListener(LessShare);shareUp.onClick.AddListener(MoreShare);reset.onClick.AddListener(ResetCurrent);
        }
        public void Show(){Draft=VersionRouting.Copy(router.Profile);popup.SetActive(true);Refresh();}
        public void Hide(){popup.SetActive(false);}
        void Apply(){router.Apply(Draft);}
        void CycleContent(){Draft.contentMode=(Draft.contentMode+1)%3;Refresh();}
        void CycleCohort(){Draft.cohortMode=(Draft.cohortMode+1)%3;Refresh();}
        void PreviousRegion(){MoveRegion(-1);}void NextRegion(){MoveRegion(1);}
        void MoveRegion(int delta)
        {var list=router.balance.rules.supportedCountries;int index=System.Array.IndexOf(list,Draft.country);Draft.country=list[(index+delta+list.Length)%list.Length];Refresh();}
        void LessShare(){Draft.rewardedShare=Mathf.Max(0,Draft.rewardedShare-10);Refresh();}
        void MoreShare(){Draft.rewardedShare=Mathf.Min(100,Draft.rewardedShare+10);Refresh();}
        void ResetCurrent(){router.ResetCurrentPlayer();Refresh();}
        void Refresh()
        {
            VersionRouting.Resolve(Draft);
            contentValue.text=Draft.contentMode==0?"自动分流（本地测试）":Draft.contentMode==1?"基础版 / 内置包":"收益版 / 热更新包";
            cohortValue.text=Draft.cohortMode==0?"自动："+Draft.cohort:Draft.cohort;
            regionValue.text=Draft.country;shareValue.text=Draft.rewardedShare+"% 收益版";
            shareDown.interactable=shareUp.interactable=Draft.contentMode==0;
            status.text="将进入："+(Draft.rewardedVariant?"收益版":"基础版")+" | AB "+Draft.cohort+" | "+Draft.country+
                "\n测试桶："+Draft.localBucket+" / 尾号："+Draft.localTail+
                "\n原包 AB 分组与内容分流独立。\n清档保留分流；应用后重新进入场景。";
        }
        void OnDestroy()
        {
            open.onClick.RemoveListener(Show);close.onClick.RemoveListener(Hide);backdrop.onClick.RemoveListener(Hide);
            apply.onClick.RemoveListener(Apply);content.onClick.RemoveListener(CycleContent);cohort.onClick.RemoveListener(CycleCohort);
            regionPrevious.onClick.RemoveListener(PreviousRegion);regionNext.onClick.RemoveListener(NextRegion);
            shareDown.onClick.RemoveListener(LessShare);shareUp.onClick.RemoveListener(MoreShare);reset.onClick.RemoveListener(ResetCurrent);
        }
    }
}

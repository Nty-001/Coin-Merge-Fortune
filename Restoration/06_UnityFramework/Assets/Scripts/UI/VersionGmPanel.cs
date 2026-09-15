using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class VersionGmPanel : MonoBehaviour
    {
        public GameVersionRouter router;
        public GameObject popup;
        public Button open,close,backdrop,apply,content,baseVersion,rewardedVersion,regionPrevious,regionNext,shareDown,shareUp,reset;
        public Text contentValue,baseValue,rewardedValue,cohortValue,regionValue,shareValue,status;
        public bool IsOpen=>popup.activeSelf;
        public VersionProfile Draft {get;private set;}
        void Awake()
        {
            open.onClick.AddListener(Show);close.onClick.AddListener(Hide);backdrop.onClick.AddListener(Hide);
            apply.onClick.AddListener(Apply);content.onClick.AddListener(ToggleAutomatic);
            baseVersion.onClick.AddListener(SelectBase);rewardedVersion.onClick.AddListener(SelectRewarded);
            regionPrevious.onClick.AddListener(PreviousRegion);regionNext.onClick.AddListener(NextRegion);
            shareDown.onClick.AddListener(LessShare);shareUp.onClick.AddListener(MoreShare);reset.onClick.AddListener(ResetCurrent);
        }
        public void Show(){Draft=VersionRouting.Copy(router.Profile);popup.SetActive(true);Refresh();}
        public void Hide(){popup.SetActive(false);}
        void Apply(){router.Apply(Draft);}
        // A/B are explicit LOCAL content test names. Original cohort metadata stays independent.
        void SelectBase(){Draft.contentMode=VersionRouting.Packaged;Refresh();}
        void SelectRewarded(){Draft.contentMode=VersionRouting.Rewarded;Refresh();}
        void ToggleAutomatic()
        {Draft.contentMode=Draft.contentMode==VersionRouting.Automatic?(Draft.rewardedVariant?VersionRouting.Rewarded:VersionRouting.Packaged):VersionRouting.Automatic;Refresh();}
        void PreviousRegion(){MoveRegion(-1);}void NextRegion(){MoveRegion(1);}
        void MoveRegion(int delta)
        {var list=router.balance.rules.supportedCountries;int index=System.Array.IndexOf(list,Draft.country);Draft.country=list[(index+delta+list.Length)%list.Length];Refresh();}
        void LessShare(){Draft.rewardedShare=Mathf.Max(0,Draft.rewardedShare-10);Refresh();}
        void MoreShare(){Draft.rewardedShare=Mathf.Min(100,Draft.rewardedShare+10);Refresh();}
        void ResetCurrent(){router.ResetCurrentPlayer();Refresh();}
        void Refresh()
        {
            VersionRouting.Resolve(Draft);
            baseValue.text=Draft.contentMode==VersionRouting.Packaged?"已选 A：基础版":"A：基础版";
            rewardedValue.text=Draft.contentMode==VersionRouting.Rewarded?"已选 B：收益版":"B：收益版";
            contentValue.text=Draft.contentMode==VersionRouting.Automatic?"自动比例分流：开（点击关闭）":"自动比例分流：关（点击开启）";
            cohortValue.text=Draft.cohort+"（仅记录，不切换界面）";
            regionValue.text=Draft.country;shareValue.text=Draft.contentMode==VersionRouting.Automatic?Draft.rewardedShare+"% 进入 B 收益版":"仅自动分流时生效";
            shareDown.interactable=shareUp.interactable=Draft.contentMode==0;
            status.text="当前："+(router.isRewarded?"B 收益版":"A 基础版")+" / "+router.Profile.country+
                "\n应用后："+(Draft.rewardedVariant?"B 收益版":"A 基础版")+" / "+Draft.country+
                "\n切换保留各自进度；清档保留版本选择。";
        }
        void OnDestroy()
        {
            open.onClick.RemoveListener(Show);close.onClick.RemoveListener(Hide);backdrop.onClick.RemoveListener(Hide);
            apply.onClick.RemoveListener(Apply);content.onClick.RemoveListener(ToggleAutomatic);
            baseVersion.onClick.RemoveListener(SelectBase);rewardedVersion.onClick.RemoveListener(SelectRewarded);
            regionPrevious.onClick.RemoveListener(PreviousRegion);regionNext.onClick.RemoveListener(NextRegion);
            shareDown.onClick.RemoveListener(LessShare);shareUp.onClick.RemoveListener(MoreShare);reset.onClick.RemoveListener(ResetCurrent);
        }
    }
}

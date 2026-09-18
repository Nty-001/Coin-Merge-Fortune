using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class GameplayGmPanel : MonoBehaviour
    {
        public VersionGmPanel owner;
        public GameObject versions,progress,events;
        public MenuActionBinding[] actions;
        public InputField number;
        public Text progressStatus,eventStatus,productLabel,taskLabel,slotLabel;
        public int selectedProduct,selectedTask,selectedSlot;
        public bool cashRoute;
        public Text routeLabel;
        public double counterLimit=1000000000;
        UnityAction[] callbacks;
        string message="GM 修改当前测试存档；准备条件不会跳过验证动画。";
        RecoveredGameSession Session=>owner.router.rewarded;
        PlayerProgress Player=>Session.Player;
        CashGroup Group=>RecoveredGameRules.CashConfiguration(Session.board.Config.rules,Session.Profile.country);
        WithdrawProduct[] Products=>cashRoute?Group.real_products:Group.new_Fake_products;
        WithdrawProduct Product=>Products[selectedProduct];
        int TaskCount=>cashRoute?4:5;
        static readonly string[] Tasks={"2000 筹码总数","看完广告","现金余额","有效活跃天数","累计看完视频"};
        static readonly string[] CashTasks={"现金余额","2000 筹码总数","有效活跃天数","累计看完视频"};
        static readonly string[] Outcomes={"看完了","中途取消","没有广告可播","播放失败"};
        void Awake()
        {
            callbacks=new UnityAction[actions.Length];
            for(int i=0;i<actions.Length;i++){int code=actions[i].action;callbacks[i]=()=>Execute(code);actions[i].button.onClick.AddListener(callbacks[i]);}
        }
        public void OpenVersions(){versions.SetActive(true);progress.SetActive(false);events.SetActive(false);}
        public void Execute(int code)
        {
            if(code==0){OpenVersions();return;}
            if(!Session||Session.Player==null)
            {
                versions.SetActive(false);progress.SetActive(true);events.SetActive(false);
                progressStatus.text="基础版没有收益提现链。\n请在版本页选择 B 收益版并应用。";return;
            }
            if(code==1||code==2){versions.SetActive(false);progress.SetActive(code==1);events.SetActive(code==2);Refresh();return;}
            switch(code)
            {
                case 10:selectedProduct=(selectedProduct+Products.Length-1)%Products.Length;break;
                case 11:selectedProduct=(selectedProduct+1)%Products.Length;break;
                case 12:selectedTask=(selectedTask+TaskCount-1)%TaskCount;break;
                case 13:selectedTask=(selectedTask+1)%TaskCount;break;
                case 14:PrepareTask(false);break;
                case 15:PrepareTask(true);break;
                case 16:AdvanceHours(12);break;
                case 17:AdvanceHours(24);break;
                case 18:Player.gmTimeOffsetSeconds=0;Player.RecordLoginDay(PlayerClock.Today(Player),owner.router.balance.rules.flow.validLoginMergeCount);message="仅恢复当前存档的时间偏移，不回退已获得的天数。";break;
                case 19:AddHighest(1);break;
                case 20:AddHighest(5);break;
                case 21:if(ReadNumber(out double cash))Player.fakeMoney=cash;break;
                case 22:if(ReadNumber(out double coins))Player.coin1024Number=(int)coins;break;
                case 23:if(ReadNumber(out double ads))Player.watch_video_count=(int)ads;break;
                case 24:owner.Hide();Session.menus.CloseAll();Session.menus.Act(3);Session.menus.Act(1000+selectedProduct);break;
                case 25:owner.Hide();Session.menus.CloseAll();Session.menus.Act(4);Session.menus.Act(1100+selectedProduct);break;
                case 26:Player.watch_video_count++;message="广告成功统计 +1（未发放现金奖励）。";break;
                case 27:Player.fakeMoney+=1;message="现金 +1；返回提现页查看进度。";break;
                case 30:case 31:case 32:case 33:Session.Sdk.NextAdOutcome=(AdOutcome)(code-30);message="后续 Mock 广告结果："+Outcomes[code-30]+"（持续生效直到再次选择）。";break;
                case 34:Session.GmPrepareDrop(owner.router.balance.rules.flow.firstRewardDrop);message="已准备普通奖励前一次；关闭 GM 后投币或点击真实投币。";break;
                case 35:Session.GmPrepareDrop(owner.router.balance.rules.flow.secondRewardDrop);message="已准备第二次普通奖励前一次。";break;
                case 36:Session.GmPrepareDrop(owner.router.balance.rules.flow.adRewardDrop);message="已准备 1_A 广告前一次。";break;
                case 37:if(!Ready())break;owner.Hide();Session.board.InputBlocked=false;message=Session.board.RequestDrop()?"已通过真实投币入口请求。":"预览金币尚未就绪，请稍后再试。";break;
                case 38:if(!Ready())break;owner.Hide();Session.board.InputBlocked=false;Session.board.TriggerFailure();break;
                case 39:if(!Ready())break;owner.Hide();Session.board.InputBlocked=false;
                    var board=Session.board;var point=new Vector3(board.transform.position.x,board.ground.position.y+200/board.Units,0);
                    var first=board.Spawn(1000,point+Vector3.left*20/board.Units);var second=board.Spawn(1000,point+Vector3.right*20/board.Units);
                    board.Merge(first,second);break;
                case 40:if(!Ready())break;owner.Hide();Session.ShowReward(3);break;
                case 41:if(!Ready())break;owner.Hide();Session.ShowReward(2);break;
                case 42:if(!Ready())break;owner.Hide();Session.ShowReward(5);break;
                case 43:selectedSlot=(selectedSlot+1)%owner.router.balance.rules.lotteryRewards.Length;break;
                case 44:Session.GmSetNextWheel(selectedSlot);Player.gameTotalScore=RecoveredGameRules.RequiredScore(owner.router.balance.rules.lotteryScores,Player.currentLotteryCount)-1;message="已设为转盘差 1 分；指定格子只影响下一次正常抽奖。";break;
                case 45:Player.gameTotalScore++;message="转盘分数 +1；关闭 GM 后等待正常弹窗。";break;
                case 46:Player.guideStep=9999;Session.guideView.gameObject.SetActive(false);message="已跳过新手引导（测试准备）。";break;
                case 47:Session.Sdk.Trace.Clear();message="已清空本次内存广告日志。";break;
                case 48:Player.raccountName=Player.rfullName=Player.rdocumentId=Player.raccountType="";message="已清除测试账户，下次申请将重新填写。";break;
                case 49:cashRoute=!cashRoute;selectedTask=0;message="已切换准备路线；点击差 1 或达标才会改存档。";break;
                case 50:
                    Player.today1024NumberCoinDate=PlayerClock.Today(Player);Player.today1024NumberCoin=Math.Max(0,owner.router.balance.rules.flow.validLoginMergeCount-1);
                    Player.lastLoginDate="";Player.isNewLoginDay=false;
                    message="今天已合成 4 枚，再点 2000 筹码 +1 可增加 1 个有效天。此按钮重置今天的达标标记，仅供测试。";break;
                case 51:Session.GmPrepareDrop(owner.router.balance.rules.flow.firstRewardDrop);Player.dropCointimes=owner.router.balance.rules.flow.firstRewardDrop-1;
                    message="已准备首次第 10 次投币；下一次真实投币后，奖励关闭再等 1.5 秒看评分。";break;
                case 52:if(!Ready())break;owner.Hide();Session.board.InputBlocked=false;Player.firstMergeIcon500=false;Player.gameRateScore=0;
                    var ratingBoard=Session.board;var position=new Vector3(ratingBoard.transform.position.x,ratingBoard.ground.position.y+220/ratingBoard.Units,0);
                    ratingBoard.Merge(ratingBoard.Spawn(200,position+Vector3.left*20/ratingBoard.Units),ratingBoard.Spawn(200,position+Vector3.right*20/ratingBoard.Units));break;
            }
            Session.GmRefresh();Refresh();
        }
        bool Ready(){if(Session.GmCanTrigger)return true;message="请先关闭业务弹窗/引导，或完成失败复活，再触发新事件。";return false;}
        bool ReadNumber(out double value)
        {
            bool valid=double.TryParse(number.text,NumberStyles.Float,CultureInfo.InvariantCulture,out value)&&!double.IsNaN(value)&&!double.IsInfinity(value)&&value>=0&&value<=counterLimit;
            message=valid?"已应用测试数值。":"请输入 0 到 "+counterLimit.ToString(CultureInfo.InvariantCulture)+" 的数字（小数点用 .）。";return valid;
        }
        public void AdvanceHours(double hours)
        {PlayerClock.Advance(Player,hours,owner.router.balance.rules.flow.validLoginMergeCount);message="时间 +"+hours+"h；跨天仅重置当日计数，完成每日合成才增加有效天数。";}
        public void AddHighest(int count)
        {
            for(int i=0;i<count;i++)Player.AddHighestCoinMerge(PlayerClock.Today(Player),owner.router.balance.rules.flow.validLoginMergeCount);
            message="模拟最高金币统计 +"+count+"，沿用每日达标判定；不会制造物理金币或奖励弹窗。";
        }
        public void PrepareTask(bool ready)
        {
            var p=Product;Player.guideStep=9999;Session.guideView.gameObject.SetActive(false);
            if(cashRoute){PrepareCashTask(p,ready);return;}
            Player.newFakeMoneyWithdraw[selectedProduct]=selectedTask;
            Player.coin1024Number=selectedTask>0?p.condition_merge:0;
            Player.watch_video_count=selectedTask>1?p.condition_ad:0;
            Player.fakeMoney=selectedTask>2?p.withdrawAmount:0;
            Player.loginDays=selectedTask>3?p.condition_login_days:0;
            Player.today1024NumberCoin=0;Player.today1024NumberCoinDate=PlayerClock.Today(Player);Player.lastLoginDate="";Player.isNewLoginDay=false;
            int gap=ready?0:1;
            if(selectedTask==0)Player.coin1024Number=Math.Max(0,p.condition_merge-gap);
            if(selectedTask==1)Player.watch_video_count=Math.Max(0,p.condition_ad-gap);
            if(selectedTask==2)Player.fakeMoney=Math.Max(0,p.withdrawAmount-gap);
            if(selectedTask==3){Player.loginDays=Math.Max(0,p.condition_login_days-gap);Player.lastLoginDate=PlayerClock.Today(Player);}
            if(selectedTask==4)Player.watch_video_count=Math.Max(0,p.condition_video-gap);
            message="已准备任务 "+(selectedTask+1)+(ready?" 的达标数据":" 的差 1 数据")+"；需在提现页点击申请并走验证，不自动推进任务。";
        }
        void PrepareCashTask(WithdrawProduct p,bool ready)
        {
            int gap=ready?0:1;
            Player.fakeMoney=selectedTask>0?p.withdrawAmount:Math.Max(0,p.withdrawAmount-gap);
            Player.coin1024Number=selectedTask>1?p.condition_merge:selectedTask==1?Math.Max(0,p.condition_merge-gap):0;
            Player.loginDays=selectedTask>2?p.condition_login_days:selectedTask==2?Math.Max(0,p.condition_login_days-gap):0;
            Player.watch_video_count=selectedTask==3?Math.Max(0,p.condition_video-gap):0;
            Player.today1024NumberCoin=0;Player.today1024NumberCoinDate=PlayerClock.Today(Player);
            Player.lastLoginDate=selectedTask==2?PlayerClock.Today(Player):"";Player.isNewLoginDay=false;
            message="已准备现金路线条件 "+(selectedTask+1)+(ready?"达标":"差 1")+"；打开现金提现查看。此路线自动显示下一项未达标条件。";
        }
        public void Refresh()
        {
            if(!Session||Session.Player==null)return;
            productLabel.text="档位 "+(selectedProduct+1)+" / "+Session.Locale.Money(Product.withdrawAmount);
            if(routeLabel)routeLabel.text=cashRoute?"当前：现金提现（点击切换）":"当前：筹码提现（点击切换）";
            taskLabel.text=(cashRoute?"条件 ":"任务 ")+(selectedTask+1)+" / "+(cashRoute?CashTasks:Tasks)[selectedTask];
            progressStatus.text=PlayerClock.Now(Player).ToString("yyyy-MM-dd HH:mm")+"  偏移 +"+(Player.gmTimeOffsetSeconds/3600).ToString("0.#")+"h\n"+
                "现金 "+Session.Locale.Money(Player.fakeMoney)+" | 2000 筹码 "+Player.coin1024Number+" | 看完广告 "+Player.watch_video_count+"\n"+
                "有效天 "+Player.loginDays+" | 今天合成 "+Player.today1024NumberCoin+"/"+owner.router.balance.rules.flow.validLoginMergeCount+"\n"+message;
            var trace=Session.Sdk.Trace;string last=trace.Count>0?trace[trace.Count-1]:"暂无广告请求";
            eventStatus.text="窗口投币 "+Player.windowsCointimes+" | 总投币 "+Player.dropCointimes+" | 转盘次数 "+Player.currentLotteryCount+"\n"+
                "广告测试结果："+Outcomes[(int)Session.Sdk.NextAdOutcome]+"\n"+message;
            var reward=owner.router.balance.rules.lotteryRewards[selectedSlot];slotLabel.text="下次抽中第 "+(selectedSlot+1)+" 格 / "+(reward.type=="money"?"现金":reward.amount+" 枚 2000 筹码");
        }
        void OnDestroy(){if(callbacks!=null)for(int i=0;i<callbacks.Length;i++)actions[i].button.onClick.RemoveListener(callbacks[i]);}
    }
}

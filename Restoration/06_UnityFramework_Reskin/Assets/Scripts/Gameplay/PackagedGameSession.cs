using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class PackagedActionBinding {public Button button;public int action;}
    [Serializable] public sealed class PackagedModeBinding {public GameObject price;public Text priceText,level;public Image icon;}
    [Serializable] public sealed class PackagedHonorBinding {public GameObject price,finish,check;}
    public sealed class PackagedGameSession : MonoBehaviour
    {
        public string saveNamespace="coinmerge.recovered.v1";
        public PackagedBoard board;public Camera worldCamera;public VersionGmPanel gm;
        public PackagedAudio audioCues;
        public GameObject home,map,toast;
        public Text coinsText,heartsText,scoreText,heartTime,heartCount,infoCoins,infoHearts,policyTitle,toastText;
        public RectTransform musicKnob;public Image musicButton,policyImage;
        public Color musicOn=Color.white,musicOff=new Color(79/255f,83/255f,80/255f);
        public string unlockedIconPath,lockedIconPath;public string[] policyPaths;
        public GameObject[] dialogs; // Guide, settings, honor, info, heart, failure, policy.
        public PackagedModeBinding[] modes;public PackagedHonorBinding[] honors;public PackagedActionBinding[] actions;
        public PackagedPlayer Player {get;private set;}
        public int Score {get;private set;}public int Mode {get;private set;}
        public bool automaticInput=true;
        public bool InHome=>home.activeSelf;
        public int Dialog=>dialogStack.Count==0?-1:dialogStack.Peek();
        readonly Stack<int> dialogStack=new Stack<int>(8);
        UnityAction[] callbacks;Sprite unlockedIcon,lockedIcon;float heartTick,toastLeft;int heartDown;bool dragging,initialized,retryAfterHeart;
        string PlayerKey=>saveNamespace+".packaged.player";
        void Start()
        {
            Player=new PackagedPlayer();if(PlayerPrefs.HasKey(PlayerKey))JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PlayerKey),Player);
            // Original honor view has seven rows and lazily extends its six-element default array.
            if(Player.achieveStaus.Length<7)Array.Resize(ref Player.achieveStaus,7);
            board.Initialize(Player);board.Merged+=OnMerged;board.Failed+=OnFailed;heartDown=board.balance.heartSeconds;
            unlockedIcon=Resources.Load<Sprite>(unlockedIconPath);lockedIcon=Resources.Load<Sprite>(lockedIconPath);
            callbacks=new UnityAction[actions.Length];for(int i=0;i<actions.Length;i++){int code=actions[i].action;callbacks[i]=()=>Act(code);actions[i].button.onClick.AddListener(callbacks[i]);}
            initialized=true;Home();if(Player.NewUser)Show(0);audioCues.SetMusic(Player.open_bgm);Refresh();
        }
        public void Act(int action)
        {
            audioCues.Effect(1,Player.open_music);
            if(action>=200){int i=action-200;if(Player.Spend(board.balance.modePrice)){Player.achieveStaus[i]=1;Refresh();Save();}else Toast("Not enough gold coins");return;}
            if(action>=100){ChooseMode(action-100);return;}
            switch(action)
            {
                case 0:Close();break;
                case 1:Player.NewUser=false;Close();Save();break;
                case 2:ResetPlayerKeepingProfile();Application.Quit();break;
                case 3:Show(1);break;
                case 4:Show(2);break;
                case 5:Show(3);break;
                case 6:Show(4);break;
                case 7:Home();break;
                case 8:StartRound();break;
                case 9:BuyHeart();break;
                case 10:Retry();break;
                case 11:Player.open_bgm=Player.open_music=!Player.open_bgm;audioCues.SetMusic(Player.open_bgm);Refresh();Save();break;
                case 12:Policy(0);break;
                case 13:Policy(1);break;
            }
        }
        public void ChooseMode(int index)
        {
            if(Player.passStaus[index]==0)
            {if(Player.Spend(board.balance.modePrice*index)){Player.passStaus[index]=1;Refresh();Save();}else Toast("Not enough gold coins");return;}
            if(!Player.SpendHeart()){Show(4);return;}
            Mode=index;StartRound();Save();
        }
        void StartRound(){CloseAll();home.SetActive(false);map.SetActive(true);board.gameObject.SetActive(true);Score=0;board.StartGame();Refresh();}
        public void Home(){CloseAll();board.Clear();board.gameObject.SetActive(false);map.SetActive(false);home.SetActive(true);Refresh();Save();}
        void Retry()
        {if(Player.SpendHeart()){StartRound();Save();}else{retryAfterHeart=true;Show(4);}}
        void BuyHeart()
        {
            if(!Player.Spend(board.balance.heartPrice)){Toast("Not enough gold coins");return;}
            if(Player.heart<board.balance.maxHearts)Player.heart++;
            bool retry=retryAfterHeart;Close();
            // FailureView's original add-heart callback charges an additional 100 coins.
            if(retry){retryAfterHeart=false;if(Player.Spend(board.balance.heartPrice))StartRound();else Toast("Not enough gold coins");}
            Refresh();Save();
        }
        void OnMerged(int type){audioCues.Effect(2,Player.open_music);Score+=board.balance.mergeReward;Player.coins+=board.balance.mergeReward;Player.mergeCount++;Player.mergedMaxLv=Mathf.Max(Player.mergedMaxLv,type);Refresh();Save();}
        void OnFailed(){audioCues.Effect(3,Player.open_music);Show(5);Save();}
        void Policy(int type){policyTitle.text=type==0?"Privacy Policy":"User Agreement";policyImage.sprite=Resources.Load<Sprite>(policyPaths[type]);Show(6);}
        public void Show(int kind){if(Dialog>=0)dialogs[Dialog].SetActive(false);dialogStack.Push(kind);dialogs[kind].SetActive(true);Refresh();}
        public void Close(){if(Dialog<0)return;int old=dialogStack.Pop();dialogs[old].SetActive(false);if(old==4)retryAfterHeart=false;if(Dialog>=0)dialogs[Dialog].SetActive(true);}
        void CloseAll(){while(Dialog>=0)Close();}
        void Toast(string message){toastText.text=message;toast.SetActive(true);toastLeft=2;}
        void Update()
        {
            if(!initialized)return;
            if(gm&&gm.IsOpen){dragging=false;return;}
            if(toastLeft>0&&(toastLeft-=Time.deltaTime)<=0)toast.SetActive(false);
            heartTick+=Time.deltaTime;
            if(heartTick>=1){heartTick-=1;if(heartDown>0){heartDown--;if(Player.heart<board.balance.maxHearts&&heartDown<=0){Player.heart++;heartDown=board.balance.heartSeconds;Save();}}Refresh();}
            if(home.activeSelf)return;
            if(automaticInput&&Dialog<0)ReadInput();else dragging=false;
            board.Tick(Time.deltaTime);
        }
        void ReadInput()
        {
            bool down=Input.GetMouseButtonDown(0),held=Input.GetMouseButton(0),up=Input.GetMouseButtonUp(0);Vector2 pos=Input.mousePosition;int pointer=-1;
            if(Input.touchCount>0){var t=Input.GetTouch(0);pos=t.position;pointer=t.fingerId;down=t.phase==TouchPhase.Began;up=t.phase==TouchPhase.Ended||t.phase==TouchPhase.Canceled;held=!up;}
            var point=worldCamera.ScreenToWorldPoint(pos);
            if(down)dragging=board.Preview&&!board.GameOver&&Mathf.Abs(point.x)<375/board.balance.units&&point.y<board.balance.previewY/board.balance.units+2&&point.y>-614/board.balance.units&&
                (!EventSystem.current||!(pointer<0?EventSystem.current.IsPointerOverGameObject():EventSystem.current.IsPointerOverGameObject(pointer)));
            if(!dragging)return;if(held)board.Move(point.x);if(up){dragging=false;board.Drop();}
        }
        void Refresh()
        {
            if(Player==null)return;
            coinsText.text=infoCoins.text=Player.coins.ToString();heartsText.text=infoHearts.text=Player.heart.ToString();
            scoreText.text=board.balance.modeNames[Mode]+"  Score"+Score;
            heartTime.text="After <color=#FF3218>"+(heartDown/60).ToString("00")+":"+(heartDown%60).ToString("00")+"</color> +1";heartCount.text="Your life: "+Player.heart;
            for(int i=0;i<modes.Length;i++){var m=modes[i];bool open=Player.passStaus[i]!=0;m.price.SetActive(!open);m.priceText.text=(board.balance.modePrice*i).ToString();m.icon.sprite=open?unlockedIcon:lockedIcon;}
            for(int i=0;i<honors.Length;i++){bool done=Player.achieveStaus[i]!=0;honors[i].price.SetActive(!done);honors[i].finish.SetActive(done);honors[i].check.SetActive(done);}
            var position=musicKnob.anchoredPosition;position.x=Player.open_bgm?16:-16;musicKnob.anchoredPosition=position;musicButton.color=Player.open_bgm?musicOn:musicOff;
        }
        public void ResetPlayerKeepingProfile()
        {Player=new PackagedPlayer();Array.Resize(ref Player.achieveStaus,7);board.Initialize(Player);heartDown=board.balance.heartSeconds;heartTick=0;Home();Show(0);audioCues.SetMusic(Player.open_bgm);Save();}
        public void Save(){if(!initialized)return;PlayerPrefs.SetString(PlayerKey,JsonUtility.ToJson(Player));PlayerPrefs.Save();}
        void OnApplicationPause(bool pause){if(pause)Save();}void OnApplicationQuit(){Save();}
        void OnDestroy(){Save();board.Merged-=OnMerged;board.Failed-=OnFailed;if(callbacks!=null)for(int i=0;i<actions.Length;i++)actions[i].button.onClick.RemoveListener(callbacks[i]);}
    }
}

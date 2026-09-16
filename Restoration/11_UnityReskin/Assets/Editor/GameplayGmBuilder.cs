using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class GameplayGmBuilder
    {
        static Font font;
        static readonly List<MenuActionBinding> bindings=new List<MenuActionBinding>();
        static GameObject Rect(string name,Transform parent,Vector2 size,Vector2 position)
        {var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.sizeDelta=size;r.anchoredPosition=position;return go;}
        static Text Label(string name,Transform parent,string value,Vector2 size,Vector2 position,int point=22)
        {var node=Rect(name,parent,size,position);var t=node.AddComponent<Text>();t.font=font;t.fontSize=point;t.text=value;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;return t;}
        static void Button(Transform parent,int code,string label,float x,float y,float width=280)
        {
            var node=Rect("TestAction_"+code,parent,new Vector2(width,48),new Vector2(x,y));var image=node.AddComponent<Image>();image.color=new Color(.17f,.3f,.44f);
            var b=node.AddComponent<Button>();b.targetGraphic=image;b.transition=Selectable.Transition.ColorTint;
            var t=Label("Label",node.transform,label,new Vector2(width-12,44),Vector2.zero,22);t.resizeTextForBestFit=true;t.resizeTextMinSize=18;t.resizeTextMaxSize=22;
            bindings.Add(new MenuActionBinding {button=b,action=code});
        }
        public static void Author(VersionGmPanel owner,GameObject card,Font sourceFont)
        {
            font=sourceFont;bindings.Clear();((RectTransform)card.transform).sizeDelta=new Vector2(650,1120);
            var old=new List<Transform>();foreach(Transform child in card.transform)old.Add(child);
            var panel=owner.gameObject.AddComponent<GameplayGmPanel>();owner.gameplay=panel;panel.owner=owner;
            panel.versions=Rect("Versions",card.transform,new Vector2(620,900),Vector2.zero);
            foreach(var child in old)
            {
                if(child.name=="Title"||child.name=="Close")((RectTransform)child).anchoredPosition=new Vector2(((RectTransform)child).anchoredPosition.x,495);
                else child.SetParent(panel.versions.transform,false);
            }
            card.transform.Find("Title").GetComponent<Text>().text="GM / 测试工具";
            Button(card.transform,0,"版本",-200,425,185);Button(card.transform,1,"提现 / 时间",0,425,185);Button(card.transform,2,"事件 / 广告",200,425,185);
            panel.progress=Rect("Progress",card.transform,new Vector2(620,930),Vector2.zero);
            panel.events=Rect("Events",card.transform,new Vector2(620,930),Vector2.zero);
            var p=panel.progress.transform;var e=panel.events.transform;
            panel.progressStatus=Label("Status",p,"",new Vector2(590,190),new Vector2(0,295),21);
            panel.progressStatus.alignment=TextAnchor.UpperLeft;
            panel.productLabel=Label("Product",p,"",new Vector2(405,45),new Vector2(0,175));
            Button(p,10,"<",-260,175,55);Button(p,11,">",260,175,55);
            panel.taskLabel=Label("Task",p,"",new Vector2(405,45),new Vector2(0,115));
            Button(p,12,"<",-260,115,55);Button(p,13,">",260,115,55);
            Button(p,14,"准备：当前任务差 1",-150,55);Button(p,15,"准备：当前任务达标",150,55);
            Button(p,16,"时间 +12h",-200,-5,185);Button(p,17,"时间 +24h",0,-5,185);Button(p,18,"恢复时间",200,-5,185);
            Button(p,19,"最高币统计 +1",-150,-65);Button(p,20,"今日合成统计 +5",150,-65);
            var input=Rect("Number",p,new Vector2(580,48),new Vector2(0,-125));input.AddComponent<Image>().color=new Color(.92f,.95f,1);
            panel.number=input.AddComponent<InputField>();var inputText=Label("Text",input.transform,"0",new Vector2(540,44),Vector2.zero,25);inputText.color=new Color(.07f,.1f,.18f);
            panel.number.textComponent=inputText;panel.number.contentType=InputField.ContentType.DecimalNumber;panel.number.characterLimit=16;panel.number.text="0";
            Button(p,21,"设置现金",-200,-185,185);Button(p,22,"设置最高币数",0,-185,185);Button(p,23,"设置广告次数",200,-185,185);
            Button(p,26,"广告统计 +1",-150,-245);Button(p,27,"现金 +1",150,-245);
            Button(p,24,"打开现金提现",-150,-305);Button(p,25,"打开金币提现",150,-305);
            Button(p,48,"清除测试账户",-150,-365);Button(p,46,"跳过新手引导",150,-365);
            Label("Help",p,"时间只推进日历，不快放动画。跨天后每天合成 5 枚最高币才计有效天。准备数据会替换相关计数。",new Vector2(590,90),new Vector2(0,-455),21);
            panel.eventStatus=Label("Status",e,"",new Vector2(590,170),new Vector2(0,295),21);panel.eventStatus.alignment=TextAnchor.UpperLeft;
            Button(e,30,"广告：成功",-150,175);Button(e,31,"广告：取消",150,175);
            Button(e,32,"广告：无填充",-150,115);Button(e,33,"广告：失败",150,115);
            Button(e,34,"准备第 10 次投币",-200,55,185);Button(e,35,"准备第 16 次",0,55,185);Button(e,36,"准备第 22 次",200,55,185);
            Button(e,37,"真实投币一次",-150,-5);Button(e,38,"触发失败 / 复活",150,-5);
            Button(e,39,"真实合成 1000+1000",-150,-65);Button(e,40,"预览普通奖励",150,-65);
            Button(e,41,"预览双倍奖励",-150,-125);Button(e,42,"预览引导奖励",150,-125);
            panel.slotLabel=Label("Slot",e,"",new Vector2(580,45),new Vector2(0,-185),22);
            Button(e,43,"下一个转盘格子",-150,-245);Button(e,44,"准备转盘差 1 分",150,-245);
            Button(e,45,"转盘分数 +1",-150,-305);Button(e,47,"清空广告日志",150,-305);
            Label("Help",e,"预览奖励关闭后照常结算；不经过广告。事件测试请用投币、失败和正常抽奖入口。当前主链无定时插屏。",new Vector2(590,120),new Vector2(0,-420),21);
            panel.actions=bindings.ToArray();panel.progress.SetActive(false);panel.events.SetActive(false);
        }
    }
}

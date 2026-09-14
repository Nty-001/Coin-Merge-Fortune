"""Emit native Unity YAML assets without executing the unlicensed editor.
The exact source graph is preserved as metadata; engine semantic parity remains unverified.
"""
import json,re,hashlib,shutil,collections
from pathlib import Path
R=Path(__file__).resolve().parents[2];O=R/'Restoration';U=O/'06_UnityFramework';A=U/'Assets'
E=Path('C:/Program Files/Unity/Hub/Editor/2022.3.62f3c1/Editor/Data')
def read(p):return json.loads(p.read_text(encoding='utf-8'))
def guid(p):return hashlib.md5(str(Path(p).relative_to(U)).replace('\\','/').encode()).hexdigest()
def ref(i=0,g=None,t=3):return dict(fileID=i,**({'guid':g,'type':t} if g else {}))
def vec(v,names):return dict(zip(names,v))
def scalar(x):
    if x is None:return 'null'
    if isinstance(x,bool):return '1' if x else '0'
    if isinstance(x,str):return json.dumps(x,ensure_ascii=False)
    return str(x)
def flow(d):return '{'+', '.join(f'{k}: {scalar(v)}' for k,v in d.items())+'}'
def yaml(x,indent=0):
    pad=' '*indent;lines=[]
    if isinstance(x,dict):
        for k,v in x.items():
            if isinstance(v,dict) and all(not isinstance(z,(dict,list)) for z in v.values()):lines.append(pad+k+': '+flow(v))
            elif isinstance(v,(dict,list)) and v:lines.append(pad+k+':');lines.extend(yaml(v,indent+2))
            elif isinstance(v,(dict,list)):lines.append(pad+k+(': {}' if isinstance(v,dict) else ': []'))
            else:lines.append(pad+k+': '+scalar(v))
    elif isinstance(x,list):
        for v in x:
            if isinstance(v,(dict,list)):lines.append(pad+'-');lines.extend(yaml(v,indent+2))
            else:lines.append(pad+'- '+scalar(v))
    return lines
def meta(p):
    g=guid(p);text='fileFormatVersion: 2\nguid: '+g+'\n'
    if p.suffix=='.cs':text+='MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    elif p.suffix.lower() in ['.png','.jpg','.jpeg']:
        b=borders.get(str(p.relative_to(U)).replace('\\','/'),[0,0,0,0]);b=[b[0],b[3],b[2],b[1]]
        text+='TextureImporter:\n  internalIDToNameTable: []\n  externalObjects: {}\n  serializedVersion: 12\n  mipmaps:\n    enableMipMap: 0\n    sRGBTexture: 1\n  isReadable: 0\n  textureType: 8\n  textureShape: 1\n  maxTextureSize: 8192\n  textureSettings:\n    serializedVersion: 2\n    filterMode: 1\n    aniso: 1\n    mipBias: 0\n    wrapU: 1\n    wrapV: 1\n    wrapW: 1\n  spriteMode: 1\n  spriteExtrude: 1\n  spriteMeshType: 1\n  alignment: 0\n  spritePivot: {x: 0.5, y: 0.5}\n  spritePixelsToUnits: 100\n  spriteBorder: '+flow(vec(b,'xyzw'))+'\n  alphaUsage: 1\n  alphaIsTransparency: 1\n  spriteSheet:\n    serializedVersion: 2\n    sprites: []\n    outline: []\n    physicsShape: []\n    bones: []\n    spriteID: '+g+'\n    internalID: 0\n    vertices: []\n    indices: \n    edges: []\n    weights: []\n    secondaryTextures: []\n  platformSettings:\n  - serializedVersion: 3\n    buildTarget: DefaultTexturePlatform\n    maxTextureSize: 8192\n    resizeAlgorithm: 0\n    textureFormat: -1\n    textureCompression: 0\n    compressionQuality: 100\n    overridden: 0\n'
    elif p.suffix=='.ttf':text+='TrueTypeFontImporter:\n  externalObjects: {}\n  serializedVersion: 4\n  fontSize: 40\n  forceTextureCase: -2\n  characterSpacing: 0\n  characterPadding: 1\n  includeFontData: 1\n  fontNames: []\n  fallbackFontReferences: []\n  customCharacters: \n  fontRenderingMode: 0\n'
    else:text+='DefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    Path(str(p)+'.meta').write_text(text,encoding='utf-8')
ui={p.stem.replace('.cs',''):re.search(r'guid: (\w+)',p.read_text()).group(1) for p in (E/'Resources/PackageManager/BuiltInPackages/com.unity.ugui/Runtime').rglob('*.cs.meta')}
sprites=read(A/'Resources/Recovered/sprite_import.json')['sprites'];borders={s['path']:s['border'] for s in sprites};sm={(s['variant'],s['uuid']):ref(21300000,guid(U/s['path'])) for s in sprites}
font=next((A/'Art/HotUpdate/Fonts').rglob('*.ttf'));fontref=ref(12800000,guid(font))
shutil.copytree(O/'04_Assets/HotUpdate/SkeletalReusable',A/'Art/HotUpdate/SkeletalReusable',dirs_exist_ok=True)
shutil.copytree(O/'03_Configuration/EffectiveRuntime',A/'Config/EffectiveRuntime',dirs_exist_ok=True)
node_script=guid(A/'Scripts/Core/RecoveredNode.cs')
def common(go):return {'m_ObjectHideFlags':0,'m_CorrespondingSourceObject':ref(),'m_PrefabInstance':ref(),'m_PrefabAsset':ref(),'m_GameObject':ref(go)}
def mono(go,g,props):return {**common(go),'m_Enabled':1,'m_EditorHideFlags':0,'m_Script':ref(11500000,g),'m_Name':'','m_EditorClassIdentifier':'',**props}
def graphic(go,g,color,props):return mono(go,g,{'m_Material':ref(),'m_Color':vec(color,'rgba'),'m_RaycastTarget':0,'m_RaycastPadding':vec([0]*4,'xyzw'),'m_Maskable':1,'m_OnCullStateChanged':{'m_PersistentCalls':{'m_Calls':[]}},**props})
allstats=[];unmapped=[]
def emit(model,target):
    documents=[];nextid=1000
    def add(classid,name,body):
        nonlocal nextid
        idx=nextid;nextid+=1;documents.append((classid,idx,name,body));return idx
    nodes=model['nodes'];mapping={};rects={};components={};gameobjs={}
    for n in nodes:
        go=add(1,'GameObject',{});tr=add(224,'RectTransform',{});mapping[n['id']]=go;rects[n['id']]=tr;components[n['id']]=[tr]
    for n in nodes:
        i=n['id'];go=mapping[i];tr=rects[i];cs=components[i]
        nodebyid={x['id']:x for x in nodes}
        anchor=nodebyid.get(n['parent'],{}).get('pivot',[.5,.5])
        trbody={**common(go),'m_LocalRotation':vec(n['rotation'],'xyzw'),'m_LocalPosition':vec(n['position'],'xyz'),'m_LocalScale':vec(n['scale'],'xyz'),'m_ConstrainProportionsScale':0,'m_Children':[ref(rects[x]) for x in n['children'] if x in rects],'m_Father':ref(rects.get(n['parent'],0)),'m_RootOrder':0,'m_LocalEulerAnglesHint':vec([0,0,0],'xyz'),'m_AnchorMin':vec(anchor,'xy'),'m_AnchorMax':vec(anchor,'xy'),'m_AnchoredPosition':vec(n['position'][:2],'xy'),'m_SizeDelta':vec(n['size'],'xy'),'m_Pivot':vec(n['pivot'],'xy')}
        documents[next(j for j,d in enumerate(documents) if d[1]==tr)]=(224,tr,'RectTransform',trbody)
        cs.append(add(114,'MonoBehaviour',mono(go,node_script,{'sourceUuid':model['uuid'],'variant':model['variant'],'sourceObjectId':i,'originalNodeJson':n['rawJson'],'originalComponents':n['components']})))
        hasgraphic=False;native=[]
        def graphicadd(g,props):
            nonlocal hasgraphic
            if hasgraphic:return None
            hasgraphic=True;cs.append(add(222,'CanvasRenderer',{**common(go),'m_CullTransparentMesh':1}));idx=add(114,'MonoBehaviour',graphic(go,g,n['color'],props));cs.append(idx);return idx
        for c in n['components']:
            typ=c['type'];raw=json.loads(c['rawJson']);props={};status='metadata_only'
            if typ=='cc.Sprite':
                graphicadd(ui['Image'],{'m_Sprite':sm.get((model['variant'],c['sprite']),ref()),'m_Type':c['spriteType'],'m_PreserveAspect':0,'m_FillCenter':1,'m_FillMethod':c['fillType'],'m_FillAmount':c['fillRange'],'m_FillClockwise':1,'m_FillOrigin':0,'m_UseSpriteMesh':0,'m_PixelsPerUnitMultiplier':1});status='native_Image'
            elif typ in ['cc.Label','cc.RichText']:
                graphicadd(ui['Text'],{'m_FontData':{'m_Font':fontref,'m_FontSize':c['fontSize'],'m_FontStyle':0,'m_BestFit':0,'m_MinSize':10,'m_MaxSize':40,'m_Alignment':max(0,min(2,c['verticalAlign']))*3+max(0,min(2,c['horizontalAlign'])),'m_AlignByGeometry':0,'m_RichText':typ=='cc.RichText','m_HorizontalOverflow':0,'m_VerticalOverflow':1,'m_LineSpacing':c['lineHeight']/max(1,c['fontSize'])},'m_Text':c['text']});status='native_Text'
            elif typ=='cc.Canvas':
                cs.append(add(223,'Canvas',{**common(go),'m_Enabled':1,'serializedVersion':3,'m_RenderMode':0,'m_Camera':ref(),'m_PlaneDistance':100,'m_PixelPerfect':0,'m_ReceivesEvents':1,'m_OverrideSorting':0,'m_OverridePixelPerfect':0,'m_SortingBucketNormalizedSize':0,'m_VertexColorAlwaysGammaSpace':0,'m_AdditionalShaderChannelsFlag':25,'m_UpdateRectTransformForStandalone':0,'m_SortingLayerID':0,'m_SortingOrder':0,'m_TargetDisplay':0}))
                cs.append(add(114,'MonoBehaviour',mono(go,ui['CanvasScaler'],{'m_UiScaleMode':1,'m_ReferencePixelsPerUnit':100,'m_ScaleFactor':1,'m_ReferenceResolution':{'x':750,'y':1624},'m_ScreenMatchMode':0,'m_MatchWidthOrHeight':0,'m_PhysicalUnit':3,'m_FallbackScreenDPI':96,'m_DefaultSpriteDPI':96,'m_DynamicPixelsPerUnit':1,'m_PresetInfoIsWorld':0})))
                cs.append(add(114,'MonoBehaviour',mono(go,ui['GraphicRaycaster'],{'m_IgnoreReversedGraphics':1,'m_BlockingObjects':0,'m_BlockingMask':{'serializedVersion':2,'m_Bits':4294967295}})));status='native_Canvas'
            elif typ in ['cc.LabelOutline','cc.LabelShadow']:
                name='Outline' if typ=='cc.LabelOutline' else 'Shadow';col=raw.get('_color',{}).get('values',[4278190080])[0]
                cs.append(add(114,'MonoBehaviour',mono(go,ui[name],{'m_EffectColor':vec([(col>>s&255)/255 for s in [0,8,16,24]],'rgba'),'m_EffectDistance':{'x':raw.get('_width',2),'y':-raw.get('_width',2)},'m_UseGraphicAlpha':1})));status='native_'+name
            elif typ=='cc.Mask':
                cs.append(add(114,'MonoBehaviour',mono(go,ui['RectMask2D'],{'m_Padding':vec([0]*4,'xyzw'),'m_Softness':{'x':0,'y':0}})));status='native_rectangular_mask_only'
            elif typ=='cc.Button':
                imageid=next((x[1] for x in documents if x[0]==114 and x[3].get('m_GameObject')==ref(go) and x[3].get('m_Script',{}).get('guid')==ui['Image']),0)
                if imageid:
                    next(x[3] for x in documents if x[1]==imageid)['m_RaycastTarget']=1
                cs.append(add(114,'MonoBehaviour',mono(go,ui['Button'],{'m_Navigation':{'m_Mode':3,'m_WrapAround':0,'m_SelectOnUp':ref(),'m_SelectOnDown':ref(),'m_SelectOnLeft':ref(),'m_SelectOnRight':ref()},'m_Transition':1,'m_Colors':{'m_NormalColor':vec([1]*4,'rgba'),'m_HighlightedColor':vec([.9,.9,.9,1],'rgba'),'m_PressedColor':vec([.7,.7,.7,1],'rgba'),'m_SelectedColor':vec([1]*4,'rgba'),'m_DisabledColor':vec([.5,.5,.5,.5],'rgba'),'m_ColorMultiplier':1,'m_FadeDuration':.1},'m_Interactable':1,'m_TargetGraphic':ref(imageid),'m_OnClick':{'m_PersistentCalls':{'m_Calls':[]}}})));status='native_Button_callbacks_preserved_in_source'
            elif typ=='cc.ScrollView':
                cs.append(add(114,'MonoBehaviour',mono(go,ui['ScrollRect'],{'m_Content':ref(rects.get(c['content'],0)),'m_Horizontal':raw.get('horizontal',False),'m_Vertical':raw.get('vertical',True),'m_MovementType':1,'m_Elasticity':.1,'m_Inertia':1,'m_DecelerationRate':.135,'m_ScrollSensitivity':1,'m_Viewport':ref(rects.get(c['viewport'],0)),'m_HorizontalScrollbar':ref(),'m_VerticalScrollbar':ref(),'m_OnValueChanged':{'m_PersistentCalls':{'m_Calls':[]}}})));status='native_ScrollRect'
            elif typ=='cc.Animation':
                cs.append(add(95,'Animator',{**common(go),'m_Enabled':1,'m_Avatar':ref(),'m_Controller':ref(),'m_CullingMode':0,'m_UpdateMode':0,'m_ApplyRootMotion':0,'m_LinearVelocityBlending':0,'m_StabilizeFeet':0,'m_WarningMessage':'','m_HasTransformHierarchy':1,'m_AllowConstantClipSamplingOptimization':1,'m_KeepAnimatorControllerStateOnDisable':0}));status='native_Animator_unbound'
            elif typ=='cc.RigidBody':
                cs.append(add(50,'Rigidbody2D',{**common(go),'serializedVersion':4,'m_BodyType':{0:2,1:1,2:0}.get(c['bodyType'],0),'m_Simulated':1,'m_UseFullKinematicContacts':0,'m_UseAutoMass':0,'m_Mass':1,'m_LinearDrag':c['linearDamping'],'m_AngularDrag':c['angularDamping'],'m_GravityScale':c['gravityScale'],'m_Material':ref(),'m_Interpolate':0,'m_SleepingMode':1,'m_CollisionDetection':0,'m_Constraints':0}));status='native_Rigidbody2D_units_unverified'
            elif typ in ['cc.PhysicsCircleCollider','cc.CircleCollider','cc.PhysicsBoxCollider','cc.BoxCollider']:
                circle='Circle' in typ
                cs.append(add(58 if circle else 61,'CircleCollider2D' if circle else 'BoxCollider2D',{**common(go),'m_Enabled':1,'m_Density':c['density'],'m_Material':ref(),'m_IsTrigger':c['sensor'],'m_UsedByEffector':0,'m_UsedByComposite':0,'m_Offset':vec(c['offset'],'xy'),'serializedVersion':2,**({'m_Radius':c['radius']} if circle else {'m_Size':vec(c['size'],'xy'),'m_EdgeRadius':0})}));status='native_collider_units_unverified'
            native.append({'sourceType':typ,'status':status})
            if status=='metadata_only':unmapped.append({'prefab':model['name'],'variant':model['variant'],'node':n['name'],'type':typ,'className':c['className']})
        body={'m_ObjectHideFlags':0,'m_CorrespondingSourceObject':ref(),'m_PrefabInstance':ref(),'m_PrefabAsset':ref(),'serializedVersion':6,'m_Component':[{'component':ref(v)} for v in cs],'m_Layer':5,'m_Name':n['name'],'m_TagString':'Untagged','m_Icon':ref(),'m_NavMeshLayer':0,'m_StaticEditorFlags':0,'m_IsActive':n['active']}
        documents[next(j for j,d in enumerate(documents) if d[1]==go)]=(1,go,'GameObject',body)
    if model.get('mockFlow'):
        def findmono(node,script):return next(x[1] for x in documents if x[0]==114 and x[3].get('m_GameObject')==ref(mapping[node]) and x[3].get('m_Script',{}).get('guid')==ui[script])
        panel=add(114,'MonoBehaviour',mono(mapping[0],guid(A/'Scripts/UI/MockFlowPanel.cs'),{'status':ref(findmono(1,'Text')),'rewardButton':ref(findmono(2,'Button')),'cancelButton':ref(findmono(4,'Button')),'withdrawButton':ref(findmono(6,'Button'))}))
        next(x[3] for x in documents if x[1]==mapping[0])['m_Component'].append({'component':ref(panel)})
        go=add(1,'GameObject',{});tr=add(4,'Transform',{**common(go),'m_LocalRotation':vec([0,0,0,1],'xyzw'),'m_LocalPosition':vec([0,0,0],'xyz'),'m_LocalScale':vec([1,1,1],'xyz'),'m_Children':[],'m_Father':ref(),'m_RootOrder':1,'m_LocalEulerAnglesHint':vec([0,0,0],'xyz')})
        event=add(114,'MonoBehaviour',mono(go,ui['EventSystem'],{'m_FirstSelected':ref(),'m_sendNavigationEvents':1,'m_DragThreshold':10}))
        inp=add(114,'MonoBehaviour',mono(go,ui['StandaloneInputModule'],{'m_SendPointerHoverToParent':1,'m_HorizontalAxis':'Horizontal','m_VerticalAxis':'Vertical','m_SubmitButton':'Submit','m_CancelButton':'Cancel','m_InputActionsPerSecond':10,'m_RepeatDelay':.5,'m_ForceModuleActive':0}))
        next(x[3] for x in documents if x[1]==go).update({'m_ObjectHideFlags':0,'m_CorrespondingSourceObject':ref(),'m_PrefabInstance':ref(),'m_PrefabAsset':ref(),'serializedVersion':6,'m_Component':[{'component':ref(x)} for x in [tr,event,inp]],'m_Layer':0,'m_Name':'EventSystem','m_TagString':'Untagged','m_Icon':ref(),'m_NavMeshLayer':0,'m_StaticEditorFlags':0,'m_IsActive':1})
    target.parent.mkdir(parents=True,exist_ok=True)
    text=['%YAML 1.1','%TAG !u! tag:unity3d.com,2011:']
    for cls,idx,name,body in documents:text+=['--- !u!'+str(cls)+' &'+str(idx),name+':']+yaml(body,2)
    target.write_text('\n'.join(text)+'\n',encoding='utf-8');meta(target)
    allstats.append({'path':str(target.relative_to(U)).replace('\\','/'),'nodes':len(nodes),'serializedObjects':len(documents),'editorImported':False})

for p in sorted((A/'Resources/Recovered').glob('*.json')):
    if p.name=='sprite_import.json':continue
    m=read(p);name=re.sub(r'[<>:"\\|?*]','_',m['name'].replace('.fire',''))
    emit(m,A/'Prefabs'/m['variant']/(name+'.prefab'))
# Also deliver the scene graphs as native .unity files. No scene execution is claimed.
for p in sorted((A/'Resources/Recovered').glob('*.json')):
    if p.name=='sprite_import.json':continue
    m=read(p)
    if m['type']=='cc.SceneAsset':emit(m,A/'Scenes'/(m['variant']+'_'+Path(m['name']).stem+'.unity'))
def mockcomponent(typ,text=''):
    return dict(type=typ,className=typ,sourceId=0,rawJson='{}',sprite='',text=text,fontSize=30,lineHeight=36,horizontalAlign=1,verticalAlign=1,font='',spriteType=0,fillRange=1,fillStart=0,fillType=0,enabled=True,target=-1,content=-1,viewport=-1,radius=25,offset=[0,0],size=[100,100],bodyType=2,gravityScale=1,linearDamping=0,angularDamping=0,friction=.2,restitution=0,density=1,sensor=False,isCustom=False)
def mocknode(i,name,parent,pos,size,comps,color=[1,1,1,1]):return dict(id=i,name=name,parent=parent,children=[],active=True,position=pos+[0],rotation=[0,0,0,1],scale=[1,1,1],size=size,pivot=[.5,.5],color=color,rawJson='{}',components=comps)
mocknodes=[mocknode(0,'MockFlowCanvas',-1,[0,0],[750,1624],[mockcomponent('cc.Canvas')]),mocknode(1,'Status',0,[0,350],[700,240],[mockcomponent('cc.Label','Local mock / no network or money transfer')])]
for i,title,y in [(2,'Watch reward ad (mock)',100),(4,'Cancel reward ad (mock)',-40),(6,'Request withdrawal (mock)',-180)]:
    mocknodes.append(mocknode(i,title,0,[0,y],[580,100],[mockcomponent('cc.Sprite'),mockcomponent('cc.Button')],[.14,.35,.65,1]))
    mocknodes.append(mocknode(i+1,'Label',i,[0,0],[560,90],[mockcomponent('cc.Label',title)]))
for n in mocknodes:n['children']=[x['id'] for x in mocknodes if x['parent']==n['id']]
emit({'nodes':mocknodes,'uuid':'local-mock-flow','variant':'Framework','name':'MockFlow','mockFlow':True},A/'Scenes/MockFlow.unity')
for p in sorted(A.rglob('*')):
    if p.is_file() and p.suffix!='.meta':meta(p)
(U/'ProjectSettings/EditorBuildSettings.asset').write_text('%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n--- !u!1045 &1\nEditorBuildSettings:\n  m_ObjectHideFlags: 0\n  serializedVersion: 2\n  m_Scenes:\n  - enabled: 1\n    path: Assets/Scenes/MockFlow.unity\n    guid: '+guid(A/'Scenes/MockFlow.unity')+'\n  m_configObjects: {}\n',encoding='utf-8')
(O/'07_Verification/native_yaml_inventory.json').write_text(json.dumps(allstats,indent=2),encoding='utf-8')
(O/'07_Verification/native_yaml_unported_components.json').write_text(json.dumps(unmapped,ensure_ascii=False,indent=2),encoding='utf-8')
print({'prefabs':len(list((A/'Prefabs').rglob('*.prefab'))),'scenes':len(list((A/'Scenes').glob('*.unity'))),'metadataOnlyComponents':len(unmapped),'editorValidation':'blocked: Unity license'})

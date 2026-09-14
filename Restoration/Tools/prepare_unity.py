import json,pathlib,shutil,re,collections
R=pathlib.Path(__file__).resolve().parents[2];O=R/'Restoration';U=O/'06_UnityFramework'
def read(p):return json.loads(p.read_text(encoding='utf-8'))
def write(p,v):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(v,ensure_ascii=False,indent=2),encoding='utf-8')
def values(v,default):return v.get('values',default) if isinstance(v,dict) else v if v is not None else default
def ref(v):return v.get('$ref',-1) if isinstance(v,dict) else -1
def asset(v):return v.get('$asset','') if isinstance(v,dict) else ''
write(U/'Packages/manifest.json',{'dependencies':{'com.unity.ugui':'1.0.0','com.unity.modules.animation':'1.0.0','com.unity.modules.audio':'1.0.0','com.unity.modules.imageconversion':'1.0.0','com.unity.modules.jsonserialize':'1.0.0','com.unity.modules.physics2d':'1.0.0','com.unity.modules.ui':'1.0.0'}})
(U/'ProjectSettings').mkdir(parents=True,exist_ok=True)
(U/'ProjectSettings/ProjectVersion.txt').write_text('m_EditorVersion: 2022.3.62f3c1\n',encoding='utf-8')
for folder in ['Scripts/Core','Scripts/Gameplay','Scripts/SDK','Scripts/UI','Editor','Prefabs','Scenes','Resources/Recovered','Art','Config']:(U/'Assets'/folder).mkdir(parents=True,exist_ok=True)
known={'cc.Canvas','cc.Camera','cc.Sprite','cc.Label','cc.RichText','cc.LabelOutline','cc.LabelShadow','cc.Button','cc.Widget','cc.BlockInputEvents','cc.Mask','cc.ScrollView','cc.EditBox','cc.Layout','cc.ProgressBar','cc.Animation','cc.RigidBody','cc.PhysicsCircleCollider','cc.PhysicsBoxCollider','cc.PhysicsPolygonCollider','cc.PhysicsChainCollider','cc.CircleCollider','cc.BoxCollider','cc.PolygonCollider','sp.Skeleton'}
models=[];migration=[];import_sprites=[]
for variant in ['Packaged','HotUpdate']:
    shutil.copytree(O/'04_Assets'/variant,U/'Assets/Art'/variant,dirs_exist_ok=True,ignore=shutil.ignore_patterns('ContactSheets'))
    manifest=read(O/'02_Gameplay'/variant/'module_manifest.json');names={m['classId']:m['name'] for m in manifest if m['classId']}
    for sprite in read(O/'04_Assets'/variant/'sprites.json'):
        import_sprites.append({'uuid':sprite['uuid'],'variant':variant,'path':'Assets/Art/'+variant+'/'+sprite['file'],'border':sprite['capInsets']})
    for record in read(O/'05_PrefabModel'/variant/'prefab_inventory.json'):
        d=read(O/'05_PrefabModel'/variant/'Decoded'/(record['uuid']+'.json'));objects=d['objects'];nodes=[]
        for i,n in enumerate(objects):
            if n.get('__type__') not in ['cc.Node','cc.Scene']:continue
            trs=n.get('_trs',[0,0,0,0,0,0,1,1,1,1]);trs=values(trs,[0,0,0,0,0,0,1,1,1,1])
            rawcolor=values(n.get('_color'),[4294967295])[0];color=[(rawcolor>>s&255)/255 for s in [0,8,16,24]];color[3]*=n.get('_opacity',255)/255
            comps=[]
            for ci,c in enumerate(objects):
                if ref(c.get('node'))!=i:continue
                typ=c['__type__'];comp={'type':typ,'className':names.get(typ,typ),'sourceId':ci,'rawJson':json.dumps(c,ensure_ascii=False),'sprite':asset(c.get('_spriteFrame')),'text':c.get('_string',''),'fontSize':c.get('_fontSize',40),'lineHeight':c.get('_lineHeight',40),'horizontalAlign':c.get('_N$horizontalAlign',0),'verticalAlign':c.get('_N$verticalAlign',0),'font':asset(c.get('_N$file')),'spriteType':c.get('_type',0),'fillRange':c.get('_fillRange',1),'fillStart':c.get('_fillStart',0),'fillType':c.get('_fillType',0),'enabled':c.get('_enabled',True),'target':ref(c.get('_N$target',c.get('_target'))),'content':ref(c.get('content',c.get('_N$content'))),'viewport':ref(c.get('_N$view')),'radius':c.get('_radius',c.get('radius',25)),'offset':values(c.get('_offset',c.get('offset')),[0,0]),'size':values(c.get('_size'),[100,100]),'bodyType':c.get('_type',2),'gravityScale':c.get('_gravityScale',1),'linearDamping':c.get('_linearDamping',0),'angularDamping':c.get('_angularDamping',0),'friction':c.get('_friction',0.2),'restitution':c.get('_restitution',0),'density':c.get('_density',1),'sensor':c.get('_sensor',False),'isCustom':typ not in known}
                comps.append(comp);migration.append({'variant':variant,'prefab':record['name'],'node':n.get('_name','Node'),'component':typ,'module':names.get(typ,''),'sourceId':ci,'status':'metadata + native approximation' if typ in known else 'full JS body preserved; C# behavior port pending'})
            children=[ref(a) for a in n.get('_children',[])]
            nodes.append({'id':i,'name':n.get('_name','Scene'),'parent':ref(n.get('_parent')),'children':children,'active':True if n.get('__type__')=='cc.Scene' else n.get('_active',True),'position':trs[:3],'rotation':trs[3:7],'scale':trs[7:10],'size':values(n.get('_contentSize'),[0,0]),'pivot':values(n.get('_anchorPoint'),[.5,.5]),'color':color,'rawJson':json.dumps(n,ensure_ascii=False),'components':comps})
        model={'variant':variant,'uuid':record['uuid'],'name':record['name'],'type':record['type'],'nodes':nodes,'originalObjectCount':len(objects)}
        write(U/'Assets/Resources/Recovered'/(variant+'_'+record['uuid']+'.json'),model)
        models.append({'variant':variant,**record,'nodes':len(nodes)})
    shutil.copytree(O/'03_Configuration'/variant,U/'Assets/Config'/variant,dirs_exist_ok=True)
shutil.copytree(O/'03_Configuration/EffectiveRuntime',U/'Assets/Config/EffectiveRuntime',dirs_exist_ok=True)
write(U/'Assets/Resources/Recovered/sprite_import.json',{'sprites':import_sprites})
write(O/'07_Verification/component_migration.json',migration)
write(O/'07_Verification/unity_model_inventory.json',models)
print({'models':len(models),'nodes':sum(m['nodes'] for m in models),'components':len(migration),'sprites':len(import_sprites)})

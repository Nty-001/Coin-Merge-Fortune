// Offline reference only. The external evaluation library is NOT copied into Unity or Git artifacts.
const fs=require('fs'),path=require('path'),vm=require('vm'),crypto=require('crypto');
const R=path.resolve(__dirname,'..'),workspace=path.dirname(R);
const library=path.join(workspace,'paid_ui_work/spine_authoring/spine-core.js');
const code=fs.readFileSync(library,'utf8'),digest=crypto.createHash('sha256').update(code).digest('hex');
if(digest!=='f1e0a31b9906e4d4daf2733857d21381ddbbe75adec7f4d83e1cc9b2b070dfc1')throw Error('Reference library hash differs; review before use.');
const context={console};vm.runInNewContext(code,context,{timeout:10000});const s=context.spine;
const output={reference:'Official spine-ts 3.8 evaluation build; offline numeric oracle only',url:'https://raw.githubusercontent.com/EsotericSoftware/spine-runtimes/3.8/spine-ts/build/spine-core.js',sha256:digest,assets:[]};
const poses={assets:[]};
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
for(const file of fs.readdirSync(R+'/04_Assets/HotUpdate/SkeletalSource').filter(x=>x.endsWith('.json'))){
  const source=read(R+'/04_Assets/HotUpdate/SkeletalSource/'+file),json=source._skeletonJson;
  const row={name:source._name,uuid:file.slice(0,-5),bones:json.bones.length,slots:json.slots.length,transformModes:[...new Set(json.bones.map(x=>x.transform||'normal'))],animations:[],error:null};
  output.assets.push(row);
  try{
    const textures={};source.textureNames.forEach((name,i)=>{
      const folder=R+'/04_Assets/HotUpdate/Native/'+source.textures[i].$asset;
      const buffer=fs.readFileSync(folder+'/'+fs.readdirSync(folder).find(x=>x.endsWith('.png')));
      textures[name]={width:buffer.readUInt32BE(16),height:buffer.readUInt32BE(20)};
    });
    const atlas=new s.TextureAtlas(source._atlasText,name=>({getImage:()=>textures[name],setFilters(){},setWraps(){}}));
    const loader=new s.SkeletonJson(new s.AtlasAttachmentLoader(atlas));
    const input=JSON.parse(JSON.stringify(json));
    if(!Array.isArray(input.skins))input.skins=Object.entries(input.skins).map(([name,attachments])=>({name,attachments}));
    function legacyCurves(value){
      if(!value||typeof value!=='object')return;
      if(Array.isArray(value.curve)){const [c1,c2,c3,c4]=value.curve;Object.assign(value,{curve:c1,c2,c3,c4});}
      for(const child of Object.values(value))legacyCurves(child);
    }
    legacyCurves(input);
    const skeletonData=loader.readSkeletonData(input);const skeleton=new s.Skeleton(skeletonData);
    const all=[];
    for(const animation of skeletonData.animations){
      const frames=[];let maxVertices=0;
      for(const portion of [0,.137,.333,.617,.999]){
        skeleton.setToSetupPose();animation.apply(skeleton,0,animation.duration*portion,false,[],1,s.MixBlend.replace,s.MixDirection.mixIn);skeleton.updateWorldTransform();
        const bones=skeleton.bones.map(b=>({name:b.data.name,a:b.a,b:b.b,c:b.c,d:b.d,x:b.worldX,y:b.worldY}));
        const slots=[];let count=0;
        for(const slot of skeleton.drawOrder){
          const attachment=slot.getAttachment();if(!attachment)continue;
          let vertices=[];
          if(attachment instanceof s.RegionAttachment){vertices=new Array(8);attachment.computeWorldVertices(slot.bone,vertices,0,2);}
          else if(attachment instanceof s.MeshAttachment||attachment instanceof s.ClippingAttachment){vertices=new Array(attachment.worldVerticesLength);attachment.computeWorldVertices(slot,0,attachment.worldVerticesLength,vertices,0,2);}
          count+=vertices.length/2;
          slots.push({name:slot.data.name,attachment:attachment.name,vertices,color:[slot.color.r,slot.color.g,slot.color.b,slot.color.a]});
        }
        maxVertices=Math.max(maxVertices,count);frames.push({time:animation.duration*portion,bones,slots});
      }
      row.animations.push({name:animation.name,duration:animation.duration,maxVertices,referenceSamples:frames.length});all.push({name:animation.name,frames});
      if(maxVertices===0)throw Error('Animation has no evaluated vertices: '+animation.name);
    }
    poses.assets.push({name:source._name,uuid:row.uuid,animations:all});
  }catch(error){row.error=error.message;}
}
fs.writeFileSync(R+'/07_Verification/skeletal_reference_probe.json',JSON.stringify(output,null,2));
fs.writeFileSync(R+'/07_Verification/skeletal_reference_poses.json',JSON.stringify(poses));
console.log(JSON.stringify(output,null,2));
if(output.assets.some(x=>x.error))process.exitCode=1;

// Convert source skeletal structure and curve data to Unity authoring DTOs. No external runtime ships in Unity.
const fs=require('fs'),path=require('path'),vm=require('vm'),crypto=require('crypto');
const R=path.resolve(__dirname,'..'),A=R+'/06_UnityFramework/Assets';
const code=fs.readFileSync(path.dirname(R)+'/paid_ui_work/spine_authoring/spine-core.js','utf8');
if(crypto.createHash('sha256').update(code).digest('hex')!=='f1e0a31b9906e4d4daf2733857d21381ddbbe75adec7f4d83e1cc9b2b070dfc1')throw Error('Reference tool must match reviewed hash.');
const context={console};vm.runInNewContext(code,context,{timeout:10000});const s=context.spine;
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
const destination=R+'/05_PrefabModel/NativeSkeletal';fs.mkdirSync(destination,{recursive:true});
function normalize(input){
  if(!Array.isArray(input.skins))input.skins=Object.entries(input.skins).map(([name,attachments])=>({name,attachments}));
  function walk(v){if(!v||typeof v!=='object')return;if(Array.isArray(v.curve)){let[c1,c2,c3,c4]=v.curve;Object.assign(v,{curve:c1,c2,c3,c4});}for(const c of Object.values(v))walk(c);}
  walk(input);return input;
}
function color(c){return [c.r,c.g,c.b,c.a];}
function track(timeline,entries,column,initial,scale=1,offset=0,rotate=false){
  const frames=timeline.frames,n=frames.length/entries,keys=[];let previousRaw=0,previousValue=0;
  const values=[];
  for(let i=0;i<n;i++){
    const raw=frames[i*entries+column];let value=raw;
    if(rotate&&i>0){let delta=raw-previousRaw;delta-=Math.floor((delta+180)/360)*360;value=previousValue+delta;}
    values.push(offset+scale*value);previousRaw=raw;previousValue=value;
  }
  if(frames[0]>0)keys.push({time:0,value:initial,hold:true});
  for(let i=0;i<n;i++){
    const time=frames[i*entries],value=values[i];
    const type=i<n-1?timeline.getCurveType(i):0;keys.push({time,value,hold:type===1});
    if(type===2&&i<n-1){
      let base=i*s.CurveTimeline.BEZIER_SIZE;
      for(let k=base+1;k<base+s.CurveTimeline.BEZIER_SIZE;k+=2){
        const x=timeline.curves[k],y=timeline.curves[k+1];
        keys.push({time:time+x*(frames[(i+1)*entries]-time),value:value+(values[i+1]-value)*y,hold:false});
      }
    }
  }
  return keys;
}
const report=[];
for(const file of fs.readdirSync(R+'/04_Assets/HotUpdate/SkeletalSource').filter(x=>x.endsWith('.json'))){
  const source=read(R+'/04_Assets/HotUpdate/SkeletalSource/'+file),input=normalize(source._skeletonJson),images={};
  if(source.textureNames.length!==1)throw Error('Multiple pages need a material split: '+source._name);
  source.textureNames.forEach((name,i)=>{
    const folder=R+'/04_Assets/HotUpdate/Native/'+source.textures[i].$asset;
    const from=folder+'/'+fs.readdirSync(folder).find(x=>x.endsWith('.png')),buffer=fs.readFileSync(from);
    images[name]={width:buffer.readUInt32BE(16),height:buffer.readUInt32BE(20)};
    const to=A+'/Resources/Skeletal/Atlases/'+source._name+'.png';fs.mkdirSync(path.dirname(to),{recursive:true});fs.copyFileSync(from,to);
  });
  const atlas=new s.TextureAtlas(source._atlasText,name=>({getImage:()=>images[name],setFilters(){},setWraps(){}}));
  const data=new s.SkeletonJson(new s.AtlasAttachmentLoader(atlas)).readSkeletonData(input);
  const model={name:source._name,uuid:file.slice(0,-5),texturePath:'Skeletal/Atlases/'+source._name,bones:[],slots:[],attachments:[],animations:[]};
  for(const bone of data.bones){
    if(bone.transformMode!==s.TransformMode.Normal)throw Error('Unsupported bone inheritance mode');
    model.bones.push({name:bone.name,parent:bone.parent?bone.parent.index:-1,x:bone.x,y:bone.y,rotation:bone.rotation,scaleX:bone.scaleX,scaleY:bone.scaleY,shearX:bone.shearX,shearY:bone.shearY});
  }
  const attachments=new Map();
  for(const skin of data.skins){
    for(const entry of skin.getAttachments()){
      const a=entry.attachment;if(attachments.has(a))continue;
      const out={name:entry.name,slot:entry.slotIndex,skin:skin.name,type:0,color:a.color?color(a.color):[1,1,1,1],offsets:[0],boneIndices:[],positions:[],weights:[],uvs:[],triangles:[],endSlot:-1,weighted:!!a.bones};
      if(a instanceof s.RegionAttachment){
        for(let v=0;v<4;v++){out.boneIndices.push(data.slots[entry.slotIndex].boneData.index);out.positions.push(a.offset[2*v],a.offset[2*v+1]);out.weights.push(1);out.offsets.push(v+1);}
        out.uvs=Array.from(a.uvs);out.triangles=[0,1,2,2,3,0];
      }else if(a instanceof s.VertexAttachment){
        out.type=a instanceof s.ClippingAttachment?2:1;
        const count=a.worldVerticesLength/2;
        if(a.bones){let cursor=0,vertex=0;for(let v=0;v<count;v++){const n=a.bones[cursor++];for(let b=0;b<n;b++){out.boneIndices.push(a.bones[cursor++]);out.positions.push(a.vertices[vertex],a.vertices[vertex+1]);out.weights.push(a.vertices[vertex+2]);vertex+=3;}out.offsets.push(out.weights.length);}}
        else for(let v=0;v<count;v++){out.boneIndices.push(data.slots[entry.slotIndex].boneData.index);out.positions.push(a.vertices[2*v],a.vertices[2*v+1]);out.weights.push(1);out.offsets.push(v+1);}
        if(out.type===2){
          out.endSlot=a.endSlot?a.endSlot.index:-1;
          if(out.weighted)throw Error('Weighted clipping shape needs verified decomposition.');
          let sign=0;out.clipConvex=true;
          for(let j=0;j<count;j++){
            const k=(j+1)%count,l=(j+2)%count;
            const cross=(out.positions[k*2]-out.positions[j*2])*(out.positions[l*2+1]-out.positions[k*2+1])-(out.positions[k*2+1]-out.positions[j*2+1])*(out.positions[l*2]-out.positions[k*2]);
            if(Math.abs(cross)>1e-5){if(sign&&Math.sign(cross)!==sign)out.clipConvex=false;sign=Math.sign(cross);}
          }
          out.clipTriangles=Array.from(new s.Triangulator().triangulate(out.positions));
        }
        else {out.uvs=Array.from(a.uvs);out.triangles=Array.from(a.triangles);}
      }else throw Error('Unsupported attachment '+a.name);
      // Unity texture coordinate origin is bottom left.
      for(let i=1;i<out.uvs.length;i+=2)out.uvs[i]=1-out.uvs[i];
      attachments.set(a,model.attachments.length);model.attachments.push(out);
    }
  }
  for(const slot of data.slots){
    const initial=data.defaultSkin&&data.defaultSkin.getAttachment(slot.index,slot.attachmentName);
    model.slots.push({name:slot.name,bone:slot.boneData.index,initialAttachment:initial?attachments.get(initial):-1,color:color(slot.color),blend:slot.blendMode,deformLength:Math.max(0,...model.attachments.filter(x=>x.slot===slot.index).map(x=>x.positions.length))});
  }
  function findAttachment(slot,name){if(name==null)return -1;const a=data.defaultSkin&&data.defaultSkin.getAttachment(slot,name);if(!a)throw Error('Missing attachment '+name);return attachments.get(a);}
  for(const animation of data.animations){
    const out={name:animation.name,duration:animation.duration,curves:[],deforms:[]};
    const seen=new Set();
    function add(target,index,property,keys){out.curves.push({target,index,property,keys});seen.add(target+':'+index+':'+property);}
    for(const timeline of animation.timelines){
      let bone=timeline.boneIndex,base=model.bones[bone];
      if(timeline instanceof s.RotateTimeline)add(0,bone,'rotation',track(timeline,2,1,base.rotation,1,base.rotation,true));
      else if(timeline instanceof s.ScaleTimeline){add(0,bone,'scaleX',track(timeline,3,1,base.scaleX,base.scaleX));add(0,bone,'scaleY',track(timeline,3,2,base.scaleY,base.scaleY));}
      else if(timeline instanceof s.ShearTimeline){add(0,bone,'shearX',track(timeline,3,1,base.shearX,1,base.shearX));add(0,bone,'shearY',track(timeline,3,2,base.shearY,1,base.shearY));}
      else if(timeline instanceof s.TranslateTimeline){add(0,bone,'x',track(timeline,3,1,base.x,1,base.x));add(0,bone,'y',track(timeline,3,2,base.y,1,base.y));}
      else if(timeline instanceof s.ColorTimeline){for(const [column,property] of ['r','g','b','a'].entries())add(1,timeline.slotIndex,property,track(timeline,5,column+1,model.slots[timeline.slotIndex].color[column]));}
      else if(timeline instanceof s.AttachmentTimeline){const keys=[];if(timeline.frames[0]>0)keys.push({time:0,value:model.slots[timeline.slotIndex].initialAttachment,hold:true});for(let i=0;i<timeline.frames.length;i++)keys.push({time:timeline.frames[i],value:findAttachment(timeline.slotIndex,timeline.attachmentNames[i]),hold:true});add(1,timeline.slotIndex,'attachment',keys);}
      else if(timeline instanceof s.DeformTimeline){
        const attachment=attachments.get(timeline.attachment),shape=model.attachments[attachment],curves=[];
        for(let v=0;v<timeline.frameVertices[0].length;v++){
          const fake={frames:[],curves:timeline.curves,getCurveType:i=>timeline.getCurveType(i)};
          for(let f=0;f<timeline.frames.length;f++)fake.frames.push(timeline.frames[f],timeline.frameVertices[f][v]);
          curves.push({target:2,index:v,property:'deform',keys:track(fake,2,1,shape.weighted?0:shape.positions[v])});
        }
        out.deforms.push({slot:timeline.slotIndex,attachment,firstTime:timeline.frames[0],curves});
      }else throw Error('Timeline needs explicit port: '+timeline.constructor.name);
    }
    for(let i=0;i<model.bones.length;i++)for(const property of ['x','y','rotation','scaleX','scaleY','shearX','shearY'])if(!seen.has('0:'+i+':'+property))add(0,i,property,[{time:0,value:model.bones[i][property],hold:true}]);
    for(let i=0;i<model.slots.length;i++)for(const [c,property] of ['r','g','b','a','attachment'].entries())if(!seen.has('1:'+i+':'+property))add(1,i,property,[{time:0,value:c===4?model.slots[i].initialAttachment:model.slots[i].color[c],hold:true}]);
    model.animations.push(out);
  }
  fs.writeFileSync(destination+'/'+source._name+'.json',JSON.stringify(model));
  report.push({name:model.name,bones:model.bones.length,slots:model.slots.length,attachments:model.attachments.length,animations:model.animations.length});
}
fs.writeFileSync(R+'/07_Verification/native_skeletal_projection.json',JSON.stringify({assets:report,scope:'Authored skeleton, weighted geometry, slot and native-curve projection. Unity import and numeric verification follow separately.'},null,2));
console.log(JSON.stringify(report,null,2));

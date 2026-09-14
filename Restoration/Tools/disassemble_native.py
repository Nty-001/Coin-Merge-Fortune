import sys,json,csv,hashlib,bisect,time
from pathlib import Path
R=Path(__file__).resolve().parents[2];sys.path.insert(0,str(Path(__file__).parent/'python_deps'))
from capstone import Cs,CS_ARCH_ARM64,CS_MODE_LITTLE_ENDIAN
from elftools.elf.elffile import ELFFile
source=R/'export_20260914/unpacked/split_config.arm64_v8a/lib/arm64-v8a/libcocos2djs.so'
out=R/'Restoration/02_Gameplay/NativeCocosAssembly';out.mkdir(parents=True,exist_ok=True)
functions={};section_results=[];references=[];start=time.time()
with source.open('rb') as f:
    elf=ELFFile(f)
    if elf.header['e_machine']!='EM_AARCH64':raise ValueError('Unexpected architecture')
    for table in [elf.get_section_by_name('.symtab'),elf.get_section_by_name('.dynsym')]:
        if table is None:continue
        for s in table.iter_symbols():
            if s['st_info']['type']=='STT_FUNC' and s['st_value']:
                entry=functions.setdefault(s['st_value'],{'address':s['st_value'],'size':s['st_size'],'names':set()});entry['names'].add(s.name);entry['size']=max(entry['size'],s['st_size'])
    symbols=sorted(functions);engine=Cs(CS_ARCH_ARM64,CS_MODE_LITTLE_ENDIAN);engine.skipdata=True
    for section in elf.iter_sections():
        if not section['sh_flags']&4 or not section['sh_size']:continue
        body=section.data();address=section['sh_addr'];sectionname=section.name;path=out/(sectionname.lstrip('.')+'.asm')
        count=coverage=unknown=0
        with path.open('w',encoding='utf-8') as assembly:
            assembly.write(f'; Full executable section {sectionname}; VA 0x{address:x}; {len(body)} bytes\n')
            for a,size,mnemonic,operands in engine.disasm_lite(body,address):
                if a in functions:
                    entry=functions[a];entry['assemblyFile']=path.name;entry['fileByteOffset']=assembly.tell();entry['line']=count+2
                    assembly.write('\n; FUNCTION '+ ' | '.join(sorted(entry['names']))+'\n')
                offset=a-address;assembly.write(f'{a:016x}: {body[offset:offset+size].hex():<12} {mnemonic:<9} {operands}\n')
                count+=1;coverage+=size
                if mnemonic=='.byte':unknown+=size
                if mnemonic in ['bl','blr','br','b']:
                    i=bisect.bisect_right(symbols,a)-1;owner=functions[symbols[i]] if i>=0 else None
                    target=int(operands[1:],16) if operands.startswith('#0x') else None
                    references.append({'address':hex(a),'instruction':mnemonic,'operand':operands,'sourceSymbolAddress':hex(owner['address']) if owner and a<owner['address']+owner['size'] else '', 'target':hex(target) if target is not None else '', 'targetNames':' | '.join(sorted(functions.get(target,{}).get('names',[])))})
        section_results.append({'section':sectionname,'address':hex(address),'bytes':len(body),'coveredBytes':coverage,'pseudoDataBytes':unknown,'instructions':count,'file':path.name})
        print(sectionname,'bytes',coverage,'/',len(body),'instructions',count,flush=True)
rows=[]
for address in symbols:
    x=functions[address];rows.append({'address':hex(address),'size':x['size'],'names':' | '.join(sorted(x['names'])),'assemblyFile':x.get('assemblyFile',''),'fileByteOffset':x.get('fileByteOffset','')})
for name,data in [('functions.csv',rows),('branch_references.csv',references)]:
    with (out/name).open('w',newline='',encoding='utf-8-sig') as f:w=csv.DictWriter(f,fieldnames=data[0].keys());w.writeheader();w.writerows(data)
result={'source':str(source.relative_to(R)),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'architecture':'AArch64','functionSymbols':len(rows),'sections':section_results,'allExecutableBytesCovered':all(x['bytes']==x['coveredBytes'] for x in section_results),'elapsedSeconds':round(time.time()-start,2),'limitations':['Disassembly is not source-level C++ recovery.','Stripped function names and indirect call destinations are not invented.','Original package has no libil2cpp.so; this is the actual Cocos native library.','Complete executable section coverage does not imply all higher-level semantics are understood.']}
(out/'coverage.json').write_text(json.dumps(result,indent=2),encoding='utf-8');print(json.dumps(result),flush=True)

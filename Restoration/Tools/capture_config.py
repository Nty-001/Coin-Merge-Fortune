import json,urllib.request,base64,hashlib,datetime
from pathlib import Path
R=Path(__file__).resolve().parents[2];O=R/'Restoration/03_Configuration/ServerCapture';O.mkdir(parents=True,exist_ok=True)
client=json.loads((R/'Restoration/03_Configuration/CapturedDevice/coin_clientId.json').read_text(encoding='utf-8'))
url='https://coinmergefortune.top/coin?conf=GameData'
record={'url':url,'method':'GET','requestHeaders':{'coin':client},'capturedUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'scope':'This client, this response time only'}
try:
    req=urllib.request.Request(url,headers={'coin':client})
    with urllib.request.urlopen(req,timeout=25) as response:
        body=response.read();record.update(status=response.status,responseHeaders=dict(response.headers))
    (O/'GameData.response.txt').write_bytes(body)
    record['sha256']=hashlib.sha256(body).hexdigest()
    try:decoded=json.loads(base64.b64decode(body))
    except Exception:decoded=json.loads(body)
    (O/'GameData.decoded.json').write_text(json.dumps(decoded,ensure_ascii=False,indent=2),encoding='utf-8')
    record['decoded']=True
except Exception as e:record['error']=repr(e)
(O/'request_response_metadata.json').write_text(json.dumps(record,ensure_ascii=False,indent=2),encoding='utf-8')
print({k:v for k,v in record.items() if k not in ('requestHeaders','responseHeaders')})

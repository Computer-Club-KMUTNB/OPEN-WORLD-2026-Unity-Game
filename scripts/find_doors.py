import re

with open('Assets/Scenes/Dev_Dungeon_Flow.unity', 'r', encoding='utf-8') as f:
    text = f.read()

blocks = re.findall(r'--- !u!(\d+) &(\d+)\n([\s\S]*?)(?=\n--- !u!|\Z)', text)

print('All GOs with door/gate/portal/exit in name:')
for tid, fid, body in blocks:
    if tid == '1':
        m = re.search(r'm_Name: (.+)', body)
        if m:
            name = m.group(1).strip()
            if any(kw in name.lower() for kw in ['door', 'gate', 'portal', 'exit']):
                print(f'  fid={fid}  "{name}"')

print()
print('All MonoBehaviours (114) in scene (sample):')
count = 0
for tid, fid, body in blocks:
    if tid == '114':
        m_name = re.search(r'm_EditorClassIdentifier: (.+)', body)
        m_go = re.search(r'm_GameObject: \{fileID: (\d+)\}', body)
        if m_name:
            print(f'  &{fid} -> GO:{m_go.group(1) if m_go else "?"} class:{m_name.group(1).strip()}')
        count += 1
        if count > 20:
            print('  ...(truncated)')
            break

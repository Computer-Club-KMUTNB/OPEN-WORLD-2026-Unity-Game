import glob
import re

def clean_orphan_components():
    yaml_header = "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n"

    for s in glob.glob('Assets/Scenes/*.unity'):
        with open(s, 'r', encoding='utf-8') as f:
            text = f.read()

        blocks = re.findall(r'--- !u!(\d+) &(\d+)\n([\s\S]*?)(?=\n--- !u!|\Z)', text)
        go_ids = set([int(fid) for tid, fid, _ in blocks if tid == '1'])

        valid_blocks = []
        removed_ids = set()

        for type_id, file_id, body in blocks:
            fid = int(file_id)
            if type_id not in ('1', '29'): # not GameObject or OcclusionCulling
                go_m = re.search(r'm_GameObject: \{fileID: (\d+)\}', body)
                if go_m:
                    target_go = int(go_m.group(1))
                    if target_go not in go_ids:
                        removed_ids.add(fid)
                        continue # Skip this orphan component!
            valid_blocks.append(f'--- !u!{type_id} &{file_id}\n{body}')

        full_text = yaml_header + '\n'.join(valid_blocks) + '\n'

        # Also remove addedObject references in PrefabInstances for any removed IDs
        for rid in removed_ids:
            full_text = re.sub(rf'\s+- targetCorrespondingSourceObject: [^\n]+\n\s+insertIndex: -1\n\s+addedObject: \{{fileID: {rid}\}}\n', '\n', full_text)

        with open(s, 'w', encoding='utf-8', newline='\n') as f:
            f.write(full_text)

        if removed_ids:
            print(f'{s}: Removed {len(removed_ids)} orphan components: {removed_ids}')
        else:
            print(f'{s}: Clean')

if __name__ == '__main__':
    clean_orphan_components()

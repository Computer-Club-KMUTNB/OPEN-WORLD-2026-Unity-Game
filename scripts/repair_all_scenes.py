import glob
import re

def repair_scenes():
    scenes = glob.glob('Assets/Scenes/*.unity')

    for scene_path in scenes:
        with open(scene_path, 'r', encoding='utf-8') as f:
            content = f.read()

        blocks = re.findall(r'--- !u!(\d+) &(\d+)\n([\s\S]*?)(?=\n--- !u!|\Z)', content)
        
        # Map GameObject ID -> Transform ID
        go_to_tf = {}
        tf_ids = set()
        
        for type_id, file_id, body in blocks:
            fid = int(file_id)
            if type_id in ('4', '224'):
                tf_ids.add(fid)
                go_m = re.search(r'm_GameObject: \{fileID: (\d+)\}', body)
                if go_m:
                    go_id = int(go_m.group(1))
                    go_to_tf[go_id] = fid

        modified = False
        repaired_blocks = []

        for type_id, file_id, body in blocks:
            if type_id in ('4', '224'):
                child_sec = re.search(r'(m_Children:\s*\n)((?:\s+- \{fileID: \d+\}\s*\n)*)', body)
                if child_sec:
                    header = child_sec.group(1)
                    lines = child_sec.group(2)
                    cur_children = [int(x) for x in re.findall(r'fileID: (\d+)', lines)]
                    fixed_children = []
                    for cid in cur_children:
                        if cid in tf_ids:
                            fixed_children.append(cid)
                        elif cid in go_to_tf:
                            fixed_children.append(go_to_tf[cid])
                            modified = True
                        else:
                            print(f'Pruning missing child {cid} from Transform {file_id} in {scene_path}')
                            modified = True
                    
                    new_lines = ''.join([f'  - {{fileID: {x}}}\n' for x in fixed_children])
                    new_sec = header + new_lines
                    body = body[:child_sec.start()] + new_sec + body[child_sec.end():]
                    
            repaired_blocks.append(f'--- !u!{type_id} &{file_id}\n{body}')

        if modified:
            new_content = '\n'.join(repaired_blocks) + '\n'
            with open(scene_path, 'w', encoding='utf-8', newline='\n') as f:
                f.write(new_content)
            print(f'Repaired {scene_path}')
        else:
            print(f'No repairs needed for {scene_path}')

if __name__ == '__main__':
    repair_scenes()

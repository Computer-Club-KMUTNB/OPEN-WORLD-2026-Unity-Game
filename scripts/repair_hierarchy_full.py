import glob
import re

def repair_scene_hierarchies():
    yaml_header = "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n"
    
    for s in glob.glob('Assets/Scenes/*.unity'):
        with open(s, 'r', encoding='utf-8') as f:
            text = f.read()

        blocks = re.findall(r'--- !u!(\d+) &(\d+)\n([\s\S]*?)(?=\n--- !u!|\Z)', text)
        tf_ids = set()
        go_to_tf = {}
        
        for type_id, file_id, body in blocks:
            fid = int(file_id)
            if type_id in ('4', '224'):
                tf_ids.add(fid)
                go_m = re.search(r'm_GameObject: \{fileID: (\d+)\}', body)
                if go_m:
                    go_to_tf[int(go_m.group(1))] = fid

        # Get existing SceneRoots
        roots_match = re.search(r'--- !u!29 &1\n[\s\S]*?m_SceneRoots:\s*\n((?:\s+- \{fileID: \d+\}\s*\n)*)', text)
        scene_roots = []
        if roots_match:
            scene_roots = [int(x) for x in re.findall(r'fileID: (\d+)', roots_match.group(1))]

        new_roots_to_add = set()
        repaired_blocks = []
        fixed_fathers_count = 0
        fixed_children_count = 0

        for type_id, file_id, body in blocks:
            fid = int(file_id)
            if type_id in ('4', '224'):
                # 1. Repair m_Father
                f_m = re.search(r'm_Father: \{fileID: (\d+)\}', body)
                if f_m:
                    father_id = int(f_m.group(1))
                    if father_id != 0:
                        if father_id in tf_ids:
                            pass # Valid
                        elif father_id in go_to_tf:
                            new_fid = go_to_tf[father_id]
                            body = body[:f_m.start()] + f'm_Father: {{fileID: {new_fid}}}' + body[f_m.end():]
                            fixed_fathers_count += 1
                        else:
                            # Parent deleted -> Make root
                            body = body[:f_m.start()] + 'm_Father: {fileID: 0}' + body[f_m.end():]
                            new_roots_to_add.add(fid)
                            fixed_fathers_count += 1

                # 2. Repair m_Children
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
                            fixed_children_count += 1
                        else:
                            fixed_children_count += 1 # Prune deleted child

                    new_lines = ''.join([f'  - {{fileID: {x}}}\n' for x in fixed_children])
                    new_sec = header + new_lines
                    body = body[:child_sec.start()] + new_sec + body[child_sec.end():]

            repaired_blocks.append(f'--- !u!{type_id} &{file_id}\n{body}')

        full_repaired = yaml_header + '\n'.join(repaired_blocks) + '\n'

        # Update SceneRoots if new roots were created
        if new_roots_to_add:
            for r in new_roots_to_add:
                if r not in scene_roots:
                    scene_roots.append(r)
            new_roots_text = '  m_SceneRoots:\n' + ''.join([f'  - {{fileID: {x}}}\n' for x in scene_roots])
            full_repaired = re.sub(r'  m_SceneRoots:\s*\n(?:\s+- \{fileID: \d+\}\s*\n)*', new_roots_text, full_repaired)

        with open(s, 'w', encoding='utf-8', newline='\n') as f:
            f.write(full_repaired)
        print(f'{s}: Repaired {fixed_fathers_count} fathers, {fixed_children_count} children. (New roots added: {len(new_roots_to_add)})')

if __name__ == '__main__':
    repair_scene_hierarchies()

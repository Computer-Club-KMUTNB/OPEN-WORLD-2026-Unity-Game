import glob
import re
import os

def fix_all_build_issues():
    print("=== Starting Project-Wide Build Fix ===")

    # 1. Disable Unity Services Prompt in UnityConnectSettings
    connect_settings = "ProjectSettings/UnityConnectSettings.asset"
    if os.path.exists(connect_settings):
        with open(connect_settings, 'r', encoding='utf-8') as f:
            cs = f.read()
        cs = re.sub(r'UnityConnectSettings:\n  m_ObjectHideFlags: 0\n  serializedVersion: 1\n  m_Enabled: 1', 
                    'UnityConnectSettings:\n  m_ObjectHideFlags: 0\n  serializedVersion: 1\n  m_Enabled: 0', cs)
        with open(connect_settings, 'w', encoding='utf-8', newline='\n') as f:
            f.write(cs)
        print("[OK] Disabled Unity Services prompt in UnityConnectSettings.asset")

    # 2. Fix broken PPtrs & Add solid boundary walls in restaurant scenes
    for rscene in ['Assets/Scenes/restaurant-scene.unity', 'Assets/Scenes/Dev_Restaurant_Flow.unity']:
        if not os.path.exists(rscene): continue
        with open(rscene, 'r', encoding='utf-8') as f:
            content = f.read()

        # Delete the 4 broken orphan BoxCollider blocks
        broken_ids = ['32766487', '375478568', '832506903', '1752142393']
        for bid in broken_ids:
            content = re.sub(rf'--- !u!65 &{bid}\n[\s\S]*?(?=--- !u!|\Z)', '', content)
            content = re.sub(rf'\s+- targetCorrespondingSourceObject: [^\n]+\n\s+insertIndex: -1\n\s+addedObject: \{{fileID: {bid}\}}\n', '\n', content)

        # Clean perimeter boundary walls
        boundary_walls_yaml = """--- !u!1 &990000050
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 990000051}
  - component: {fileID: 990000052}
  m_Layer: 0
  m_Name: Wall_North_Boundary
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &990000051
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000050}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 3, z: 7.2}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!65 &990000052
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000050}
  m_Material: {fileID: 0}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 0
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {x: 16, y: 8, z: 1}
  m_Center: {x: 0, y: 0, z: 0}
--- !u!1 &990000053
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 990000054}
  - component: {fileID: 990000055}
  m_Layer: 0
  m_Name: Wall_South_Boundary
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &990000054
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000053}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 3, z: -7.2}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!65 &990000055
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000053}
  m_Material: {fileID: 0}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 0
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {x: 16, y: 8, z: 1}
  m_Center: {x: 0, y: 0, z: 0}
--- !u!1 &990000056
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 990000057}
  - component: {fileID: 990000058}
  m_Layer: 0
  m_Name: Wall_East_Boundary
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &990000057
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000056}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 7.2, y: 3, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!65 &990000058
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000056}
  m_Material: {fileID: 0}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 0
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {x: 1, y: 8, z: 16}
  m_Center: {x: 0, y: 0, z: 0}
--- !u!1 &990000059
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 990000060}
  - component: {fileID: 990000061}
  m_Layer: 0
  m_Name: Wall_West_Boundary
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &990000060
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000059}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: -7.2, y: 3, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!65 &990000061
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000059}
  m_Material: {fileID: 0}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 0
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {x: 1, y: 8, z: 16}
  m_Center: {x: 0, y: 0, z: 0}
"""
        # Clean any old boundary blocks
        content = re.sub(r'--- !u!1 &99000005\d[\s\S]*?(?=--- !u!|\Z)', '', content)
        content = re.sub(r'--- !u!4 &99000005\d[\s\S]*?(?=--- !u!|\Z)', '', content)
        content = re.sub(r'--- !u!65 &99000005\d[\s\S]*?(?=--- !u!|\Z)', '', content)
        content = content.replace('  - {fileID: 990000051}\n', '')
        content = content.replace('  - {fileID: 990000054}\n', '')
        content = content.replace('  - {fileID: 990000057}\n', '')
        content = content.replace('  - {fileID: 990000060}\n', '')

        # Insert new boundary walls
        idx = content.rfind("SceneRoots:")
        if idx != -1:
            content = content[:idx] + boundary_walls_yaml + content[idx:]
            content += "  - {fileID: 990000051}\n"
            content += "  - {fileID: 990000054}\n"
            content += "  - {fileID: 990000057}\n"
            content += "  - {fileID: 990000060}\n"

        with open(rscene, 'w', encoding='utf-8', newline='\n') as f:
            f.write(content)
        print(f"[OK] Fixed PPtrs and added boundary walls to {rscene}")

    # 3. Clean EventSystems from additive pause scenes
    for pscene in ['Assets/Scenes/PauseMenu_Cute.unity', 'Assets/Scenes/PauseMenu_Hunt.unity']:
        if not os.path.exists(pscene): continue
        with open(pscene, 'r', encoding='utf-8') as f:
            pcontent = f.read()

        pcontent = re.sub(r'--- !u!1 &300\n[\s\S]*?(?=--- !u!|\Z)', '', pcontent)
        pcontent = re.sub(r'--- !u!4 &301\n[\s\S]*?(?=--- !u!|\Z)', '', pcontent)
        pcontent = re.sub(r'--- !u!114 &302\n[\s\S]*?(?=--- !u!|\Z)', '', pcontent)
        pcontent = re.sub(r'--- !u!114 &303\n[\s\S]*?(?=--- !u!|\Z)', '', pcontent)
        pcontent = pcontent.replace('  - {fileID: 301}\n', '')

        with open(pscene, 'w', encoding='utf-8', newline='\n') as f:
            f.write(pcontent)
        print(f"[OK] Cleaned redundant EventSystem from {pscene}")

    # 4. Repair YAML headers and all transform hierarchies across ALL 11 scenes
    yaml_header = "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n"
    for sc in glob.glob('Assets/Scenes/*.unity'):
        with open(sc, 'r', encoding='utf-8') as f:
            raw = f.read()

        if not raw.startswith("%YAML"):
            raw = yaml_header + raw.lstrip()

        blocks = re.findall(r'--- !u!(\d+) &(\d+)\n([\s\S]*?)(?=\n--- !u!|\Z)', raw)
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
                    
                    new_lines = ''.join([f'  - {{fileID: {x}}}\n' for x in fixed_children])
                    new_sec = header + new_lines
                    body = body[:child_sec.start()] + new_sec + body[child_sec.end():]

            repaired_blocks.append(f'--- !u!{type_id} &{file_id}\n{body}')

        final_content = yaml_header + '\n'.join(repaired_blocks) + '\n'
        with open(sc, 'w', encoding='utf-8', newline='\n') as f:
            f.write(final_content)
        print(f"[OK] Validated & repaired hierarchy in {sc}")

    print("=== All Build Fixes Completed Successfully! ===")

if __name__ == '__main__':
    fix_all_build_issues()

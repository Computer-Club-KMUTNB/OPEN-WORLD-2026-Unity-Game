import os
import re

def rebuild_restaurant_flow():
    src = "Assets/Scenes/restaurant-scene.unity"
    dst = "Assets/Scenes/Dev_Restaurant_Flow.unity"
    
    with open(src, "r", encoding="utf-8") as f:
        content = f.read()

    # Ensure CustomerSpawner autoStart is 0
    content = content.replace("autoStart: 1", "autoStart: 0")

    # Deactivate plain MoneyText (1247094590) to remove plain unstyled money text
    content = content.replace(
        "m_Name: MoneyText\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 1",
        "m_Name: MoneyText\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 0"
    )

    # Fit DoortoJRscenc BoxCollider tightly to door object and set isTrigger: 1
    # Find BoxCollider on DoortoJRscenc (594767802)
    content = re.sub(
        r'(!u!65 &594767802\nBoxCollider:[\s\S]*?m_IsTrigger: )\d+',
        r'\g<1>1',
        content
    )
    content = re.sub(
        r'(!u!65 &594767802\nBoxCollider:[\s\S]*?m_Size: )\{x: [^,]+, y: [^,]+, z: [^}]+\}',
        r'\1{x: 1.2, y: 2.2, z: 1.2}',
        content
    )

    manager_yaml = """--- !u!1 &990000001
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 990000002}
  - component: {fileID: 990000003}
  m_Layer: 0
  m_Name: RestaurantFlowManager
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &990000002
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000001}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &990000003
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000001}
  serializedVersion: 2
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 9a01142b540149ef9e03d749ab6e0022, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::RestaurantFlowController
  dungeonDoorObjectName: DoortoJRscenc
  doorInteractionDistance: 2
  dungeonSceneName: Dev_Dungeon_Flow
  shiftSummarySceneName: ShiftSummary_Cute
  pauseSceneName: PauseMenu_Cute
  servedOrders: 0
  happyGuests: 0
  totalGuests: 0
  dishesCooked: 0
  shiftTimer: 0
  isShiftActive: 0
"""
    # Insert before SceneRoots
    idx = content.rfind("SceneRoots:")
    if idx != -1:
        new_content = content[:idx] + manager_yaml + content[idx:]
        new_content += "  - {fileID: 990000002}\n"
        with open(dst, "w", encoding="utf-8") as f:
            f.write(new_content)
        print("Rebuilt Dev_Restaurant_Flow.unity with adjusted door collider and parameters!")

def rebuild_dungeon_flow():
    src = "Assets/Scenes/DemoScene.unity"
    dst = "Assets/Scenes/Dev_Dungeon_Flow.unity"

    with open(src, "r", encoding="utf-8") as f:
        content = f.read()

    # Disable legacy GameOverPanel
    content = re.sub(r'(!u!1 &661331842[\s\S]*?m_IsActive: )1', r'\g<1>0', content)

    manager_yaml = """--- !u!1 &990000010
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 990000011}
  - component: {fileID: 990000012}
  m_Layer: 0
  m_Name: DungeonFlowManager
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &990000011
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000010}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &990000012
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000010}
  serializedVersion: 2
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 9a01142b540149ef9e03d749ab6e0021, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::DungeonFlowController
  doorObjectName: Door to SRN
  interactionDistance: 2
  summarySceneName: ExpeditionSummary_Hunt
  directRestaurantSceneName: Dev_Restaurant_Flow
  pauseSceneName: PauseMenu_Hunt
  sessionKills: 0
  sessionDamage: 0
  sessionDuration: 0
"""
    # Insert before SceneRoots
    idx = content.rfind("SceneRoots:")
    if idx != -1:
        new_content = content[:idx] + manager_yaml + content[idx:]
        new_content += "  - {fileID: 990000011}\n"
        with open(dst, "w", encoding="utf-8") as f:
            f.write(new_content)
        print("Rebuilt Dev_Dungeon_Flow.unity successfully!")

def check_duplicates(path):
    with open(path, "r", encoding="utf-8") as f:
        text = f.read()
    ids = re.findall(r'^--- !u!\d+ &(\d+)', text, re.MULTILINE)
    seen = set()
    dups = []
    for x in ids:
        if x in seen:
            dups.append(x)
        seen.add(x)
    print(f"[{path}] Total objects: {len(ids)}, Duplicates: {len(dups)}")

if __name__ == "__main__":
    rebuild_restaurant_flow()
    rebuild_dungeon_flow()
    check_duplicates("Assets/Scenes/Dev_Restaurant_Flow.unity")
    check_duplicates("Assets/Scenes/Dev_Dungeon_Flow.unity")

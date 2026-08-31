import os

def setup_restaurant_dev():
    path = "Assets/Scenes/Dev_Restaurant_Flow.unity"
    with open(path, "r", encoding="utf-8") as f:
        content = f.read()

    if "RestaurantFlowManager" in content:
        print("Dev_Restaurant_Flow already has RestaurantFlowManager")
        return

    # Create GameObject for RestaurantFlowManager
    manager_yaml = """
--- !u!1 &990000001
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
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 9a01142b540149ef9e03d749ab6e0022, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::RestaurantFlowController
  dungeonDoorObjectName: DoortoJRscenc
  doorInteractionDistance: 3.5
  dungeonSceneName: Dev_Dungeon_Flow
  shiftSummarySceneName: ShiftSummary_Cute
  closeShiftKey: 99
  pauseSceneName: PauseMenu_Cute
  servedOrders: 0
  happyGuests: 0
  totalGuests: 0
  dishesCooked: 0
"""
    # Insert before SceneRoots
    idx = content.rfind("SceneRoots:")
    if idx != -1:
        new_content = content[:idx] + manager_yaml + content[idx:]
        # Add to roots list
        new_content += "  - {fileID: 990000002}\n"
        with open(path, "w", encoding="utf-8") as f:
            f.write(new_content)
        print("Updated Dev_Restaurant_Flow with RestaurantFlowManager")

def setup_dungeon_dev():
    path = "Assets/Scenes/Dev_Dungeon_Flow.unity"
    with open(path, "r", encoding="utf-8") as f:
        content = f.read()

    if "DungeonFlowManager" in content:
        print("Dev_Dungeon_Flow already has DungeonFlowManager")
        return

    # Create GameObject for DungeonFlowManager and Door
    dungeon_yaml = """
--- !u!1 &990000010
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
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 9a01142b540149ef9e03d749ab6e0021, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::DungeonFlowController
  doorObjectName: Door
  interactionDistance: 3.5
  summarySceneName: ExpeditionSummary_Hunt
  directRestaurantSceneName: Dev_Restaurant_Flow
  pauseSceneName: PauseMenu_Hunt
  sessionKills: 0
  sessionDamage: 0
  sessionDuration: 0
--- !u!1 &990000020
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 990000021}
  - component: {fileID: 990000022}
  m_Layer: 0
  m_Name: Door
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &990000021
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000020}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 1.5, z: -5}
  m_LocalScale: {x: 2, y: 3, z: 2}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!65 &990000022
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 990000020}
  m_Material: {fileID: 0}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 1
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {x: 3, y: 3, z: 3}
  m_Center: {x: 0, y: 0, z: 0}
"""
    idx = content.rfind("SceneRoots:")
    if idx != -1:
        new_content = content[:idx] + dungeon_yaml + content[idx:]
        new_content += "  - {fileID: 990000011}\n"
        new_content += "  - {fileID: 990000021}\n"
        with open(path, "w", encoding="utf-8") as f:
            f.write(new_content)
        print("Updated Dev_Dungeon_Flow with DungeonFlowManager and Door")

if __name__ == "__main__":
    setup_restaurant_dev()
    setup_dungeon_dev()

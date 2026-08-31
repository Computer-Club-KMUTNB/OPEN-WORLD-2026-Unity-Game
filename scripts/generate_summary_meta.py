import os
import hashlib

def get_guid(filepath):
    rel = os.path.relpath(filepath).replace('\\', '/').lower()
    return hashlib.md5(f"openworld2026_summary_salt_{rel}".encode('utf-8')).hexdigest()

def make_meta(filepath, is_sprite=True, border=(0,0,0,0)):
    guid = get_guid(filepath)
    b_l, b_b, b_r, b_t = border
    
    meta_content = f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: {b_l}, y: {b_b}, z: {b_r}, w: {b_t}}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 5e97eb03825dee720800000000000000
    internalID: 21300000
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  spritePackingTag: 
  pSDRemoveMatte: 0
  pSDShowRemoveMatteOption: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    with open(filepath + ".meta", "w", encoding="utf-8") as f:
        f.write(meta_content)
    print(f"Generated {filepath}.meta (GUID: {guid})")

def main():
    assets = [
        # Hunt
        ("Assets/Textures/Summary/Hunt/card_loot_bg.png", True, (16, 16, 16, 16)),
        ("Assets/Textures/Summary/Hunt/icon_meat.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Hunt/icon_bone.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Hunt/icon_venom.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Hunt/icon_herb.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Hunt/badge_rank_s.png", True, (0, 0, 0, 0)),
        # Cute
        ("Assets/Textures/Summary/Cute/card_cute_slot.png", True, (18, 18, 18, 18)),
        ("Assets/Textures/Summary/Cute/icon_coin.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Cute/icon_dish.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Cute/icon_customer.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Cute/icon_star_gold.png", True, (0, 0, 0, 0)),
        ("Assets/Textures/Summary/Cute/icon_star_empty.png", True, (0, 0, 0, 0)),
    ]
    for path, is_sprite, border in assets:
        make_meta(path, is_sprite, border)

if __name__ == "__main__":
    main()

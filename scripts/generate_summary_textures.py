import os
import math
from PIL import Image, ImageDraw, ImageFilter

os.makedirs("Assets/Textures/Summary/Hunt", exist_ok=True)
os.makedirs("Assets/Textures/Summary/Cute", exist_ok=True)

SCALE = 4

def save_scaled(img, path):
    w, h = img.size
    final = img.resize((w // SCALE, h // SCALE), Image.Resampling.LANCZOS)
    final.save(path, "PNG")
    print(f"Saved: {path}")

# ==================== HUNT THEME ASSETS ====================

# 1. Dark Iron Loot Card Slot (120x130)
def draw_loot_card():
    w, h = 120 * SCALE, 130 * SCALE
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    r = 16 * SCALE
    
    # Outer dark iron border
    d.rounded_rectangle([0, 0, w-1, h-1], radius=r, fill=(20, 16, 22, 230), outline=(90, 30, 35, 255), width=3*SCALE)
    # Inner dark obsidian recess
    pad = 6 * SCALE
    d.rounded_rectangle([pad, pad, w-1-pad, h-1-pad], radius=r-pad//2, fill=(12, 10, 14, 255), outline=(50, 40, 50, 200), width=2*SCALE)
    # Corner rivets
    rivet_r = 3 * SCALE
    for rx, ry in [(pad*2, pad*2), (w-pad*2, pad*2), (pad*2, h-pad*2), (w-pad*2, h-pad*2)]:
        d.ellipse([rx-rivet_r, ry-rivet_r, rx+rivet_r, ry+rivet_r], fill=(160, 140, 130, 255), outline=(40, 30, 30, 255), width=1*SCALE)
    save_scaled(img, "Assets/Textures/Summary/Hunt/card_loot_bg.png")

# 2. Monster Meat Cut Icon (128x128)
def draw_meat_icon():
    s = 128 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Glow behind
    glow = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    gd.ellipse([15*SCALE, 15*SCALE, (128-15)*SCALE, (128-15)*SCALE], fill=(220, 40, 50, 80))
    glow = glow.filter(ImageFilter.GaussianBlur(8*SCALE))
    img.paste(glow, (0,0), glow)
    d = ImageDraw.Draw(img)

    # Meat steak shape (reddish marbled steak with bone)
    # Main meat body
    d.ellipse([20*SCALE, 30*SCALE, 105*SCALE, 100*SCALE], fill=(185, 30, 45, 255), outline=(70, 10, 15, 255), width=4*SCALE)
    d.ellipse([40*SCALE, 20*SCALE, 115*SCALE, 85*SCALE], fill=(215, 45, 60, 255))
    # Marbling fat lines
    d.arc([35*SCALE, 40*SCALE, 95*SCALE, 85*SCALE], start=30, end=150, fill=(240, 200, 200, 200), width=4*SCALE)
    d.arc([50*SCALE, 30*SCALE, 105*SCALE, 70*SCALE], start=40, end=160, fill=(240, 210, 210, 180), width=3*SCALE)
    # Bone center
    d.ellipse([32*SCALE, 55*SCALE, 60*SCALE, 83*SCALE], fill=(245, 240, 230, 255), outline=(100, 85, 75, 255), width=3*SCALE)
    d.ellipse([40*SCALE, 63*SCALE, 52*SCALE, 75*SCALE], fill=(160, 140, 130, 255))
    
    save_scaled(img, "Assets/Textures/Summary/Hunt/icon_meat.png")

# 3. Beast Bone Icon (128x128)
def draw_bone_icon():
    s = 128 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Diagonal thick beast bone
    # Shaft
    d.polygon([
        (35*SCALE, 95*SCALE), (95*SCALE, 35*SCALE),
        (105*SCALE, 45*SCALE), (45*SCALE, 105*SCALE)
    ], fill=(240, 235, 220, 255), outline=(80, 70, 60, 255))
    # Knobs at top-right
    d.ellipse([85*SCALE, 15*SCALE, 115*SCALE, 45*SCALE], fill=(245, 240, 225, 255), outline=(80, 70, 60, 255), width=3*SCALE)
    d.ellipse([100*SCALE, 30*SCALE, 125*SCALE, 60*SCALE], fill=(235, 230, 215, 255), outline=(80, 70, 60, 255), width=3*SCALE)
    # Knobs at bottom-left
    d.ellipse([10*SCALE, 75*SCALE, 40*SCALE, 105*SCALE], fill=(245, 240, 225, 255), outline=(80, 70, 60, 255), width=3*SCALE)
    d.ellipse([25*SCALE, 90*SCALE, 55*SCALE, 120*SCALE], fill=(235, 230, 215, 255), outline=(80, 70, 60, 255), width=3*SCALE)
    # Highlights & blood splatter
    d.ellipse([45*SCALE, 75*SCALE, 55*SCALE, 85*SCALE], fill=(160, 20, 30, 220))
    d.ellipse([70*SCALE, 48*SCALE, 78*SCALE, 56*SCALE], fill=(180, 25, 35, 200))
    
    save_scaled(img, "Assets/Textures/Summary/Hunt/icon_bone.png")

# 4. Drake Venom Flask Icon (128x128)
def draw_venom_icon():
    s = 128 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Toxic green glow
    glow = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    gd.ellipse([25*SCALE, 30*SCALE, 105*SCALE, 110*SCALE], fill=(50, 255, 100, 100))
    glow = glow.filter(ImageFilter.GaussianBlur(10*SCALE))
    img.paste(glow, (0,0), glow)
    d = ImageDraw.Draw(img)
    
    # Glass flask round bottom
    d.ellipse([28*SCALE, 45*SCALE, 100*SCALE, 115*SCALE], fill=(20, 50, 30, 220), outline=(100, 200, 140, 255), width=3*SCALE)
    # Flask neck
    d.rectangle([52*SCALE, 20*SCALE, 76*SCALE, 55*SCALE], fill=(20, 50, 30, 220), outline=(100, 200, 140, 255), width=3*SCALE)
    # Cork stopper
    d.rectangle([50*SCALE, 12*SCALE, 78*SCALE, 22*SCALE], fill=(150, 100, 60, 255), outline=(60, 35, 20, 255), width=2*SCALE)
    # Glowing toxic liquid inside
    d.ellipse([34*SCALE, 60*SCALE, 94*SCALE, 110*SCALE], fill=(40, 230, 80, 255))
    # Bubbles
    d.ellipse([45*SCALE, 70*SCALE, 55*SCALE, 80*SCALE], fill=(180, 255, 200, 255))
    d.ellipse([70*SCALE, 80*SCALE, 78*SCALE, 88*SCALE], fill=(180, 255, 200, 255))
    d.ellipse([58*SCALE, 90*SCALE, 64*SCALE, 96*SCALE], fill=(180, 255, 200, 255))
    
    save_scaled(img, "Assets/Textures/Summary/Hunt/icon_venom.png")

# 5. Savage Wild Herbs Icon (128x128)
def draw_herb_icon():
    s = 128 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Wild rare spice leaves with glowing spores
    d.ellipse([25*SCALE, 30*SCALE, 80*SCALE, 90*SCALE], fill=(40, 150, 60, 255), outline=(15, 60, 25, 255), width=3*SCALE)
    d.ellipse([55*SCALE, 20*SCALE, 110*SCALE, 80*SCALE], fill=(60, 190, 80, 255), outline=(15, 60, 25, 255), width=3*SCALE)
    d.ellipse([40*SCALE, 50*SCALE, 95*SCALE, 110*SCALE], fill=(30, 120, 50, 255), outline=(15, 60, 25, 255), width=3*SCALE)
    # Stems
    d.line([(64*SCALE, 50*SCALE), (64*SCALE, 115*SCALE)], fill=(120, 80, 40, 255), width=4*SCALE)
    # Glowing golden spice berries
    for bx, by in [(45*SCALE, 45*SCALE), (85*SCALE, 35*SCALE), (75*SCALE, 70*SCALE)]:
        d.ellipse([bx-6*SCALE, by-6*SCALE, bx+6*SCALE, by+6*SCALE], fill=(255, 215, 0, 255), outline=(180, 100, 0, 255), width=2*SCALE)
        
    save_scaled(img, "Assets/Textures/Summary/Hunt/icon_herb.png")

# 6. S-Rank Hunt Badge (140x140)
def draw_s_rank_badge():
    s = 140 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Blood-gold glowing shield crest
    center = s // 2
    r = 60 * SCALE
    
    # Shield polygon
    pts = [
        (center, 8*SCALE),
        (s - 12*SCALE, 25*SCALE),
        (s - 22*SCALE, 95*SCALE),
        (center, s - 10*SCALE),
        (22*SCALE, 95*SCALE),
        (12*SCALE, 25*SCALE),
    ]
    # Gold border
    d.polygon(pts, fill=(150, 15, 25, 255), outline=(255, 215, 0, 255), width=5*SCALE)
    # Inner dark plate
    pts_inner = [
        (center, 18*SCALE),
        (s - 20*SCALE, 32*SCALE),
        (s - 30*SCALE, 90*SCALE),
        (center, s - 22*SCALE),
        (30*SCALE, 90*SCALE),
        (20*SCALE, 32*SCALE),
    ]
    d.polygon(pts_inner, fill=(25, 10, 15, 255), outline=(200, 160, 40, 200), width=2*SCALE)
    
    # Flaming 'S' letter
    d.arc([42*SCALE, 28*SCALE, 98*SCALE, 78*SCALE], start=120, end=360, fill=(255, 215, 0, 255), width=10*SCALE)
    d.arc([42*SCALE, 62*SCALE, 98*SCALE, 112*SCALE], start=300, end=180, fill=(255, 80, 20, 255), width=10*SCALE)
    
    save_scaled(img, "Assets/Textures/Summary/Hunt/badge_rank_s.png")


# ==================== CUTE THEME ASSETS ====================

# 1. Cute Slot Card for Financial / Stat Rows (520x70)
def draw_cute_slot():
    w, h = 520 * SCALE, 70 * SCALE
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    r = 18 * SCALE
    
    # Soft lavender-pink pill slot with white inner glow
    d.rounded_rectangle([0, 0, w-1, h-1], radius=r, fill=(245, 235, 250, 240), outline=(255, 182, 193, 255), width=3*SCALE)
    # Inner highlight line at top
    d.line([(r, 4*SCALE), (w-r, 4*SCALE)], fill=(255, 255, 255, 200), width=2*SCALE)
    
    save_scaled(img, "Assets/Textures/Summary/Cute/card_cute_slot.png")

# 2. Golden Coin Icon (128x128)
def draw_coin_icon():
    s = 128 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Outer coin gold rim
    d.ellipse([15*SCALE, 15*SCALE, (128-15)*SCALE, (128-15)*SCALE], fill=(255, 200, 0, 255), outline=(210, 140, 0, 255), width=4*SCALE)
    # Inner face
    d.ellipse([25*SCALE, 25*SCALE, (128-25)*SCALE, (128-25)*SCALE], fill=(255, 230, 80, 255), outline=(230, 170, 20, 255), width=3*SCALE)
    # Star / Crown emblem inside
    d.polygon([
        (64*SCALE, 35*SCALE), (72*SCALE, 52*SCALE), (92*SCALE, 55*SCALE),
        (76*SCALE, 68*SCALE), (82*SCALE, 88*SCALE), (64*SCALE, 76*SCALE),
        (46*SCALE, 88*SCALE), (52*SCALE, 68*SCALE), (36*SCALE, 55*SCALE),
        (56*SCALE, 52*SCALE)
    ], fill=(255, 250, 180, 255), outline=(210, 140, 0, 255), width=2*SCALE)
    
    save_scaled(img, "Assets/Textures/Summary/Cute/icon_coin.png")

# 3. Cute Cloche Dish Plate Icon (128x128)
def draw_dish_icon():
    s = 128 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Silver cloche plate
    d.ellipse([15*SCALE, 80*SCALE, 113*SCALE, 110*SCALE], fill=(220, 230, 245, 255), outline=(140, 160, 190, 255), width=4*SCALE)
    # Dome cover
    d.chord([25*SCALE, 30*SCALE, 103*SCALE, 95*SCALE], start=180, end=0, fill=(240, 245, 255, 255), outline=(150, 170, 200, 255), width=4*SCALE)
    # Knob
    d.ellipse([54*SCALE, 18*SCALE, 74*SCALE, 35*SCALE], fill=(255, 200, 60, 255), outline=(180, 130, 20, 255), width=3*SCALE)
    # Little cute sparkle steam rising
    d.arc([40*SCALE, 10*SCALE, 55*SCALE, 25*SCALE], start=40, end=200, fill=(255, 180, 200, 220), width=3*SCALE)
    d.arc([75*SCALE, 10*SCALE, 90*SCALE, 25*SCALE], start=40, end=200, fill=(255, 180, 200, 220), width=3*SCALE)
    
    save_scaled(img, "Assets/Textures/Summary/Cute/icon_dish.png")

# 4. Cute Happy Customer Icon (128x128)
def draw_customer_icon():
    s = 128 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Cute round happy head (pink chibi style)
    d.ellipse([20*SCALE, 20*SCALE, 108*SCALE, 108*SCALE], fill=(255, 235, 220, 255), outline=(220, 160, 140, 255), width=4*SCALE)
    # Cute hair / ears
    d.chord([15*SCALE, 15*SCALE, 113*SCALE, 60*SCALE], start=180, end=0, fill=(120, 80, 70, 255))
    # Happy closed curve eyes ^ ^
    d.arc([38*SCALE, 50*SCALE, 56*SCALE, 68*SCALE], start=200, end=340, fill=(70, 40, 40, 255), width=4*SCALE)
    d.arc([72*SCALE, 50*SCALE, 90*SCALE, 68*SCALE], start=200, end=340, fill=(70, 40, 40, 255), width=4*SCALE)
    # Pink blush
    d.ellipse([32*SCALE, 66*SCALE, 48*SCALE, 78*SCALE], fill=(255, 140, 160, 180))
    d.ellipse([80*SCALE, 66*SCALE, 96*SCALE, 78*SCALE], fill=(255, 140, 160, 180))
    # Smiling mouth
    d.arc([52*SCALE, 68*SCALE, 76*SCALE, 88*SCALE], start=0, end=180, fill=(200, 50, 70, 255), width=4*SCALE)
    
    save_scaled(img, "Assets/Textures/Summary/Cute/icon_customer.png")

# 5. Golden Star for Rating (64x64)
def draw_star_gold():
    s = 64 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    center = s // 2
    pts = []
    for i in range(10):
        ang = i * (math.pi / 5) - math.pi / 2
        r = (28 * SCALE) if i % 2 == 0 else (12 * SCALE)
        pts.append((center + r * math.cos(ang), center + r * math.sin(ang)))
    
    d.polygon(pts, fill=(255, 215, 0, 255), outline=(210, 140, 0, 255), width=2*SCALE)
    # Inner shine
    shine_pts = []
    for i in range(10):
        ang = i * (math.pi / 5) - math.pi / 2
        r = (22 * SCALE) if i % 2 == 0 else (9 * SCALE)
        shine_pts.append((center + r * math.cos(ang), center + r * math.sin(ang)))
    d.polygon(shine_pts, fill=(255, 245, 130, 255))
    
    save_scaled(img, "Assets/Textures/Summary/Cute/icon_star_gold.png")

# 6. Star Empty (64x64)
def draw_star_empty():
    s = 64 * SCALE
    img = Image.new("RGBA", (s, s), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    center = s // 2
    pts = []
    for i in range(10):
        ang = i * (math.pi / 5) - math.pi / 2
        r = (26 * SCALE) if i % 2 == 0 else (11 * SCALE)
        pts.append((center + r * math.cos(ang), center + r * math.sin(ang)))
    
    d.polygon(pts, fill=(220, 210, 225, 200), outline=(180, 165, 190, 255), width=2*SCALE)
    save_scaled(img, "Assets/Textures/Summary/Cute/icon_star_empty.png")


def main():
    draw_loot_card()
    draw_meat_icon()
    draw_bone_icon()
    draw_venom_icon()
    draw_herb_icon()
    draw_s_rank_badge()
    
    draw_cute_slot()
    draw_coin_icon()
    draw_dish_icon()
    draw_customer_icon()
    draw_star_gold()
    draw_star_empty()
    print("All Summary icons generated successfully!")

if __name__ == "__main__":
    main()

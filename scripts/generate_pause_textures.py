import math
import os
from PIL import Image, ImageDraw, ImageFilter

def make_supersampled(draw_fn, width, height, scale=4):
    sw, sh = width * scale, height * scale
    img = Image.new("RGBA", (sw, sh), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw_fn(draw, sw, sh, scale)
    return img.resize((width, height), Image.Resampling.LANCZOS)

# ----------------- CUTE THEME -----------------
def create_cute_panel(draw, sw, sh, scale):
    r = int(36 * scale)
    # Outer pastel border
    draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-3*scale], radius=r, fill=(255, 240, 246, 230), outline=(255, 182, 193, 255), width=int(4*scale))
    # Inner soft glow card
    ir = int(30 * scale)
    draw.rounded_rectangle([int(8*scale), int(8*scale), sw-int(9*scale), sh-int(9*scale)], radius=ir, fill=(255, 255, 255, 210), outline=(255, 220, 230, 200), width=int(2*scale))

def create_candy_button(color_top, color_bot, color_shadow, border_color):
    def _draw(draw, sw, sh, scale):
        r = int(24 * scale)
        bw = int(3 * scale)
        # Drop shadow / 3D bottom bevel
        draw.rounded_rectangle([2*scale, int(8*scale), sw-3*scale, sh-2*scale], radius=r, fill=color_shadow)
        # Main body
        steps = 60
        for i in range(steps):
            t = i / float(steps)
            curr_y = int(2*scale + (sh - int(14*scale)) * (i / steps))
            h_step = max(1, int((sh - int(14*scale)) / steps) + 1)
            cr = int(color_top[0] * (1 - t) + color_bot[0] * t)
            cg = int(color_top[1] * (1 - t) + color_bot[1] * t)
            cb = int(color_top[2] * (1 - t) + color_bot[2] * t)
            draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-int(10*scale)], radius=r, fill=(cr, cg, cb, 255), outline=border_color, width=bw)
        # Gloss shine
        gloss_h = int((sh - int(14*scale)) * 0.42)
        draw.rounded_rectangle([int(6*scale), int(5*scale), sw-int(7*scale), int(5*scale)+gloss_h], radius=int(r*0.7), fill=(255, 255, 255, 90))
    return _draw

def create_cute_slider_track(draw, sw, sh, scale):
    r = int(sh / 2) - int(2 * scale)
    draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-3*scale], radius=r, fill=(230, 225, 245, 240), outline=(190, 180, 220, 255), width=int(3*scale))

def create_cute_slider_fill(draw, sw, sh, scale):
    r = int(sh / 2) - int(4 * scale)
    draw.rounded_rectangle([int(3*scale), int(4*scale), sw-int(4*scale), sh-int(5*scale)], radius=r, fill=(255, 105, 140, 255), outline=(220, 60, 100, 255), width=int(2*scale))
    draw.rounded_rectangle([int(6*scale), int(6*scale), sw-int(7*scale), int(sh*0.4)], radius=int(r*0.6), fill=(255, 180, 200, 180))

def create_cute_slider_knob(draw, sw, sh, scale):
    r = int(sw / 2) - int(4 * scale)
    cx, cy = sw // 2, sh // 2
    # Shadow
    draw.ellipse([cx-r, cy-r+int(4*scale), cx+r, cy+r+int(4*scale)], fill=(200, 120, 150, 180))
    # Outer circle
    draw.ellipse([cx-r, cy-r, cx+r, cy+r], fill=(255, 255, 255, 255), outline=(255, 130, 165, 255), width=int(4*scale))
    # Inner candy heart/dot
    ir = int(r * 0.65)
    draw.ellipse([cx-ir, cy-ir, cx+ir, cy+ir], fill=(255, 100, 145, 255))
    # Specular shine
    sr = int(r * 0.28)
    draw.ellipse([cx-int(r*0.4)-sr, cy-int(r*0.4)-sr, cx-int(r*0.4)+sr, cy-int(r*0.4)+sr], fill=(255, 255, 255, 230))

def create_cute_toggle_bg(draw, sw, sh, scale):
    r = int(sh / 2) - int(2 * scale)
    draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-3*scale], radius=r, fill=(240, 235, 248, 255), outline=(210, 195, 235, 255), width=int(3*scale))

def create_cute_toggle_check(draw, sw, sh, scale):
    cx, cy = sw // 2, sh // 2
    r = int(sw * 0.38)
    draw.ellipse([cx-r, cy-r, cx+r, cy+r], fill=(80, 205, 130, 255), outline=(40, 160, 85, 255), width=int(3*scale))
    # Cute checkmark
    draw.line([cx-int(r*0.45), cy, cx-int(r*0.1), cy+int(r*0.4)], fill=(255, 255, 255, 255), width=int(4*scale))
    draw.line([cx-int(r*0.1), cy+int(r*0.4), cx+int(r*0.5), cy-int(r*0.4)], fill=(255, 255, 255, 255), width=int(4*scale))

# ----------------- HUNT THEME -----------------
def create_hunt_panel(draw, sw, sh, scale):
    r = int(18 * scale)
    # Heavy stone slab base
    draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-3*scale], radius=r, fill=(20, 20, 24, 240), outline=(85, 25, 30, 255), width=int(5*scale))
    # Inner iron border with riveted corners
    ir = int(12 * scale)
    draw.rounded_rectangle([int(10*scale), int(10*scale), sw-int(11*scale), sh-int(11*scale)], radius=ir, fill=(28, 26, 32, 230), outline=(130, 45, 55, 200), width=int(2*scale))
    # Corner rivets / studs
    stud_r = int(5 * scale)
    stud_coords = [
        (int(20*scale), int(20*scale)), (sw-int(20*scale), int(20*scale)),
        (int(20*scale), sh-int(20*scale)), (sw-int(20*scale), sh-int(20*scale))
    ]
    for x, y in stud_coords:
        draw.ellipse([x-stud_r, y-stud_r, x+stud_r, y+stud_r], fill=(180, 150, 90, 255), outline=(70, 50, 20, 255), width=int(2*scale))
        draw.ellipse([x-int(stud_r*0.5), y-int(stud_r*0.5), x, y], fill=(255, 230, 170, 200))

def create_hunt_button(c_top, c_bot, c_bevel, c_border, c_glow):
    def _draw(draw, sw, sh, scale):
        r = int(12 * scale)
        bw = int(4 * scale)
        # Heavy 3D iron shadow
        draw.rounded_rectangle([2*scale, int(8*scale), sw-3*scale, sh-2*scale], radius=r, fill=c_bevel)
        # Button face
        draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-int(10*scale)], radius=r, fill=c_bot, outline=c_border, width=bw)
        # Inner metallic highlight
        draw.rounded_rectangle([int(5*scale), int(4*scale), sw-int(6*scale), int(sh*0.38)], radius=int(r*0.7), fill=c_top)
        # Edge rivets
        stud_r = int(3.5 * scale)
        for (rx, ry) in [(int(12*scale), int(14*scale)), (sw-int(12*scale), int(14*scale)), (int(12*scale), sh-int(20*scale)), (sw-int(12*scale), sh-int(20*scale))]:
            draw.ellipse([rx-stud_r, ry-stud_r, rx+stud_r, ry+stud_r], fill=(160, 130, 80, 255), outline=(50, 30, 10, 255), width=int(1.5*scale))
    return _draw

def create_hunt_slider_track(draw, sw, sh, scale):
    r = int(8 * scale)
    draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-3*scale], radius=r, fill=(15, 14, 18, 255), outline=(65, 30, 35, 255), width=int(3*scale))

def create_hunt_slider_fill(draw, sw, sh, scale):
    r = int(6 * scale)
    # Molten blood-crimson gradient
    draw.rounded_rectangle([int(3*scale), int(3*scale), sw-int(4*scale), sh-int(4*scale)], radius=r, fill=(185, 25, 35, 255), outline=(255, 75, 80, 255), width=int(2*scale))
    draw.rounded_rectangle([int(5*scale), int(5*scale), sw-int(6*scale), int(sh*0.4)], radius=int(r*0.6), fill=(255, 120, 90, 190))

def create_hunt_slider_knob(draw, sw, sh, scale):
    cx, cy = sw // 2, sh // 2
    r = int(sw / 2) - int(6 * scale)
    # Outer dark iron spiked ring
    draw.ellipse([cx-r, cy-r, cx+r, cy+r], fill=(30, 28, 35, 255), outline=(160, 120, 60, 255), width=int(4*scale))
    # Crimson dragon eye / ruby core
    ir = int(r * 0.65)
    draw.ellipse([cx-ir, cy-ir, cx+ir, cy+ir], fill=(195, 20, 30, 255), outline=(255, 90, 100, 255), width=int(2.5*scale))
    # Slit pupil / highlight
    draw.ellipse([cx-int(ir*0.3), cy-int(ir*0.8), cx+int(ir*0.3), cy+int(ir*0.8)], fill=(255, 200, 50, 255))
    draw.ellipse([cx-int(ir*0.1), cy-int(ir*0.7), cx+int(ir*0.1), cy+int(ir*0.7)], fill=(20, 5, 5, 255))

def create_hunt_toggle_bg(draw, sw, sh, scale):
    r = int(8 * scale)
    draw.rounded_rectangle([2*scale, 2*scale, sw-3*scale, sh-3*scale], radius=r, fill=(18, 16, 20, 255), outline=(75, 35, 40, 255), width=int(3*scale))

def create_hunt_toggle_check(draw, sw, sh, scale):
    cx, cy = sw // 2, sh // 2
    r = int(sw * 0.38)
    draw.rounded_rectangle([cx-r, cy-r, cx+r, cy+r], radius=int(4*scale), fill=(180, 25, 35, 255), outline=(255, 80, 80, 255), width=int(3*scale))
    # Cross/Rune marks
    draw.line([cx-int(r*0.5), cy-int(r*0.5), cx+int(r*0.5), cy+int(r*0.5)], fill=(255, 220, 120, 255), width=int(3.5*scale))
    draw.line([cx-int(r*0.5), cy+int(r*0.5), cx+int(r*0.5), cy-int(r*0.5)], fill=(255, 220, 120, 255), width=int(3.5*scale))

def create_hunt_ember(draw, sw, sh, scale):
    cx, cy = sw // 2, sh // 2
    r = int(sw * 0.4)
    for i in range(r, 0, -1):
        t = i / r
        alpha = int((1 - t**1.8) * 255)
        cr = 255
        cg = int(120 * (1 - t) + 240 * t)
        cb = int(20 * (1 - t) + 180 * t)
        draw.ellipse([cx-i, cy-i, cx+i, cy+i], fill=(cr, cg, cb, alpha))

def main():
    cute_dir = "Assets/Textures/PauseMenu/Cute"
    hunt_dir = "Assets/Textures/PauseMenu/Hunt"
    
    # Generate Cute Assets
    make_supersampled(create_cute_panel, 512, 512).save(f"{cute_dir}/panel_cute_frame.png")
    make_supersampled(create_candy_button((255, 120, 160), (245, 75, 125), (190, 45, 90, 255), (255, 170, 195, 255)), 256, 128).save(f"{cute_dir}/btn_cute_pink.png")
    make_supersampled(create_candy_button((105, 225, 140), (60, 185, 100), (35, 130, 65, 255), (170, 245, 190, 255)), 256, 128).save(f"{cute_dir}/btn_cute_green.png")
    make_supersampled(create_candy_button((255, 220, 80), (245, 175, 40), (190, 125, 20, 255), (255, 240, 150, 255)), 256, 128).save(f"{cute_dir}/btn_cute_yellow.png")
    make_supersampled(create_candy_button((110, 195, 255), (65, 150, 235), (35, 105, 180, 255), (180, 230, 255, 255)), 256, 128).save(f"{cute_dir}/btn_cute_blue.png")
    make_supersampled(create_cute_slider_track, 256, 64).save(f"{cute_dir}/slider_cute_track.png")
    make_supersampled(create_cute_slider_fill, 256, 64).save(f"{cute_dir}/slider_cute_fill.png")
    make_supersampled(create_cute_slider_knob, 128, 128).save(f"{cute_dir}/slider_cute_knob.png")
    make_supersampled(create_cute_toggle_bg, 128, 64).save(f"{cute_dir}/toggle_cute_bg.png")
    make_supersampled(create_cute_toggle_check, 64, 64).save(f"{cute_dir}/toggle_cute_check.png")
    
    # Generate Hunt Assets
    make_supersampled(create_hunt_panel, 512, 512).save(f"{hunt_dir}/panel_hunt_frame.png")
    make_supersampled(create_hunt_button((180, 30, 45, 160), (110, 15, 25, 255), (55, 8, 12, 255), (195, 45, 55, 255), (255, 60, 60)), 256, 128).save(f"{hunt_dir}/btn_hunt_crimson.png")
    make_supersampled(create_hunt_button((200, 155, 80, 160), (140, 100, 45, 255), (75, 50, 20, 255), (225, 180, 95, 255), (255, 210, 110)), 256, 128).save(f"{hunt_dir}/btn_hunt_gold.png")
    make_supersampled(create_hunt_button((90, 85, 95, 160), (45, 42, 50, 255), (22, 20, 25, 255), (130, 125, 140, 255), (180, 175, 195)), 256, 128).save(f"{hunt_dir}/btn_hunt_iron.png")
    make_supersampled(create_hunt_slider_track, 256, 64).save(f"{hunt_dir}/slider_hunt_track.png")
    make_supersampled(create_hunt_slider_fill, 256, 64).save(f"{hunt_dir}/slider_hunt_fill.png")
    make_supersampled(create_hunt_slider_knob, 128, 128).save(f"{hunt_dir}/slider_hunt_knob.png")
    make_supersampled(create_hunt_toggle_bg, 128, 64).save(f"{hunt_dir}/toggle_hunt_bg.png")
    make_supersampled(create_hunt_toggle_check, 64, 64).save(f"{hunt_dir}/toggle_hunt_check.png")
    make_supersampled(create_hunt_ember, 128, 128).save(f"{hunt_dir}/hunt_ember_particle.png")
    
    print("All UI textures successfully generated!")

if __name__ == "__main__":
    main()

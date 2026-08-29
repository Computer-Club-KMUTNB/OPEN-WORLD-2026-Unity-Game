import math
from PIL import Image, ImageDraw, ImageFilter

def create_cute_bg():
    w, h = 1920, 1080
    img = Image.new("RGB", (w, h), (255, 245, 250))
    draw = ImageDraw.Draw(img)
    # Warm pastel gradient
    for y in range(h):
        t = y / float(h)
        r = int(255 * (1 - t) + 245 * t)
        g = int(225 * (1 - t) + 235 * t)
        b = int(240 * (1 - t) + 250 * t)
        draw.line([(0, y), (w, y)], fill=(r, g, b))
    
    # Soft pastel circles / bokeh
    bokeh_colors = [
        (255, 210, 225), (220, 245, 255), (255, 245, 210), (225, 255, 230)
    ]
    for i, (bx, by, br) in enumerate([
        (200, 300, 250), (1700, 200, 300), (960, 900, 450),
        (500, 800, 280), (1400, 750, 320), (300, 100, 180), (1600, 900, 260)
    ]):
        c = bokeh_colors[i % len(bokeh_colors)]
        draw.ellipse([bx-br, by-br, bx+br, by+br], fill=c)
    
    img = img.filter(ImageFilter.GaussianBlur(radius=40))
    img.save("Assets/Textures/PauseMenu/Cute/cute_bg_cozy.jpg", quality=92)

def create_hunt_bg():
    w, h = 1920, 1080
    img = Image.new("RGB", (w, h), (15, 10, 15))
    draw = ImageDraw.Draw(img)
    
    # Dark moody crimson/obsidian gradient
    for y in range(h):
        t = y / float(h)
        r = int(45 * (1 - t) + 12 * t)
        g = int(10 * (1 - t) + 8 * t)
        b = int(18 * (1 - t) + 14 * t)
        draw.line([(0, y), (w, y)], fill=(r, g, b))
    
    # Blood Moon in upper sky
    mx, my, mr = 960, 320, 160
    # Outer blood moon glow
    for i in range(250, mr, -4):
        alpha = int((1 - (i - mr) / (250 - mr)) * 40)
        draw.ellipse([mx-i, my-i, mx+i, my+i], fill=(160, 20, 30))
    # Moon body
    draw.ellipse([mx-mr, my-mr, mx+mr, my+mr], fill=(220, 40, 50))
    draw.ellipse([mx-mr+20, my-mr+10, mx+mr-20, my+mr-30], fill=(255, 80, 90))
    
    # Dark mist / jagged silhouette
    img = img.filter(ImageFilter.GaussianBlur(radius=25))
    draw = ImageDraw.Draw(img)
    
    # Mountain / Forest silhouettes
    draw.polygon([(0, 650), (300, 520), (600, 680), (900, 480), (1250, 640), (1600, 510), (1920, 670), (1920, 1080), (0, 1080)], fill=(10, 8, 12))
    draw.polygon([(0, 780), (400, 660), (800, 820), (1200, 690), (1650, 770), (1920, 720), (1920, 1080), (0, 1080)], fill=(5, 4, 6))

    img.save("Assets/Textures/PauseMenu/Hunt/hunt_bg_wilderness.jpg", quality=92)

if __name__ == "__main__":
    create_cute_bg()
    create_hunt_bg()
    print("Backgrounds generated!")

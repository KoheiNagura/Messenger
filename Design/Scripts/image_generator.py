from PIL import Image, ImageDraw, ImageFont
import glob
import os

font_files = glob.glob("../Fonts/**/*.ttf")
for file in font_files:
    img = Image.new("1", (512, 512), 0)
    draw = ImageDraw.Draw(img)
    font = ImageFont.truetype(file, 512)
    draw.text((0, -179), "つ", fill=1, font=font)
    filename = os.path.splitext(os.path.basename(file))[0]
    img.save(f'../Images/{filename}.png')
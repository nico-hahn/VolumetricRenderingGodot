import tifffile as tiff
import numpy as np
from PIL import Image
import argparse
import os
import sys

def extract(input_path, output_dir):
	os.makedirs(output_dir, exist_ok=True)
	tif = tiff.imread(input_path)  # Shape: (Z, H, W)
	print(f"Loaded TIFF: {input_path}, Shape: {tif.shape}")
	
	for i, slice in enumerate(tif):
		img = Image.fromarray(slice)
		output_path = os.path.join(output_dir, f"slice_{i:03}.png")
		img.save(output_path)
		print(f"Saved: {output_path}")

if __name__ == "__main__":
	input_path = sys.argv[1]
	out_dir = sys.argv[2] if len(sys.argv) > 2 else "out"
	extract(input_path, out_dir)

# LLM Generated code!
import sys
import numpy as np
from PIL import Image

def convert_tiff_to_npy(tiff_path, npy_path=None):
    img = Image.open(tiff_path)
    arr = np.array(img)

    if npy_path is None:
        npy_path = tiff_path.rsplit('.', 1)[0] + '.npy'

    np.save(npy_path, arr)
    print(f"Saved: {npy_path}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python tiff2npy.py <input.tiff> [output.npy]")
        sys.exit(1)

    tiff_path = sys.argv[1]
    npy_path = sys.argv[2] if len(sys.argv) > 2 else None
    convert_tiff_to_npy(tiff_path, npy_path)
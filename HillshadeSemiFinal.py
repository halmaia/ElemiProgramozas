# -*- coding: utf-8 -*-

import math

class ASCIIRaster:
    def __init__(self, path, ncols,  nrows, xllcorner, yllcorner, cellsize, nodata, data):
        self.path = path
        self.ncols = ncols
        self.nrows = nrows
        self.xllcorner = xllcorner
        self.yllcorner = yllcorner
        self.cellsize = cellsize
        self.nodata = nodata
        self.data = data
        
        
def GetASCIIRaster(path):        
    file = open(path)
    
    ncols = int(file.readline().strip().split(sep=' ')[-1])
    nrows = int(file.readline().strip().split(sep=' ')[-1])
    xllcorner = float(file.readline().strip().split(sep=' ')[-1])
    yllcorner = float(file.readline().strip().split(sep=' ')[-1])
    cellsize = float(file.readline().strip().split(sep=' ')[-1])
    nodata = float(file.readline().strip().split(sep=' ')[-1])
    
    data = []
    
    for row in file:
        data.extend([float(x) for x in row.strip().split(sep=' ')])
    
    file.close()
    del file
    
    return ASCIIRaster(path, ncols, nrows, xllcorner, yllcorner, cellsize, nodata, data)

    
    
def CalculateHillshade(raster: ASCIIRaster):
    ncols = raster.ncols
    nrows = raster.nrows
    data = raster.data
    
    Sun_azimuth = 345.0 # Ebből az irányból süt a Nap.
    Sun_altitude = 45.0 # Ilyen magasam áll a nap a horizont felett.
    Zenith_rad = math.radians(90.0 - Sun_altitude) # Zenittávolság radiánban.
    Azimuth_rad = math.radians(360.0 - Sun_azimuth + 90.0) # Irány, pozitív körüljárás szerint.
    cellsize = raster.cellsize
    z_factor = 1.0
    
    finalRaster = []
    
    for y in range(ncols, ncols*(nrows - 1) , ncols):
        rowList = []
        for x in range(1, ncols-1, 1):
            
            a = data[x + y - ncols - 1]
            b = data[x + y - ncols + 0]
            c = data[x + y - ncols + 1]
            
            d = data[x + y - 1]
            e = data[x + y + 0]
            f = data[x + y + 1]
    
            g = data[x + y + ncols - 1]
            h = data[x + y + ncols + 0]
            i = data[x + y + ncols + 1]
    
            dz_dx = ((c + 2.0 * f + i) - (a + 2.0 * d + g)) / (8.0 * raster.cellsize)
            dz_dy = ((g + 2.0 * h + i) - (a + 2.0 * b + c)) / (8.0 * raster.cellsize)
            Slope_rad = math.atan(z_factor * ((dz_dx * dz_dx + dz_dy * dz_dy) ** .5))

            if dz_dx != 0.0:
                Aspect_rad = math.atan2(dz_dy, -dz_dx)
                if Aspect_rad < 0.0:
                    Aspect_rad = 2.0 * math.pi + Aspect_rad
            else:
                if dz_dy > 0.0:
                    Aspect_rad = math.pi / 2.0
                elif dz_dy < 0.0:
                    Aspect_rad = 2.0 * math.pi - math.pi / 2.0
                else:
                    Aspect_rad = 0.0
                    
            
            Hillshade = int(255.0 * ((math.cos(Zenith_rad) * math.cos(Slope_rad)) + (math.sin(Zenith_rad) * math.sin(Slope_rad) *  math.cos(Azimuth_rad - Aspect_rad))))
            rowList.append(Hillshade)
        finalRaster.append(rowList)
        
    return finalRaster
        
        
def write_grayscale(filename, pixels):
    # Original source: https://github.com/kentoj/python-fundamentals/blob/master/bmp.py
    """Creates and writes a grayscale BMP file
    Args:
        filename: The name of the BMP file to be crated.
        pixels: A rectangular image stored as a sequence of rows.
            Each row must be an iterable series of integers in the range 0-255.
    Raises:
        OSError: If the file couldn't be written.
    """
    height = len(pixels)
    width = len(pixels[0])

    with open(filename, 'wb') as bmp:
        # BMP Header
        bmp.write(b'BM')

        size_bookmark = bmp.tell()  # The next four bytes hold the filesize as a 32-bit
        bmp.write(b'\x00\x00\x00\x00')  # little-endian integer.  Zero placeholder for now.

        bmp.write(b'\x00\x00')  # Unused 16-bit integer - should be zero
        bmp.write(b'\x00\x00')  # Unused 16-bit integer - should be zero

        pixel_offset_bookmark = bmp.tell()  # The next four bytes hold the integer offset
        bmp.write(b'\x00\x00\x00\x00')  # to the pixel data.  Zero placeholder for now.

        # Image header
        bmp.write(b'\x28\x00\x00\x00')  # Image header size in bytes - 40 decimal
        bmp.write(_int32_to_bytes(width))  # Image width in pixels
        bmp.write(_int32_to_bytes(height))  # Image height in pixels
        bmp.write(b'\x01\x00')  # Number of image planes
        bmp.write(b'\x08\x00')  # Bits per pixel 8 for grayscale
        bmp.write(b'\x00\x00\x00\x00')  # No compression
        bmp.write(b'\x00\x00\x00\x00')  # Zero for uncompressed images
        bmp.write(b'\x00\x00\x00\x00')  # Unused pixels per meter
        bmp.write(b'\x00\x00\x00\x00')  # Unused pixels per meter
        bmp.write(b'\x00\x00\x00\x00')  # Use whole color table
        bmp.write(b'\x00\x00\x00\x00')  # All colors are important

        # Color palette - a linear grayscale
        for c in range(256):
            bmp.write(bytes((c, c, c, 0)))

        # Pixel data
        pixel_data_bookmark = bmp.tell()
        for row in reversed(pixels):  # BMP files are bottom to top
            row_data = bytes(row)
            bmp.write(row_data)
            padding = b'\x00' * ((4 - (len(row) % 4)) % 4)  # Pad row to multiple of four bytes
            bmp.write(padding)

        # End of file
        eof_bookmark = bmp.tell()

        # Fill in file size placeholder
        bmp.seek(size_bookmark)
        bmp.write(_int32_to_bytes(eof_bookmark))

        # Fill in pixel
        bmp.seek(pixel_offset_bookmark)
        bmp.write(_int32_to_bytes(pixel_data_bookmark))


def _int32_to_bytes(i):
    """Convert an integer to four bytes in little-endian format."""
    return bytes((i & 0xff,
                  i >> 8 & 0xff,
                  i >> 16 & 0xff,
                  i >> 24 & 0xff))

raster = GetASCIIRaster(r"C:\Users\Win10\Desktop\hill3.asc")
hillshade = CalculateHillshade(raster)
write_grayscale(r"C:\Users\Win10\Desktop\h180.bmp", hillshade)


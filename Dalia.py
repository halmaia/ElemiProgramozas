# -*- coding: utf-8 -*-
"""
Created on Thu Feb 27 17:04:31 2020

@author: Win10
"""

import matplotlib.pyplot as plt

class rectange:
    def __init__(self, xMin: int, xMax: int, yMin: int, yMax: int):
        self.xMin, self.xMax, self.yMin, self.yMax = xMin, xMax, yMin, yMax
    
    def __repr__(self):
        return str(self.xMin) + "; " + str(self.xMax) + "; " + str(self.yMin) + "; " + str(self.yMax)

def openImageForView(path: str):
    return plt.imread(path)

def openImageForEdit(path: str):
    return openImageForView(path).copy()
    
def showImage(im):
    plt.imshow(im)
    plt.show()

def maskImage(im, rect: rectange, color: list):
    for sor in range(rect.yMin, rect.yMax):
        for oszlop in range(rect.xMin, rect.xMax):
            im[sor][oszlop] = color
    return im
    
def clipImage(im, rect: rectange):
    shape = (rect.yMax - rect.yMin, rect.xMax -rect.xMin, 3)
    na = plt.np.ndarray(shape, im.dtype)
    
    nasor = 0
    for sor in range(rect.yMin, rect.yMax):
        naoszlop = 0
        for oszlop in range(rect.xMin, rect.xMax):
            na[nasor][naoszlop] = im[sor][oszlop]
            naoszlop += 1
        nasor += 1
    return na

def maskImageFast(im, rect: rectange, color: list):
    for sor in range(rect.yMin, rect.yMax):
        im[sor][rect.xMin : rect.xMax] = color
    return im
    
path = r"C:\Users\Win10\Desktop\Dalia.jpg"
rect = rectange(480, 590, 200, 240)
im = openImageForEdit(path)
im = clipImage(im, rect)
showImage(im)

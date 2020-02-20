
import csv
import math
import matplotlib.pyplot as plt

def readCSV(path: str):
    with open(path) as textFile:
        csvFile = csv.reader(textFile)
        lon, lat = [], []
        for row in csvFile:
            lon.append(float(row[0]))
            lat.append(float(row[1]))
    return lon, lat

def centralisSikvetulet(lon: list, lat: list):
    R = 6371.0 * 1000.0
    n = len(lon)
    x, y = [0.0] * n, [0.0] * n
    for i in range(n):
        x[i] = R * math.tan(math.radians(90.0 - lat[i])) * math.cos(math.radians(lon[i] - 90.0))
        y[i] = R * math.tan(math.radians(90.0 - lat[i])) * math.sin(math.radians(lon[i] - 90.0))
    return x,y

path = r"D:\Egyetem\2019-20.2\Python alapok\2. hét\Europe.csv"
lon, lat = readCSV(path)
x, y = centralisSikvetulet(lon, lat)
plt.fill(x,y)
plt.show()
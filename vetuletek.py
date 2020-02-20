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
        rtg = R * math.tan(math.radians(90.0 - lat[i]))
        lonr = math.radians(lon[i] - 90.0)
        x[i] = rtg * math.cos(lonr)
        y[i] = rtg * math.sin(lonr)
    return x, y
    
def mercatorHenger(lon: list, lat: list):
    R = 6371.0 * 1000.0
    y = [R * math.radians(_lat) for _lat in lat]
    x = [R * math.log(1.0 / math.tan(math.radians((90.0 - _lon) / 2.0))) for _lon in lon]
    return x, y  
    
def negyzetesHenger(lon: list, lat: list):
    R = 6371.0 * 1000.0
    y = [R * math.radians(_lat) for _lat in lat]
    x = [R * math.radians(_lon) for _lon in lon]
    return x, y  
         
    
def getArea(x: list, y: list):
    n = len(x)
    a = 0.0
    for i in range(n - 1):
        a += (y[i] + y[i + 1]) * 0.5 * (x[i + 1] - x[i])
    return a

def kontinensekRajzolasa(paths: list, proj):
    for path in paths:
        lon, lat = readCSV(path)
        x, y = proj(lon, lat)
        plt.fill(x, y)

    plt.gca().set_aspect('equal', adjustable='box')
    plt.show()
 
    
paths = [r"C:\Users\Win10\Desktop\Asia.csv",
          r"C:\Users\Win10\Desktop\Europe.csv",
          r"C:\Users\Win10\Desktop\Africa.csv",
          r"C:\Users\Win10\Desktop\Antarctica.csv"]
         
proj = lambda lon, lat: negyzetesHenger(lon, lat)
          
kontinensekRajzolasa(paths, proj)
    

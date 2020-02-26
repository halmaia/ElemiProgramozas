import csv
import math
import matplotlib.pyplot as plt


def polarisKoordinatakOlvasasa(path: str):
    with open(path) as textFile:
        lon, lat = [], []
        csvFile = csv.reader(textFile)
        for row in csvFile:
            lon.append(float(row[0]))
            lat.append(float(row[1]))
        return lon, lat


def kontinensekRajzolasa(paths: list, proj):
    for path in paths:
        lon, lat = polarisKoordinatakOlvasasa(path)
        x, y = proj(lon, lat)
        plt.fill(x, y)

    plt.gca().set_aspect('equal', adjustable='box')
    plt.show()


def centralisSikvetulet(lon, lat):
    R = 6371.0 * 1000.0
    n = len(lon)
    x, y = [0.0] * n, [0.0] * n
    for i in range(n):
        _tg = R * math.tan(math.radians(90.0 - lat[i]))
        _lon = math.radians(lon[i] - 90.0)
        x[i] = _tg * math.cos(_lon)
        y[i] = _tg * math.sin(_lon)
    return x, y


def ortografikusSikvetulet(lon, lat):
    R = 6371.0 * 1000.0
    n = len(lon)
    x, y = [0.0] * n, [0.0] * n
    for i in range(n):
        _sin = R * math.sin(math.radians(90.0 - lat[i]))
        _lon = math.radians(lon[i] - 90.0)
        x[i] = _sin * math.cos(_lon)
        y[i] = _sin * math.sin(_lon)
    return x, y


def sztereografikusSikvetulet(lon, lat):
    R = 6371.0 * 1000.0
    n = len(lon)
    x, y = [0.0] * n, [0.0] * n
    for i in range(n):
        _tg2 = 2.0 * R * math.tan(math.radians(0.5 * (90.0 - lat[i])))
        _lon = math.radians(lon[i] - 90.0)
        x[i] = _tg2 * math.cos(_lon)
        y[i] = _tg2 * math.sin(_lon)
    return x, y


def postelSikvetulet(lon, lat):
    R = 6371.0 * 1000.0
    n = len(lon)
    x, y = [0.0] * n, [0.0] * n
    for i in range(n):
        _lon = math.radians(lon[i] - 90.0)
        _lat = R * math.radians(90.0 - lat[i])
        x[i] = _lat * math.cos(_lon)
        y[i] = _lat * math.sin(_lon)
    return x, y


def lambertSikvetulet(lon, lat):
    R = 6371.0 * 1000.0
    n = len(lon)
    x, y = [0.0] * n, [0.0] * n
    for i in range(n):
        _sin2 = 2.0 * R * math.sin(math.radians(0.5 * (90.0 - lat[i])))
        _lon = math.radians(lon[i] - 90.0)
        x[i] = _sin2 * math.cos(_lon)
        y[i] = _sin2 * math.sin(_lon)
    return x, y


def negyzetesHengervetulet(lon: list, lat: list):
    R = 6371.0 * 1000.0
    x = [R * math.radians(_lon) for _lon in lon]
    y = [R * math.radians(_lat) for _lat in lat]
    return x, y


def mercatorHengervetulet(lon: list, lat: list):
    R = 6371.0 * 1000.0
    x = [R * math.radians(_lon) for _lon in lon]
    y = [R * math.log(math.tan(math.radians(45.0 + 0.5 * _lat)))
         if _lat > -89.0 and _lat < 89.0
         else
         R * math.log(math.tan(math.radians(45.0 + 0.5 * math.copysign(89.0, _lat))))
         for _lat in lat]
    return x, y


def lambertHengervetulet(lon: list, lat: list):
    R = 6371.0 * 1000.0
    x = [R * math.radians(_lon) for _lon in lon]
    y = [R * math.cos(math.radians(90.0 - _lat)) for _lat in lat]
    return x, y


def getArea(x: list, y: list):
    n = len(x)
    a = 0.0
    for i in range(n - 1):
        a += (y[i] + y[i + 1]) * 0.5 * (x[i + 1] - x[i])
    return a

paths = (r"F:\Coords\Asia.csv",
         r"F:\Coords\Europe.csv",
         r"F:\Coords\Africa.csv",
         r"F:\Coords\Antarctica.csv",
         r"F:\Coords\NorthAmerica.csv",
         r"F:\Coords\SouthAmerica.csv",
         r"F:\Coords\Australia.csv",
         r"F:\Coords\Hungary.csv")

def proj(lon, lat): return lambertHengervetulet(lon, lat)

#kontinensekRajzolasa([r"F:\Coords\Europe.csv"], proj)
kontinensekRajzolasa(paths, proj)

import csv
import matplotlib.pyplot as plt

class Point:
    def __init__(self, x: float, y:float):
        self.x, self.y = x, y

    def __init__(self, pair: list):
        self.x, self.y = float(pair[0]), float(pair[1])

    def getDistance(self, other):
        dx, dy = other.x-self.x,other.y-self.y
        return (dx*dx+dy*dy)**.5

    def __repr__(self):
        return '{} {}'.format(self.x, self.y)

def readPoints(path: str):
    with open(path) as file:
        reader = csv.reader(file)
        points = []
        for row in reader:
            points.append(Point(float(row[0]),float(row[1])))
        return points

def readPointsShorter1(path: str):
    with open(path) as file:
        return [Point(float(row[0]),float(row[1])) for row in csv.reader(file)]

def readPointsShorter2(path: str):
    with open(path) as file:
        return [Point(row) for row in csv.reader(file)]

def getMostCentral(points: list):
    distlist = []
    for outer in points:
        dist = 0.0
        for inner in points:
            dist+=inner.getDistance(outer)
        distlist.append(dist)

        minimum = min(distlist)
        index = distlist.index(minimum)
    return points[index]


def getMostCentralFaster(points: list):
    distlist = []
    for outer in points:
        dist = 0.0
        for inner in points:
            dist+=inner.getDistance(outer)
        distlist.append(dist)

        minimum = min(distlist)
        index = distlist.index(minimum)
    return points[index]


def getMostCentralShorter1(points: list):
    distlist = []
    for outer in points:
        distlist.append(sum([inner.getDistance(outer) for inner in points]))
    return points[distlist.index(min(distlist))]

def getMostCentralShorter2(points: list):
    distlist = [sum([inner.getDistance(outer) for inner in points]) for outer in points]
    return points[distlist.index(min(distlist))]


def drawPoints(points: list):
    for point in points:
        plt.scatter(point.x, point.y, c='gray') # marker='x'
    
    plt.gca().set_aspect('equal', adjustable='box')
    plt.show()

points = readPointsShorter2(r'F:\Fire.txt')
print(getMostCentralShorter1(points))

drawPoints(points)

import csv
import matplotlib.pyplot as plt


class Point:
    def __init__(self, pair: list):
        self.x, self.y = float(pair[0]), float(pair[1])


    def __repr__(self):
        return f'{self.x} {self.y}'


    def getDistance(self, other):
        dx, dy = other.x - self.x, other.y - self.y
        return (dx * dx + dy * dy) ** .5
        

def readPointsShorter(path: str):
    with open(path) as file:
        return [Point(row) for row in csv.reader(file)]


def getMostCentralShorter(points: list):
    distlist = []
    for outer in points:
        distlist.append(sum([inner.getDistance(outer) for inner in points]))

    return points[distlist.index(min(distlist))]


def drawPoints(points: list, central):
    for point in points:
        plt.scatter(point.x, point.y, c='gray')

    plt.scatter(central.x, central.y, c='red', marker='x')
    plt.gca().set_aspect('equal', adjustable='box')
    plt.show()


points = readPointsShorter(r'F:\Fire.txt')
drawPoints(points, getMostCentralShorter(points))

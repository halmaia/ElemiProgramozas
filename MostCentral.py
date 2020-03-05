import csv
import matplotlib.pyplot as plt


def koordinatakOlvasasa(path: str):
    with open(path) as textFile:
        x, y = [], []
        csvFile = csv.reader(textFile)
        for row in csvFile:
            x.append(float(row[0]))
            y.append(float(row[1]))
        return x, y
        
def koordinataKirajzolasa(x: list, y: list):
    n = len(x)
    for i in range(n):
        plt.scatter(x[i], y[i])
    plt.show()
    
x,y=koordinatakOlvasasa(r"C:\Users\Win10\Desktop\Fire.txt")
koordinataKirajzolasa(x, y)

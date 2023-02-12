#! C:\Python310\python.exe
import math
import matplotlib.pyplot as plt

def checkValidity(v: float, n:str):
    """TODO: write the final form"""
    if v <= 0:
        print(f"The variable {n} should be bigger than zero!")
        return False
              
def getRightTriangleParams(a: float, b: float) -> None:
    """TODO: Checks are not ready"""
    print(f"a: {a} unit")
    print(f"b: {b} unit")
    if a == b:
        print("Invalid input!")
        return
    if a <= 0:
        print("Invalid 'a' value!")
        return
    if b <= 0:
        print("invalid 'b' value!")
        return
    if math.isnan(a):
        print("The 'a' is 'nan'!")
        return
    if math.isnan(b):
        print("The 'b' is 'nan'!")
        return
    c = math.hypot(a,b)
    print(f"c: {c} unit")
    P = a+b+c
    print(f"P: {P}")
    A = a*b*.5
    α = math.degrees(math.atan2(a, b))
    print(f"α: {α}")
    β = 180 - (90 + α)
    print(f"β: {β}")
    γ = 90
    print(f"γ⦝: {γ}")
    x = (2/3)*b
    y = a / 3
    print(f"x: {x}")
    print(f"y: {y}")
    p = 2*math.tau*A/(P*P)
    print(f"Polsby–Popper’s score: {p}")
    plt.axis('equal')
    plt.plot([0,b,b,0],[0,0,a,0])
    plt.scatter(x,y,marker='x',color='red')
    plt.show()
    

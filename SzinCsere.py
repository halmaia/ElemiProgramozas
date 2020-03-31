import matplotlib.pyplot as plt

W, K, T = [255, 255, 255], [0, 0, 0], (144, 112, 65)
im = plt.imread(r"C:\OneDrive\Oktatás\Sky.jpg").copy()
ncol, nrow, band = im.shape
for y in range(nrow):
    row = im[y]
    for x in range(ncol):
        im[y][x] = W if tuple(row[x]) == T else K

plt.imshow(im)
plt.show()

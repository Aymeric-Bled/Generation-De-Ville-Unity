from PIL import Image
import matplotlib.pyplot as plt
import numpy as np
from math import sqrt, pi, cos, sin
# load the image
image = Image.open('carte bordeaux.jpg')
# convert image to numpy array
data = np.asarray(image)
print(type(data))
# summarize shape
print(data.shape)

# create Pillow image
image2 = Image.fromarray(data)
print(type(image2))

# summarize image details
print(image2.mode)
print(image2.size)

print(data[:,:,0])
print(data[:,:,1])
print(data[:,:,2])

sets = []

for i in range(data.shape[0]):
    for j in range(data.shape[1]):
        #if data[i,j,0]-data[i,j,1]>200 and data[i,j,0]-data[i,j,2]>200:
        if abs(data[i,j,0]-237) < 50 and abs(data[i,j,1] - 28) < 50 and abs(data[i,j,2] - 36) < 50:
            sets.append([(i,j)])


def findGroup(sets):
    x = 0
    y = 0
    for i in range(len(sets)):
        set1 = sets[i]
        for j in range(i):
            set2 = sets[j]
            for (x1, y1) in set1:
                for (x2, y2) in set2:
                    if abs(x1 - x2) + abs(y1 - y2) == 1:
                        x = i
                        y = j
                        return (x,y)
    return None

def getCenter(set):
    sum_x = 0
    sum_y = 0
    length = len(set)
    for (x,y) in set:
        sum_x += x
        sum_y += y
    sum_x //= length
    sum_y //= length
    return (sum_x, sum_y)

def getLines(x,y, radius, n, p):
    lines = []
    for i in range(0, 100):
        theta = 2 * pi * i / 100
        line = []
        for dx in range(0, int(radius * cos(theta)) + 1):
            for dy in range(0, int(radius * sin(theta)) + 1):
                if x + dx >= 0 and y + dy >= 0 and x + dx < n and y + dy < p and not (x + dx, y + dy) in line:
                    line.append((x + dx, y + dy))
                    
        if line != []:
            lines.append((theta,line))
    return lines


while True:
    result = findGroup(sets)
    if result is None:
        break
    x,y = result
    #print(x,y)
    for (i,j) in sets[y]:
        sets[x].append((i,j))
    del sets[y]

i = 0
while i < len(sets):
    if len(sets[i]) < 3:
        del sets[i]
    else:
        i += 1

centers =[getCenter(set) for set in sets]

copy = np.copy(data)

n,p,c = copy.shape

for (i,j) in centers:
    lines = getLines(x,y,15, n, p)
    for (theta, line) in lines:
        diff = sum([abs(copy[x,y,0] - 34) + abs(copy[x,y,1] - 177) + abs(copy[x,y,2] - 76) for (x,y) in line]) / len(line)
        if diff < 100:
            (x,y) = (i + int(10 * cos(theta)), j + int(10 * sin(theta)))
            copy[x,y,0] = 0
            copy[x,y,1] = 0
            copy[x,y,2] = 255
    #plt.plot(list(range(0,length)), dark)
    #plt.show()
    '''
    for k in range(length):
        if x >= 0 and x < n and y >= 0 and y < p:
            #if copy[x,y,0] < 30 and copy[x,y,1] < 30 and copy[x,y,2] < 30:
                #print(x,y)
            copy[x,y,0] = 0
            copy[x,y,1] = 255
            copy[x,y,2] = 0
    '''



for i in range(len(sets)):
    set = sets[i]
    for (x,y) in set:
        if i == 4:
            print(centers[i])
            copy[x,y,0] = 0
            copy[x,y,1] = 255
            copy[x,y,2] = 0
        else:
            copy[x,y,0] = 0
            copy[x,y,1] = 0
            copy[x,y,2] = 255
#plt.scatter(X,Y)
#plt.show()
plt.imshow(copy)
plt.show()
#print(sets)

#points à relier : (30, 325)
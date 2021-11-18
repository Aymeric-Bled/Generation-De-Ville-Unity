from PIL import Image
from numpy import asarray
# load the image
image = Image.open('plan_bordeaux.jpg')
# convert image to numpy array
data = asarray(image)
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


for i in range(data.shape[0]):
    for j in range(data.shape[1]):
        if data[i,j,0]-data[i,j,1]>200 and data[i,j,0]-data[i,j,2]>200:
            print(j,i)


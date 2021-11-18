import cv2 as cv
img = cv.imread("plan_bordeaux.jpg")
n,p=img.shape[:-1]
img = cv.resize(img, (n*2,p*2))

print(n,p)
file=open("Coordinatew.txt","w")

#mouse callback function
def draw_circle(event,x,y,flags,param):
    print(event)
    if event == cv.EVENT_LBUTTONDOWN:
        file.write(str(x))
        file.write("\t")
        file.write(str(y))
        file.write("\n")
    
# Create a black image, a window and bind the function to window

cv.namedWindow('image')
cv.setMouseCallback('image',draw_circle)

cv.imshow("Image",img)
cv.waitKey(0) 

cv.destroyAllWindows()
file.close()
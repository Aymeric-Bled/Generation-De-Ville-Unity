from tkinter import *
#import tkinter.filedialog as tkFileDialog
from tkinter.filedialog import askopenfilename
from PIL import Image, ImageTk

if __name__ == "__main__":
    root = Tk()

    #setting up a tkinter canvas with scrollbars
    frame = Frame(root, bd=2, relief=SUNKEN)
    frame.grid_rowconfigure(0, weight=1)
    frame.grid_columnconfigure(0, weight=1)
    xscroll = Scrollbar(frame, orient=HORIZONTAL)
    xscroll.grid(row=1, column=0, sticky=E+W)
    yscroll = Scrollbar(frame)
    yscroll.grid(row=0, column=1, sticky=N+S)
    canvas = Canvas(frame, bd=0, xscrollcommand=xscroll.set, yscrollcommand=yscroll.set)
    canvas.grid(row=0, column=0, sticky=N+S+E+W)
    xscroll.config(command=canvas.xview)
    yscroll.config(command=canvas.yview)
    frame.pack(fill=BOTH,expand=1)

    #adding the image
    File = askopenfilename(parent=root, initialdir="C:/",title='Choose an image.')
    img = ImageTk.PhotoImage(Image.open(File))
    canvas.create_image(0,0,image=img,anchor="nw")
    canvas.config(scrollregion=canvas.bbox(ALL))

    begin = True
    x = 0
    y = 0
    file = open('water1.txt', 'w')
    
    #function to be called when mouse is clicked
    def getBegin(event):
        global begin,x,y
        #outputting x and y coords to console
        if begin:
            (x,y) = (event.x,event.y)
            begin = False

    def getEnd(event):
        global begin,x,y,file
        if not begin:
            file.write(str(x) + "\t" + str(y) + "\t" + str(event.x) + "\t" + str(event.y) + "\n")
            begin = True
    #mouseclick event
    canvas.bind("<Button 1>",getBegin)
    canvas.bind("<Button 3>",getEnd)

    root.mainloop()
    file.close()
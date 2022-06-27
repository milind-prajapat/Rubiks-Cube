#!/usr/bin/env python
# coding: utf-8

# In[ ]:


import cv2
import time
import copy
import pyautogui
import numpy as np


# In[ ]:


def init():
    global Face_Dict, Color_Dict, Rotation_Dict, Block_Connection_Dict
    
    Face_Dict = {0:"Top", 1:"Front", 2:"Left", 3:"Back", 4:"Right", 5:"Bottom"}

    # 0:lower, 1:upper
    Color_Dict = {"Orange":[[0  ,103,250],[5  ,113,260]],
                  "Yellow":[[4  ,199,248],[14 ,209,255]],
                  "Green" :[[79 ,152,0  ],[89 ,162,5  ]],
                  "White" :[[250,250,250],[255,255,255]],
                  "Blue"  :[[241,124,56 ],[251,134,66 ]],
                  "Red"   :[[42 ,61 ,215],[52 ,71 ,225]]}

    # down, clockwise, right is positive # up, anti-clockwise, left is negative
    # taken with respect to front face
    # 0:positive, 1:negative
    Rotation_Dict = {"Vertical"  :[["L","M","r"],["l","m","R"]],
                     "Face"      :[["F","S","b"],["f","s","B"]],
                     "Horizontal":[["u","e","D"],["U","E","d"]],
                     "Turn"      :[["x","y","Z"],["X","Y","z"]]}
    
    Block_Connection_Dict = {"Centre":[[["Top",[1,1]]],[["Front",[1,1]]],[["Left",[1,1]]],[["Back",[1,1]]],[["Right",[1,1]]],[["Bottom",[1,1]]]],
                              
                             "Adjacent":[[["Bottom",[0,1]],["Front",[2,1]]],[["Bottom",[1,0]],["Left" ,[2,1]]],[["Bottom",[1,2]],["Right",[2,1]]],[["Bottom",[2,1]],["Back" ,[2,1]]],
                                         [["Left"  ,[1,2]],["Front",[1,0]]],[["Right" ,[1,0]],["Front",[1,2]]],[["Back"  ,[1,0]],["Right",[1,2]]],[["Back"  ,[1,2]],["Left" ,[1,0]]],
                                         [["Top"   ,[0,1]],["Back" ,[0,1]]],[["Top"   ,[1,0]],["Left" ,[0,1]]],[["Top"   ,[1,2]],["Right",[0,1]]],[["Top"   ,[2,1]],["Front",[0,1]]]],
                              
                             "Corner":  [[["Top"   ,[2,2]],["Front",[0,2]],["Right",[0,0]]],[["Top"   ,[0,2]],["Right",[0,2]],["Back" ,[0,0]]],
                                         [["Top"   ,[0,0]],["Back" ,[0,2]],["Left" ,[0,0]]],[["Top"   ,[2,0]],["Left" ,[0,2]],["Front",[0,0]]],
                                         [["Bottom",[0,0]],["Left" ,[2,2]],["Front",[2,0]]],[["Bottom",[0,2]],["Right",[2,0]],["Front",[2,2]]],
                                         [["Bottom",[2,0]],["Left" ,[2,0]],["Back" ,[2,2]]],[["Bottom",[2,2]],["Right",[2,2]],["Back" ,[2,0]]]]}


# In[ ]:


def Grab_Face():
    img = np.array(pyautogui.screenshot(region = (5,143,949-5,1019-143)))
    w,h = img.shape[:2]

    img = cv2.cvtColor(img, cv2.COLOR_RGB2BGR)

    black_img = cv2.inRange(img, (0,0,0), (5,5,5))
    contours, _ = cv2.findContours(black_img, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
    contours.sort(key=cv2.contourArea, reverse = True)
    
    mask = np.zeros((w,h),np.uint8)
    cv2.drawContours(mask,contours,0,255,-1)
    new_img = cv2.bitwise_and(img, img, mask = mask)

    Bottom = [0,0]
    Bottom_Right = [0,0]
    Top_Right = [0,0]
    R = 0
    L = h+w

    for contour in contours[0]:
        x, y = contour[0]
        if y > Bottom[1]:
            Bottom = [x,y]
        if x+y > R:
            R = x+y
            Bottom_Right = [x,y]
        if y-x < L:
            L = y-x
            Top_Right = [x,y]

    sobelx = cv2.Sobel(black_img,cv2.CV_16S,1,0)
    sobelx = cv2.convertScaleAbs(sobelx)

    kernelx = cv2.getStructuringElement(cv2.MORPH_RECT,(1,12))
    sobelx = cv2.erode(sobelx,kernelx)
    sobelx = cv2.dilate(sobelx,np.ones([12,3],np.uint8),iterations=3)

    contours, _ = cv2.findContours(sobelx, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
    y_max = 0
    best_contour = None

    for contour in contours:
        x,y,w,h = cv2.boundingRect(contour)
        if y+h > y_max:
            y_max = y+h
            best_contour = contour

    x,y,h,w = cv2.boundingRect(best_contour)
    Centre = [int(x+h/2),y]

    src = np.array([Centre, Top_Right, Bottom, Bottom_Right],np.float32)
    dst = np.array([[0,0],[300,0],[0,300],[300,300]],np.float32)

    M = cv2.getPerspectiveTransform(src,dst)
    warp = cv2.warpPerspective(img,M,(300,300))
    
    return warp


# In[ ]:


def Read_Face(warp):
    global Color_Dict
    
    Face = [[0, 0, 0],
            [0, 0, 0],
            [0, 0, 0]]
    for key in Color_Dict:
        lower, upper = Color_Dict[key]
        mask = cv2.inRange(warp,np.array(lower),np.array(upper))
        res = cv2.bitwise_and(warp, warp, mask = mask)
        gray = cv2.cvtColor(res,cv2.COLOR_BGR2GRAY)
        contours, _ = cv2.findContours(gray, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)

        for contour in contours:
            if cv2.contourArea(contour) > 200:
                m = cv2.moments(contour)
                X = int(m["m01"]/m["m00"])//100
                Y = int(m["m10"]/m["m00"])//100
                Face[X][Y] = key[0]
    
    return Face


# In[ ]:


def Form_Cube():
    global Face_Dict, Rotation_Dict, Cube_Dict
    
    Cube_Dict = {}

    warp = Grab_Face()
    Cube_Dict[Face_Dict[0]] = Read_Face(warp)
    pyautogui.press(Rotation_Dict["Turn"][1][0])
    time.sleep(0.5)
    
    for i in range(1,5):
        warp = Grab_Face()
        Cube_Dict[Face_Dict[i]] = Read_Face(warp)
        pyautogui.press(Rotation_Dict["Turn"][0][1])
        time.sleep(0.5)

    pyautogui.press(Rotation_Dict["Turn"][1][0])
    time.sleep(0.5)
    warp = Grab_Face()
    Cube_Dict[Face_Dict[5]] = Read_Face(warp)
    Take_Move(["X"],False)


# In[ ]:


def Clockwise(Face):
    List =  [[0, 0, 0],
             [0, 0, 0],
             [0, 0, 0]]
    
    for i in range(3):
        for j in range(3):
            List[i][j] = Face[2-j][i]
            
    return List 


# In[ ]:


def AntiClockwise(Face):
    List =  [[0, 0, 0],
             [0, 0, 0],
             [0, 0, 0]]
    
    for i in range(3):
        for j in range(3):
            List[i][j] = Face[j][2-i]
            
    return List  


# In[ ]:


def Rotate_180(Face):
    List =  [[0, 0, 0],
             [0, 0, 0],
             [0, 0, 0]]
    
    for i in range(3):
        for j in range(3):
            List[i][j] = Face[2-i][2-j]
            
    return List 


# In[ ]:


def Transform(Input_Face,Axis_1,Index_1,Output_Face,Axis_2,Index_2, reverse = False):
    if Axis_1 == "R":
        List = Input_Face[Index_1]
    else:
        List = [Input_Face[i][Index_1] for i in range(3)]
        
    if reverse:
        List.reverse()
        
    if Axis_2 == "R":
        Output_Face[Index_2] = List
    else:
        for i in range(3):
            Output_Face[i][Index_2] = List[i]
    
    return Output_Face


# In[ ]:


def Take_Move(List, press = True):
    global Rotation_Dict, Cube_Dict
    
    for Value in List:
        if Value in Rotation_Dict["Vertical"][0]:
            Index = Rotation_Dict["Vertical"][0].index(Value)

            if Index == 0:
                Cube_Dict["Left"] = Clockwise(Cube_Dict["Left"])
            elif Index == 2:
                Cube_Dict["Right"] = AntiClockwise(Cube_Dict["Right"])

            Front = Transform(Cube_Dict["Top"],"C",Index,copy.deepcopy(Cube_Dict["Front"]),"C",Index)
            Bottom = Transform(Cube_Dict["Front"],"C",Index,copy.deepcopy(Cube_Dict["Bottom"]),"C",Index)
            Back = Transform(Cube_Dict["Bottom"],"C",Index,copy.deepcopy(Cube_Dict["Back"]),"C",2-Index,True)
            Top = Transform(Cube_Dict["Back"],"C",2-Index,copy.deepcopy(Cube_Dict["Top"]),"C",Index,True)

            Cube_Dict["Front"] = Front
            Cube_Dict["Bottom"] = Bottom
            Cube_Dict["Back"] = Back
            Cube_Dict["Top"] = Top

        elif Value in Rotation_Dict["Vertical"][1]:
            Index = Rotation_Dict["Vertical"][1].index(Value)

            if Index == 0:
                Cube_Dict["Left"] = AntiClockwise(Cube_Dict["Left"])
            elif Index == 2:
                Cube_Dict["Right"] = Clockwise(Cube_Dict["Right"])

            Back = Transform(Cube_Dict["Top"],"C",Index,copy.deepcopy(Cube_Dict["Back"]),"C",2-Index,True)
            Top = Transform(Cube_Dict["Front"],"C",Index,copy.deepcopy(Cube_Dict["Top"]),"C",Index)
            Front = Transform(Cube_Dict["Bottom"],"C",Index,copy.deepcopy(Cube_Dict["Front"]),"C",Index)
            Bottom = Transform(Cube_Dict["Back"],"C",2-Index,copy.deepcopy(Cube_Dict["Bottom"]),"C",Index,True)

            Cube_Dict["Back"] = Back
            Cube_Dict["Top"] = Top
            Cube_Dict["Front"] = Front
            Cube_Dict["Bottom"] = Bottom

        elif Value in Rotation_Dict["Face"][0]:
            Index = Rotation_Dict["Face"][0].index(Value)

            if Index == 0:
                Cube_Dict["Front"] = Clockwise(Cube_Dict["Front"])
            elif Index == 2:
                Cube_Dict["Back"] = AntiClockwise(Cube_Dict["Back"])

            Right = Transform(Cube_Dict["Top"],"R",2-Index,copy.deepcopy(Cube_Dict["Right"]),"C",Index)
            Bottom = Transform(Cube_Dict["Right"],"C",Index,copy.deepcopy(Cube_Dict["Bottom"]),"R",Index,True)
            Left = Transform(Cube_Dict["Bottom"],"R",Index,copy.deepcopy(Cube_Dict["Left"]),"C",2-Index)
            Top = Transform(Cube_Dict["Left"],"C",2-Index,copy.deepcopy(Cube_Dict["Top"]),"R",2-Index,True)

            Cube_Dict["Right"] = Right
            Cube_Dict["Bottom"] = Bottom
            Cube_Dict["Left"] = Left
            Cube_Dict["Top"] = Top

        elif Value in Rotation_Dict["Face"][1]:
            Index = Rotation_Dict["Face"][1].index(Value)

            if Index == 0:
                Cube_Dict["Front"] = AntiClockwise(Cube_Dict["Front"])
            elif Index == 2:
                Cube_Dict["Back"] = Clockwise(Cube_Dict["Back"])

            Left = Transform(Cube_Dict["Top"],"R",2-Index,copy.deepcopy(Cube_Dict["Left"]),"C",2-Index,True)
            Bottom = Transform(Cube_Dict["Left"],"C",2-Index,copy.deepcopy(Cube_Dict["Bottom"]),"R",Index)
            Right = Transform(Cube_Dict["Bottom"],"R",Index,copy.deepcopy(Cube_Dict["Right"]),"C",Index,True)
            Top = Transform(Cube_Dict["Right"],"C",Index,copy.deepcopy(Cube_Dict["Top"]),"R",2-Index)

            Cube_Dict["Left"] = Left
            Cube_Dict["Bottom"] = Bottom
            Cube_Dict["Right"] = Right
            Cube_Dict["Top"] = Top

        elif Value in Rotation_Dict["Horizontal"][0]:
            Index = Rotation_Dict["Horizontal"][0].index(Value)

            if Index == 0:
                Cube_Dict["Top"] = AntiClockwise(Cube_Dict["Top"])
            elif Index == 2:
                Cube_Dict["Bottom"] = Clockwise(Cube_Dict["Bottom"])

            Right = Transform(Cube_Dict["Front"],"R",Index,copy.deepcopy(Cube_Dict["Right"]),"R",Index)
            Back = Transform(Cube_Dict["Right"],"R",Index,copy.deepcopy(Cube_Dict["Back"]),"R",Index)
            Left = Transform(Cube_Dict["Back"],"R",Index,copy.deepcopy(Cube_Dict["Left"]),"R",Index)
            Front = Transform(Cube_Dict["Left"],"R",Index,copy.deepcopy(Cube_Dict["Front"]),"R",Index)

            Cube_Dict["Right"] = Right
            Cube_Dict["Back"] = Back
            Cube_Dict["Left"] = Left
            Cube_Dict["Front"] = Front

        elif Value in Rotation_Dict["Horizontal"][1]:
            Index = Rotation_Dict["Horizontal"][1].index(Value)

            if Index == 0:
                Cube_Dict["Top"] = Clockwise(Cube_Dict["Top"])
            elif Index == 2:
                Cube_Dict["Bottom"] = AntiClockwise(Cube_Dict["Bottom"])

            Left = Transform(Cube_Dict["Front"],"R",Index,copy.deepcopy(Cube_Dict["Left"]),"R",Index)
            Back = Transform(Cube_Dict["Left"],"R",Index,copy.deepcopy(Cube_Dict["Back"]),"R",Index)
            Right = Transform(Cube_Dict["Back"],"R",Index,copy.deepcopy(Cube_Dict["Right"]),"R",Index)
            Front = Transform(Cube_Dict["Right"],"R",Index,copy.deepcopy(Cube_Dict["Front"]),"R",Index)

            Cube_Dict["Left"] = Left
            Cube_Dict["Back"] = Back
            Cube_Dict["Right"] = Right
            Cube_Dict["Front"] = Front

        elif Value == Rotation_Dict["Turn"][0][0]:
            Cube_Dict["Left"] = Clockwise(Cube_Dict["Left"])
            Cube_Dict["Right"] = AntiClockwise(Cube_Dict["Right"])

            Front = copy.deepcopy(Cube_Dict["Top"])
            Bottom = copy.deepcopy(Cube_Dict["Front"])
            Back = Rotate_180(copy.deepcopy(Cube_Dict["Bottom"]))
            Top = Rotate_180(copy.deepcopy(Cube_Dict["Back"]))

            Cube_Dict["Front"] = Front
            Cube_Dict["Bottom"] = Bottom
            Cube_Dict["Back"] = Back
            Cube_Dict["Top"] = Top

        elif Value == Rotation_Dict["Turn"][0][1]:
            Cube_Dict["Bottom"] = Clockwise(Cube_Dict["Bottom"])
            Cube_Dict["Top"] = AntiClockwise(Cube_Dict["Top"])

            Right = copy.deepcopy(Cube_Dict["Front"])
            Back = copy.deepcopy(Cube_Dict["Right"])
            Left = copy.deepcopy(Cube_Dict["Back"])
            Front = copy.deepcopy(Cube_Dict["Left"])

            Cube_Dict["Right"] = Right
            Cube_Dict["Back"] = Back
            Cube_Dict["Left"] = Left
            Cube_Dict["Front"] = Front

        elif Value == Rotation_Dict["Turn"][0][2]:
            Cube_Dict["Front"] = Clockwise(Cube_Dict["Front"])
            Cube_Dict["Back"] = AntiClockwise(Cube_Dict["Back"])

            Right = Clockwise(Cube_Dict["Top"])
            Bottom = Clockwise(Cube_Dict["Right"])
            Left = Clockwise(Cube_Dict["Bottom"])
            Top = Clockwise(Cube_Dict["Left"])

            Cube_Dict["Right"] = Right
            Cube_Dict["Bottom"] = Bottom
            Cube_Dict["Left"] = Left
            Cube_Dict["Top"] = Top

        elif Value == Rotation_Dict["Turn"][1][0]:
            Cube_Dict["Right"] = Clockwise(Cube_Dict["Right"])
            Cube_Dict["Left"] = AntiClockwise(Cube_Dict["Left"])

            Back = Rotate_180(copy.deepcopy(Cube_Dict["Top"]))
            Bottom = Rotate_180(copy.deepcopy(Cube_Dict["Back"]))
            Front = copy.deepcopy(Cube_Dict["Bottom"])
            Top = copy.deepcopy(Cube_Dict["Front"])

            Cube_Dict["Back"] = Back
            Cube_Dict["Bottom"] = Bottom
            Cube_Dict["Front"] = Front
            Cube_Dict["Top"] = Top

        elif Value == Rotation_Dict["Turn"][1][1]:
            Cube_Dict["Top"] = Clockwise(Cube_Dict["Top"])
            Cube_Dict["Bottom"] = AntiClockwise(Cube_Dict["Bottom"])

            Left = copy.deepcopy(Cube_Dict["Front"])
            Back = copy.deepcopy(Cube_Dict["Left"])
            Right = copy.deepcopy(Cube_Dict["Back"])
            Front = copy.deepcopy(Cube_Dict["Right"])

            Cube_Dict["Left"] = Left
            Cube_Dict["Back"] = Back
            Cube_Dict["Right"] = Right
            Cube_Dict["Front"] = Front

        elif Value == Rotation_Dict["Turn"][1][2]:
            Cube_Dict["Back"] = Clockwise(Cube_Dict["Back"])
            Cube_Dict["Front"] = AntiClockwise(Cube_Dict["Front"])

            Left = AntiClockwise(Cube_Dict["Top"])
            Bottom = AntiClockwise(Cube_Dict["Left"])
            Right = AntiClockwise(Cube_Dict["Bottom"])
            Top = AntiClockwise(Cube_Dict["Right"])

            Cube_Dict["Left"] = Left
            Cube_Dict["Bottom"] = Bottom
            Cube_Dict["Right"] = Right
            Cube_Dict["Top"] = Top
        if press:
            pyautogui.press(Value)


# In[ ]:


def Allign_White():
    global Cube_Dict
    
    best_key = None
    for key in Cube_Dict:
        if Cube_Dict[key][1][1] == "W":
            best_key = key

    if best_key != "Top":
        if best_key == "Bottom":
            Take_Move(["X","X"])
        elif best_key == "Front":
            Take_Move(["X"])
        elif best_key == "Back":
            Take_Move(["x"])
        elif best_key == "Right":
            Take_Move(["z"])
        elif best_key == "Left":
            Take_Move(["Z"])


# In[ ]:


def White_Cross():
    global Cube_Dict, Face_Dict
    
    while True:
        Start = []
        End = []
        Start_Face = None
        
        Temp_Dict = {"Left" :["y"],
                     "Right":["Y"],
                     "Back" :["y","y"]}
        while True:
            if Cube_Dict["Top"][0][1] != "W":
                End = [0,1]
            elif Cube_Dict["Top"][1][0] != "W":
                End = [1,0]
            elif Cube_Dict["Top"][1][2] != "W":
                End = [1,2]
            elif Cube_Dict["Top"][2][1] != "W":
                End = [2,1]
            else:
                return None
                
            for i in range(1,6):
                if Cube_Dict[Face_Dict[i]][0][1] == "W":
                    Start = [0,1]
                    Start_Face = Face_Dict[i]
                    break
                if Cube_Dict[Face_Dict[i]][1][0] == "W":
                    Start = [1,0]
                    Start_Face = Face_Dict[i]
                    break
                if Cube_Dict[Face_Dict[i]][1][2] == "W":
                    Start = [1,2]
                    Start_Face = Face_Dict[i]
                    break
                if Cube_Dict[Face_Dict[i]][2][1] == "W":
                    Start = [2,1]
                    Start_Face = Face_Dict[i]
                    break
            
            if Start_Face in Temp_Dict:
                Take_Move(Temp_Dict[Start_Face])
            elif Start_Face in ["Front","Bottom"]:
                break

        if Start == [0,1]:
            if Start_Face == "Front":
                Take_Move(["f","U","l"])
            else:
                if End == [0,1]:
                    Take_Move(["u","u","F","F"])
                elif End == [1,0]:
                    Take_Move(["u","F","F"])
                elif End == [1,2]:
                    Take_Move(["U","F","F"])
                elif End == [2,1]:
                    Take_Move(["F","F"])

        elif Start == [1,0]:
            if End == [0,1]:
                Temp_Dict = {"Front":["u","l"],
                            "Bottom":["u","l","l"]}
            elif End == [1,0]:
                Temp_Dict = {"Front":["l"],
                            "Bottom":["l","l"]}
            elif End == [1,2]:
                Temp_Dict = {"Front":["u","u","l"],
                            "Bottom":["u","u","l","l"]}
            elif End == [2,1]:
                Temp_Dict = {"Front":["U","l"],
                            "Bottom":["U","l","l"]}
            Take_Move(Temp_Dict[Start_Face])

        elif Start == [1,2]:
            if End == [0,1]:
                Temp_Dict = {"Front":["U","R"],
                            "Bottom":["U","R","R"]}
            elif End == [1,0]:
                Temp_Dict = {"Front":["U","U","R"],
                            "Bottom":["U","U","R","R"]}
            elif End == [1,2]:
                Temp_Dict = {"Front":["R"],
                            "Bottom":["R","R"]}
            elif End == [2,1]:
                Temp_Dict = {"Front":["u","R"],
                            "Bottom":["u","R","R"]}
            Take_Move(Temp_Dict[Start_Face])

        elif Start == [2,1]:
            if End == [0,1]:
                Temp_Dict = {"Front":["u","u","F","U","l"],
                            "Bottom":["b","b"]}
            elif End == [1,0]:
                Temp_Dict = {"Front":["u","F","U","l"],
                            "Bottom":["D","l","l"]}
            elif End == [1,2]:
                Temp_Dict = {"Front":["U","F","U","l"],
                            "Bottom":["d","R","R"]}
            elif End == [2,1]:
                Temp_Dict = {"Front":["F","U","l"],
                            "Bottom":["d","d","F","F"]}
            Take_Move(Temp_Dict[Start_Face])


# In[ ]:


def Rotate_List(List):
    L = []
    for i in range(-1,3):
        L.append(List[i])
    return L


# In[ ]:


def The_I():
    global Cube_Dict
    
    while True:
        L1 = []
        L2 = []
        List = ["Front","Right","Back","Left"]

        for Face in List:
            L1.append(Cube_Dict[Face][0][1])
            L2.append(Cube_Dict[Face][1][1])

        Check = False
        Best_Face = None
        Move = 0
        for i in range(4):
            A = []
            N_A = []
            for j in range(len(L1)):
                if L1[j] == L2[j]:
                    A.append(List[j])
                elif L1[j] != L2[j]:
                    N_A.append(List[j])

            if len(A) == 1:
                Best_Face = A[0]
                Move = i
            elif len(A) == 2:
                if A == List[0:2]:
                    Best_Face = "Back"
                    Move = i
                elif A == List[1:3]:
                    Best_Face = "Left"
                    Move = i
                elif A == List[2:4]:
                    Best_Face = "Front"
                    Move = i
                elif A[0] == List[3] and A[1] == List[0]:
                    Best_Face = "Right"
                    Move = i
                else:
                    if "Front" in A:
                        Best_Face = "Front" 
                        Move = 0
                    else:
                        Best_Face = "Left"
                        Move = 3
                break
            elif len(A) == 4:
                Check = True
                Move = i
            L1 = Rotate_List(L1)

        if Move in [1,2]:
            Take_Move(["u" for _ in range(Move)])
        elif Move == 3:
            Take_Move(["U"])

        if Check:
            break

        Temp_Dict = {"Front":["R","u","u","r","u","R","u","r"],
                     "Right":["B","u","u","b","u","B","u","b"],
                     "Back" :["L","u","u","l","u","L","u","l"],
                     "Left" :["F","u","u","f","u","F","u","f"]}
        Take_Move(Temp_Dict[Best_Face])


# In[ ]:


def White_Corners():
    global Block_Connection_Dict, Cube_Dict
    
    while True:
        Not_White = []
        Start = []
        Start_Face = None

        while True:
            no_of_alligned = 0
            for Corner_Piece in Block_Connection_Dict["Corner"]:
                Equal = True
                Non_White = []
                White = []
                for Block in Corner_Piece:
                    if Cube_Dict[Block[0]][Block[1][0]][Block[1][1]] != Cube_Dict[Block[0]][1][1]:
                        Equal = False
                    if Cube_Dict[Block[0]][Block[1][0]][Block[1][1]] == "W":
                        White = [Block[1][0],Block[1][1]]
                        Start_Face = Block[0]
                    elif Cube_Dict[Block[0]][Block[1][0]][Block[1][1]] != "W":
                        Non_White.append(Cube_Dict[Block[0]][Block[1][0]][Block[1][1]])
                
                if len(White) and not Equal:
                    Start = White[:]
                    Not_White = Non_White[:]
                    break
                elif Equal and len(White):
                    no_of_alligned+=1

            if no_of_alligned == 4:
                return None

            if Start_Face in ["Left","Right","Back"]:
                Temp_Dict = {"Left" :["y"],
                             "Right":["Y"],
                             "Back" :["y","y"]}
                Take_Move(Temp_Dict[Start_Face])
            elif Start_Face == "Top":
                Temp_Dict = {"0,0":["B","D","b"],
                             "0,2":["b","d","B"],
                             "2,0":["L","D","l"],
                             "2,2":["r","d","R"]}
                Take_Move(Temp_Dict[str(Start[0])+","+str(Start[1])])
            else:
                break

        End = []
        List = ["Front","Right","Back","Left","Front"]
        Temp_Dict = {"Front":[2,2],
                     "Right":[0,2],
                     "Back" :[0,0],
                     "Left" :[2,0]}

        for i in range(4):
            if Cube_Dict[List[i]][1][1] in Not_White and Cube_Dict[List[i+1]][1][1] in Not_White:
                End = Temp_Dict[List[i]]
                
        if Start == [0,0]:
            if End == [0,0]:
                Temp_Dict = {"Front" :["f","B","d","F","b"],
                             "Bottom":["d","l","D","L","B","d","d","b"]}                 
            elif End == [0,2]:
                Temp_Dict = {"Front" :["f","d","R","d","r","F"],
                             "Bottom":["D","D","R","d","r","b","d","d","B"]}
            elif End == [2,0]:
                Temp_Dict = {"Front" :["f","d","F","D","D","L","d","l"],
                             "Bottom":["f","D","F","d","L","d","l"]}
            elif End == [2,2]:
                Temp_Dict = {"Front" :["f","d","d","F","F","d","f"],
                             "Bottom":["D","r","D","R","F","d","d","f"]}    
        elif Start == [0,2]:
            if End == [0,0]:
                Temp_Dict = {"Front" :["F","D","f","l","D","L"],
                             "Bottom":["d","d","l","D","L","B","d","d","b"]}
            elif End == [0,2]:
                Temp_Dict = {"Front" :["F","D","f","d","b","D","B"],
                             "Bottom":["D","R","d","r","b","D","D","B"]}
            elif End == [2,0]:
                Temp_Dict = {"Front" :["F","d","d","f","f","D","F"],
                             "Bottom":["d","f","D","F","d","L","d","l"]}
            elif End == [2,2]:
                Temp_Dict = {"Front" :["F","D","f","d","d","r","D","R"],
                             "Bottom":["r","d","d","R","D","D","F","d","f"]}    
        elif Start == [2,0]:
            if End == [0,0]:
                Temp_Dict = {"Front" :["B","d","b"],
                             "Bottom":["B","d","b","D","l","D","L"]}
            elif End == [0,2]:
                Temp_Dict = {"Front" :["R","D","D","r"],
                             "Bottom":["D","D","D","R","d","r","b","d","d","B"]}
            elif End == [2,0]:
                Temp_Dict = {"Front" :["D","L","d","l"],
                             "Bottom":["D","L","D","D","l","d","d","f","D","F"]}  
            elif End == [2,2]:
                Temp_Dict = {"Front" :["D","D","F","d","f"],
                             "Bottom":["r","d","R","d","r","D","R"]}
        elif Start == [2,2]:
            if End == [0,0]:
                Temp_Dict = {"Front" :["D","l","D","L"],
                             "Bottom":["d","B","D","b","D","B","d","b"]}
            elif End == [0,2]:
                Temp_Dict = {"Front" :["b","D","B"],
                             "Bottom":["d","d","b","d","B","d","b","D","B"]}         
            elif End == [2,0]:
                Temp_Dict = {"Front" :["d","d","f","D","F"],
                             "Bottom":["f","d","F","d","f","D","F"]}
            elif End == [2,2]:
                Temp_Dict = {"Front" :["d","r","D","R"],
                             "Bottom":["D","r","d","R","d","r","D","R"]}
        Take_Move(Temp_Dict[Start_Face])


# In[ ]:


def Second_Layer():
    global Cube_Dict, Block_Connection_Dict
    
    while True:
        Start = []
        Start_Face = None
        Best_Piece = False

        no_of_alligned = 0
        for Adjacent_Piece in Block_Connection_Dict["Adjacent"]:
            Equal = True
            Correct_Piece = True
            best_piece = True
            for Block in Adjacent_Piece:
                if Cube_Dict[Block[0]][Block[1][0]][Block[1][1]] != Cube_Dict[Block[0]][1][1]:
                    Equal = False
                if Cube_Dict[Block[0]][Block[1][0]][Block[1][1]] in ["W","Y"]:
                    Correct_Piece = False
                if Block[0]!="Bottom" and Block[1][0] == 1:
                    best_piece = False

            if Correct_Piece and not Equal:
                Start = Adjacent_Piece[1][1][:]
                Start_Face = Adjacent_Piece[1][0]
                if best_piece:
                    Best_Piece = True
                    break
            elif Equal and Correct_Piece:
                no_of_alligned+=1

        if no_of_alligned == 4:
            break
        
        if Best_Piece:
            List = ["Front","Right","Back","Left"]
            Index = List.index(Start_Face)

            Temp_Dict = {0:[],-1:["D"],1:["d"],2:["d","d"],-2:["d","d"],3:["D"],-3:["d"]}
            for Face in List:
                if Cube_Dict[Start_Face][Start[0]][Start[1]] == Cube_Dict[Face][1][1]:
                    Take_Move(Temp_Dict[Index - List.index(Face)])
                    Start_Face = Face
                    break

        if Start_Face in ["Left","Right","Back"]:
            Temp_Dict = {"Left" :["y"],
                         "Right":["Y"],
                         "Back" :["y","y"]}
            Take_Move(Temp_Dict[Start_Face])
            Start_Face = "Front"

        if Cube_Dict["Bottom"][0][1] == Cube_Dict["Right"][1][1] or (not Best_Piece and Start == [1,2]):
            Take_Move(["d","r","D","R","D","F","d","f"])
        elif Cube_Dict["Bottom"][0][1] == Cube_Dict["Left"][1][1] or (not Best_Piece and Start == [1,0]):
            Take_Move(["D","L","d","l","d","f","D","F"])


# In[ ]:


def Yellow_Cross():
    global Cube_Dict

    while True:
        L = [0,0,0,0]
        if Cube_Dict["Top"][0][1] == "Y":
            L[0] = 1
        if Cube_Dict["Top"][1][0] == "Y":
            L[1] = 1
        if Cube_Dict["Top"][1][2] == "Y":
            L[2] = 1
        if Cube_Dict["Top"][2][1] == "Y":
            L[3] = 1

        if all(L):
            break
        elif all([L[0],L[2]]):
            Take_Move("y")
        elif all([L[1],L[3]]):
            Take_Move(["Y"])
        elif all(L[2:4]):
            Take_Move(["y","y"])
        elif all([L[0],L[3]]):
            Take_Move(["y"])

        Take_Move(["F","R","U","r","u","f"])


# In[ ]:


def Fix_Yellow_Corners():
    global Cube_Dict, Block_Connection_Dict
    
    while True:
        Start_Face = None

        no_of_alligned_pieces = 0
        no_of_correct_pieces = 0
        Alligned_Face = None

        for Corner_Piece in Block_Connection_Dict["Corner"][:4]:
            List = [Cube_Dict[i][1][1] for i,_ in Corner_Piece]
            Correct_Piece = True
            Equal = True
            for Block in Corner_Piece:
                if Cube_Dict[Block[0]][Block[1][0]][Block[1][1]] != Cube_Dict[Block[0]][1][1]:
                    Equal = False
                if Cube_Dict[Block[0]][Block[1][0]][Block[1][1]] not in List:
                    Correct_Piece = False

            if Correct_Piece and not Equal:
                no_of_correct_pieces+=1
                Start_Face = Corner_Piece[1][0]
            elif Equal:
                no_of_alligned_pieces+=1
                Alligned_Face = Corner_Piece[1][0]
                

        if no_of_alligned_pieces == 4 or no_of_correct_pieces + no_of_alligned_pieces == 4:
            break
            
        if Start_Face == None:
            Start_Face = Alligned_Face

        if Start_Face in ["Left","Right","Back"]:
            Temp_Dict = {"Left" :["y"],
                         "Right":["Y"],
                         "Back" :["y","y"]}
            Take_Move(Temp_Dict[Start_Face])

        Take_Move(["U","R","u","l","U","r","u","L"])


# In[ ]:


def Allign_Yellow_Corners():
    global Cube_Dict, Block_Connection_Dict
    is_first = True

    while True:
        no_of_alligned_pieces = 0
        Start_Face = None

        for Corner_Piece in Block_Connection_Dict["Corner"][:4]:
            if Cube_Dict[Corner_Piece[0][0]][Corner_Piece[0][1][0]][Corner_Piece[0][1][1]] == "Y":
                no_of_alligned_pieces+=1
            else:
                Start_Face = Corner_Piece[1][0]
                break

        if no_of_alligned_pieces == 4:
            break

        if is_first:
            if Start_Face in ["Left","Right","Back"]:
                Temp_Dict = {"Left" :["y"],
                             "Right":["Y"],
                             "Back" :["y","y"]}
                Take_Move(Temp_Dict[Start_Face])
            is_first = False
        elif Start_Face in ["Left","Right","Back"]:
            Temp_Dict = {"Left" :["u"],
                         "Right":["U"],
                         "Back" :["u","u"]}
            Take_Move(Temp_Dict[Start_Face])

        Take_Move(["r","d","R","D"])


# In[ ]:


def Finish_Cube():
    global Cube_Dict
    
    Best_Face = None
    for Face in ["Front","Right","Back","Left"]:
        if Cube_Dict["Front"][0][1] == Cube_Dict[Face][1][1]:
            Best_Face = Face

    Temp_Dict = {"Front":[],
                 "Left" :["U"],
                 "Right":["u"],
                 "Back" :["u","u"]}
    Take_Move(Temp_Dict[Best_Face])


# In[ ]:


time.sleep(2)
if __name__ == "__main__":
    init()
    Form_Cube()
    Allign_White()
    White_Cross()
    The_I()
    White_Corners()
    Second_Layer()
    Take_Move(["X","X"])
    Yellow_Cross()
    The_I()
    Fix_Yellow_Corners()
    Allign_Yellow_Corners()
    Finish_Cube()


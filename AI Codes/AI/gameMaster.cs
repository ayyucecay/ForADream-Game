﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class gameMaster : MonoBehaviour
{
    public GameObject[] spaceList;
    public GameObject marker;

    public Button leftButton, dropButton, rightButton, winButton;
    public Text winText;
    bool lInteract = true;
    bool rInteract = true;
    bool dInteract = true;
    float markerPos = 0;
    int colValue;
    int[] corrSpaces = new int[6];
    int[] bottomMark = new int[7] {5,5,5,5,5,5,5};
    int dropCount = 0;

    public Sprite[] DefaultImages;
    public Sprite AIImage;
    public Sprite PlayerImage;

    int PLAYERNUMBER = 1;
    int AINUMBER = 2;

    Sprite corrImage;
    int corrPiece;

    int ROW_COUNT = 6;
    int COLUMN_COUNT = 7;
    int WINDOW_LENGTH = 4;

    int[,] board = new int[6,7];
    

    private int calcCount(int[] arr, int piece){
        int count = 0;
        for(int i=0; i< arr.Length; i++){
            if(arr[i] == piece){
                count+=1;
            }
        }
        return count;
    }

    private int[,] putPiece(int[,] board, int row, int col, int piece){
        board[row,col] = piece;
        return board;
    }
    private bool isValidLocation(int[,] board, int col){
        return (board[ROW_COUNT-1,col] == 0);
    }
    private int getNextOpenRow(int[,] board, int col){

        for(int i = 0; i<ROW_COUNT; i++){
            if(board[i,col] == 0){
                return i;
            }
        }
        return 0;
    }

    private bool isWinningMove(int[,] board, int piece){

        for(int c=0; c<COLUMN_COUNT-3; c++){
            for(int r=0; r<ROW_COUNT; r++){
                if(board[r,c]==piece && board[r,c+1]==piece && board[r,c+2]==piece && board[r,c+3]==piece){
                    return true;
                }
            }
        }

        for(int c=0; c<COLUMN_COUNT; c++){
            for(int r=0; r<ROW_COUNT-3; r++){
                if(board[r,c]==piece && board[r+1,c]==piece && board[r+2,c]==piece && board[r+3,c]==piece){
                    return true;
                }               
            }
        }

        for(int c=0; c<COLUMN_COUNT-3; c++){
            for(int r=0; r<ROW_COUNT-3; r++){
                if(board[r,c]==piece && board[r+1,c+1]==piece && board[r+2,c+2]==piece && board[r+3,c+3]==piece){
                    return true;
                }
            }
                    
        }
        for(int c=0; c<COLUMN_COUNT-3; c++){
            for(int r=3; r<ROW_COUNT; r++){
                if(board[r,c]==piece && board[r-1,c+1]==piece && board[r-2,c+2]==piece && board[r-3,c+3]==piece){
                    return true;
                }   
            }
        }
        return false;
    }

    private int calcScore(int[] window, int piece){
        int score = 0;
        int opponent = PLAYERNUMBER;
        if(piece == PLAYERNUMBER){
            opponent = AINUMBER;
        }

        int countP = calcCount(window,piece);
        int countE = calcCount(window,0);
        int countO = calcCount(window,opponent);

        if(countP == 4 ){
            score -=1;
        }
        else if(countP ==3 && countE == 1){
            score +=1;
        }
        else if(countP ==2 && countE == 2){
            score +=2;
        }

        if (countO == 3 && countE ==1){
            score +=3;
        }
        return score;
    }
    

    private int findScore(int[,] board, int piece){
        int score = 0;
        int[] centerColArray = new int[ROW_COUNT];

        for(int i= 0; i<ROW_COUNT; i++){
            centerColArray[i] = board[i,3];
        }

        int centerCount = calcCount(centerColArray,piece);
        score+= centerCount*3;

        int[] rowArray;
        int[] colArray;

        //Score horizontal
        for(int i = 0; i<ROW_COUNT; i++){
            rowArray = new int[COLUMN_COUNT];
            for(int j= 0; j<COLUMN_COUNT; j++){
                rowArray[j] = board[i,j];
            }
            for(int k = 0; k<COLUMN_COUNT-3; k++){
                int[] window = new int[WINDOW_LENGTH];
                for(int f = k; f<k+WINDOW_LENGTH; f++){
                    window[f-k] = rowArray[f];
                }
                score+=calcScore(window,piece);
            }
        }
        
        
        //Score vertical
        for(int i = 0; i<COLUMN_COUNT; i++){
            colArray = new int[ROW_COUNT];
            for(int j= 0; j<ROW_COUNT; j++){
                colArray[j] = board[j,i];
            }
            for(int k = 0; k<ROW_COUNT-3; k++){
                int[] window = new int[WINDOW_LENGTH];
                for(int f = k; f<k+WINDOW_LENGTH; f++){
                    window[f-k] = colArray[f];
                }
                score+=calcScore(window,piece);
            }
        }
        
        //Score Diag
        for(int i = 0; i<ROW_COUNT-3; i++){
            for(int j= 0; j<COLUMN_COUNT-3; j++){ 
                int[] window = new int[WINDOW_LENGTH];
                for(int k=0; k<WINDOW_LENGTH; k++){
                    window[k] = board[i+k,j+k];
                }
                score+= calcScore(window,piece);
            }
        }

        //Score Diag2
        for(int i = 0; i<ROW_COUNT-3; i++){
            for(int j= 0; j<COLUMN_COUNT-3; j++){ 
                int[] window = new int[WINDOW_LENGTH];
                for(int k=0; k<WINDOW_LENGTH; k++){
                    window[k] = board[i+3-k,j+k];
                }
                score+= calcScore(window,piece);
            }
        }
        
        return score;
    }


    private bool isTerminalNode(int [,] board){
        return isWinningMove(board,PLAYERNUMBER) || isWinningMove(board,AINUMBER) || (getValidLocations(board).Count == 0);
    }


    private int[] minimax(int[,] board, int depth, int alpha, int beta, bool maximizingPlayer){
        ArrayList validLocations = getValidLocations(board);
        bool isTerminal = isTerminalNode(board);

        if(depth == 0 || isTerminal == true){
            if(isTerminal){
                if(isWinningMove(board,AINUMBER)){
                    int[] x = {0,10000000};
                    return x;
                }
                else if(isWinningMove(board,PLAYERNUMBER)){
                    int[] x = {0,-10000000};
                    return x;
                }
                else{
                    int[] x = {0,0};
                    return x;
                }
            }
            else{
                int[] x = {0,findScore(board,AINUMBER)};
                return x;
            }
        }

        if(maximizingPlayer){
            int value = -100000000;
            System.Random random = new System.Random();
            int column = Convert.ToInt32(validLocations[random.Next(0,validLocations.Count)]);

            foreach (int col in validLocations){
                int row = getNextOpenRow(board,col);
                int[,] bCopy = board.Clone() as int[,];
                bCopy = putPiece(bCopy,row,col,AINUMBER);
                int newScore = minimax(bCopy,depth-1,alpha,beta,false)[1];

                if(newScore>value){
                    value = newScore;
                    column = col;
                }
                alpha = Math.Max(alpha,value);
                if(alpha >= beta){
                    break;
                }
            }
            int[] x = {column,value};
            return x;
        }
        else{
            int value = 10000000;
            System.Random random = new System.Random();
            int column = Convert.ToInt32(validLocations[random.Next(0,validLocations.Count)]);

            foreach (int col in validLocations){
                int row = getNextOpenRow(board,col);
                int[,] bCopy = board.Clone() as int[,];
                bCopy = putPiece(bCopy,row,col,PLAYERNUMBER);
                int newScore = minimax(bCopy,depth-1,alpha,beta,true)[1];

                if(newScore<value){
                    value = newScore;
                    column = col;
                }
                beta = Math.Min(beta,value); 
                if(alpha >= beta){
                    break;
                }
            }
            int[] x = {column,value};
            return x;
        }
    }

    private ArrayList getValidLocations(int[,] board){
        ArrayList validLocations = new ArrayList();
        for (int col = 0; col<COLUMN_COUNT; col++){
            if(isValidLocation(board, col)){
                validLocations.Add(col);
            }
        }
        return validLocations;
    }

    bool isDefaultImage(){
        for(int i= 0; i<DefaultImages.Length; i++){
            if (spaceList[corrSpaces[bottomMark[colValue]]].GetComponent<Image>().sprite == DefaultImages[i]){
                return true;
            }
        }
        return false;
        
    }
    
    void DropClick(){
        if(isDefaultImage()){
            spaceList[corrSpaces[bottomMark[colValue]]].GetComponent<Image>().sprite = PlayerImage;
            board = putPiece(board,5-bottomMark[colValue],colValue,corrPiece);
            bottomMark[colValue]--;
            dropCount++;

            if(isWinningMove(board,corrPiece)){
                Debug.Log("Player Wins");
            }
        }

        ColUpdate();
    }

    void ColUpdate(){
        for(int i=0; i<7; i++){
            if(markerPos == (100*i)- 300){
                colValue = i;
                break;
            }
        }

        for( int j=0; j<6; j++){
            corrSpaces[j] = (7*j) + colValue;
        }

        if(bottomMark[colValue]<= -1){
            dInteract = false;
            bottomMark[colValue] = -1;
        }
        else{
            dInteract = true;
        }

        if(dropCount%2 == 0){
            corrImage = PlayerImage;
            corrPiece = PLAYERNUMBER;
        }
        else if(dropCount%2 != 0){
            corrImage = AIImage;
            corrPiece = AINUMBER;

            int[] arr = minimax(board,5,-10000000,1000000,true);

            int col = arr[0];
            while(isValidLocation(board,col)==false){
                arr = minimax(board,5,-10000000,1000000,true);
            }
            if(isValidLocation(board,col)){
                int row = getNextOpenRow(board,col);
                board = putPiece(board,row,col,corrPiece);

                spaceList[((spaceList.Length-1)-(row*COLUMN_COUNT + (ROW_COUNT-col)))].GetComponent<Image>().sprite = corrImage;
                if(isWinningMove(board,corrPiece)){
                    Debug.Log("AI Wins");
                }
            }
            bottomMark[col]--;
            dropCount++;
            corrImage = PlayerImage;
            corrPiece = PLAYERNUMBER;
        }

        marker.GetComponent<Image>().sprite = corrImage;
        leftButton.interactable = lInteract;
        rightButton.interactable = rInteract;
        dropButton.interactable = dInteract;
    }
    
    void Start(){

        ColUpdate();
        markerPos = 0;
        leftButton.onClick.AddListener(LeftClick);
        rightButton.onClick.AddListener(RightClick);
        dropButton.onClick.AddListener(DropClick);

    }

    public void Update(){
        leftButton.interactable = lInteract;
        rightButton.interactable = rInteract;
        dropButton.interactable = dInteract;

        if(markerPos <= -300){
            lInteract = false;
        }
        else if (markerPos > -300){
            lInteract = true;
        }

        if(markerPos >= 300){
            rInteract = false;
        }
        else if (markerPos < 300){
            rInteract = true;
        }
    }


    public void LeftClick(){
        if(markerPos >-300 && lInteract == true){
            lInteract = true;
            marker.transform.position += new Vector3(-100f,0f,0f);
            markerPos -= 100f;
        }
        ColUpdate();
    }

    public void RightClick(){
        if(markerPos < 300 && rInteract == true){
            rInteract = true;
            marker.transform.position += new Vector3(100f,0f,0f);
            markerPos += 100f;
        }
        ColUpdate();
    }
}


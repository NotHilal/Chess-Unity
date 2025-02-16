using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SpecialMove
{
    None = 0,
    EnPassant = 1,
    Casteling = 2

}
public class ChessBoard : MonoBehaviour
{
    
    #region definitions 
    [Header("Art Stuff")]
    
    [SerializeField] private Material tileMat;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float DeathSize = .3F;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private GameObject VictoryScreen;
    [SerializeField] private Button RematchButton;
    [SerializeField] private GameObject WhiteWin;
    [SerializeField] private GameObject BlackWin;
    [SerializeField] private GameObject Checkmate;
    [SerializeField] private GameObject Tie;
    [SerializeField] private GameObject abandon;
    public GameObject daGame;

    public AudioSource source;
    public AudioClip clipCheck;
    public AudioClip clipCheckMate;
    public AudioClip clipPromote;
    public AudioClip clipMovePiece;
    public AudioClip egalite;

    public bool tie = false;


    [Header("Prefabs and Materials")]
    public List<List<Vector2Int>> PositionsB = new List<List<Vector2Int>>();
    public List<List<Vector2Int>> PositionsW = new List<List<Vector2Int>>();
    public int turn = 0;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;


    [Header("Evolves")]
    [SerializeField] private GameObject Evolve;
    [SerializeField] private GameObject Q1;
    [SerializeField] private GameObject Q2;
    [SerializeField] private GameObject K1;
    [SerializeField] private GameObject K2;
    [SerializeField] private GameObject B1;
    [SerializeField] private GameObject B2;
    [SerializeField] private GameObject R1;
    [SerializeField] private GameObject R2;
    private int EvoX = -1;
    private int EvoY = -1;
    private int SaveColor = -1;

    //Logic 
    private ChessPiece[,] chessPieces;
    private List<ChessPiece[,]> boards = new List<ChessPiece[,]>();
    private List<ChessPiece[,]> boards2 = new List<ChessPiece[,]>();
    private ChessPiece[,] chessPiecesBefore = new ChessPiece[numberOfTilesX, numberOfTilesY];
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhite = new List<ChessPiece>();
    private List<ChessPiece> deadBlack = new List<ChessPiece>();
    private const int numberOfTilesX = 8;
    private const int numberOfTilesY = 8;
    private GameObject[,] tiles;
    private Camera currentcam;
    public GameObject lettersW;
    public GameObject lettersB;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool IsWhiteTurn;
    private bool IsBlackTurn;
    private List<Vector2Int[]> movelist = new List<Vector2Int[]>();
    private List<Vector2Int> dedwhites = new List<Vector2Int>();
    private List<Vector2Int> dedblacks = new List<Vector2Int>();
    private SpecialMove SpeMove;
    private List<GameObject> corpses = new List<GameObject>();
    public GameObject goback;
    public GameObject rot;
    public GameObject surrenderButton;
    
    private List<int[]> UpgradeW = new List<int[]>(); // {turn, x positionbefore ,y positionbefore, currentX,currentY}
    private List<int[]> UpgradeB = new List<int[]>();// {turn, x positionbefore ,y positionbefore, currentX,currentY}

    
    //Multiplayer items 
    public int currentTeam = -1;
    public int ALED = -100;
    private int playerCount = -1;
    public bool localGame = true;
    private bool rotationCam = true;
    private bool[] playerRematch = new bool[2];
    public GameObject wannaplay;
    public GameObject dontwannaplay;
    public GameObject goagain;
    public GameObject waitin;
    public GameObject LeaveSolo;
    public GameObject LeaveSoloButton;
    public GameObject TurnAfficheCanvas;
    public Text TurnAffiche;
    public GameObject FFCanvas;
    public int TurnDisplay = 0;
    public PlayfabManager playfabManager;
    

    #region PatManager
    public int LastTurnDied = 0;
    public GameObject MovesNoKill;
    public GameObject SameBoards;
    public GameObject NotEnoughPieces;
    
    #endregion
    
    #region Timer 
    float currentTimeW;
    float currentTimeB;
    public float startMinutes=1;
    private int AddTime=3;
    public Text currentTimeTextW;
    public Text currentTimeTextB;
    public bool countingTime =false;
    string a = "";
    string b = "";
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;
    public Slider Slider;
    public Slider Slider2;
    public bool GameHost = false;
    bool removeOnce = true;
    bool aled = true;
    public GameObject viewTimers;
    public GameObject DoUWantToTie;

    #endregion
    #region SoloCamFix

    bool Ingame=false;
    public GameObject CamWhites;
    public GameObject CamBlacks;
    public GameObject CamMenu1;
    public GameObject CamMenu2;
    public bool smoothCams=true;

    #endregion

    #region AskTie

    public GameObject AskTieRequest;
    public GameObject RequestSent;
    public GameObject AskTieButton;
    public GameObject AgreeTie;
    public GameObject DisagreeTie;
    private int turnbeforeReAsk = -1;
    private bool isAllowedToAsk = true;
    #endregion

    public AudioSource audio;
    public AudioManager audioManager;

    #region Chat



    #endregion

    #endregion

    private void Start()
    {

        viewTimers.SetActive(false);
        AskTieRequest.SetActive(false);
        AskTieButton.SetActive(false);
        RequestSent.SetActive(false);
        AgreeTie.SetActive(false);
        WannaTieF();
        text.text = startMinutes.ToString() + ":00";
        currentTimeB = startMinutes * 60;
        currentTimeW = startMinutes * 60;
        countingTime = false;
        TurnAfficheCanvas.SetActive(false);
        TurnAffiche.text = TurnDisplay.ToString();
        daGame.SetActive(true);
        IsWhiteTurn = true;
        GenerateAllTiles(tileSize, numberOfTilesX, numberOfTilesY);
        SpawnAllPieces();
        PositionAllPieces();
        RegisterEvents();
        VictoryScreen.SetActive(false);
        Evolve.SetActive(false);
        Checkmate.SetActive(false);
        goback.SetActive(false);
       
        List<Vector2Int> movelistW = new List<Vector2Int>();
        List<Vector2Int> movelistB = new List<Vector2Int>();
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    if (chessPieces[i, j].color == 0)
                    {
                        movelistW.Add(new Vector2Int(i, j));
                    }

                }


            }
        }
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    if (chessPieces[i, j].color == 1)
                    {
                        movelistB.Add(new Vector2Int(i, j));
                    }

                }


            }
        }
        PositionsB.Add(movelistB);
        PositionsW.Add(movelistW);

    }
    private void Update()
    {
        
        

        #region fixCamSolo
        if (TurnDisplay > 0)
        {
            if (localGame)
            {
                if(Ingame)
                {
                    if (smoothCams == false)
                    {
                        CamWhites.SetActive(false);
                        CamBlacks.SetActive(false);
                    }
                    else
                    {
                        CamWhites.SetActive(true);
                        CamBlacks.SetActive(true);
                    }
                    if (rotationCam)
                    {
                        RotateCam();
                    }
                } 
            }

            #region Pat par manque de pieces
            if(CheckIfNotPlayable()==true&&countingTime)
            {
                WannaTieYes3();
                NetRepet ayo = new NetRepet();
                ayo.teamId = currentTeam;
                ayo.type = 3;
                Client.Instance.SendToServer(ayo); //send to other player
            }
            #endregion

        }
        #endregion

        #region Timers

        if(countingTime==false)
        {
            TurnAfficheCanvas.SetActive(false);
        }
        //whites :
        if (TurnDisplay==0)
        {
            ResetBoard();
            TurnAfficheCanvas.SetActive(false);
            countingTime = false;
            if(localGame==false)
            {
                if(GameHost)
                {
                    NetTimer rmw = new NetTimer();
                    rmw.TeamId = currentTeam;
                    rmw.Timers = (int)startMinutes;

                    if (startMinutes - (int)startMinutes != 0)
                    {
                        rmw.half = 1;
                    }
                    else
                    {
                        rmw.half = 0;
                    }

                    Client.Instance.SendToServer(rmw);
                }
                
            }
            
        }
        if (TurnDisplay == 1)
        {
            if (removeOnce)
            {
                countingTime = true;
                if (currentTeam == 1)
                {
                    currentTimeW -= 2.2f;
                }
                removeOnce = false;
            }
        }
        if (TurnDisplay == 3&& countingTime)
        {
            if (!localGame)
            {
                AskTieButton.SetActive(true);
            }
        }
        
        if (TurnDisplay%2==0)
        {
            if(countingTime)
            {
                currentTimeW -= Time.deltaTime;
            }
            
            if(currentTimeW <= 0)
            {
                countingTime = false;
                currentTimeW = 0;
                DisplayWin(1);
                
            }
            
        }
        else
        {
            if (countingTime)
            {
                currentTimeB -= Time.deltaTime;
            }
                
            if (currentTimeB <= 0)
            {
                countingTime = false;
                currentTimeB = 0;
                DisplayWin(0);
            }
            
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTimeW);
        
        if (time.Minutes.ToString().Length<=1)
        {
            a += "0";
        }
        a += time.Minutes.ToString() + ":";
        if (time.Seconds.ToString().Length<=1)
        {
            a += "0";
        }
        a+= time.Seconds.ToString();
        currentTimeTextW.text =a;
        a = "";
        TimeSpan time2 = TimeSpan.FromSeconds(currentTimeB);
        if (time2.Minutes.ToString().Length <= 1)
        {
            b += "0";
        }
        b += time2.Minutes.ToString() + ":" ;
        if (time2.Seconds.ToString().Length <= 1)
        {
            b += "0";
        }
        b += time2.Seconds.ToString();
        currentTimeTextB.text = b;
        b = "";

        if(tie)
        {
            countingTime=false;
        }
        #endregion

        if(countingTime)
        {
            if(turnbeforeReAsk>=0)
            {
                if(TurnDisplay - turnbeforeReAsk<0)
                {
                    AskTieButton.SetActive(false);
                }
                else
                {
                    AskTieButton.SetActive(true);
                    isAllowedToAsk = true;
                }

            }
            if(TurnDisplay%2==currentTeam&& TurnDisplay > 2)
            {
                if (isAllowedToAsk)
                {
                    AskTieButton.SetActive(true);
                }
            }
            else
            {
                AskTieButton.SetActive(false);
            }
        }
        else
        {
            surrenderButton.SetActive(false);
            rot.SetActive(false);
            goback.SetActive(false);
            AskTieButton.SetActive(false);
            TurnAfficheCanvas.SetActive(false);
        }


        if (TurnDisplay > 2 && countingTime)
        {
            //if (TurnDisplay % 2 == currentTeam)
            {
                surrenderButton.SetActive(true);
            }
            //else
            {
                //surrenderButton.SetActive(false);
            }
        }
        else
        {
            surrenderButton.SetActive(false);
        }
        
        if (turn == 0)
        {
            
            goback.SetActive(false);
            rot.SetActive(false);
            daGame.SetActive(true);
            LeaveSoloButton.SetActive(false);
            #region ui postgame secure
            wannaplay.SetActive(false);
            dontwannaplay.SetActive(false);
            waitin.SetActive(false);
            goagain.SetActive(true);

            #endregion
        }
        if (turn == 1)
        {
            if (localGame)
            {
                LeaveSoloButton.SetActive(true);
            }
            else
            {
                LeaveSoloButton.SetActive(false);
            }
            TurnAfficheCanvas.SetActive(true);
            if(countingTime)
            {
                goback.SetActive(true);
            }
            
        }
        TurnAffiche.text = TurnDisplay.ToString();
        if (localGame)
        {
            if(turn>0&&countingTime)
            {
                rot.SetActive(true);
            }
            if (rotationCam==false)
            {

            }else
            {
                if (currentTeam == 1)
                {
                    lettersW.SetActive(false);
                    lettersB.SetActive(true);
                }
                else
                {
                    lettersB.SetActive(false);
                    lettersW.SetActive(true);
                }
            }
        }
        else
        {
            if (currentTeam == 1)
            {
                lettersW.SetActive(false);
                lettersB.SetActive(true);
            }
            else
            {
                lettersB.SetActive(false);
                lettersW.SetActive(true);
            }
        }
       

        if (!currentcam)
        {
            currentcam = Camera.main;
            return;
        }


        RaycastHit info;
        Ray ray = currentcam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight", "Checked")))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }


            if (Input.GetMouseButtonDown(0)) // if we are clicking the mouse bouton
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {

                    if ((chessPieces[hitPosition.x, hitPosition.y].color == 0 && IsWhiteTurn  && currentTeam==0) || (chessPieces[hitPosition.x, hitPosition.y].color == 1 && !IsWhiteTurn && currentTeam == 1))   // Is it our turn? 
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                        //get a list of where i can go and highlighting the possible positions
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);

                        //Get a list of SpecialMoves 
                        SpeMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref movelist, ref availableMoves, numberOfTilesX, numberOfTilesY);
                        PreventCheck();
                        HighlightTiles();

                    }
                }
            }
            if (currentlyDragging != null && Input.GetMouseButtonUp(0)) // if we are releasing the mouse bouton
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.CurrentX, currentlyDragging.CurrentY);
                if (ContainsValidMove(ref availableMoves, new Vector2Int(hitPosition.x, hitPosition.y)))
                {
                    


                    MoveTo(previousPosition.x, previousPosition.y, hitPosition.x, hitPosition.y);
                    turn++;
                    aled = true;
                    if (TurnDisplay % 2 == 0)
                    {
                        currentTimeW += AddTime;

                    }
                    if (TurnDisplay % 2 == 1)
                    {
                        currentTimeB += AddTime;
                    }
                    //Net implimatention
                    NetMakeMove mm = new NetMakeMove();
                    mm.originalX = previousPosition.x;
                    mm.originalY = previousPosition.y;
                    mm.destinationX = hitPosition.x;
                    mm.destinationY = hitPosition.y;
                    mm.teamId = currentTeam;
                    mm.TimeW = currentTimeW;
                    mm.TimeB = currentTimeB;
                    Client.Instance.SendToServer(mm);
                    source.PlayOneShot(clipMovePiece);

                    

                    #region GoBack
                    List<Vector2Int> movelistW = new List<Vector2Int>();
                    List<Vector2Int> movelistB = new List<Vector2Int>();
                    for (int i = 0; i < numberOfTilesX; i++)
                    {
                        for (int j = 0; j < numberOfTilesY; j++)
                        {
                            if (chessPieces[i, j] != null)
                            {
                                if (chessPieces[i, j].color == 0)
                                {
                                    movelistW.Add(new Vector2Int(i, j));
                                }

                            }


                        }
                    }
                    for (int i = 0; i < numberOfTilesX; i++)
                    {
                        for (int j = 0; j < numberOfTilesY; j++)
                        {
                            if (chessPieces[i, j] != null)
                            {
                                if (chessPieces[i, j].color == 1)
                                {
                                    movelistB.Add(new Vector2Int(i, j));
                                }

                            }


                        }
                    }
                    if (turn % 2 == 1)
                    {
                        PositionsB.Add(movelistB);

                    }
                    else
                    {
                        PositionsW.Add(movelistW);

                    }



                    #endregion
                    
                    
                    for (int i = 0; i < boards2.Count; i++)
                    {
                        int lameme = 0;
                        for (int j = 0; j < boards2.Count; j++)
                        {
                            if(Compare2ChessBoards(boards2[i], boards2[j]))
                            {
                                lameme++;
                               
                            }
                            if (lameme == 3)
                            {
                                tie=true;
                                WannaTieT();
                            }
                        }
                        
                       
                    }
                    if (TurnDisplay- LastTurnDied >= 49) //Tie if 50 turns and noone ate anything
                    {
                        WannaTieYes2();
                        NetRepet ayo = new NetRepet();
                        ayo.teamId = currentTeam;
                        ayo.type = 2;
                        Client.Instance.SendToServer(ayo); //send to other player
                        
                    }

                }
                else
                {
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                    currentlyDragging = null;
                    RemoveHighlightTiles();

                }


            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {

                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.CurrentX, currentlyDragging.CurrentY));
                currentlyDragging = null;
                RemoveHighlightTiles();

            }
        }

        // If we're dragging a piece
        if (currentlyDragging)
        {
            Plane horizonPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizonPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * 0.2f);
            }

        }
        if (turn % 2 == 1)
        {
            IsBlackTurn = !IsWhiteTurn;
        }
        if (turn % 2 == 0)
        {
            IsBlackTurn = IsWhiteTurn;
        }

        if (IsBlackTurn != IsWhiteTurn && turn > 1)
        {

            List<Vector2Int> whitemovs = new List<Vector2Int>();
            List<Vector2Int> blackmovs = new List<Vector2Int>();

            for (int i = 0; i < numberOfTilesX; i++)
            {
                for (int j = 0; j < numberOfTilesY; j++)
                {
                    if (chessPieces[i, j] != null && chessPieces[i, j].color == 0)
                    {
                        List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                        for (int d = 0; d < b.Count; d++)
                        {
                            whitemovs.Add(b[d]);
                        }
                    }
                    if (chessPieces[i, j] != null && chessPieces[i, j].color == 1)
                    {
                        List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                        for (int d = 0; d < b.Count; d++)
                        {
                            blackmovs.Add(b[d]);
                        }
                    }
                }
            }
            {
                for (int i = 0; i < numberOfTilesX; i++) //Check for tie 
                {
                    for (int j = 0; j < numberOfTilesY; j++)
                    {

                        if (chessPieces[i, j] != null && chessPieces[i, j].color == 0) //whitecheck 
                        {

                            if (chessPieces[i, j].type == ChessPieceType.King)
                            {
                                List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                                bool res = true;
                                for (int s = 0; s < blackmovs.Count; s++)
                                {
                                    if (blackmovs[s].x == i && blackmovs[s].y == j)
                                    {
                                        res = false;
                                    }
                                }

                                if (IsKingPat2(chessPieces[i, j], 0, b) && res == true)
                                {
                                    if (tie == true)
                                    {
                                        PAT();
                                    }

                                }
                            }
                        }
                    }
                }

            }

            {
                for (int i = 0; i < numberOfTilesX; i++) //Check for tie 
                {
                    for (int j = 0; j < numberOfTilesY; j++)
                    {
                        if (chessPieces[i, j] != null && chessPieces[i, j].color == 1) //blackcheck 
                        {

                            if (chessPieces[i, j].type == ChessPieceType.King)
                            {
                                List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                                bool res = true;
                                for (int s = 0; s < whitemovs.Count; s++)
                                {
                                    if (whitemovs[s].x == i && whitemovs[s].y == j)
                                    {
                                        res = false;
                                    }
                                }

                                if (IsKingPat2(chessPieces[i, j], 1, b) && res == true)
                                {
                                    if (tie == true)
                                    {
                                        PAT();
                                    }

                                }
                            }
                        }
                    }
                }

            }
        }



    }


    public void WannaTieF()
    {
        DoUWantToTie.SetActive(false);
        tie=false;
        boards2.Clear();
    }
    public void WannaTieT()
    {
        tie = true;
        DoUWantToTie.SetActive(true);
        
    }
    public void WannaTieYes() //sameBoards
    {
        SameBoards.SetActive(true);
        ResetBoard();
        source.PlayOneShot(clipPromote);
        DoUWantToTie.SetActive(false);
        countingTime = false;
        VictoryScreen.SetActive(true);
        Tie.SetActive(true);
        Checkmate.SetActive(false);
        GameUI.Instance.ChangeCamera(CameraAngle.EndGame);
        TurnAfficheCanvas.SetActive(false);
        goback.SetActive(false);
        rot.SetActive(false);
        surrenderButton.SetActive(false);

    }
    public void WannaTieYes2() //50 moves no kills
    {
        MovesNoKill.SetActive(true);
        ResetBoard();
        source.PlayOneShot(clipPromote);
        DoUWantToTie.SetActive(false);
        countingTime = false;
        VictoryScreen.SetActive(true);
        Tie.SetActive(true);
        Checkmate.SetActive(false);
        GameUI.Instance.ChangeCamera(CameraAngle.EndGame);
        TurnAfficheCanvas.SetActive(false);
        goback.SetActive(false);
        rot.SetActive(false);
        surrenderButton.SetActive(false);

    }
    public void WannaTieYes3() //Not Enough Pieces
    {
        NotEnoughPieces.SetActive(true);
        ResetBoard();
        source.PlayOneShot(clipPromote);
        DoUWantToTie.SetActive(false);
        countingTime = false;
        VictoryScreen.SetActive(true);
        Tie.SetActive(true);
        Checkmate.SetActive(false);
        GameUI.Instance.ChangeCamera(CameraAngle.EndGame);
        TurnAfficheCanvas.SetActive(false);
        goback.SetActive(false);
        rot.SetActive(false);
        surrenderButton.SetActive(false);

    }
    public void WannaTieYes4() //Not Enough Pieces
    {
        AgreeTie.SetActive(true);
        ResetBoard();
        source.PlayOneShot(clipPromote);
        DoUWantToTie.SetActive(false);
        countingTime = false;
        VictoryScreen.SetActive(true);
        Tie.SetActive(true);
        Checkmate.SetActive(false);
        GameUI.Instance.ChangeCamera(CameraAngle.EndGame);
        TurnAfficheCanvas.SetActive(false);
        goback.SetActive(false);
        rot.SetActive(false);
        surrenderButton.SetActive(false);
        AskTieRequest.SetActive(false);
        RequestSent.SetActive(false);
        AskTieButton.SetActive(false);

}
    public void SendTieToOther()
    {
        NetRepet ayo = new NetRepet();
        ayo.teamId = currentTeam;
        ayo.type = 1;
        Client.Instance.SendToServer(ayo);
    }
    public void SendAgreedTieToOther()
    {
        NetRepet ayo = new NetRepet();
        ayo.teamId = currentTeam;
        ayo.type = 4;
        Client.Instance.SendToServer(ayo);
    }
    public void AskForTie()
    {
        NetAskPat ayo = new NetAskPat();
        ayo.teamId = currentTeam;
        Client.Instance.SendToServer(ayo);
        DisplayRequestTieSent();
        turnbeforeReAsk = TurnDisplay + 4;
        isAllowedToAsk = false;

    }
    private void DisplayAskTie()
    {
        AskTieRequest.SetActive(true);
    }
    public void NotDisplayAskTie()
    {
        AskTieRequest.SetActive(false);
        NetRepet ayo = new NetRepet();
        ayo.teamId = currentTeam;
        ayo.type = 5;
        Client.Instance.SendToServer(ayo);
    }
    private void DisplayRequestTieSent()
    {
        RequestSent.SetActive(true);
        Invoke("NotDisplayRequestTieSent", 2);

    }
    private void NotDisplayRequestTieSent()
    {
        RequestSent.SetActive(false);
        
    }
    private void DisplayNoTieAccept()
    {
        RequestSent.SetActive(false);
        DisagreeTie.SetActive(true);
        Invoke("NoDisplayNoTieAccept", 2);
    }
    private void NoDisplayNoTieAccept()
    {
        DisagreeTie.SetActive(false);
    }
    private bool CheckIfNotPlayable()
    {
        int cptpieces = 0;
        List<ChessPiece> pieceTypes = new List<ChessPiece>();
        bool res =false;
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i,j]!=null)
                {
                    cptpieces++;
                    pieceTypes.Add(chessPieces[i, j]);
                }
            }
        }
        if(cptpieces==2)
        {
            res = true;
        }
        if (cptpieces == 3)
        {
            for (int i = 0; i < pieceTypes.Count; i++)
            {
                if (pieceTypes[i].type!=ChessPieceType.King)
                {
                    if (pieceTypes[i].type == ChessPieceType.Knight || pieceTypes[i].type == ChessPieceType.Bishop)
                    {
                        res = true;
                    }
                }
            }
        }
        if (cptpieces == 4)
        {
            int chev = -1;
            int fou = -1;
            for (int i = 0; i < pieceTypes.Count; i++)
            {
                if (pieceTypes[i].type != ChessPieceType.King)
                {
                    if (pieceTypes[i].type == ChessPieceType.Knight)
                    {
                        chev = i;
                    }
                    if (pieceTypes[i].type == ChessPieceType.Bishop)
                    {
                        fou = i;
                    }
                }
            }
            if(fou!=-1)
            {
                if(chev!=-1)
                {
                    if(pieceTypes[chev].color != pieceTypes[fou].color)
                    {
                        res = true;
                    }
                }
            }
            else
            {
                if (chev != -1)
                {
                    res = true;
                }
            }
                    

        }
        return res;
    }

    private bool Compare2ChessBoards(ChessPiece[,]a , ChessPiece[,] b)
    {
        bool res = true;
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if((a[i, j] == null && b[i,j]!=null)|| (a[i, j] != null && b[i, j] == null))
                {
                    res = false;
                }
                if(a[i, j] != null && b[i, j] != null)
                {
                    if (a[i, j].type != b[i, j].type || a[i, j].color != b[i, j].color)
                    {
                        res = false;
                    }
                }
                
            }
        }

        return res;
    }
    private void AddToBoards()
    {
        ChessPiece[,] a = new ChessPiece[numberOfTilesX, numberOfTilesY];

        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                a[i, j] = chessPieces[i, j];
            }
        }
        boards.Add(a);
        boards2.Add(a);
    }
    public void AdjustTime(float NewTime)
    {
        int val =(int)NewTime;
        if(NewTime-val<0.5)
        {
            startMinutes = val;
            text.text = val.ToString() + ":00";
            text2.text = val.ToString() + ":00";




        }
        if(NewTime - val > 0.5&& NewTime - val < 0.99)
        {
            startMinutes = val+0.5f;
            text.text = val.ToString() + ":30";
            text2.text = val.ToString() + ":30";

        }
        currentTimeB = startMinutes * 60;

        currentTimeW = startMinutes * 60;
        
        
        
    }

    public void smoothCamsT()
    {
        smoothCams = true;

    }
    public void smoothCamsF()
    {
        smoothCams = false;
    }
    public void TimerbackToOneForMulti()
    {
        startMinutes = 1;
        Slider.value = 1;
        Slider2.value = 1;
        text2.text = startMinutes.ToString() + ":00";
        text.text = startMinutes.ToString() + ":00";
    }
    public void StartTimer()
    {
        countingTime = true;
    }
    public void StopTimer()
    {
        countingTime = false;
    }

    public void gameHostFalse()
    {
        GameHost = false;
    }
    public void gameHostTrue()
    {
        GameHost = true;
    }

    private void ReceiveTimers(float value)
    {
        startMinutes = value;
        currentTimeB = startMinutes * 60;

        currentTimeW = startMinutes * 60;

    }
    #region BoardGeneration
    private void GenerateAllTiles(float tilesize, int Xtile, int Ytile)
    {
        yOffset += transform.position.y;
        bounds = new Vector3(-3.5f + (numberOfTilesX / 2) * tileSize, 0.3f, -3.5f + ((numberOfTilesX / 2) * tileSize));
        tiles = new GameObject[numberOfTilesX, numberOfTilesY];
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                tiles[i, j] = GenerateATile(tilesize, i, j);
            }
        }

    }
    private GameObject GenerateATile(float tilesize, int xtile, int ytile)
    {
        GameObject tileobject = new GameObject(string.Format("X:{0}, Y:{1}", xtile, ytile));
        tileobject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileobject.AddComponent<MeshFilter>().mesh = mesh;
        tileobject.AddComponent<MeshRenderer>().material = tileMat;


        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(xtile * tilesize, yOffset, ytile * tilesize) - bounds;
        vertices[1] = new Vector3(xtile * tilesize, yOffset, (ytile + 1) * tilesize) - bounds;
        vertices[2] = new Vector3((xtile + 1) * tilesize, yOffset, ytile * tilesize) - bounds;
        vertices[3] = new Vector3((xtile + 1) * tilesize, yOffset, (ytile + 1) * tilesize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileobject.layer = LayerMask.NameToLayer("Tile");
        tileobject.AddComponent<BoxCollider>();

        return tileobject;
    }
    #endregion

    #region PieceSpawn

    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[numberOfTilesX, numberOfTilesY];

        int whiteTeam = 0;
        int blackTeam = 1;

        //White team 
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < numberOfTilesX; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        }


        //Black team 

        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < numberOfTilesX; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }


    }

    private ChessPiece SpawnSinglePiece(ChessPieceType type, int color)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.color = color;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[color];
        return cp;
    }



    #endregion

    #region Positioning

    private void PositionAllPieces()
    {
        for (int x = 0; x < numberOfTilesX; x++)
        {
            for (int y = 0; y < numberOfTilesY; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    PositionSinglePiece(x, y, true);
                }
            }
        }
    }

    private void PositionSinglePiece(int x, int y, bool force = false) // force = false : smooth movement | force = true : pop in spot
    {
        chessPieces[x, y].CurrentX = x;
        chessPieces[x, y].CurrentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    #endregion

    #region HighlightTiles

    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }

    #endregion

    #region Operation 

    private Vector2Int LookupTileIndex(GameObject hit)
    {
        Vector2Int a = new Vector2Int();
        a = -Vector2Int.one; // invalid

        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (tiles[i, j] == hit)
                {
                    a = new Vector2Int(i, j);
                }
            }
        }
        return a;
    }
   
    private void MoveTo(int OriginalX,int OriginalY, int x, int y)
    {
        {
            ChessPiece cp = chessPieces[OriginalX, OriginalY];
            Vector2Int previousPosition = new Vector2Int(OriginalX, OriginalY);

            if (chessPieces[x, y] != null) // Is there another piece on the location we are going 
            {
                ChessPiece ocp = chessPieces[x, y];

                if (cp.color == ocp.color) //if same color
                {
                    return;
                }

                // If different Color
                if (ocp.color == 0)
                {

                    if (ocp.type == ChessPieceType.King)
                    {
                        CheckMate(1);
                    }

                    deadWhite.Add(ocp);
                    ocp.SetScale(Vector3.one * DeathSize);
                    ocp.SetPosition(
                        new Vector3(-1 * tileSize, yOffset, 8 * tileSize)
                        - bounds
                        + new Vector3(tileSize / 2, 0.1f, tileSize / 2)
                        + (Vector3.back * deathSpacing) * deadWhite.Count);
                    corpses.Add(ocp.gameObject);
                    dedwhites.Add(new Vector2Int(x, y));
                    LastTurnDied = TurnDisplay;
}
                else
                {
                    if (ocp.type == ChessPieceType.King)
                    {
                        CheckMate(0);
                    }
                    deadBlack.Add(ocp);
                    ocp.SetScale(Vector3.one * DeathSize);
                    ocp.SetPosition(
                        new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                        - bounds
                        + new Vector3(tileSize / 2, 0.1f, tileSize / 2)
                        + (Vector3.forward * deathSpacing) * deadBlack.Count);
                    corpses.Add(ocp.gameObject);
                    dedblacks.Add(new Vector2Int(x, y));
                    LastTurnDied = TurnDisplay;
                }

            }

            chessPieces[x, y] = cp;
            chessPieces[previousPosition.x, previousPosition.y] = null;
            PositionSinglePiece(x, y);


            IsWhiteTurn = !IsWhiteTurn;
            if(localGame)
            {
                currentTeam = (currentTeam == 0) ? 1 : 0;
                if(rotationCam)
                {
                    GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.whiteTeam : CameraAngle.BlackTeam);
                }
                
            }
            movelist.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });
            if (cp.type == ChessPieceType.Pawn && cp.color == 0 && cp.CurrentY == 7)
            {
                if (chessPieces[cp.CurrentX, cp.CurrentY].type == ChessPieceType.Pawn && chessPieces[cp.CurrentX, cp.CurrentY].color == 0)
                {
                    chessPieces[cp.CurrentX, cp.CurrentY].Promoted = true;
                }
                if (localGame)
                {
                    Upgrade(0, cp.CurrentX, cp.CurrentY);
                }
                else
                {
                    if (currentTeam == 0)
                    {
                        Upgrade(0, cp.CurrentX, cp.CurrentY);
                    }else
                    {

                    }
                }
                int[] addup = { turn, previousPosition.x, previousPosition.y, x, y };
                UpgradeW.Add(addup);

            }
            if (cp.type == ChessPieceType.Pawn && cp.color == 1 && cp.CurrentY == 0)
            {
                if (chessPieces[cp.CurrentX, cp.CurrentY].type == ChessPieceType.Pawn && chessPieces[cp.CurrentX, cp.CurrentY].color == 1)
                {
                    chessPieces[cp.CurrentX, cp.CurrentY].Promoted = true;
                }

                if (localGame)
                {
                    Upgrade(1, cp.CurrentX, cp.CurrentY);
                }
                else
                {
                    if (currentTeam == 1)
                    {
                        Upgrade(1, cp.CurrentX, cp.CurrentY);
                    }
                }

                int[] addup = { turn, previousPosition.x, previousPosition.y, x, y };
                UpgradeB.Add(addup);

            }
            ProcessSpecialMove();

            if(currentlyDragging!=null)
            {
                currentlyDragging = null;
            }
            RemoveHighlightTiles();

            if (CheckForCheckmate() == 1)
            {
                CheckMate(cp.color);
            }
            if (CheckForCheckmate() == 0)
            {
                PAT();
            }
            AddToBoards();
            
        }

        

        return;
    }
    private void RotateCam()
    {
        if (turn % 2 == 0)
        {

            currentcam.transform.SetPositionAndRotation(new Vector3(3, 8, -5), Quaternion.Euler(45, 0, 0));
        }
        else
        {


            currentcam.transform.SetPositionAndRotation(new Vector3(4, 8, 12), Quaternion.Euler(-225, 0, 180));
        }
    }
    public void SaveBoard()
    {
        if (turn > 1)
        {
            for (int i = 0; i < numberOfTilesX; i++)
            {
                for (int j = 0; j < numberOfTilesY; j++)
                {
                    chessPiecesBefore[i, j] = chessPieces[i, j];
                }
            }
        }

    }
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos)
    {
        bool res = false;
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                res = true;
            }
        }
        return res;
    }



    #endregion
    #region SpecialMove
    private void ProcessSpecialMove()
    {
        if (SpeMove == SpecialMove.EnPassant)
        {
            var newmove = movelist[movelist.Count - 1];
            ChessPiece Mypawn = chessPieces[newmove[1].x, newmove[1].y];
            var targetPawn = movelist[movelist.Count - 2];
            ChessPiece enemypawn = chessPieces[targetPawn[1].x, targetPawn[1].y];

            if (Mypawn.CurrentX == enemypawn.CurrentX) // check if both pawns are one the same line
            {
                if (Mypawn.CurrentY == enemypawn.CurrentY - 1 || Mypawn.CurrentY == enemypawn.CurrentY + 1)  // check if my piece is in front (or behind) the other pawn
                {
                    if (enemypawn.color == 0)
                    {
                        deadWhite.Add(enemypawn);
                        enemypawn.SetScale(Vector3.one * DeathSize);
                        enemypawn.SetPosition(
                            new Vector3(-1 * tileSize, yOffset, 8 * tileSize)
                            - bounds
                            + new Vector3(tileSize / 2, 0.1f, tileSize / 2)
                            + (Vector3.back * deathSpacing) * deadWhite.Count);
                        corpses.Add(enemypawn.gameObject);

                    }
                    else
                    {
                        deadBlack.Add(enemypawn);
                        enemypawn.SetScale(Vector3.one * DeathSize);
                        enemypawn.SetPosition(
                            new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                            - bounds
                            + new Vector3(tileSize / 2, 0.1f, tileSize / 2)
                            + (Vector3.forward * deathSpacing) * deadBlack.Count);
                        corpses.Add(enemypawn.gameObject);

                    }
                    chessPieces[enemypawn.CurrentX, enemypawn.CurrentY] = null;
                }

            }
        }
        if (SpeMove == SpecialMove.Casteling)
        {
            Vector2Int[] lastmove = movelist[movelist.Count - 1];

            if (lastmove[1].x == 2) // leftside casteling
            {
                if (lastmove[1].y == 0) //whiteSide
                {
                    ChessPiece rook = chessPieces[0, 0];
                    chessPieces[3, 0] = rook;
                    PositionSinglePiece(3, 0);
                    chessPieces[0, 0] = null;
                }
                else if (lastmove[1].y == 7) // blackSide
                {
                    ChessPiece rook = chessPieces[0, 7];
                    chessPieces[3, 7] = rook;
                    PositionSinglePiece(3, 7);
                    chessPieces[0, 7] = null;
                }
            }
            if (lastmove[1].x == 6) // Rightside casteling
            {
                if (lastmove[1].y == 0) //whiteSide
                {
                    ChessPiece rook = chessPieces[7, 0];
                    chessPieces[5, 0] = rook;
                    PositionSinglePiece(5, 0);
                    chessPieces[7, 0] = null;
                }
                else if (lastmove[1].y == 7) // blackSide
                {
                    ChessPiece rook = chessPieces[7, 7];
                    chessPieces[5, 7] = rook;
                    PositionSinglePiece(5, 7);
                    chessPieces[7, 7] = null;
                }
            }
        }
    }

    private void PreventCheck()
    {
        ChessPiece tagetKing = null;
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    if (chessPieces[i, j].type == ChessPieceType.King)
                    {
                        if (chessPieces[i, j].color == currentlyDragging.color)
                        {
                            tagetKing = chessPieces[i, j];
                        }

                    }
                }

            }
        }
        SimulateMoveSinglePiece(currentlyDragging, ref availableMoves, tagetKing);

    }
    private void SimulateMoveSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece TargetKing)  //remove the moves that can cause a check to urself
    {

        // Save values for later
        int actualX = cp.CurrentX;
        int actualY = cp.CurrentY;
        List<Vector2Int> removemoves = new List<Vector2Int>();

        //Check if moves put us in check

        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int KingPositioninCurrentSimulation = new Vector2Int(TargetKing.CurrentX, TargetKing.CurrentY);
            if (cp.type == ChessPieceType.King) //are we moving the king ?
            {
                KingPositioninCurrentSimulation = new Vector2Int(simX, simY);
            }

            ChessPiece[,] simulation = new ChessPiece[numberOfTilesX, numberOfTilesY];
            List<ChessPiece> attackers = new List<ChessPiece>();
            for (int x = 0; x < numberOfTilesX; x++)  //Copy without reference so that i can change safely the copy
            {
                for (int y = 0; y < numberOfTilesY; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        simulation[x, y] = chessPieces[x, y];
                        if (simulation[x, y].color != cp.color) //save all possible enemies
                        {
                            attackers.Add(simulation[x, y]);
                        }
                    }
                }
            }

            //Simulate the move on the simulation board
            simulation[actualX, actualY] = null;
            cp.CurrentX = simX;
            cp.CurrentY = simY;
            simulation[simX, simY] = cp;

            //did we take down a piece during the simulation?
            var dedpiece = attackers.Find(c => c.CurrentX == simX && c.CurrentY == simY);
            if (dedpiece != null)
            {
                attackers.Remove(dedpiece);

            }

            //Get All the possible moves of the attackers
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int a = 0; a < attackers.Count; a++)
            {
                var pieceMoves = attackers[a].GetAvailableMoves(ref simulation, numberOfTilesX, numberOfTilesY); // Get all the possible moves of the piece to see if any move attacks the king
                for (int b = 0; b < pieceMoves.Count; b++) //add to all the enemy pieces moves
                {
                    simMoves.Add(pieceMoves[b]);
                }
            }

            // Check if the king is in Check

            if (ContainsValidMove(ref simMoves, KingPositioninCurrentSimulation)) // we check if one of the positions in the simMoves attacks the king position
            {
                removemoves.Add(moves[i]);

            }

            //Restore the data for next loop
            cp.CurrentX = actualX;
            cp.CurrentY = actualY;

        }

        //Remove the problematic moves from movelist

        for (int i = 0; i < removemoves.Count; i++)
        {
            moves.Remove(removemoves[i]);
        }

    }



    #region Upgrade

    private void Upgrade(int color, int x, int y)
    {

        Evolve.SetActive(true);
        SaveColor = chessPieces[x, y].color;
        EvoX = x;
        if (SaveColor == 0)
        {
            EvoY = 7;
        }
        else
        {
            EvoY = 0;
        }

        Destroy(chessPieces[x, y].gameObject);

        if (color == 1)
        {
            Q1.SetActive(true);
            R1.SetActive(true);
            K1.SetActive(true);
            B1.SetActive(true);
            Q2.SetActive(false);
            R2.SetActive(false);
            K2.SetActive(false);
            B2.SetActive(false);

        }
        else
        {
            Q1.SetActive(false);
            R1.SetActive(false);
            K1.SetActive(false);
            B1.SetActive(false);
            Q2.SetActive(true);
            R2.SetActive(true);
            K2.SetActive(true);
            B2.SetActive(true);
        }



    }
    public void EvolveQ()
    {
        Q2.SetActive(false);
        R2.SetActive(false);
        K2.SetActive(false);
        B2.SetActive(false);
        Q1.SetActive(false);
        R1.SetActive(false);
        K1.SetActive(false);
        B1.SetActive(false);
        Evolve.SetActive(false);

        int aa = EvoX;
        int bb = EvoY;
        if (chessPieces[aa, bb] == null)
        {
            chessPieces[aa, bb] = SpawnSinglePiece(ChessPieceType.Queen, SaveColor);
        }
        PositionSinglePiece(aa, bb, true);
        if (CheckForCheckmate() == 1)
        {
            CheckMate(SaveColor);
        }
        source.PlayOneShot(clipPromote);

        NetUpgrade rmw = new NetUpgrade();
        rmw.type = 1;
        rmw.TeamId = SaveColor;
        rmw.destinationX = aa;
        rmw.destinationY = bb;
        Client.Instance.SendToServer(rmw);

    }
    public void EvolveB()
    {
        Q2.SetActive(false);
        R2.SetActive(false);
        K2.SetActive(false);
        B2.SetActive(false);
        Q1.SetActive(false);
        R1.SetActive(false);
        K1.SetActive(false);
        B1.SetActive(false);
        Evolve.SetActive(false);

        int aa = EvoX;
        int bb = EvoY;
        if (chessPieces[aa, bb] == null)
        {
            chessPieces[aa, bb] = SpawnSinglePiece(ChessPieceType.Bishop, SaveColor);
        }
        PositionSinglePiece(aa, bb, true);
        if (CheckForCheckmate() == 1)
        {
            CheckMate(SaveColor);
        }
        source.PlayOneShot(clipPromote);
        NetUpgrade rmw = new NetUpgrade();
        rmw.type = 2;
        rmw.TeamId = SaveColor;
        rmw.destinationX = aa;
        rmw.destinationY = bb;
        Client.Instance.SendToServer(rmw);


    }

    public void EvolveK()
    {
        Q2.SetActive(false);
        R2.SetActive(false);
        K2.SetActive(false);
        B2.SetActive(false);
        Q1.SetActive(false);
        R1.SetActive(false);
        K1.SetActive(false);
        B1.SetActive(false);
        Evolve.SetActive(false);

        int aa = EvoX;
        int bb = EvoY;
        if (chessPieces[aa, bb] == null)
        {
            chessPieces[aa, bb] = SpawnSinglePiece(ChessPieceType.Knight, SaveColor);
        }
        PositionSinglePiece(aa, bb, true);
        if (CheckForCheckmate() == 1)
        {
            CheckMate(SaveColor);
        }
        NetUpgrade rmw = new NetUpgrade();
        rmw.type = 3;
        rmw.TeamId = SaveColor;
        rmw.destinationX = aa;
        rmw.destinationY = bb;
        Client.Instance.SendToServer(rmw);
        source.PlayOneShot(clipPromote);
    }
    public void EvolveR()
    {
        Q2.SetActive(false);
        R2.SetActive(false);
        K2.SetActive(false);
        B2.SetActive(false);
        Q1.SetActive(false);
        R1.SetActive(false);
        K1.SetActive(false);
        B1.SetActive(false);
        Evolve.SetActive(false);

        int aa = EvoX;
        int bb = EvoY;
        if (chessPieces[aa, bb] == null)
        {
            chessPieces[aa, bb] = SpawnSinglePiece(ChessPieceType.Rook, SaveColor);
        }
        PositionSinglePiece(aa, bb, true);
        if (CheckForCheckmate() == 1)
        {
            CheckMate(SaveColor);
        }
        source.PlayOneShot(clipPromote);
        NetUpgrade rmw = new NetUpgrade();
        rmw.type = 4;
        rmw.TeamId = SaveColor;
        rmw.destinationX = aa;
        rmw.destinationY = bb;
        Client.Instance.SendToServer(rmw);
    }

    public void EndEvolve()
    {

        Evolve.SetActive(false);
    }

    #endregion


    #endregion

    #region CheckMate

    private bool IsKingPat2(ChessPiece cp, int color, List<Vector2Int> kingmoves)
    {
        List<Vector2Int> removemoves = new List<Vector2Int>();
        int actualX = cp.CurrentX;
        int actualY = cp.CurrentY;
        int cpt = 0;
        List<ChessPiece> defenders = new List<ChessPiece>();
        List<Vector2Int> defpiecemoves = new List<Vector2Int>();
        for (int i = 0; i < kingmoves.Count; i++)
        {

            int simX = kingmoves[i].x;
            int simY = kingmoves[i].y;

            Vector2Int KingPositioninCurrentSimulation = new Vector2Int(simX, simY);


            ChessPiece[,] simulation = new ChessPiece[numberOfTilesX, numberOfTilesY];
            List<ChessPiece> attackers = new List<ChessPiece>();
            for (int x = 0; x < numberOfTilesX; x++)  //Copy without reference so that i can change safely the copy
            {
                for (int y = 0; y < numberOfTilesY; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        simulation[x, y] = chessPieces[x, y];
                        if (simulation[x, y].color != color) //save all possible enemies
                        {
                            attackers.Add(simulation[x, y]);
                        }
                        if (simulation[x, y].color == color && simulation[x, y].type != ChessPieceType.King) //save all possible enemies
                        {
                            defenders.Add(simulation[x, y]);
                        }

                    }
                }
            }

            //Simulate the move on the simulation board
            simulation[actualX, actualY] = null;
            cp.CurrentX = simX;
            cp.CurrentY = simY;
            simulation[simX, simY] = cp;

            //did we take down a piece during the simulation?
            var dedpiece = attackers.Find(c => c.CurrentX == simX && c.CurrentY == simY);
            if (dedpiece != null)
            {
                attackers.Remove(dedpiece);

            }

            //Get All the possible moves of the attackers
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int a = 0; a < attackers.Count; a++)
            {
                var pieceMoves = attackers[a].GetAvailableMoves(ref simulation, numberOfTilesX, numberOfTilesY); // Get all the possible moves of the piece to see if any move attacks the king
                for (int b = 0; b < pieceMoves.Count; b++) //add to all the enemy pieces moves
                {
                    simMoves.Add(pieceMoves[b]);
                }
            }
            //Get All the possible moves of the deffenders

            for (int a = 0; a < defenders.Count; a++)
            {
                var pieceMoves = defenders[a].GetAvailableMoves(ref simulation, numberOfTilesX, numberOfTilesY); // Get all the possible moves of the piece to see if any move attacks the king
                for (int b = 0; b < pieceMoves.Count; b++) //add to all the enemy pieces moves
                {
                    defpiecemoves.Add(pieceMoves[b]);
                }
            }


            // Check if the king is in Check

            if (ContainsValidMove(ref simMoves, KingPositioninCurrentSimulation)) // we check if one of the positions in the simMoves attacks the king position
            {
                removemoves.Add(kingmoves[i]);
                cpt++;
            }


            //Restore the data for next loop
            cp.CurrentX = actualX;
            cp.CurrentY = actualY;

        }
        //Remove the problematic moves from movelist
        string s = "";
        for (int i = 0; i < removemoves.Count; i++)
        {
            kingmoves.Remove(removemoves[i]);
        }
        for (int i = 0; i < defpiecemoves.Count; i++)
        {
            s += "( " + defpiecemoves[i].x + "," + defpiecemoves[i].y + " )    ";
        }
        bool rep = false;



        List<Vector2Int> opponentmoves = new List<Vector2Int>();
        bool possible = true;
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    if (chessPieces[i, j].color != cp.color)
                    {
                        List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                        for (int ss = 0; ss < b.Count; ss++)
                        {
                            opponentmoves.Add(b[ss]);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < opponentmoves.Count; i++)
        {
            if (opponentmoves[i].x == cp.CurrentX && opponentmoves[i].y == cp.CurrentY)
            {

                possible = false;
            }

        }

        


        if (kingmoves.Count == 0 && defpiecemoves.Count == 0 && turn > 0 && possible && cpt > 0)
        {

            rep = true;
            tie = true;
            
        }
        else
        {
            tie = false;
        }
        return rep;

    }

    private int CheckForCheckmate()
    {


        var lastmove = movelist[movelist.Count - 1];
        int targetTeam = (chessPieces[lastmove[1].x, lastmove[1].y]).color;
        if (targetTeam == 0) //get the oposite color
        {
            targetTeam = 1;
        }
        else
        {
            targetTeam = 0;
        }



        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();
        ChessPiece tagetKing = null;
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    if (chessPieces[i, j].color == targetTeam)
                    {
                        defendingPieces.Add(chessPieces[i, j]);
                        if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            tagetKing = chessPieces[i, j];
                        }
                    }
                    else
                    {
                        attackingPieces.Add(chessPieces[i, j]);
                    }
                }

            }
        }

        //Is the king attacked Rn?

        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
        for (int i = 0; i < attackingPieces.Count; i++)
        {
            var PieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
            for (int a = 0; a < PieceMoves.Count; a++)
            {
                currentAvailableMoves.Add(PieceMoves[a]);
            }
        }
        // Are We In Check Rn?
        if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(tagetKing.CurrentX, tagetKing.CurrentY)))
        {
            tiles[tagetKing.CurrentX, tagetKing.CurrentY].layer = LayerMask.NameToLayer("Checked");
            source.PlayOneShot(clipCheck);

            // King is Under Attack, can we block the attack by moving?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                SimulateMoveSinglePiece(defendingPieces[i], ref defendingMoves, tagetKing); //This will remove all the moves that are not defending the king from the attack for every piece

                if (defendingMoves.Count != 0)
                {
                    return -1; // there is a possible way to remove checkmate
                }
            }
            return 1; // there is no possible way to remove the check

        }
        if (currentAvailableMoves.Count == 0 && defendingPieces.Count == 1) // Pas en echec mais plus de mouvement possible (PAT)
        {
            return 0;
        }
        return 3;

    }
    private void CheckMate(int color)
    {
        
        TurnAfficheCanvas.SetActive(false);

        source.PlayOneShot(clipCheckMate);
        
        DisplayWin(color);
        
    }
    private void DisplayWin(int winer)
    {
        countingTime = false;
        GameUI.Instance.ChangeCamera(CameraAngle.EndGame);
        Tie.SetActive(false);
        Checkmate.SetActive(true);
        VictoryScreen.SetActive(true);
        if (winer == 0)
        {
            WhiteWin.SetActive(true);
            BlackWin.SetActive(false);
        }
        else
        {
            WhiteWin.SetActive(false);
            BlackWin.SetActive(true);
        }
        goback.SetActive(false);
        rot.SetActive(false);
        surrenderButton.SetActive(false);

        if (!localGame)
        {
            if (currentTeam == winer)
            {
                playfabManager.SendLeaderBoard(1);
            }

        }
    }
   
    
    public void FFWhite()
    {
        
        NetFF rmw = new NetFF();
        rmw.TeamId = currentTeam;
        Client.Instance.SendToServer(rmw);

        
    }
    public void FFBlack()
    {
        
        DisplayWin(1);

        abandon.SetActive(true);
        Checkmate.SetActive(false);
        


    }
    public void PAT()
    {
        
        if (tie)
        {
            countingTime = false;
            VictoryScreen.SetActive(true);
            Tie.SetActive(true);
            Checkmate.SetActive(false);
            GameUI.Instance.ChangeCamera(CameraAngle.EndGame);
            TurnAfficheCanvas.SetActive(false);
            goback.SetActive(false);
            rot.SetActive(false);
            surrenderButton.SetActive(false);
        }


    }

    
    public void OnRematch()
    {

        tie=false;
        NotEnoughPieces.SetActive(false);
        SameBoards.SetActive(false);
        MovesNoKill.SetActive(false);
        turnbeforeReAsk = -1;
        isAllowedToAsk = true;

        AskTieRequest.SetActive(false);
        RequestSent.SetActive(false);
        AskTieButton.SetActive(false);
        AgreeTie.SetActive(false);
        DisagreeTie.SetActive(false);

        ResetBoard();
        LastTurnDied = 0;
        currentTimeB = startMinutes * 60;
        currentTimeW = startMinutes * 60;
        TurnAfficheCanvas.SetActive(true);
        if (localGame)
        {
            
            LeaveSoloButton.SetActive(true);
            
            currentTeam = 0;
            int saveturns = TurnDisplay;
            NetRematch rmw = new NetRematch();
            rmw.teamId = 0;
            rmw.wantRematch = 1;
            Client.Instance.SendToServer(rmw);

            NetRematch rmb = new NetRematch();
            rmb.teamId = 1;
            rmb.wantRematch = 1;
            Client.Instance.SendToServer(rmb);
            
            rotationCam = true;

            
        }
        else
        {
            LeaveSoloButton.SetActive(false);
            NetRematch rm = new NetRematch();
            rm.teamId = currentTeam;
            rm.wantRematch = 1;
            Client.Instance.SendToServer(rm);

        }
        

    }
    
    public void ResetBoard()
    {
        for (int i = 0; i < boards.Count; i++)
        {
            boards.Remove(boards[i]);
            
        }
        for (int i = 0; i < boards2.Count; i++)
        {
            boards2.Remove(boards2[i]);

        }
    }
    public void GameReset()
    {
        
        LastTurnDied = 0;
        countingTime = false;
        // reset UI
        daGame.SetActive(true);
        wannaplay.SetActive(false);
        dontwannaplay.SetActive(false);
        goagain.SetActive(true);
        waitin.SetActive(false);
        NotEnoughPieces.SetActive(false);
        SameBoards.SetActive(false);
        MovesNoKill.SetActive(false);
        turnbeforeReAsk = -1;
        isAllowedToAsk = true;

        RematchButton.interactable = true;
        rotationCam = true;
        BlackWin.SetActive(false);
        WhiteWin.SetActive(false);
        abandon.SetActive(false);
        VictoryScreen.SetActive(false);
        Tie.SetActive(false);
        AskTieRequest.SetActive(false);
        RequestSent.SetActive(false);
        AskTieButton.SetActive(false);
        AgreeTie.SetActive(false);
        DisagreeTie.SetActive(false);

        tie = false;
        ResetBoard();
        // CleanUp

        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    Destroy(chessPieces[i, j].gameObject);
                }
                chessPieces[i, j] = null;
            }
        }
        for (int i = 0; i < deadWhite.Count; i++)
        {
            Destroy(deadWhite[i].gameObject);
        }
        for (int i = 0; i < deadBlack.Count; i++)
        {
            Destroy(deadBlack[i].gameObject);
        }
        deadBlack.Clear();
        deadWhite.Clear();

        //Fields reset

        currentlyDragging = null;
        availableMoves.Clear();
        SpawnAllPieces();
        PositionAllPieces();
        movelist.Clear();
        playerRematch[0] = playerRematch[1] = false;
        IsWhiteTurn = true;
        turn = 0;
        TurnDisplay = 0;
        removeOnce = true;
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                tiles[i, j].layer = LayerMask.NameToLayer("Tile");
            }
        }
    }
    public void StopRotation()
    {
        rotationCam = !rotationCam;

        if(rotationCam)
        {
            GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.whiteTeam : CameraAngle.BlackTeam);
        }
        
    }

    public void LeaveSol()
    {
        countingTime = false;
        LeaveSolo.SetActive(true);
        LeaveSoloButton.SetActive(false);
        GameUI.Instance.ChangeCamera(CameraAngle.menu);

    }
    public void LeaveSolNo()
    {
        countingTime = true;
        LeaveSolo.SetActive(false);
        LeaveSoloButton.SetActive(true);
        daGame.SetActive(true);
        GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.whiteTeam : CameraAngle.BlackTeam);
    }
    public void Gonext()
    {
        FFCanvas.SetActive(true);
        GameUI.Instance.ChangeCamera(CameraAngle.EndGame);
        if(localGame)
        {
            countingTime = false;
        }

    }
    public void GoNextNo()
    {
        FFCanvas.SetActive(false);
        daGame.SetActive(true);
        GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.whiteTeam : CameraAngle.BlackTeam);

        if (localGame)
        {
            countingTime = true;
        }

    }



    public void RemoveplayercauseCancel()
    {
        playerCount--;
        currentTeam = -1;
        gameHostFalse();
        
    }
    
    public void OnMenu()
    {
        
        TimerbackToOneForMulti();
        viewTimers.SetActive(false);
        currentTimeB = startMinutes * 60;
        currentTimeW = startMinutes * 60;
        countingTime = false;
        tie =false;
        TurnAfficheCanvas.SetActive(false);
        NetRematch rm = new NetRematch();
        rm.teamId = currentTeam;
        rm.wantRematch = 0;
        Client.Instance.SendToServer(rm);

        wannaplay.SetActive(false);
        dontwannaplay.SetActive(false);
        waitin.SetActive(false);
        goagain.SetActive(true);
        LeaveSoloButton.SetActive(false);
        LeaveSolo.SetActive(false);
        
        RematchButton.interactable = true;
        GameReset();
        GameUI.Instance.OnLeaveFromGame();

        Invoke("ShutdownRelay", 0.5f);

        //Reset values
        playerCount = -1;
        currentTeam = -1;
        GameUI.Instance.ChangeCamera(CameraAngle.menu);
        audioManager.isPlaying = true;

        audioManager.PreviousMusic();
        audioManager.NextMusic();
        audio.Play();
    }
    public void ResetCams()
    {
        if(Ingame==false)
        {
            CamMenu1.SetActive(true);
            CamMenu2.SetActive(true);
            CamBlacks.SetActive(true);
            CamWhites.SetActive(true);
        }
        
    }
    public void InGameT()
    {
        Ingame = true;
    }
    public void InGameF()
    {
        Ingame = false;
        Invoke("ResetCams", 0.2f);
    }

    public void OnExit()
    {
        Application.Quit();
    }

    #endregion

    public void DagameActive()
    {
        daGame.SetActive(true);
    }
    #region RegisterEvent
    public void GoBl()
    {
        currentTeam = 1;
        Invoke("gameHostTrue", 0.1f);
        
    }
    public void GoWh()
    {
     
        currentTeam = 0;
        Invoke("gameHostTrue", 0.1f);


    }
    public void GoRand()
    {

        currentTeam = -1;
        Invoke("gameHostTrue", 0.1f);


    }
    private void RegisterEvents()
    {
        GameUI.Instance.SetLocalGame += OnSetLocalGame;

        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.S_REMATCH += OnRematchServer;
        NetUtility.S_UPGRADE += OnUpgradeServer;
        NetUtility.S_FF += OnFFServer;
        NetUtility.S_TIMER += OnTimerServer;
        NetUtility.S_REPET += OnRepetServer;
        NetUtility.S_ASKTIE += OnAsktieServer;


        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.C_REMATCH += OnRematchClient;
        NetUtility.C_UPGRADE += OnUpgradeClient;
        NetUtility.C_FF += OnFFClient;
        NetUtility.C_TIMER += OnTimerClient;
        NetUtility.C_REPET += OnRepetClient;
        NetUtility.C_ASKTIE += OnAsktieClient;
    }


    

    private void UnregisterEvents()
    {
        GameUI.Instance.SetLocalGame -= OnSetLocalGame;

        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.S_REMATCH -= OnRematchServer;
        NetUtility.S_UPGRADE -= OnUpgradeServer;
        NetUtility.S_FF -= OnFFServer;
        NetUtility.S_TIMER -= OnTimerServer;
        NetUtility.S_REPET -= OnRepetServer;
        NetUtility.S_ASKTIE -= OnAsktieServer;



        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
        NetUtility.C_REMATCH -= OnRematchClient;
        NetUtility.C_UPGRADE -= OnUpgradeClient;
        NetUtility.C_FF -= OnFFClient;
        NetUtility.C_TIMER -= OnTimerClient;
        NetUtility.C_REPET -= OnRepetClient;
        NetUtility.C_ASKTIE -= OnAsktieClient;

    }

    //Server :
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        //Client has connected, assign a team and return the message back to him
        NetWelcome nw = msg as NetWelcome;

        //AssignTeam 
        
        

        playerCount++;
        if (playerCount == 0)
        {
            if (currentTeam == -1)
            {
                System.Random a = new System.Random();
                currentTeam = a.Next(0, 2);
            }
            if(localGame)
            {
                currentTeam = 0;
            }

            nw.AssignedTeam = currentTeam;
        }
        if (playerCount == 1)
        {
            if(currentTeam==0)
            {
                nw.AssignedTeam = 1;
            }else
            {
                nw.AssignedTeam = 0;
            }
        }
        
        // Return back to the client

        Server.Instance.SendToClient(cnn, nw);

        //if Full start the game :
        if(playerCount==1)
        {
            
            Server.Instance.Broadcast(new NetStartGame());

        }

    }
    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
        
        NetMakeMove mm = msg as NetMakeMove;
        //Validation checks for potential cheating



        // Receive and brodcast it back
        Server.Instance.Broadcast(mm);
    }
    private void OnRematchServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnUpgradeServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnFFServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnTimerServer(NetMessage msg, NetworkConnection cnn)
    {
        NetTimer nw = msg as NetTimer;

        Server.Instance.Broadcast(nw);
    }
    private void OnRepetServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnAsktieServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }


    //Client :

    private void OnWelcomeClient(NetMessage msg)
    {
        //Receive the connection message

        NetWelcome nw = msg as NetWelcome;

        //Assign the team 
        currentTeam = nw.AssignedTeam;

        

        if(localGame && currentTeam==0)
        {
            
            Server.Instance.Broadcast(new NetStartGame());
        }
        if (localGame)
        {
            audioManager.isPlaying = false;
            audio.Pause();
        }
        
        
    }
    private void OnStartGameClient(NetMessage msg)
    {
        //We need to change the camera
        daGame.SetActive(true);
        viewTimers.SetActive(true);
        GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.whiteTeam : CameraAngle.BlackTeam);
        if(!localGame)
        {
            if (currentTeam==0)
            {
                if(audioManager.isPlaying)
                {
                    audioManager.isPlaying = false;
                    audio.Pause();
                }
                
            }
            if (currentTeam == 1)
            {
                if(audioManager.isPlaying)
                {
                    audioManager.isPlaying = false;
                    audio.Pause();
                }
            }

        }
    }
    private void OnMakeMoveClient(NetMessage msg)
    {
        NetMakeMove mm = msg as NetMakeMove;
        TurnDisplay++;
        if (mm.teamId!=currentTeam)
        {
            ChessPiece target = chessPieces[mm.originalX, mm.originalY];
            availableMoves = target.GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
            SpeMove = target.GetSpecialMoves(ref chessPieces, ref movelist, ref availableMoves, numberOfTilesX, numberOfTilesY);
            MoveTo(mm.originalX, mm.originalY, mm.destinationX, mm.destinationY);
            source.PlayOneShot(clipMovePiece);
            if (mm.destinationY==7&& target.color==0&& target.type==ChessPieceType.Pawn)
            {
                Destroy(chessPieces[mm.destinationX, mm.destinationY].gameObject);

                
            }
            if (mm.destinationY == 0 && target.color == 1 && target.type == ChessPieceType.Pawn)
            {
                Destroy(chessPieces[mm.destinationX, mm.destinationY].gameObject);
               
                PositionSinglePiece(mm.destinationX, mm.destinationY, true);
            }

            currentTimeW = mm.TimeW;
            currentTimeB = mm.TimeB;

            
            
            

        }
        # region CheckForTie

        List<Vector2Int> whitemovs = new List<Vector2Int>();
        List<Vector2Int> blackmovs = new List<Vector2Int>();

        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null && chessPieces[i, j].color == 0)
                {
                    List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                    for (int d = 0; d < b.Count; d++)
                    {
                        whitemovs.Add(b[d]);
                    }
                }
                if (chessPieces[i, j] != null && chessPieces[i, j].color == 1)
                {
                    List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                    for (int d = 0; d < b.Count; d++)
                    {
                        blackmovs.Add(b[d]);
                    }
                }
            }
        }
        {
            for (int i = 0; i < numberOfTilesX; i++) //Check for tie 
            {
                for (int j = 0; j < numberOfTilesY; j++)
                {

                    if (chessPieces[i, j] != null && chessPieces[i, j].color == 0) //whitecheck 
                    {

                        if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                            bool res = true;
                            for (int s = 0; s < blackmovs.Count; s++)
                            {
                                if (blackmovs[s].x == i && blackmovs[s].y == j)
                                {
                                    res = false;
                                }
                            }

                            if (IsKingPat2(chessPieces[i, j], 0, b) && res == true)
                            {
                                if (tie == true)
                                {
                                    PAT();
                                    
                                }

                            }
                        }
                    }
                }
            }

        }

        {
            for (int i = 0; i < numberOfTilesX; i++) //Check for tie 
            {
                for (int j = 0; j < numberOfTilesY; j++)
                {
                    if (chessPieces[i, j] != null && chessPieces[i, j].color == 1) //blackcheck 
                    {

                        if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                            bool res = true;
                            for (int s = 0; s < whitemovs.Count; s++)
                            {
                                if (whitemovs[s].x == i && whitemovs[s].y == j)
                                {
                                    res = false;
                                }
                            }

                            if (IsKingPat2(chessPieces[i, j], 1, b) && res == true)
                            {
                                if (tie == true)
                                {
                                    PAT();
                                    
                                }

                            }
                        }
                    }
                }
            }

        }
        #endregion


        


    }
    private void OnRematchClient(NetMessage msg)
    {
        //Receive connectionMessage
        NetRematch mm = msg as NetRematch;

        //Set the boolean for rematch
        playerRematch[mm.teamId] = mm.wantRematch==1; //if WantsRematch == 1 then true, else its false

        //Activate UI
        if(localGame)
        {
            GameUI.Instance.ChangeCamera(CameraAngle.whiteTeam);
            
        }
        
        if (mm.teamId != currentTeam)
        {
            if(mm.wantRematch==0)
            {
                wannaplay.SetActive(false);
                dontwannaplay.SetActive(true);
                goagain.SetActive(false);
                waitin.SetActive(false);
            }
            else
            {
                wannaplay.SetActive(true);
                dontwannaplay.SetActive(false);
                goagain.SetActive(true);
                waitin.SetActive(false);
            }
            
        }
        else
        {
            if(mm.wantRematch == 1)
            {
                waitin.SetActive(true);
            }
        }

        //If both want a rematch
        if (playerRematch[0] && playerRematch[1])
        {
            GameReset();
            {
                GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.whiteTeam : CameraAngle.BlackTeam);
            }
        }
    }
    private void OnUpgradeClient(NetMessage msg)
    {
        //Receive connectionMessage
        NetUpgrade mm = msg as NetUpgrade;

        if(localGame)
        {
            Destroy(chessPieces[mm.destinationX, mm.destinationY].gameObject);
        }
        if (mm.TeamId != currentTeam)
        {
            if (mm.TeamId == 0)
            {
                if (mm.type == 1)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Queen, 0);
                }
                if (mm.type == 2)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Bishop, 0);
                }
                if (mm.type == 3)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Knight, 0);
                }
                if (mm.type == 4)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Rook, 0);
                }
            }
            else
            {
                if (mm.type == 1)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Queen, 1);
                }
                if (mm.type == 2)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
                }
                if (mm.type == 3)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Knight, 1);
                }
                if (mm.type == 4)
                {
                    chessPieces[mm.destinationX, mm.destinationY] = SpawnSinglePiece(ChessPieceType.Rook, 1);
                }
            }
            source.PlayOneShot(clipPromote);
            PositionSinglePiece(mm.destinationX, mm.destinationY, true);



        }
        #region CheckForCheckMate
        int res = -2;
        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();
        ChessPiece tagetKing = null;
        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    if (chessPieces[i, j].color == currentTeam)
                    {
                        defendingPieces.Add(chessPieces[i, j]);
                        if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            tagetKing = chessPieces[i, j];
                        }
                    }
                    else
                    {
                        attackingPieces.Add(chessPieces[i, j]);
                    }
                }

            }
        }

        //Is the king attacked Rn?

        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
        for (int i = 0; i < attackingPieces.Count; i++)
        {
            var PieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
            for (int a = 0; a < PieceMoves.Count; a++)
            {
                currentAvailableMoves.Add(PieceMoves[a]);
            }
        }
        // Are We In Check Rn?
        if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(tagetKing.CurrentX, tagetKing.CurrentY)))
        {
            tiles[tagetKing.CurrentX, tagetKing.CurrentY].layer = LayerMask.NameToLayer("Checked");
            //source.PlayOneShot(clipCheck);

            // King is Under Attack, can we block the attack by moving?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                SimulateMoveSinglePiece(defendingPieces[i], ref defendingMoves, tagetKing); //This will remove all the moves that are not defending the king from the attack for every piece

                if (defendingMoves.Count != 0)
                {

                    res = -1; // there is a possible way to remove checkmate
                }
            }
            if(res!=-1)
            {
                res = 1; // there is no possible way to remove the check
            }
            

        }


        if (res == 1)//checkmate
        {
            CheckMate(mm.TeamId);
        }


        #endregion

        # region CheckForTie

        List<Vector2Int> whitemovs = new List<Vector2Int>();
        List<Vector2Int> blackmovs = new List<Vector2Int>();

        for (int i = 0; i < numberOfTilesX; i++)
        {
            for (int j = 0; j < numberOfTilesY; j++)
            {
                if (chessPieces[i, j] != null && chessPieces[i, j].color == 0)
                {
                    List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                    for (int d = 0; d < b.Count; d++)
                    {
                        whitemovs.Add(b[d]);
                    }
                }
                if (chessPieces[i, j] != null && chessPieces[i, j].color == 1)
                {
                    List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                    for (int d = 0; d < b.Count; d++)
                    {
                        blackmovs.Add(b[d]);
                    }
                }
            }
        }
        {
            for (int i = 0; i < numberOfTilesX; i++) //Check for tie 
            {
                for (int j = 0; j < numberOfTilesY; j++)
                {

                    if (chessPieces[i, j] != null && chessPieces[i, j].color == 0) //whitecheck 
                    {

                        if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                            bool result = true;
                            for (int s = 0; s < blackmovs.Count; s++)
                            {
                                if (blackmovs[s].x == i && blackmovs[s].y == j)
                                {
                                    result = false;
                                }
                            }

                            if (IsKingPat2(chessPieces[i, j], 0, b) && result == true)
                            {
                                if (tie == true)
                                {

                                    PAT();
                                    
                                }

                            }
                        }
                    }
                }
            }

        }

        {
            for (int i = 0; i < numberOfTilesX; i++) //Check for tie 
            {
                for (int j = 0; j < numberOfTilesY; j++)
                {
                    if (chessPieces[i, j] != null && chessPieces[i, j].color == 1) //blackcheck 
                    {

                        if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            List<Vector2Int> b = chessPieces[i, j].GetAvailableMoves(ref chessPieces, numberOfTilesX, numberOfTilesY);
                            bool result = true;
                            for (int s = 0; s < whitemovs.Count; s++)
                            {
                                if (whitemovs[s].x == i && whitemovs[s].y == j)
                                {
                                    result = false;
                                }
                            }

                            if (IsKingPat2(chessPieces[i, j], 1, b) && result == true)
                            {
                                if (tie == true)
                                {
                                    PAT();
                                    
                                }

                            }
                        }
                    }
                }
            }

        }
        #endregion

    }
    private void OnFFClient(NetMessage msg)
    {
        //Receive connectionMessage
        NetFF mm = msg as NetFF;
        countingTime = false;
        FFCanvas.SetActive(false);
        source.PlayOneShot(clipCheckMate);
        int a = 0;
        //int b = TurnDisplay % 2;
        if (mm.TeamId == 0)
        {
            a = 1;
        }
        else
        { a = 0; }

        DisplayWin(a);
        abandon.SetActive(true);
        Checkmate.SetActive(false);
        LeaveSolo.SetActive(false);



    }
    private void OnTimerClient(NetMessage msg)
    {
        NetTimer mm = msg as NetTimer;
        if(localGame==false)
        {
            if(mm.TeamId != currentTeam)
            {
                startMinutes = mm.Timers;
                if (mm.half == 1)
                {
                    startMinutes += 0.5f;
                }
                currentTimeB = startMinutes * 60;
                currentTimeW = startMinutes * 60;
            }
        }

    }

    private void OnRepetClient(NetMessage msg)
    {
        NetRepet mm = msg as NetRepet;
        if (localGame == false)
        {
            if (mm.teamId != currentTeam)
            {
                if(mm.type==1)
                {
                    WannaTieYes();
                }
                if(mm.type==2)
                {
                    WannaTieYes2();
                }
                if(mm.type==3)
                {
                    WannaTieYes3();
                }
                if (mm.type == 4)
                {
                    WannaTieYes4();
                }
                if (mm.type == 5) //AskTie Enemy refuses
                {
                    DisplayNoTieAccept();
                }


            }
        }

    }
    private void OnAsktieClient(NetMessage msg)
    {
        NetAskPat mm = msg as NetAskPat;
        if (localGame == false)
        {
            if (mm.teamId != currentTeam)
            {
                DisplayAskTie();
            }
        }

    }



    private void ShutdownRelay()
    {
        Client.Instance.ShutDown();
        Server.Instance.ShutDown();
        daGame.SetActive(false);
    }

    // Generate local game if local game
    private void OnSetLocalGame(bool value)
    {
        localGame = value;
        
        
    }

    #endregion
}

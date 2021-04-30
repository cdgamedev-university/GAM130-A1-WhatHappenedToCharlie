﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzles {
    public class SlidingPuzzle : MonoBehaviour {
        public GameObject defaultTile;

        public float drawTileSize;

        [Range(3, 10)] public int boardSize = 3;

        public Texture2D wholeTexture;

        public SlidingPuzzleBoard puzzleBoard;
        public SlidingPuzzleBoard puzzleBoardSolution;

        bool created = false;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (gameObject.activeSelf == true && created == false) {
                CreateBoard();
                created = true;
                print("create");
            }
        }

        public SlidingPuzzleBoard SplitTexture() {
            int imageWidth = wholeTexture.width / boardSize;
            int imageHeight = wholeTexture.height / boardSize;

            SlidingPuzzleBoard board = new SlidingPuzzleBoard();

            int sqrSize = boardSize * boardSize;

            board.board1d = new Sprite[sqrSize];

            int count = 0;
            for (int i = 0; i < boardSize; i++) {
                for (int j = boardSize - 1; j >= 0; j--) {
                    if (i == boardSize - 1 && j == 0) {
                        break;
                    }

                    Texture2D texture = new Texture2D(imageWidth, imageHeight);
                    texture.SetPixels(wholeTexture.GetPixels(i * imageWidth, j * imageHeight, imageWidth, imageHeight));
                    texture.Apply();
                    Rect rect = new Rect(0, 0, imageWidth, imageHeight);
                    Sprite sprite = Sprite.Create(texture, rect, new Vector2(0, 0), .2f);
                    sprite.name = $"({i}, {j})";
                    board.board1d[count] = sprite;
                    count++;
                }
            }

            board.SetupBoard(boardSize);

            board.board2d.Convert1D();

            return board;
        }

        public void CreateBoard() {
            puzzleBoard = SplitTexture();
            puzzleBoardSolution = new SlidingPuzzleBoard(puzzleBoard);
            DisplayBoard();
        }

        void DisplayBoard() {
            Canvas childCanvas = CreateBlankCanvas();

            boardSize = puzzleBoard.boardSize;
            Rect newTransform = new Rect();

            newTransform.size = new Vector2(drawTileSize, drawTileSize);

            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < puzzleBoard.boardSize; j++) {

                    CreateNewTile(i, j, childCanvas);
                }
            }
        }

        Canvas CreateBlankCanvas() {
            GameObject newObject = new GameObject();
            newObject.name = "Sliding Puzzle Canvas";
            newObject.transform.parent = transform;
            Canvas newCanvas = newObject.AddComponent<Canvas>();
            newObject.AddComponent<GraphicRaycaster>();
            newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            return newCanvas;
        }

        void CreateNewTile(int x, int y, Canvas canvas) {
            if (!puzzleBoard.board2d[x, y])
                return;

            int xOffset = x - (boardSize / 2);
            int yOffset = (boardSize / 2) - y;

            Vector2 position = new Vector2(xOffset * drawTileSize, yOffset * drawTileSize);
            GameObject newTile = Instantiate(defaultTile, position, Quaternion.identity, canvas.transform);
            newTile.transform.localPosition = position;
            newTile.name = $"Sliding Puzzle Tile ({x}, {y})";

            Vector2 size = new Vector2(drawTileSize, drawTileSize);
            newTile.GetComponent<RectTransform>().sizeDelta = size;

            Button newButton = newTile.GetComponent<Button>();
            newButton.onClick.AddListener(() => ClickTileButton(newButton, new Vector2Int(x, y)));

            print("listener added");

            Image newImage = newTile.GetComponent<Image>();
            newImage.sprite = puzzleBoard.board2d[x, y];
        }

        bool IsTileEmpty(int x, int y) {
            if (x < 0 || y < 0 || x >= boardSize || y >= boardSize)
                return false;

            if (!puzzleBoard.board2d[x, y])
                return true;

            return false;
        }

        void MoveTileTo(Button self, Vector2Int oldPos, Vector2Int newPos) {
            puzzleBoard.board2d[newPos.x, newPos.y] = puzzleBoard.board2d[oldPos.x, oldPos.y];
            puzzleBoard.board2d[oldPos.x, oldPos.y] = null;

            int xOffset = newPos.x - (boardSize / 2);
            int yOffset = (boardSize / 2) - newPos.y;
            Vector2 position = new Vector2(xOffset * drawTileSize, yOffset * drawTileSize);
            self.transform.localPosition = position;

            Debug.Log($"[{oldPos.x}, {oldPos.y}] moving to [{newPos.x}, {newPos.y}]");

            puzzleBoard.board1d = puzzleBoard.board2d.Convert1D();

            self.onClick.RemoveAllListeners();
            self.onClick.AddListener(() => ClickTileButton(self, newPos));
        }

        void RandomizeBoard(Button self) {
            self.onClick?.Invoke();
        }

        public void ClickTileButton(Button self, Vector2Int buttonPos) {
            if (IsTileEmpty(buttonPos.x - 1, buttonPos.y)) {
                MoveTileTo(self, buttonPos, buttonPos + new Vector2Int(-1, 0));
            } else if (IsTileEmpty(buttonPos.x + 1, buttonPos.y)) {
                MoveTileTo(self, buttonPos, buttonPos + new Vector2Int(1, 0));
            } else if (IsTileEmpty(buttonPos.x, buttonPos.y - 1)) {
                MoveTileTo(self, buttonPos, buttonPos + new Vector2Int(0, -1));
            } else if (IsTileEmpty(buttonPos.x, buttonPos.y + 1)) {
                MoveTileTo(self, buttonPos, buttonPos + new Vector2Int(0, 1));
            }

            if (IsPuzzleComplete()) {
                Debug.Log("Puzzle has been completed!");
                gameObject.SetActive(false);
                Destroy(transform.parent.gameObject);
            }
        }

        bool IsPuzzleComplete() {
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    if (puzzleBoard.board2d[i, j] != puzzleBoardSolution.board2d[i, j]) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
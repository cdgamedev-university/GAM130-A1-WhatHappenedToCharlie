﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public int offset = 10;
    public GameObject piecePlace;
    private Vector2 lastMousePosition;
    private bool isDragable = true;
    public bool isDone = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragable)
        {
            lastMousePosition = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragable)
        {
            Vector2 currentMousePosition = eventData.position;
            Vector2 diff = currentMousePosition - lastMousePosition;
            RectTransform rect = GetComponent<RectTransform>();

            Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, 0);
            Vector3 oldPos = rect.position;
            rect.position = newPosition;
            if (!IsRectTransformInsideSreen(rect))
            {
                rect.position = oldPos;
            }
            lastMousePosition = currentMousePosition;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 pieceVec = this.transform.position;
        Vector3 placeVec = piecePlace.GetComponent<BoxCollider2D>().transform.position;
        if (Mathf.Abs(pieceVec.x - placeVec.x) <= 30 &&
            Mathf.Abs(pieceVec.y - placeVec.y) <= 30)
        {
            this.gameObject.transform.position = piecePlace.transform.position;
            isDragable = false;
            isDone = true;
            
            

        }

    }

    private bool IsRectTransformInsideSreen(RectTransform rectTransform)
    {
        bool isInside = false;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int visibleCorners = 0;
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        foreach (Vector3 corner in corners)
        {
            if (rect.Contains(corner))
            {
                visibleCorners++;
            }
        }
        if (visibleCorners == 4)
        {
            isInside = true;
        }
        return isInside;
    }
}

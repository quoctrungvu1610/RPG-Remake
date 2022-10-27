using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Control
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRayCast(PlayerController callingController);
    }
}


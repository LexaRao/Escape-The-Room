using UnityEngine;

public class EquipmentPiece : PuzzleInteractable
{
    public EquipmentPuzzle equipmentPuzzle; // drag parent puzzle here
    public int equipmentIndex;              // 0, 1, or 2

    void Start()
    {
        puzzleNumber = 3;
    }

    public override void Interact()
    {
        equipmentPuzzle.EquipmentClicked(equipmentIndex);
    }
}
